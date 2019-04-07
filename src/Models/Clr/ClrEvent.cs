// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

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
