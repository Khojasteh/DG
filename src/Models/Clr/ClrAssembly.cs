// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using Document.Generator.Formatters;
using Document.Generator.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Document.Generator.Models.Clr
{
    public class ClrAssembly : ClrItem
    {
        private IReadOnlyDictionary<string, ClrItem> _crefs;

        private ClrAssembly(Assembly assembly)
        {
            Name = assembly.GetName().Name;
            Version = assembly.GetName().Version;
            Modules = assembly.Modules.Select(module => module.Name).ToList();
            Namespaces = assembly.ExportedTypes.GroupBy(type => type.Namespace, (ns, types) => new ClrNamespace(this, ns, types));
        }

        public virtual string DocFile { get; set; }

        public override string Name { get; }

        public Version Version { get; }

        public ICollection<string> Modules { get; }

        public IEnumerable<ClrNamespace> Namespaces { get; }

        public override string Title => $"{Name} Assembly (version {Version})";

        public virtual bool TryGet(string cref, out ClrItem item)
        {
            if (_crefs == null)
                _crefs = CollectCRefs();

            return _crefs.TryGetValue(cref, out item);
        }

        public override string GetDocFile(OutputContext context)
        {
            return context.ToFileName(DocFile ?? $"_toc.{Name}");
        }

        public override void WriteSummaryLine(DocumentFormatter output, OutputContext context)
        {
            output.Section("The ", (Name, TextStyles.Teletype), " assembly exports types in the following namespaces.");
        }

        public override void Write(DocumentFormatter output, OutputContext context)
        {
            output.Header(1, Title);
            WriteSummaryLine(output, context);
            Namespaces.OrderBy(ns => ns.Name).ForEach(ns => ns.Write(output, context));
        }

        private IReadOnlyDictionary<string, ClrItem> CollectCRefs()
        {
            var dictionary = new Dictionary<string, ClrItem>(1013);

            foreach (var ns in Namespaces)
            {
                dictionary.Add(ns.CRef, ns);
                foreach (var type in ns.TypeCategories.SelectMany(category => category))
                {
                    dictionary.Add(type.CRef, type);
                    AddTypeMembers(type);
                }
            }

            void AddTypeMembers(ClrType type)
            {
                foreach (var member in type.Members.Values.SelectMany(category => category))
                {
                    dictionary.Add(member.CRef, member);
                    if (member is ClrType nestedType)
                        AddTypeMembers(nestedType);
                }
            }

            return dictionary;
        }

        public static ClrAssembly LoadFile(string file)
        {
            return new ClrAssembly(Assembly.LoadFile(file));
        }
    }
}
