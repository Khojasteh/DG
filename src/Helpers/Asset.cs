// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using System.IO;
using System.Text;

namespace Document.Generator.Helpers
{
    public static class Asset
    {
        public static Stream GetStream(string assetName)
        {
            var entry = typeof(Program);
            var resourcePath = $"{entry.Namespace}.Assets.{assetName}";
            return entry.Assembly.GetManifestResourceStream(resourcePath);
        }

        public static string ReadAsString(string assetName)
        {
            using (var stream = GetStream(assetName))
            using (var reader = new StreamReader(stream, Encoding.UTF8))
                return reader.ReadToEnd();
        }

        public static void WriteToFile(string assetName, string outputPath)
        {
            using (var stream = GetStream(assetName))
            using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
                stream.CopyTo(writer.BaseStream);
        }
    }
}
