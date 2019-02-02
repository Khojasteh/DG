using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public class XmlException : XmlReferenceNode
    {
        public XmlException(XElement node)
            : base(node) { }
    }
}
