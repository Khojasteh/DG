// Copyright (c) 2019 Kambiz Khojasteh
// Released under the MIT software license, see the accompanying
// file LICENSE.txt or http://www.opensource.org/licenses/mit-license.php.

using System;
using System.Collections.Generic;
using System.Linq;

namespace Document.Generator.Helpers
{
    public static class Utils
    {
        public static string TrimIndent(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return string.Empty;

            var lines = FindIndentationAndSplit(out var indentLen);
            for (var i = 1; i < lines.Length; i++)
                lines[i] = AdjustIndentation(lines[i]);
            return string.Join("\n", lines);

            string[] FindIndentationAndSplit(out int indent)
            {
                indent = 0;
                var firstLine = 0;
                for (var i = 0; i < text.Length && char.IsWhiteSpace(text[i]); i++)
                {
                    indent++;
                    if (text[i] == '\n')
                    {
                        firstLine = i + 1;
                        indent = 0;
                    }
                }
                return text.TrimEnd().Substring(firstLine + indent).Split('\n');
            }

            string AdjustIndentation(string s)
            {
                var count = Math.Min(s.Length, indentLen);
                for (var i = 0; i < count; i++)
                {
                    if (!char.IsWhiteSpace(s[i]))
                    {
                        if (i == 0)
                            return s;
                        count = i;
                        break;
                    }
                }
                return s.Substring(count);
            }
        }

        public static string FormatCRef(string cref)
        {
            var name = cref.Substring(2);
            var parameters = cref.StartsWith("M:") ? "()" : null;

            var i = name.IndexOfAny(new[] { '(', '[' });
            if (i != -1)
            {
                parameters = name.Substring(i).Replace(",", ", ");
                if (parameters.Contains('{'))
                    parameters = parameters.Replace('{', '<').Replace('}', '>');

                name = name.Substring(0, i);
            }

            if (name.EndsWith(".#ctor"))
                name = name.Remove(name.Length - 6);

            var foundTypeParameters = FindTypeParameters().ToList();
            if (foundTypeParameters.Count != 0)
            {
                var offset = foundTypeParameters.Where(p => !p.IsMethod).Sum(p => p.Count);
                var typeName = 'T';
                foreach (var (start, end, count, isMethod) in FindTypeParameters())
                {
                    name = name.Remove(start, end - start + 1).Insert(start, GetTypeParameters(count, typeName));
                    if (parameters != null)
                    {
                        if (isMethod)
                            ApplyMethdTypeParameters(count, typeName);
                        else
                            ApplyTypeParameters(offset -= count, count, typeName);
                    }
                    if (--typeName < 'A')
                        typeName = 'Z';
                }
            }

            return (parameters != null) ? name + parameters : name;

            string GetTypeParameters(int count, char typeName)
            {
                if (count == 1)
                    return $"<{typeName}>";
                else
                    return $"<{string.Join(", ", Enumerable.Range(1, count).Select(n => $"{typeName}{n}"))}>";
            }

            void ApplyMethdTypeParameters(int count, char typeName)
            {
                if (count == 1)
                {
                    parameters = parameters.Replace($"``0", $"{typeName}");
                }
                else
                {
                    for (var n = 0; n < count; n++)
                        parameters = parameters.Replace($"``{n}", $"{typeName}{n+1}");
                }
            }

            void ApplyTypeParameters(int start, int count, char typeName)
            {
                if (count == 1)
                {
                    parameters = parameters.Replace($"`{start}", $"{typeName}");
                }
                else
                {
                    for (var n = 0; n < count; n++)
                        parameters = parameters.Replace($"`{start + n}", $"{typeName}{n+1}");
                }
            }

            IEnumerable<(int Start, int End, int Count, bool IsMethod)> FindTypeParameters()
            {
                var startIndex = name.LastIndexOf('`');
                while (startIndex != -1)
                {
                    var endIndex = startIndex + 1;
                    while (endIndex < name.Length && char.IsDigit(name[endIndex]))
                    {
                        endIndex++;
                    }

                    var paramCount = int.Parse(name.Substring(startIndex + 1, endIndex - startIndex - 1));

                    var method = false;
                    if (name[startIndex - 1] == '`')
                    {
                        startIndex--;
                        method = true;
                    }

                    yield return (startIndex, endIndex - 1, paramCount, method);

                    startIndex = name.LastIndexOf('`', startIndex - 1);
                }
            }
        }
    }
}
