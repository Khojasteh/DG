using Document.Generator.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security;

namespace Document.Generator.Formatters
{
    public class HtmlFormatter : DocumentFormatter
    {
        public HtmlFormatter(TextWriter textWriter, ICRefResolver crefResolver, string stylesheet = null, bool leaveOpen = false)
            : base(textWriter, crefResolver, "HTML", leaveOpen)
        {
            Stylesheet = stylesheet;
        }

        public string Stylesheet { get; }

        protected override string Escape(string text)
        {
            return SecurityElement.Escape(text);
        }

        protected override void BeginTextStyles(TextStyles styles)
        {
            if (styles.HasFlag(TextStyles.Emphasize))
                RawText("<em>");
            if (styles.HasFlag(TextStyles.Strong))
                RawText("<strong>");
            if (styles.HasFlag(TextStyles.Teletype))
                RawText("<code>");
        }

        protected override void EndTextStyles(TextStyles styles)
        {
            if (styles.HasFlag(TextStyles.Teletype))
                RawText("</code>");
            if (styles.HasFlag(TextStyles.Strong))
                RawText("</strong>");
            if (styles.HasFlag(TextStyles.Emphasize))
                RawText("</em>");
        }

        public override void LineBreak()
        {
            RawText("<br/>");
            RawLineBreak();
        }

        public override void Image(string url, string title)
        {
            if (string.IsNullOrWhiteSpace(url))
                return;

            RawText("<img src=\"");
            RawText(EscapeUrl(url));
            RawText("\" alt=\"");
            RawText(Escape(title));
            RawText("\" title=\"");
            RawText(Escape(title));
            RawText("\">");
        }

        public override void Link(string url, Action contentFactory)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                contentFactory();
                return;
            }

            RawText("<a href=\"");
            RawText(EscapeUrl(url));
            RawText("\">");
            contentFactory();
            RawText("</a>");
        }

        public override void Header(int level, Action contentFactory, string id = null)
        {
            RawLineBreak();
            RawText("<h");
            RawText(level.ToString());
            if (!string.IsNullOrEmpty(id))
            {
                RawText(" id=\"");
                RawText(Escape(id));
                RawText("\"");
            }
            RawText(">");
            contentFactory();
            RawText("</h");
            RawText(level.ToString());
            RawText(">");
            RawLineBreak();
        }

        public override void Section(Action contentFactory)
        {
            RawLineBreak();
            RawText("<div class=\"block\">");
            contentFactory();
            RawText("</div>");
            RawLineBreak();
        }

        public override void Code(string code, string lang = null)
        {
            RawLineBreak();
            RawText("<pre class=\"prettyprint");
            if (!string.IsNullOrWhiteSpace(lang))
            {
                RawText(" lang-");
                RawText(Escape(lang));
            }
            RawText("\">");
            RawText(Escape(Utils.TrimIndent(code)));
            RawText("</pre>");
            RawLineBreak();
        }

        public override void Quote<T>(IEnumerable<T> lines, Action<T> lineFactory)
        {
            RawLineBreak();
            RawText("<blockquote>");
            RawLineBreak();
            var first = true;
            foreach (var line in lines)
            {
                if (first)
                    first = false;
                else
                    LineBreak();
                lineFactory(line);
            }
            RawLineBreak();
            RawText("</blockquote>");
            RawLineBreak();
        }

        public override void List<T>(IEnumerable<T> items, Action<T> itemFactory, bool ordered = false)
        {
            RawLineBreak();
            RawText(ordered ? "<ol>" : "<ul>");
            RawLineBreak();
            foreach (var item in items)
            {
                RawText("<li>");
                itemFactory(item);
                RawText("</li>");
                RawLineBreak();
            }
            RawText(ordered ? "</ol>" : "</ul>");
            RawLineBreak();
        }

        public override void DefinitionList<T>(IEnumerable<T> items, Action<T> termFactory, Action<T> contentFactory)
        {
            RawLineBreak();
            RawText("<dl>");
            RawLineBreak();
            foreach (var item in items)
            {
                RawText("<dt>");
                termFactory(item);
                RawText("</dt>");
                RawText("<dd>");
                contentFactory(item);
                RawText("</dd>");
            }
            RawText("</dl>");
            RawLineBreak();
        }

        public override void Table<T>(IEnumerable<object> columns, IEnumerable<T> rows, Action<T>[] columnFactories)
        {
            RawLineBreak();
            RawText("<table>");
            RawLineBreak();
            RawText("<thead>");
            RawLineBreak();
            RawText("<tr>");
            var colCount = 0;
            foreach (var col in columns)
            {
                RawText("<th>");
                Text(col);
                RawText("</th>");
                colCount++;
            }
            RawText("</tr>");
            RawLineBreak();
            RawText("</thead>");
            RawLineBreak();
            RawText("<tbody>");
            RawLineBreak();
            foreach (var row in rows)
            {
                RawText("<tr>");
                for (var col = 0; col < colCount && col < columnFactories.Length; col++)
                {
                    RawText("<td>");
                    if (col < columnFactories.Length)
                        columnFactories[col](row);
                    else
                        RawText("&nbsp;");
                    RawText("</td>");
                }
                RawText("</tr>");
                RawLineBreak();
            }
            RawText("</tbody>");
            RawLineBreak();
            RawText("</table>");
            RawLineBreak();
        }

        public override void Prologue(string title)
        {
            RawText("<!doctype html>");
            RawLineBreak();
            RawText("<html lang=\"en\">");
            RawLineBreak();
            RawText("<head>");
            RawLineBreak();
            RawText("<title>");
            RawText(Escape(title));
            RawText("</title>");
            RawLineBreak();
            RawText("<meta charset=\"UTF-8\"/>");
            RawLineBreak();
            if (!string.IsNullOrWhiteSpace(Stylesheet))
            {
                RawText("<link rel=\"stylesheet\" type=\"text/css\" href=\"");
                RawText(EscapeUrl(Stylesheet));
                RawText("\"/>");
            }
            RawText("</head>");
            RawLineBreak();
            RawText("<body>");
            RawLineBreak();
            base.Prologue(title);
        }

        public override void Epilogue()
        {
            RawLineBreak();
            RawText("<hr/>");
            RawLineBreak();
            RawText("<footer>");
            RawLineBreak();
            base.Epilogue();
            RawLineBreak();
            RawText("</footer>");
            RawLineBreak();
            RawText("<script src=\"https://cdn.rawgit.com/google/code-prettify/master/loader/run_prettify.js\" type=\"text/javascript\"></script>");
            RawLineBreak();
            RawText("</body>");
            RawLineBreak();
            RawText("</html>");
            RawLineBreak();
        }
    }
}
