// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using Document.Generator.Models.Clr;
using Document.Generator.Models.Xml;
using System;
using System.IO;

namespace Document.Generator.Models
{
    public class InputContext
    {
        public InputContext(ClrAssembly assembly, XmlDocument document)
        {
            Assembly = assembly ?? throw new ArgumentNullException(nameof(assembly));
            Document = document ?? throw new ArgumentNullException(nameof(document));
        }

        public ClrAssembly Assembly { get; }

        public XmlDocument Document { get; }

        public static InputContext Create(string assemblyFile)
        {
            var assembly = ClrAssembly.LoadFile(assemblyFile);
            var document = XmlDocument.LoadFile(Path.ChangeExtension(assemblyFile, ".xml"));
            return new InputContext(assembly, document);
        }
    }
}
