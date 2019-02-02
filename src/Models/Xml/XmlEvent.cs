using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public class XmlEvent : XmlReferenceNode
    {
        public XmlEvent(XElement node)
            : base(node) { }
    }
}
