using System;
using System.Collections.Generic;

namespace Drey
{
    static class EnumExtensions
    {
        public static IEnumerable<T> Apply<T>(this IEnumerable<T> items, Action<T> applyOn)
        {
            foreach (var item in items)
            {
                applyOn(item);
            }
            return items;
        }
    }
}