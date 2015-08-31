using System;
using System.Collections.Generic;

namespace Drey.Configuration
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Encapsulates a foreach... loop for cleaner code.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="toApply">To apply.</param>
        /// <returns></returns>
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