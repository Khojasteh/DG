using System;

namespace Document.Generator.Models.Clr
{
    public class ClrStructure : ClrType
    {
        public ClrStructure(ClrNamespace ns, Type typeInfo) 
            : base(ns, typeInfo) { }

        public ClrStructure(ClrType owner, Type typeInfo)
            : base(owner, typeInfo) { }

        public override TypeCategory Category => TypeCategory.Structure;

        public override bool HasInterface => Info.GetInterfaces().Length != 0;

        public override bool IsDerived => false;
    }
}
