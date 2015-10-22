using System;
using System.Collections.Generic;

namespace Drey
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Wrapper function to create a foreach... loop over an IEnumerable{T}.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <param name="applyOn">The apply on.</param>
        /// <returns></returns>
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