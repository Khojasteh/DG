using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public class XmlRevision : XmlNode
    {
        public XmlRevision(XElement node)
            : base(node)
        {
            Date = node.Attribute("date")?.Value;
            Version = node.Attribute("version")?.Value;
            Author = node.Attribute("author")?.Value;
            if (node.Attribute("visible")?.Value == "false")
                IsVisible = false;
        }

        public string Date { get; }

        public string Version { get; }

        public string Author { get; }

        public bool IsVisible { get; } = true;
    }
}
