// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public class XmlPermission : XmlReferenceNode
    {
        public XmlPermission(XElement node)
            : base(node) { }
    }
}
