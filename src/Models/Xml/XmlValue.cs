using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public class XmlValue : XmlRemarks
    {
        public XmlValue(XElement node)
            : base(node) { }
    }
}
