// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

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
