// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using Document.Generator.Formatters;
using Document.Generator.Helpers;
using System.IO;

namespace Document.Generator.Config
{
    public class HtmlOptions : FormatOptions
    {
        private const string DefaultStylesheet = "style.css";

        public string Stylesheet { get; set; }

        public override string Subfolder => "html";

        public override string FileExtension => ".html";

        public override DocumentFormatter CreateFormatter(TextWriter writer, ICRefResolver crefResolver, bool leaveOpen = false)
        {
            return new HtmlFormatter(writer, crefResolver, Stylesheet ?? DefaultStylesheet, leaveOpen);
        }

        public override void Initialize(string folder)
        {
            base.Initialize(folder);
            if (Stylesheet == null)
                Asset.WriteToFile(DefaultStylesheet, Path.Combine(folder, DefaultStylesheet));
        }
    }
}
