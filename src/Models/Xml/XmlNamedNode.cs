using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public abstract class XmlNamedNode : XmlNode
    {
        public XmlNamedNode(XElement node)
            : base(node)
        {
            Name = node.Attribute("name")?.Value;
        }

        public virtual string Name { get; }
    }
}
