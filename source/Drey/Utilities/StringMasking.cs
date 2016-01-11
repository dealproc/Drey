using System;
using System.Text.RegularExpressions;

namespace Drey.Utilities
{
    public static class StringMasking
    {
        static Tuple<Regex, string>[] listOfMatchers = new[] {
            new Tuple<Regex, string>(new Regex("((?<=((?i:(password|pass|pwd))=))(.*?(?=;)))"), "******"),
            new Tuple<Regex, string>(new Regex("(?<=((((?i:(password|pass|pwd))|(?:['\"])(?i:(password|pass|pwd)(?:['\"]))):\\s*)))(['\"]).*?(['\"])"), "\"******\""),
        };
        public static string Passwords(string input)
        {

            var tmp = input;
            foreach (var tpl in listOfMatchers)
            {
                tmp = tpl.Item1.Replace(tmp, tpl.Item2);
            }
            return tmp;
        }
        public static Func<string> Passwords(Func<string> input)
        {
            return () => Passwords(input.Invoke());
        }
    }
}
