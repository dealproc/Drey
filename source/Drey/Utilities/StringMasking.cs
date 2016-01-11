using System;
using System.Text.RegularExpressions;

namespace Drey.Utilities
{
    /// <summary>
    ///  String masking utilities.
    /// </summary>
    public static class StringMasking
    {
        static Tuple<Regex, string>[] listOfMatchers = new[] {
            new Tuple<Regex, string>(new Regex("((?<=((?i:(password|pass|pwd))=))(.*?(?=;)))"), "******"),
            new Tuple<Regex, string>(new Regex("(?<=((((?i:(password|pass|pwd))|(?:['\"])(?i:(password|pass|pwd)(?:['\"]))):\\s*)))(['\"]).*?(['\"])"), "\"******\""),
        };

        /// <summary>
        /// Masks all combination of password=; password:, "password":, etc. as to avoid recording in a log, etc.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A string, with all passwords masked.</returns>
        public static string Passwords(string input)
        {

            var tmp = input;
            foreach (var tpl in listOfMatchers)
            {
                tmp = tpl.Item1.Replace(tmp, tpl.Item2);
            }
            return tmp;
        }

        /// <summary>
        /// Masks all combination of password=; password:, "password":, etc. as to avoid recording in a log, etc.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <returns>A string, with all passwords masked.</returns>
        public static Func<string> Passwords(Func<string> input)
        {
            return () => Passwords(input.Invoke());
        }
    }
}
