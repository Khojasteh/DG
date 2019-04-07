// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public abstract class XmlNode : IEquatable<XmlNode>
    {
        public XmlNode(XElement node)
        {
            Node = node ?? throw new ArgumentNullException(nameof(node));
        }

        protected XElement Node { get; }

        public bool Equals(XmlNode other) => (other != null) && Node.Equals(other.Node);

        public override bool Equals(object obj) => Equals(obj as XmlNode);

        public override int GetHashCode() => Node.GetHashCode();

        public override string ToString() => Node.ToString();
    }
}
