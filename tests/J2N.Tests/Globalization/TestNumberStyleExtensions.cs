using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Globalization
{
    public class TestNumberStyleExtensions : TestCase
    {
        [Test]
        public void TestToNumberStyles()
        {
            // Verify that our conversion function will remove any custom J2N flags.
            var style = NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier;
            var expected = NumberStyles.Float | NumberStyles.AllowHexSpecifier;

            var actual = style.ToNumberStyles();

            assertEquals(expected, actual);

            var style2 = NumberStyle.Float | (NumberStyle)0x010000000;
            var expected2 = NumberStyles.Float;

            var actual2 = style2.ToNumberStyles();

            assertEquals(expected2, actual2);
        }

        [Test]
        public void TestToNumberStyle()
        {
            // Verify that our conversion function will remove any new .NET flags.
            // In particular, we need to ensure that if a flag with the same value as one of our custom values
            // is added in .NET, we ignore it during the conversion until it can be dealt with manually.
            var style = NumberStyles.Float | (NumberStyles)NumberStyle.AllowTypeSpecifier;
            var expected = NumberStyle.Float;

            var actual = style.ToNumberStyle();

            assertEquals(expected, actual);

            var style2 = NumberStyles.Float | (NumberStyles)0x010000000;
            var expected2 = NumberStyle.Float;

            var actual2 = style2.ToNumberStyle();

            assertEquals(expected2, actual2);
        }

        [Test]
        public void TestValidateNumberStyleInteger()
        {
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleInteger(NumberStyle.Integer));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleInteger(NumberStyle.Currency));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleInteger(NumberStyle.Float));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleInteger(NumberStyle.Number));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleInteger(NumberStyle.HexNumber));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleInteger(NumberStyle.Integer | NumberStyle.AllowTypeSpecifier));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleInteger(NumberStyle.Float | NumberStyle.AllowTypeSpecifier));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleInteger(NumberStyle.HexNumber | NumberStyle.AllowTypeSpecifier));

            var ex1 = Assert.Throws<ArgumentException>(() => NumberStyleExtensions.ValidateParseStyleInteger(NumberStyle.HexFloat));
            var ex2 = Assert.Throws<ArgumentException>(() => NumberStyleExtensions.ValidateParseStyleInteger(NumberStyle.HexNumber | NumberStyle.Currency));
        }

        [Test]
        public void TestValidateNumberStyleFloatingPoint()
        {
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.Integer));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.Currency));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.Float));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.Number));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.HexNumber));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.HexFloat));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.Integer | NumberStyle.AllowTypeSpecifier));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.Float | NumberStyle.AllowTypeSpecifier));
            Assert.DoesNotThrow(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier));

            var ex1 = Assert.Throws<ArgumentException>(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.HexFloat | NumberStyle.Currency));
            var ex2 = Assert.Throws<ArgumentException>(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.HexNumber | NumberStyle.Currency));
            var ex3 = Assert.Throws<ArgumentException>(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint((NumberStyle.HexFloat | NumberStyle.AllowTypeSpecifier) & ~NumberStyle.AllowExponent));
            var ex4 = Assert.Throws<ArgumentException>(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.HexNumber | (NumberStyle)0x010000000));
            var ex5 = Assert.Throws<ArgumentException>(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.Integer | (NumberStyle)0x010000000));
            var ex6 = Assert.Throws<ArgumentException>(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.Currency | NumberStyle.AllowTypeSpecifier));
            var ex7 = Assert.Throws<ArgumentException>(() => NumberStyleExtensions.ValidateParseStyleFloatingPoint(NumberStyle.Any | NumberStyle.AllowTypeSpecifier));
        }
    }
}
