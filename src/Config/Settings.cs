using Document.Generator.Languages;
using System;
using System.Collections.Generic;

namespace Document.Generator.Config
{
    public class Settings
    {
        public ICollection<string> AssemblyFiles { get; } = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        public string OutputFolder { get; set; } = "./";

        public Language Language => CSharp.Instance;

        public MarkdownOptions Markdown { get; set; } = new MarkdownOptions { Enabled = true };

        public HtmlOptions Html { get; set; } = new HtmlOptions { Enabled = false };

        public IEnumerable<FormatOptions> OutputFormats
        {
            get
            {
                if (Markdown.Enabled)
                    yield return Markdown;
                if (Html.Enabled)
                    yield return Html;
            }
        }
    }
}
