using System;

namespace Document.Generator.Models.Clr
{
    public class ClrDelegate : ClrType
    {
        public ClrDelegate(ClrNamespace ns, Type typeInfo) 
            : base(ns, typeInfo) { }

        public ClrDelegate(ClrType owner, Type typeInfo)
            : base(owner, typeInfo) { }

        public override TypeCategory Category => TypeCategory.Delegate;

        public override bool HasInterface => false;

        public override bool IsDerived => false;
    }
}
