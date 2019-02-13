using Document.Generator.Helpers;
using Document.Generator.Models.Xml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace Document.Generator.Formatters
{
    public class MarkdownFormatter : DocumentFormatter
    {
        private static readonly Regex EscapePattern = new Regex(@"[\\`\*_\{\}#<\-]+|(?<=\])\(", RegexOptions.Compiled);
        private static readonly Regex UrlHashCleanupPattern = new Regex(@"[^a-z0-9\-]", RegexOptions.Compiled);

        private int emittedLineBreaks = int.MaxValue;

        public MarkdownFormatter(TextWriter textWriter, ICRefResolver crefResolver, bool leaveOpen = false)
            : base(textWriter, crefResolver, "Markdown", leaveOpen) { }

        protected override string Escape(string text)
        {
            return EscapePattern.Replace(text, @"\$&");
        }

        protected override string EscapeUrl(string url)
        {
            var i = url.IndexOf('#');
            if (i != -1 && !url.Contains(':'))
            {
                return url.Substring(0, i) + '#' + CleanHash(url.Substring(i + 1));
            }

            return url;

            string CleanHash(string hash)
            {
                return UrlHashCleanupPattern.Replace(Uri.UnescapeDataString(hash).ToLowerInvariant(), 
                    m => (m.Value == " ") ? "-" : string.Empty);
            }
        }

        protected override void RawText(string text)
        {
            base.RawText(text);
            emittedLineBreaks = 0;
        }

        protected override void RawLineBreak()
        {
            if (emittedLineBreaks < 2)
            {
                base.RawLineBreak();
                emittedLineBreaks++;
            }
        }

        public override void Xml(XmlNode xml)
        {
            base.Xml(xml);
            emittedLineBreaks = 0;
        }

        protected override void BeginTextStyles(TextStyles styles)
        {
            if (styles.HasFlag(TextStyles.Strong))
                RawText("**");
            if (styles.HasFlag(TextStyles.Emphasize))
                RawText("_");
            if (styles.HasFlag(TextStyles.Teletype))
                RawText("`");
        }

        protected override void EndTextStyles(TextStyles styles)
        {
            if (styles.HasFlag(TextStyles.Teletype))
                RawText("`");
            if (styles.HasFlag(TextStyles.Emphasize))
                RawText("_");
            if (styles.HasFlag(TextStyles.Strong))
                RawText("**");
        }

        public override void LineBreak()
        {
            RawText("\\");
            RawLineBreak();
            emittedLineBreaks = 2;
        }

        public override void Image(string url, string title)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            RawText("!");
            RawText("[");
            Text(title);
            RawText("](");
            RawText(EscapeUrl(url));
            RawText(" '");
            Text(title);
            RawText("')");
        }

        public override void Link(string url, Action contentFactory)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                contentFactory();
                return;
            }

            RawText("[");
            contentFactory();
            RawText("](");
            RawText(EscapeUrl(url));
            RawText(")");
        }

        public override void Header(int level, Action contentFactory, string id)
        {
            RawLineBreak();
            RawLineBreak();
            RawText(new string('#', level));
            RawText(" ");
            contentFactory();
            RawLineBreak();
            RawLineBreak();
        }

        public override void Section(Action contentFactory)
        {
            RawLineBreak();
            RawLineBreak();
            contentFactory();
            RawLineBreak();
            RawLineBreak();
        }

        public override void Code(string code, string lang = null)
        {
            RawLineBreak();
            RawLineBreak();
            RawText("```");
            if (!string.IsNullOrWhiteSpace(lang))
                Text(lang);
            RawLineBreak();
            RawText(Utils.TrimIndent(code));
            RawLineBreak();
            RawText("```");
            RawLineBreak();
            RawLineBreak();
        }

        public override void Quote<T>(IEnumerable<T> lines, Action<T> lineFactory)
        {
            RawLineBreak();
            RawLineBreak();
            var first = true;
            foreach (var line in lines)
            {
                if (first)
                    first = false;
                else
                    LineBreak();
                RawText("> ");
                lineFactory(line);
            }
            RawLineBreak();
            RawLineBreak();
        }

        public override void List<T>(IEnumerable<T> items, Action<T> itemFactory, bool ordered = false)
        {
            RawLineBreak();
            RawLineBreak();
            var number = 0;
            foreach (var item in items)
            {
                RawText(ordered ? $"{++number}. " : "- ");
                itemFactory(item);
                RawLineBreak();
            }
            RawLineBreak();
        }

        public override void DefinitionList<T>(IEnumerable<T> items, Action<T> termFactory, Action<T> contentFactory)
        {
            foreach (var item in items)
            {
                RawLineBreak();
                RawLineBreak();
                termFactory(item);
                LineBreak();
                contentFactory(item);
                RawLineBreak();
                RawLineBreak();
            }
        }

        public override void Table<T>(IEnumerable<object> columns, IEnumerable<T> rows, params Action<T>[] columnFactories)
        {
            RawLineBreak();
            RawLineBreak();
            var colCount = 0;
            foreach (var col in columns)
            {
                if (colCount++ != 0)
                    RawText(" | ");
                Text(col);
            }
            RawLineBreak();
            for (var col = 0; col < colCount; col++)
            {
                if (col != 0)
                    RawText(" | ");
                RawText("---");
            }
            RawLineBreak();
            foreach (var row in rows)
            {
                for (var col = 0; col < colCount; col++)
                {
                    if (col != 0)
                        RawText(" | ");
                    if (col < columnFactories.Length)
                        columnFactories[col](row);
                }
                RawLineBreak();
            }
            RawLineBreak();
        }

        public override void Epilogue()
        {
            RawLineBreak();
            RawLineBreak();
            RawText("---");
            RawLineBreak();
            RawLineBreak();
            base.Epilogue();
            RawLineBreak();
        }

    }
}
