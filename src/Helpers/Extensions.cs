using Document.Generator.Models.Clr;
using System;
using System.Collections.Generic;

namespace Document.Generator.Helpers
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
                action(item);
        }

        public static void For<K, V>(this IReadOnlyDictionary<K, V> dictionary, K key, Action<V> hitAction, Action<K> missAction = null)
        {
            if (key != null && dictionary.TryGetValue(key, out var value))
                hitAction(value);
            else
                missAction?.Invoke(key);
        }

        public static string ToPluralString(this TypeCategory value)
        {
            switch (value)
            {
                case TypeCategory.Class:
                    return "Classes";
                case TypeCategory.Structure:
                    return "Structures";
                case TypeCategory.Interface:
                    return "Interfaces";
                case TypeCategory.Enumeration:
                    return "Enumerations";
                case TypeCategory.Delegate:
                    return "Delegates";
                default:
                    return string.Empty;
            }
        }

        public static string ToPluralString(this MemberCategory value)
        {
            switch (value)
            {
                case MemberCategory.Constructor:
                    return "Constructors";
                case MemberCategory.Field:
                    return "Fields";
                case MemberCategory.Property:
                    return "Properties";
                case MemberCategory.Method:
                    return "Methods";
                case MemberCategory.Event:
                    return "Events";
                case MemberCategory.Operator:
                    return "Operators";
                case MemberCategory.Type:
                    return "Nested Types";
                default:
                    return "Errors";
            }
        }
    }
}
