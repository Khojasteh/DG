using System.Reflection;

namespace Document.Generator.Models.Clr
{
    public class ClrEvent : ClrMember
    {
        public ClrEvent(ClrType owner, EventInfo eventInfo)
            : base(owner, eventInfo) { }

        public new EventInfo Info => (EventInfo)base.Info;

        public override MemberCategory Kind => MemberCategory.Event;
    }
}
