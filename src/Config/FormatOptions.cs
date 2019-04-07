// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using Document.Generator.Formatters;
using Document.Generator.Helpers;
using System.IO;

namespace Document.Generator.Config
{
    public abstract class FormatOptions
    {
        public bool Enabled { get; set; }

        public abstract string Subfolder { get; }

        public abstract string FileExtension { get; }

        public virtual void Initialize(string folder) => Directory.CreateDirectory(folder);

        public abstract DocumentFormatter CreateFormatter(TextWriter writer, ICRefResolver crefResolver, bool leaveOpen = false);
    }
}
