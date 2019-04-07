// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using System;

namespace Document.Generator.Models.Clr
{
    public class ClrInterface : ClrType
    {
        public ClrInterface(ClrNamespace ns, Type typeInfo)
            : base(ns, typeInfo) { }

        public ClrInterface(ClrType owner, Type typeInfo)
            : base(owner, typeInfo) { }

        public override TypeCategory Category => TypeCategory.Interface;

        public override bool HasInterface => Info.GetInterfaces().Length != 0;

        public override bool IsDerived => Info.BaseType != null;
    }
}
