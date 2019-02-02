using System.Linq;
using System.Reflection;
using Document.Generator.Formatters;

namespace Document.Generator.Models.Clr
{
    public class ClrConstructor : ClrMethodBase
    {
        public ClrConstructor(ClrType owner, ConstructorInfo constructorInfo)
            : base(owner, constructorInfo) { }

        public new ConstructorInfo Info => (ConstructorInfo)base.Info;

        public override MemberCategory Kind => MemberCategory.Constructor;

        public override string SharedTitle => $"{Owner.Name} Constructor";

        public override string GetDocFile(OutputContext context)
        {
            return context.ToFileName(Owner.Info.FullName + ".-ctor");
        }

        public override void WriteSummaryLine(DocumentFormatter output, OutputContext context)
        {
            var summary = context.Document.Of(this)?.Summaries.FirstOrDefault();
            if (summary != null)
                output.Xml(summary);
            else
                WriteDefaultSummary(output, context);
        }

        public override void WriteSummary(DocumentFormatter output, OutputContext context)
        {
            var summary = context.Document.Of(this)?.Summaries;
            if (summary != null && summary.Count != 0)
                output.Section(() => output.Xml(summary));
            else
                output.Section(() => WriteDefaultSummary(output, context));
        }

        protected virtual void WriteDefaultSummary(DocumentFormatter output, OutputContext context)
        {
            output.Text("Initializes a new instance of ");
            Owner.WriteLink(output, context);
            output.Text(" ", Owner.Category.ToString().ToLower(), ".");
        }
    }
}
