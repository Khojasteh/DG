using System.Reflection;

namespace Document.Generator.Models.Clr
{
    public class ClrField : ClrMember
    {
        public ClrField(ClrType owner, FieldInfo fieldInfo)
            : base(owner, fieldInfo) { }

        public new FieldInfo Info => (FieldInfo)base.Info;

        public override MemberCategory Kind => MemberCategory.Field;
    }
}
