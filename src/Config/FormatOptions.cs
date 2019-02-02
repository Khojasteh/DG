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
