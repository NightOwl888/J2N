using J2N.Globalization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Numerics
{
    public class TestHexStringParser : TestCase
    {
        [Test]
        public void Test()
        {

        }

        public class CharSequences : TestCase
        {
            public abstract class ParseSingleTestCase : TestCase
            {
                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_NumberFormatInfo_Data
                {
                    get
                    {
                        yield return new TestCaseData(3.0f, "0x1.8p1", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(3.0f, "0x1,8p1", NumberStyle.HexFloat, new NumberFormatInfo { NumberDecimalSeparator = "," });
                        yield return new TestCaseData(3.0f, "0x1--8p1", NumberStyle.HexFloat, new NumberFormatInfo { NumberDecimalSeparator = "--" });
                        yield return new TestCaseData(3.0f, "0x1.8", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);

                        // Negative sign format tests
                        yield return new TestCaseData(-3.0f, "-0x1.8p1", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-3.0f, "-0x1.8", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-2.0f, "-0x1.", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-3.0f, "- 0x1.8p1", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-3.0f, "- 0x1.8", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-2.0f, "- 0x1.", NumberStyle.HexFloat, NumberFormatInfo.InvariantInfo);

                        yield return new TestCaseData(-3.0f, "0x1.8p1-", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-3.0f, "0x1.8-", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-2.0f, "0x1.-", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-3.0f, "0x1.8p1 -", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-3.0f, "0x1.8 -", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);
                        yield return new TestCaseData(-2.0f, "0x1. -", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo);

                        //yield return new TestCaseData(-3.0f, "(0x1.8p1)", NumberStyle.HexFloat | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(-3.0f, "(0x1.8)", NumberStyle.HexFloat | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo);
                        //yield return new TestCaseData(-2.0f, "(0x1.)", NumberStyle.HexFloat | NumberStyle.AllowParentheses, NumberFormatInfo.InvariantInfo);
                    }
                }

                public static IEnumerable<TestCaseData> TestParse_CharSequence_NumberStyle_NumberFormatInfo_ForException_Data
                {
                    get
                    {
                        yield return new TestCaseData(typeof(FormatException), "-0x1.8p1", NumberStyle.HexNumber, NumberFormatInfo.InvariantInfo, "NumberStyle.HexNumber doesn't allow negative sign");

                        yield return new TestCaseData(typeof(FormatException), -3.0f, "0x1.8p1-", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "Trailing negative sign failed.");
                        yield return new TestCaseData(typeof(FormatException), -3.0f, "0x1.8-", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "Trailing negative sign failed.");
                        yield return new TestCaseData(typeof(FormatException), -2.0f, "0x1.-", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "Trailing negative sign failed.");
                        yield return new TestCaseData(typeof(FormatException), -3.0f, "0x1.8p1 -", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "Trailing negative sign failed.");
                        yield return new TestCaseData(typeof(FormatException), -3.0f, "0x1.8 -", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "Trailing negative sign failed.");
                        yield return new TestCaseData(typeof(FormatException), -2.0f, "0x1. -", NumberStyle.HexFloat | NumberStyle.AllowTrailingSign, NumberFormatInfo.InvariantInfo, "Trailing negative sign failed.");
                    }
                }
            }

            public abstract class ParseSingle_CharSequence_NumberStyle_NumberFormatInfo : ParseSingleTestCase
            {
                protected abstract float GetResult(string value, NumberStyle style, NumberFormatInfo info);

                [TestCaseSource("TestParse_CharSequence_NumberStyle_NumberFormatInfo_Data")]
                public void TestParseSingle_CharSequence_NumberStyle_NumberFormatInfo(float expected, string value, NumberStyle style, NumberFormatInfo info)
                {
                    assertEquals(expected, GetResult(value, style, info));
                }

                [TestCaseSource("TestParse_CharSequence_NumberStyle_NumberFormatInfo_ForException_Data")]
                public void TestParseSingle_CharSequence_NumberStyle_ForException_NumberFormatInfo(Type expectedExceptionType, string value, NumberStyle style, NumberFormatInfo info, string message)
                {
                    Assert.Throws(expectedExceptionType, () => GetResult(value, style, info), message);
                }
            }

            public class ParseSingle_String_NumberStyle_NumberFormatInfo : ParseSingle_CharSequence_NumberStyle_NumberFormatInfo
            {
                protected override float GetResult(string value, NumberStyle style, NumberFormatInfo info)
                {
                    return HexStringParser.ParseSingle(value, style, info);
                }
            }
        }
    }
}
