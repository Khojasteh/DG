// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using Document.Generator.Config;
using Document.Generator.Formatters;
using Document.Generator.Helpers;
using Document.Generator.Languages;
using Document.Generator.Models.Clr;
using System;
using System.IO;
using System.Text;

namespace Document.Generator.Models
{
    public class OutputContext : InputContext, ICRefResolver
    {
        private static readonly char[] FileNameCharsToClean = new[] { '`', '#', '+' };

        private readonly FormatOptions _formatOptions;

        public OutputContext(InputContext inputContext, FormatOptions formatOptions, Language language, string outputPath, string indexName = null)
            : base(inputContext?.Assembly, inputContext?.Document)
        {
            _formatOptions = formatOptions ?? throw new ArgumentNullException(nameof(formatOptions));
            Language = language ?? throw new ArgumentNullException(nameof(language));
            OutputPath = outputPath ?? throw new ArgumentNullException(nameof(outputPath));
            Assembly.DocFile = indexName;
            formatOptions.Initialize(outputPath);
        }

        public Language Language { get; }

        public string OutputPath { get; }

        public void Compose()
        {
            Compose(Assembly);
        }

        public void Compose(ClrItem item)
        {
            Compose(item.GetDocFile(this), item.Title, formatter => item.Write(formatter, this));
        }

        public void Compose(string fileName, string title, Action<DocumentFormatter> contentWriter)
        {
            var path = Path.Combine(OutputPath, fileName);
            var textWriter = new StreamWriter(path, false, Encoding.UTF8);
            using (var formatter = _formatOptions.CreateFormatter(textWriter, this, leaveOpen: false))
            {
                formatter.Prologue(title);
                contentWriter(formatter);
                formatter.Epilogue();
            }
        }

        public string ToFileName(string name)
        {
            return CleanFileName(name) + _formatOptions.FileExtension;
        }

        private static string CleanFileName(string name)
        {
            var i = name.IndexOfAny(FileNameCharsToClean);
            if (i == -1)
                return name;

            var chars = name.ToCharArray();
            do
            {
                chars[i] = (chars[i]) == '+' ? '.' : '-';
                i = name.IndexOfAny(FileNameCharsToClean, i + 1);
            } while (i != -1);

            return new string(chars);
        }

        #region ICRefResolver

        public string NameOf(string cref)
        {
            if (Assembly.TryGet(cref, out var item))
                return item.Name;

            return Utils.FormatCRef(cref);
        }

        public string UrlOf(string cref)
        {
            if (Assembly.TryGet(cref, out var item))
                return item.GetUrl(this);

            if (cref.StartsWith("!:"))
                return string.Empty;

            var i = cref.IndexOfAny(new[] { '(', '[' });
            var resourceName = CleanFileName((i != -1) ? cref.Substring(2, i - 2) : cref.Substring(2));
            return "https://docs.microsoft.com/en-us/dotnet/api/" + resourceName.ToLowerInvariant();
        }

        #endregion
    }
}
