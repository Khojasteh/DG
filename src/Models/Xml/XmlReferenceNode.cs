using Document.Generator.Helpers;
using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public class XmlReferenceNode : XmlNode, ICRef
    {
        public XmlReferenceNode(XElement node)
            : base(node)
        {
            CRef = node.Attribute("cref")?.Value;
        }

        public virtual string CRef { get; }
    }
}
