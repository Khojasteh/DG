using Document.Generator.Formatters;
using Document.Generator.Helpers;
using System.IO;

namespace Document.Generator.Config
{
    public class MarkdownOptions : FormatOptions
    {
        public override string Subfolder => "markdown";

        public override string FileExtension => ".md";

        public override DocumentFormatter CreateFormatter(TextWriter writer, ICRefResolver crefResolver, bool leaveOpen = false)
        {
            return new MarkdownFormatter(writer, crefResolver, leaveOpen);
        }
    }
}
