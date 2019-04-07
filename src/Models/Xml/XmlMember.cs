// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Document.Generator.Models.Xml
{
    public class XmlMember : XmlNamedNode
    {
        private IReadOnlyList<XmlSummary> _summaries;
        private IReadOnlyDictionary<string, XmlTypeParameter> _typeParameters;
        private IReadOnlyDictionary<string, XmlParameter> _parameters;
        private IReadOnlyList<XmlValue> _propertyValues;
        private IReadOnlyList<XmlReturns> _returns;
        private IReadOnlyList<XmlEvent> _events;
        private IReadOnlyList<XmlException> _exceptions;
        private IReadOnlyList<XmlExample> _examples;
        private IReadOnlyList<XmlRemarks> _remarks;
        private IReadOnlyList<XmlPermission> _permissions;
        private IReadOnlyList<XmlSeeAlso> _seeAlso;
        private IReadOnlyList<XmlOverloads> _overloads;
        private IReadOnlyList<XmlThreadSafety> _threadSafeties;
        private IReadOnlyList<XmlPreliminary> _preliminaries;
        private IReadOnlyList<XmlRevisionHistory> _revisionHistories;

        public XmlMember(XElement node)
            : base(node) { }

        public IReadOnlyList<XmlSummary> Summaries
        {
            get => _summaries ?? (_summaries = Node.Elements("summary").Select(e => new XmlSummary(e)).ToList());
        }

        public IReadOnlyDictionary<string, XmlTypeParameter> TypeParameters
        {
            get => _typeParameters ?? (_typeParameters = Node.Elements("typeparam").Select(e => new XmlTypeParameter(e)).ToDictionary(p => p.Name));
        }

        public IReadOnlyDictionary<string, XmlParameter> Parameters
        {
            get => _parameters ?? (_parameters = Node.Elements("param").Select(e => new XmlParameter(e)).ToDictionary(p => p.Name));
        }

        public IReadOnlyList<XmlValue> PropertyValues
        {
            get => _propertyValues ?? (_propertyValues = Node.Elements("value").Select(e => new XmlValue(e)).ToList());
        }

        public IReadOnlyList<XmlReturns> Returns
        {
            get => _returns ?? (_returns = Node.Elements("returns").Select(e => new XmlReturns(e)).ToList());
        }

        public IReadOnlyList<XmlEvent> Events
        {
            get => _events ?? (_events = Node.Elements("events").Select(e => new XmlEvent(e)).ToList());
        }

        public IReadOnlyList<XmlException> Exceptions
        {
            get => _exceptions ?? (_exceptions = Node.Elements("exception").Select(e => new XmlException(e)).ToList());
        }

        public IReadOnlyList<XmlExample> Examples
        {
            get => _examples ?? (_examples = Node.Elements("example").Select(e => new XmlExample(e)).ToList());
        }

        public IReadOnlyList<XmlRemarks> Remarks
        {
            get => _remarks ?? (_remarks = Node.Elements("remarks").Select(e => new XmlRemarks(e)).ToList());
        }

        public IReadOnlyList<XmlPermission> Permissions
        {
            get => _permissions ?? (_permissions = Node.Elements("permission").Select(e => new XmlPermission(e)).ToList());
        }

        public IReadOnlyList<XmlSeeAlso> SeeAlso
        {
            get => _seeAlso ?? (_seeAlso = Node.Elements("seealso").Select(e => new XmlSeeAlso(e)).ToList());
        }

        public IReadOnlyList<XmlOverloads> Overloads
        {
            get => _overloads ?? (_overloads = Node.Elements("overloads").Select(e => new XmlOverloads(e)).ToList());
        }

        public IReadOnlyList<XmlThreadSafety> ThreadSafeties
        {
            get => _threadSafeties ?? (_threadSafeties = Node.Elements("threadsafety").Select(e => new XmlThreadSafety(e)).ToList());
        }

        public IReadOnlyList<XmlPreliminary> Preliminaries
        {
            get => _preliminaries ?? (_preliminaries = Node.Elements("preliminary").Select(e => new XmlPreliminary(e)).ToList());
        }

        public IReadOnlyList<XmlRevisionHistory> RevisionHistories
        {
            get => _revisionHistories ?? (_revisionHistories = Node.Elements("revisionHistory").Select(e => new XmlRevisionHistory(e)).ToList());
        }

    }
}
