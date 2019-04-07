// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using Document.Generator.Formatters;
using Document.Generator.Helpers;
using Document.Generator.Models.Xml;
using System.Collections.Generic;
using System.Linq;

namespace Document.Generator.Models.Clr
{
    public abstract class ClrItem
    {
        public abstract string Name { get; }

        public abstract string Title { get; }

        public virtual bool HasOwnDocFile => true;

        public abstract string GetDocFile(OutputContext context);

        public virtual string GetUrl(OutputContext context) => GetDocFile(context);

        public abstract void Write(DocumentFormatter output, OutputContext context);

        public abstract void WriteSummaryLine(DocumentFormatter output, OutputContext context);

        public virtual void WriteLink(DocumentFormatter output, OutputContext context)
        {
            output.Link(GetUrl(context), Name);
        }

        protected virtual void WriteSection(DocumentFormatter output, int level, string header, IEnumerable<XmlNode> items)
        {
            if (items == null || !items.Any())
                return;

            if (!string.IsNullOrEmpty(header))
                output.Header(level, header);

            output.Xml(items);
        }

        protected virtual void WriteSection(DocumentFormatter output, int level, string header, string label, IEnumerable<XmlNamedNode> items)
        {
            if (items == null || !items.Any())
                return;

            if (!string.IsNullOrEmpty(header))
                output.Header(level, header);

            output.Table(new[] { label, "Description" }, items,
                item => output.Text(item.Name),
                item => output.Xml(item));
        }

        protected virtual void WriteSection(DocumentFormatter output, int level, string header, string label, IEnumerable<XmlReferenceNode> items)
        {
            if (items == null || !items.Any())
                return;

            if (!string.IsNullOrEmpty(header))
                output.Header(level, header);

            output.Table(new[] { label, "Description" }, items,
                item => output.LinkCRef(item.CRef, Utils.FormatCRef(item.CRef)),
                item => output.Xml(item));
        }

        protected virtual void WriteSection(DocumentFormatter output, int level, string header, IEnumerable<XmlRevisionHistory> items)
        {
            var revisions = items?.Where(item => item.IsVisible).SelectMany(item => item.Revisions.Where(revision => revision.IsVisible));

            if (revisions == null || !revisions.Any())
                return;

            if (!string.IsNullOrEmpty(header))
                output.Header(level, header);

            output.Table(new[] { "Date", "Version", "Author", "Description" }, revisions,
                revision => output.Text(revision.Date),
                revision => output.Text(revision.Version),
                revision => output.Text(revision.Author),
                revision => output.Xml(revision));
        }

        protected virtual void WriteSection(DocumentFormatter output, int level, string header, IEnumerable<ICRef> items)
        {
            if (items == null || !items.Any())
                return;

            if (!string.IsNullOrEmpty(header))
                output.Header(level, header);

            output.List(items, item => output.LinkCRef(item.CRef, (item as ClrItem)?.Title));
        }

        public override string ToString() => Title;
    }
}
