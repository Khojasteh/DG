using System;
using System.Collections.Generic;
using System.Text;

namespace Document.Generator.Helpers
{
    public static class StringBuilderPool
    {
        private static LinkedList<StringBuilder> storage = new LinkedList<StringBuilder>();

        public static StringBuilderProxy Acquire()
        {
            var sb = storage.Last?.Value;
            if (sb == null)
                sb = new StringBuilder(255);
            else
                storage.RemoveLast();

            return new StringBuilderProxy(sb);
        }

        public static void Release(StringBuilder sb)
        {
            if (sb != null)
            {
                sb.Length = 0;
                storage.AddLast(sb);
            }
        }

        public static void Clear()
        {
            storage.Clear();
        }

        public struct StringBuilderProxy : IDisposable
        {
            public readonly StringBuilder Instance;

            public StringBuilderProxy(StringBuilder sb) => Instance = sb;

            public void Dispose() => Release(Instance);

            public override string ToString() => Instance.ToString();

            public static implicit operator StringBuilder(StringBuilderProxy proxy) => proxy.Instance;
        }
    }
}
