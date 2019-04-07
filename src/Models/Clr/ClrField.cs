// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

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
