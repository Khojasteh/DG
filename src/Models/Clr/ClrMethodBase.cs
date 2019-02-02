using Document.Generator.Formatters;
using System.Linq;
using System.Reflection;

namespace Document.Generator.Models.Clr
{
    public class ClrMethodBase : ClrOverloadable
    {
        public ClrMethodBase(ClrType owner, MethodBase methodBaseInfo)
            : base(owner, methodBaseInfo) { }

        public new MethodBase Info => (MethodBase)base.Info;

        public override MemberCategory Kind => MemberCategory.Method;

        public override void WriteDetails(int level, DocumentFormatter output, OutputContext context)
        {
            if (Info.IsGenericMethod)
                WriteTypeParametersSection(level + 1, output, context, Info.GetGenericArguments().Where(arg => arg.IsGenericMethodParameter));

            WriteParametersSection(level + 1, output, context, Info.GetParameters());
            WriteReturnValueSection(level + 1, output, context, (Info as MethodInfo)?.ReturnType);

            base.WriteDetails(level, output, context);
        }
    }
}
