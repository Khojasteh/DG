// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public class XmlRevisionHistory : XmlNode
    {
        private IReadOnlyList<XmlRevision> _revisions;

        public XmlRevisionHistory(XElement node)
            : base(node)
        {
            if (node.Attribute("visible")?.Value == "false")
                IsVisible = false;
        }

        public IReadOnlyList<XmlRevision> Revisions
        {
            get => _revisions ?? (_revisions = Node.Elements("revision").Select(e => new XmlRevision(e)).ToList());
        }

        public bool IsVisible { get; } = true;
    }
}

