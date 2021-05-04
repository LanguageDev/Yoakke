using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace Yoakke.Collections.Compatibility
{
    internal static class Extensions
    {
        public static bool TryPop<T>(this Stack<T> stack, out T value)
        {
            if (stack.Count == 0)
            {
                value = default;
                return false;
            }
            else
            {
                value = stack.Pop();
                return true;
            }
        }

        public static void Deconstruct<T1, T2>(this KeyValuePair<T1, T2> tuple, out T1 key, out T2 value)
        {
            key = tuple.Key;
            value = tuple.Value;
        }

        public static HashSet<T> ToHashSet<T>(this IEnumerable<T> values)
        {
            var result = new HashSet<T>();
            foreach (var item in values) result.Add(item);
            return result;
        }
    }
}
