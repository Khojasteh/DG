// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using Document.Generator.Formatters;
using Document.Generator.Helpers;
using Document.Generator.Models.Xml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Document.Generator.Models.Clr
{
    public abstract class ClrType : ClrMember
    {
        protected ClrType(ClrNamespace ns, Type typeInfo)
            : this(ns, null, typeInfo) { }

        protected ClrType(ClrType owner, Type typeInfo)
            : this(null, owner, typeInfo) { }

        private ClrType(ClrNamespace ns, ClrType owner, Type typeInfo)
            : base(owner, typeInfo)
        {
            Namespace = ns ?? owner.Namespace;
            Members = CollectMembers();
        }

        public new Type Info => (Type)base.Info;

        public ClrNamespace Namespace { get; }

        public abstract TypeCategory Category { get; }

        public abstract bool IsDerived { get; }

        public abstract bool HasInterface { get; }

        public override MemberCategory Kind => MemberCategory.Type;

        public IReadOnlyDictionary<MemberCategory, IOrderedEnumerable<ClrMember>> Members { get; }

        public override string BaseName => Info.FullName;

        public override string Title => $"{Name} {Category}";

        public override string GetDocFile(OutputContext context)
        {
            return context.ToFileName(Info.FullName);
        }

        public override void WriteDetails(int level, DocumentFormatter output, OutputContext context)
        {
            if (Info.IsGenericType)
                WriteTypeParametersSection(level + 1, output, context, Info.GetGenericArguments());

            WriteMembers(level, output, context);

            WriteThreadSafetySection(level, output, context.Document.Of(this)?.ThreadSafeties);

            base.WriteDetails(level, output, context);
        }

        public virtual void WriteMembers(int level, DocumentFormatter output, OutputContext context)
        {
            foreach (var category in Members.Select(g => g.Key).OrderBy(c => c))
            {
                var categoryMembers = Members[category];
                if (categoryMembers.Any())
                {
                    WriteMembersInCategory(level, output, context, category, categoryMembers);
                    categoryMembers.Where(m => m.HasOwnDocFile).ForEach(context.Compose);
                }
            }
        }

        protected void WriteMembersInCategory(int level, DocumentFormatter output, OutputContext context, MemberCategory category, IEnumerable<ClrMember> members)
        {
            output.Header(level, category.ToPluralString());

            output.Table(new[] { category.ToString(), "Description" }, members,
                member => member.WriteLink(output, context),
                member => member.WriteSummaryLine(output, context)
            );
        }

        protected virtual void WriteThreadSafetySection(int level, DocumentFormatter output, IEnumerable<XmlThreadSafety> threadSafeties)
        {
            var threadSafety = threadSafeties?.FirstOrDefault();
            if (threadSafety == null)
                return;

            output.Header(level, "Thread Safety");
            if (threadSafety.Static && threadSafety.Instance)
                output.Section("Any public member of this type, either static or instance, is thread-safe.");
            else if (threadSafety.Static && !threadSafety.Instance)
                output.Section("Any public static member of this type is thread-safe, but instance members are not guaranteed to be thread-safe.");
            else if (!threadSafety.Static && threadSafety.Instance)
                output.Section("Any public static member of this type is not guaranteed to be thread-safe, but instance members are thread-safe.");
            else
                output.Section("Neither public nor instance members of this type are guaranteed to be thread-safe.");
        }

        protected override IEnumerable<(string, Action<DocumentFormatter>)> GetInfoBoxWriters(OutputContext context)
        {
            yield return ("Namespace", output =>
            {
                Namespace.WriteLink(output, context);
            });

            yield return ("Assembly", output =>
            {
                Namespace.Assembly.WriteLink(output, context);
                output.Text(" (");
                output.Text(string.Join(", ", Namespace.Assembly.Modules));
                output.Text(") version ", Namespace.Assembly.Version);
            });

            if (HasInterface)
            {
                var interfaces = Info.GetInterfaces();
                yield return ("Implements", output =>
                {
                    for (var i = 0; i < interfaces.Length; i++)
                    {
                        if (i != 0)
                            output.Text(", ");
                        output.LinkCRef(interfaces[i].GetCRef(), interfaces[i].GetDisplayName());
                    }
                });
            }

            if (IsDerived)
            {
                yield return ("Inheritance", output =>
                {
                    foreach (var ancestor in Info.GetAncestors())
                    {
                        output.LinkCRef(ancestor.GetCRef(), ancestor.GetDisplayName());
                        output.Text("\u2002", ("\u2192", TextStyles.Teletype), "\u2002");
                    }
                    output.Text(Name);
                });
            }
        }

        protected override IEnumerable<ICRef> GetReferences()
        {
            return (Owner != null) ? base.GetReferences() : new[] { Namespace };
        }

        private IReadOnlyDictionary<MemberCategory, IOrderedEnumerable<ClrMember>> CollectMembers()
        {
            const BindingFlags bindingAttributes = BindingFlags.DeclaredOnly |
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;

            return new Dictionary<MemberCategory, IOrderedEnumerable<ClrMember>>
            {
                [MemberCategory.Type] = Info.GetNestedTypes()
                    .Where(t => t.IsVisible())
                    .Select(t => CreateNested(t))
                    .OrderBy(t => t.Name),
                [MemberCategory.Property] = Info.GetProperties(bindingAttributes)
                    .Where(p => p.IsVisible())
                    .Select(p => new ClrProperty(this, p))
                    .OrderBy(p => p.Name),
                [MemberCategory.Event] = Info.GetEvents(bindingAttributes)
                    .Where(e => e.IsVisible())
                    .Select(e => new ClrEvent(this, e))
                    .OrderBy(e => e.Name),
                [MemberCategory.Field] = Info.GetFields(bindingAttributes)
                    .Where(f => f.IsVisible())
                    .Select(f => new ClrField(this, f))
                    .OrderBy(f => f.Name),
                [MemberCategory.Constructor] = Info.GetConstructors(bindingAttributes)
                    .Where(c => c.IsVisible())
                    .Select(c => new ClrConstructor(this, c))
                    .OrderBy(c => c.Name),
                [MemberCategory.Method] = Info.GetMethods(bindingAttributes)
                    .Where(m => !m.IsSpecialName && m.IsVisible())
                    .Select(m => new ClrMethod(this, m))
                    .OrderBy(m => m.Name),
                [MemberCategory.Operator] = Info.GetMethods(bindingAttributes)
                    .Where(m => m.IsSpecialName && m.IsOperator() && m.IsVisible())
                    .Select(m => new ClrMethod(this, m))
                    .OrderBy(m => m.Name),
            };
        }

        protected ClrType CreateNested(Type type)
        {
            if (type.IsDelegate())
                return new ClrDelegate(this, type);
            if (type.IsClass)
                return new ClrClass(this, type);
            if (type.IsInterface)
                return new ClrInterface(this, type);
            if (type.IsEnum)
                return new ClrEnumeration(this, type);
            if (type.IsValueType)
                return new ClrStructure(this, type);

            throw new ArgumentException($"Unexpected type: {type}");
        }

        public static ClrType Create(ClrNamespace ns, Type type)
        {
            if (type.IsDelegate())
                return new ClrDelegate(ns, type);
            if (type.IsClass)
                return new ClrClass(ns, type);
            if (type.IsInterface)
                return new ClrInterface(ns, type);
            if (type.IsEnum)
                return new ClrEnumeration(ns, type);
            if (type.IsValueType)
                return new ClrStructure(ns, type);

            throw new ArgumentException($"Unexpected type: {type}");
        }
    }
}
