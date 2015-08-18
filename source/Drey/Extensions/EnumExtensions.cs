using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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