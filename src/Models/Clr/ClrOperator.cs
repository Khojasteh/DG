using System.Reflection;

namespace Document.Generator.Models.Clr
{
    public class ClrOperator : ClrMethod
    {
        public ClrOperator(ClrType owner, MethodInfo methodInfo)
            : base(owner, methodInfo) { }

        public override MemberCategory Kind => MemberCategory.Operator;
    }
}
