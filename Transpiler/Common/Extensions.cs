using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Transpiler
{
    public static class Extensions
    {
        /// <summary>
        /// Gets first element that satisfies a condition, if any exists.
        /// </summary>
        public static bool Collects<T>(this IEnumerable<T> collection, Func<T, bool> condition, out T item)
        {
            item = default;
            foreach (T element in collection)
            {
                if (condition(element))
                {
                    item = element;
                    return true;
                }
            }
            return false;
        }

        public static string Separate(this IEnumerable elements, string separator)
        {
            var s = new StringBuilder();
            bool first = true;

            foreach (var e in elements)
            {
                if (!first)
                {
                    s.Append(separator);
                }
                first = false;

                object e2 = e ?? "null";
                s.Append(e2.ToString());
            }

            return s.ToString();
        }

        public static string Separate(this IEnumerable elements, string separator, string prepend = "", string append = "")
        {
            string s = Separate(elements, separator);
            if (s.Length > 0)
            {
                return prepend + s + append;
            }
            return s;
        }

        public static string Indent(int indent)
        {
            return Multiply("  ", indent);
        }

        public static string Multiply(this string s, int amount)
        {
            var res = new StringBuilder();

            for (int i = 0; i < amount; i++)
            {
                res.Append(s);
            }

            return res.ToString();
        }

        public static bool KeyForValue<T, U>(this IReadOnlyDictionary<T, U> dictionary, U value, out T key)
        {
            key = default;

            foreach (var (k, v) in dictionary)
            {
                if (value.Equals(v))
                {
                    key = k;
                    return true;
                }
            }

            return false;
        }

        public static void Foreach<T>(this IEnumerable<T> items, Action<T> action)
        {
            foreach (var item in items)
            {
                action(item);
            }
        }

        public static IReadOnlyList<T> ToArr<T>(this T item)
        {
            return new T[] { item };
        }

        public static IReadOnlyList<T> RList<T>(params T[] items)
        {
            return items;
        }
    }
}