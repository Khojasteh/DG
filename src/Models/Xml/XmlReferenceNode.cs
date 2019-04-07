// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

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
