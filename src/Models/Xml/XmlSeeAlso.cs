using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public class XmlSeeAlso : XmlReferenceNode
    {
        public XmlSeeAlso(XElement node)
            : base(node) { }
    }
}
