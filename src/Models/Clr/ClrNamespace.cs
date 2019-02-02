using Document.Generator.Formatters;
using Document.Generator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Document.Generator.Models.Clr
{
    public class ClrNamespace : ClrItem, ICRef
    {
        public ClrNamespace(ClrAssembly owner, string name, IEnumerable<Type> types)
        {
            Assembly = owner;
            Name = name;
            TypeCategories = types.Where(type => !type.IsNested).Select(type => ClrType.Create(this, type)).ToLookup(type => type.Category);
        }

        public ClrAssembly Assembly { get; }

        public override string Name { get; }

        public ILookup<TypeCategory, ClrType> TypeCategories { get; }

        public string CRef => $"N:{Name}";

        public override string Title => $"{Name} Namespace";

        public override bool HasOwnDocFile => false;

        public override string GetDocFile(OutputContext context)
        {
            return Assembly.GetDocFile(context);
        }

        public override string GetUrl(OutputContext context)
        {
            return GetDocFile(context) + '#' + Uri.EscapeDataString(Title);
        }

        public override void WriteSummaryLine(DocumentFormatter output, OutputContext context)
        {
            output.Section("The ", (Name, TextStyles.Teletype), " namespace exposes the following types.");
        }

        public override void Write(DocumentFormatter output, OutputContext context)
        {
            output.Header(2, Title);
            WriteSummaryLine(output, context);

            foreach (var typeCategory in TypeCategories.OrderBy(group => group.Key))
            {
                WriteTypesInCategory(output, context, typeCategory.Key, typeCategory);
                typeCategory.ForEach(context.Compose);
            }
        }

        protected void WriteTypesInCategory(DocumentFormatter output, OutputContext context, TypeCategory category, IEnumerable<ClrType> types)
        {
            output.Header(3, category.ToPluralString());

            output.Table(new[] { category.ToString(), "Description" }, types,
                type => type.WriteLink(output, context),
                type => type.WriteSummaryLine(output, context)
            );
        }
    }
}
