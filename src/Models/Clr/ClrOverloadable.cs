using Document.Generator.Formatters;
using Document.Generator.Models.Clr;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Document.Generator.Models
{
    public abstract class ClrOverloadable : ClrMember
    {
        private IReadOnlyList<ClrOverloadable> _overloads;

        public ClrOverloadable(ClrType owner, MemberInfo memberInfo)
            : base(owner, memberInfo) { }

        public virtual string SharedTitle => $"{Owner.Name}.{BaseName} {Kind}";

        public override bool HasOwnDocFile => (Overloads.Count == 0) || Equals(_overloads[0]);

        public virtual IReadOnlyList<ClrOverloadable> Overloads
        {
            get
            {
                if (_overloads == null)
                {
                    List<ClrOverloadable> overloads = null;
                    foreach (ClrOverloadable member in Owner.Members[Kind])
                    {
                        if (BaseName.Equals(member.BaseName) && !Equals(member))
                        {
                            if (overloads == null)
                                overloads = new List<ClrOverloadable> { this };

                            overloads.Add(member);
                            member._overloads = overloads;
                        }
                    }
                    if (overloads != null)
                    {
                        overloads.Sort((x, y) => string.Compare(x.Name, y.Name));
                        _overloads = overloads;
                    }
                    else
                    {
                        _overloads = Array.Empty<ClrOverloadable>();
                    }
                }
                return _overloads;
            }
        }

        public override string GetUrl(OutputContext context)
        {
            var url = base.GetUrl(context);
            if (Overloads.Count != 0)
                url += '#' + Uri.EscapeDataString(Name);
            return url;
        }

        public override void Write(DocumentFormatter output, OutputContext context)
        {
            if (Overloads.Count == 0)
            {
                base.Write(output, context);
                return;
            }

            output.Header(1, SharedTitle);
            WriteInfoBox(output, context);
            WriteOverloadsSummary(output, context);

            output.Table(new[] { "Overload", "Description" }, Overloads,
                overload => overload.WriteLink(output, context),
                overload => overload.WriteSummaryLine(output, context)
            );

            foreach (var overload in Overloads)
            {
                output.Header(2, overload.Name);
                overload.WriteSummary(output, context);

                var syntax = overload.GetSyntax(context.Language);
                if (!string.IsNullOrEmpty(syntax))
                {
                    output.Header(3, "Syntax");
                    output.Code(syntax, context.Language.Name);
                }

                overload.WriteDetails(3, output, context);
            }
        }

        protected virtual void WriteOverloadsSummary(DocumentFormatter output, OutputContext context)
        {
            var doc = Overloads.Select(o => context.Document.Of(o)?.Overloads.FirstOrDefault()).FirstOrDefault(o => o != null);
            if (doc != null)
            {
                if (doc.Summaries.Any())
                    output.Xml(doc.Summaries);
                else
                    output.Section(() => output.Xml(doc));
            }
        }
    }
}
