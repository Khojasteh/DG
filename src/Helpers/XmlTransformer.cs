using System;
using System.Collections.Concurrent;
using System.IO;
using System.Xml;
using System.Xml.Xsl;

namespace Document.Generator.Helpers
{
    public class XmlTransformer
    {
        public const string XsltExtensionNamespaceUri = "http://xmldoc.transform";

        private static readonly ConcurrentDictionary<string, XslCompiledTransform> loadedXmlTransforms
            = new ConcurrentDictionary<string, XslCompiledTransform>();

        public static XmlTransformer Create(string xsltAssetName, ICRefResolver crefResolver)
        {
            var xslCompiledTransform = loadedXmlTransforms.GetOrAdd(xsltAssetName, _ =>
            {
                var xslt = new XslCompiledTransform();
                using (var reader = CreateXmlReaderFor(xsltAssetName))
                    xslt.Load(reader);
                return xslt;
            });

            return new XmlTransformer(xslCompiledTransform, crefResolver);
        }

        private readonly XslCompiledTransform transformer;
        private readonly XsltArgumentList arguments;

        protected XmlTransformer(XslCompiledTransform xslTtransform, ICRefResolver crefResolver)
        {
            transformer = xslTtransform ?? throw new ArgumentNullException(nameof(xslTtransform));

            arguments = new XsltArgumentList();
            arguments.AddExtensionObject(XsltExtensionNamespaceUri, new CRefResolverProxy(crefResolver));
        }

        public virtual void Transform(string xmlContent, TextWriter textWriter)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                return;

            using (var textReader = new StringReader(xmlContent))
            using (var xmlReader = XmlReader.Create(textReader))
                transformer.Transform(xmlReader, arguments, textWriter);
        }

        public string Transform(string xmlContent)
        {
            if (string.IsNullOrWhiteSpace(xmlContent))
                return string.Empty;

            using (var writer = new StringWriter())
            {
                Transform(xmlContent, writer);
                return writer.ToString();
            }
        }

        private static XmlReader CreateXmlReaderFor(string xslAssetName)
        {
            var settings = new XmlReaderSettings
            {
                CloseInput = true,
                IgnoreComments = true,
                IgnoreWhitespace = true,
            };
            return XmlReader.Create(Asset.GetStream(xslAssetName + ".xslt"), settings);
        }

        private sealed class CRefResolverProxy
        {
            private readonly ICRefResolver crefResolver;

            public CRefResolverProxy(ICRefResolver crefResolver)
            {
                this.crefResolver = crefResolver ?? throw new ArgumentNullException(nameof(crefResolver));
            }

            public string NameOf(string cref) => crefResolver.NameOf(cref);

            public string UrlOf(string cref) => crefResolver.UrlOf(cref);

            public string TrimIndent(string text) => Utils.TrimIndent(text);
        }
    }
}
