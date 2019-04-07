// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using Document.Generator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public class XmlDocument
    {
        public static readonly XmlDocument Null = new XmlDocument();

        private XmlDocument()
        {
            Members = new Dictionary<string, XmlMember>();
        }

        public XmlDocument(XDocument document)
        {
            if (document == null)
                throw new ArgumentNullException(nameof(document));

            AssemblyName = document.Descendants("assembly").Single().Element("name").Value;
            Members = document.Descendants("member").Select(e => new XmlMember(e)).ToDictionary(m => m.Name);
        }

        public string AssemblyName { get; }

        public IReadOnlyDictionary<string, XmlMember> Members { get; }

        public XmlMember Of(ICRef target)
        {
            return Members.TryGetValue(target.CRef, out var member) ? member : null;
        }

        public static XmlDocument LoadFile(string file)
        {
            return new XmlDocument(XDocument.Load(file));
        }
    }
}
