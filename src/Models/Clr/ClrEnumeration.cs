// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using Document.Generator.Formatters;
using Document.Generator.Helpers;
using System;
using System.Globalization;
using System.Linq;

namespace Document.Generator.Models.Clr
{
    public class ClrEnumeration : ClrType
    {
        public ClrEnumeration(ClrNamespace ns, Type typeInfo)
            : base(ns, typeInfo) { }

        public ClrEnumeration(ClrType owner, Type typeInfo)
            : base(owner, typeInfo) { }

        public override TypeCategory Category => TypeCategory.Enumeration;

        public override bool HasInterface => false;

        public override bool IsDerived => false;

        public override void WriteMembers(int level, DocumentFormatter output, OutputContext context)
        {
            output.Header(level, "Members");

            output.Table(new[] { "Name", "Value", "Description" }, Info.GetEnumValues().Cast<Enum>(),
                value => output.Text(Info.GetEnumName(value)),
                value => output.Text((value as IFormattable)?.ToString("D", CultureInfo.InvariantCulture)),
                value => context.Document.Members.For(value.GetCRef(), doc => output.Xml(doc.Summaries.FirstOrDefault()))
            );
        }
    }
}
