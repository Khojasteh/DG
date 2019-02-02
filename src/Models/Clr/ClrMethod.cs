using System.Reflection;

namespace Document.Generator.Models.Clr
{
    public class ClrMethod : ClrMethodBase
    {
        public ClrMethod(ClrType owner, MethodInfo methodInfo)
            : base(owner, methodInfo) { }

        public new MethodInfo Info => (MethodInfo)base.Info;

        public override MemberCategory Kind => MemberCategory.Method;
    }
}
