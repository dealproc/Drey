using Shouldly;

using System;
using System.Collections.Generic;

using Xunit;

namespace Drey.Tests.Utilities
{
    public class StringMaskingTests
    {
        public static IEnumerable<object[]> TestData
        {
            get
            {
                yield return new[] { "password=shouldbemasked;", "password=******;" };
                yield return new[] { "pass=shouldbemasked;", "pass=******;" };
                yield return new[] { "pwd=shouldbemasked;", "pwd=******;" };
                yield return new[] { "Password=shouldbemasked;", "Password=******;" };
                yield return new[] { "Pass=shouldbemasked;", "Pass=******;" };
                yield return new[] { "Pwd=shouldbemasked;", "Pwd=******;" };
                yield return new[] { "PASSWORD=shouldbemasked;", "PASSWORD=******;" };
                yield return new[] { "PASS=shouldbemasked;", "PASS=******;" };
                yield return new[] { "PWD=shouldbemasked;", "PWD=******;" };
                yield return new[] { "{ password: \"shouldbemasked\" }", "{ password: \"******\" }" };
                yield return new[] { "{ pass: \"shouldbemasked\" }", "{ pass: \"******\" }" };
                yield return new[] { "{ pwd: \"shouldbemasked\" }", "{ pwd: \"******\" }" };
                yield return new[] { "{ Password: \"shouldbemasked\" }", "{ Password: \"******\" }" };
                yield return new[] { "{ Pass: \"shouldbemasked\" }", "{ Pass: \"******\" }" };
                yield return new[] { "{ Pwd: \"shouldbemasked\" }", "{ Pwd: \"******\" }" };
                yield return new[] { "{ PASSWORD: \"shouldbemasked\" }", "{ PASSWORD: \"******\" }" };
                yield return new[] { "{ PASS: \"shouldbemasked\" }", "{ PASS: \"******\" }" };
                yield return new[] { "{ PWD: \"shouldbemasked\" }", "{ PWD: \"******\" }" };
                yield return new[] { "{ \"password\": \"shouldbemasked\" }", "{ \"password\": \"******\" }" };
                yield return new[] { "{ \"pass\": \"shouldbemasked\" }", "{ \"pass\": \"******\" }" };
                yield return new[] { "{ \"pwd\": \"shouldbemasked\" }", "{ \"pwd\": \"******\" }" };
                yield return new[] { "{ \"Password\": \"shouldbemasked\" }", "{ \"Password\": \"******\" }" };
                yield return new[] { "{ \"Pass\": \"shouldbemasked\" }", "{ \"Pass\": \"******\" }" };
                yield return new[] { "{ \"Pwd\": \"shouldbemasked\" }", "{ \"Pwd\": \"******\" }" };
                yield return new[] { "{ \"PASSWORD\": \"shouldbemasked\" }", "{ \"PASSWORD\": \"******\" }" };
                yield return new[] { "{ \"PASS\": \"shouldbemasked\" }", "{ \"PASS\": \"******\" }" };
                yield return new[] { "{ \"PWD\": \"shouldbemasked\" }", "{ \"PWD\": \"******\" }" };
                yield return new[] { "{ \"password\":\"shouldbemasked\" }", "{ \"password\":\"******\" }" };
            }
        }

        [Theory]
        [MemberData("TestData")]
        public void InputIsMasked(string input, string expected)
        {
            var output = Drey.Utilities.StringMasking.Passwords(input);
            output.ShouldNotContain("shouldbemasked");
            output.ShouldBe(expected);
        }

        [Theory]
        [MemberData("TestData")]
        public void ResultFromInvokingStringFunction_IsMasked(string input, string expected)
        {
            Func<string> ToMask = () => input;
            var output = Drey.Utilities.StringMasking.Passwords(ToMask).Invoke();
            output.ShouldNotContain("shouldbemasked");
            output.ShouldBe(expected);
        }
    }
}
