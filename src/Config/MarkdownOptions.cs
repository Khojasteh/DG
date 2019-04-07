// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

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
