using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public class XmlParameter : XmlNamedNode
    {
        public XmlParameter(XElement node)
            : base(node) { }
    }
}
