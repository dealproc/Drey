using System;
using System.Collections.Generic;

namespace Drey.Configuration
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Apply<T>(this IEnumerable<T> items, Action<T> toApply)
        {
            foreach (var item in items)
            {
                toApply(item);
            }
            return items;
        }
    }
}