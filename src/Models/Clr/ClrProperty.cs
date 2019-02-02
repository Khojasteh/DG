using Document.Generator.Formatters;
using Document.Generator.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Document.Generator.Models.Clr
{
    public class ClrProperty : ClrOverloadable
    {
        public ClrProperty(ClrType owner, PropertyInfo propertyInfo)
            : base(owner, propertyInfo) { }

        public new PropertyInfo Info => (PropertyInfo)base.Info;

        public override MemberCategory Kind => MemberCategory.Property;

        public override IReadOnlyList<ClrOverloadable> Overloads => Info.IsIndexer() ? base.Overloads : Array.Empty<ClrProperty>();

        public override void WriteDetails(int level, DocumentFormatter output, OutputContext context)
        {
            if (Info.IsIndexer())
                WriteParametersSection(level, output, context, Info.GetIndexParameters());

            WriteSection(output, level, "Property Value", context.Document.Of(this)?.PropertyValues);

            base.WriteDetails(level, output, context);
        }
    }
}
