using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public class XmlThreadSafety : XmlNode
    {
        public XmlThreadSafety(XElement node)
            : base(node)
        {
            if (node.Attribute("static")?.Value == "false")
                Static = false;
            if (node.Attribute("instance")?.Value == "true")
                Instance = true;
        }

        public bool Static { get; } = true;

        public bool Instance { get; } = false;
    }
}
