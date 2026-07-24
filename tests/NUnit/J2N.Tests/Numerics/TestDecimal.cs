using J2N.Globalization;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using static J2N.Numerics.DotNetNumber;
#nullable enable

namespace J2N.Numerics
{
    public class TestDecimal : TestCase
    {
        #region Java BigDecimal.toString() Extraction Tool

        // The following Java code was used to generate most of the data for the ToString_TestData() method below.

        /*
        public void testGenerateToStringTestData() throws Exception {

          File tempDir = new File("F:\\");

          //writeFile(new File(tempDir, "BigDecimal_ToString_CI.cs"), false);
          writeFile(new File(tempDir, "BigDecimal_ToString_All.cs"), true);

          System.out.println("Generated:");
          //System.out.println(new File(tempDir, "BigDecimal_ToString_CI.cs").getAbsolutePath());
          System.out.println(new File(tempDir, "BigDecimal_ToString_All.cs").getAbsolutePath());
        }

        private static void writeFile(File file, boolean exhaustive) throws IOException {

            BufferedWriter writer = new BufferedWriter(new FileWriter(file));

            Set<String> values = exhaustive
                ? createExhaustiveCases()
                : createCICases();

            writer.write("    //                 string value, decimal? literal, string unscaledValue, int scale, string? format, IFormatProvider? provider, string expected");
            writer.newLine();

            for (String value : values) {

                BigDecimal bd = new BigDecimal(value);

                // value, decimal value, unscaled value, scale, format, provider, expected
                writer.write(String.format(
                    "    yield return new object?[] { \"%s\", %s, \"%s\", %s, \"%s\", %s, \"%s\" };",
                    escape(value),
                    toCSharpDecimalLiteral(value, bd),
                    escape(bd.unscaledValue().toString()),
                    escape(Integer.valueOf(bd.scale()).toString()),
                    "J",
                    "null",
                    escape(bd.toString())));

                writer.newLine();
            }

            writer.close();
        }

        private static Set<String> createCICases() {

            LinkedHashSet<String> values = new LinkedHashSet<String>();

            // Harmony tests
            values.add("1234.000");
            values.add("-123.4E-5");
            values.add("-1.455E-20");
            values.add("1233456.0000");

            // zero
            values.add("0");
            values.add("0.0");
            values.add("0.00");
            values.add("-0");
            values.add("-0.0");

            // integers
            values.add("1");
            values.add("-1");
            values.add("10");
            values.add("100");
            values.add("1000");
            values.add("1000000");
            values.add("10000000");

            // trailing zeros
            values.add("1.0");
            values.add("1.00");
            values.add("1.000");
            values.add("123.4500");
            values.add("1000.0000");

            // boundary around exponent -6
            values.add("0.00001");
            values.add("0.000001");
            values.add("0.0000001");
            values.add("0.00000001");

            // leading zeros
            values.add("0.1");
            values.add("0.01");
            values.add("0.001");
            values.add("0.0001");

            // mixed
            values.add("123.45");
            values.add("-123.45");
            values.add("999999.999999");
            values.add("9999999");
            values.add("99999999");

            // decimal limits
            values.add("79228162514264337593543950335");
            values.add("-79228162514264337593543950335");
            values.add("7922816251426433759354395033.5");
            values.add("-7922816251426433759354395033.5");

            return values;
        }

        private static Set<String> createExhaustiveCases() {

            LinkedHashSet<String> values = new LinkedHashSet<String>();

            values.addAll(createCICases());

            String[] mantissas = {
                "1",
                "2",
                "5",
                "9",
                "10",
                "11",
                "99",
                "100",
                "101",
                "999",
                "1000",
                "1001",
                "12345",
                "99999",
                "123456789",
                "999999999",
                "1234567890123456789",
                "9999999999999999999",
                "12345678901234567890123456789",
                "99999999999999999999999999999"
            };

            for (String mantissa : mantissas) {

                if (significantDigits(mantissa) > 29)
                    continue;

                values.add(mantissa);
                values.add("-" + mantissa);

                for (int scale = 1; scale <= 28; scale++) {

                    if (mantissa.length() <= scale) {

                        StringBuilder sb = new StringBuilder();

                        sb.append("0.");

                        for (int i = 0; i < scale - mantissa.length(); i++)
                            sb.append('0');

                        sb.append(mantissa);

                        values.add(sb.toString());
                        values.add("-" + sb.toString());
                    }
                    else {

                        String s =
                            mantissa.substring(0, mantissa.length() - scale)
                            + "."
                            + mantissa.substring(mantissa.length() - scale);

                        values.add(s);
                        values.add("-" + s);
                    }
                }
            }

            return values;
        }

        private static int significantDigits(String s) {

            int count = 0;

            for (int i = 0; i < s.length(); i++) {

                char ch = s.charAt(i);

                if (ch >= '0' && ch <= '9')
                    count++;
            }

            return count;
        }

        private static String escape(String s) {

            return s
                .replace("\\", "\\\\")
                .replace("\"", "\\\"");
        }

        private static String toCSharpDecimalLiteral(String literal, BigDecimal value) {

          // decimal supports at most 29 significant digits
          if (value.precision() > 29)
              return "null";

          // decimal scale is 0-28
          if (value.scale() < 0 || value.scale() > 28)
              return "null";

          // .NET decimal uses a 96-bit significand.
          BigInteger maxUnscaled =
              new BigInteger("79228162514264337593543950335");

          if (value.unscaledValue().abs().compareTo(maxUnscaled) > 0)
              return "null";

          // Prefer the built-in constants when possible.
          if (value.compareTo(new BigDecimal("79228162514264337593543950335")) == 0)
              return "decimal.MaxValue";

          if (value.compareTo(new BigDecimal("-79228162514264337593543950335")) == 0)
              return "decimal.MinValue";

          // Emit the literal preserving trailing zeros.
          return literal + "m";
        }
        */

        #endregion Java BigDecimal.toString() Extraction Tool

        public static IEnumerable<object?[]> ToString_TestData()
        {
            //                 string value, decimal? literal, string unscaledValue, int scale, string? format, IFormatProvider? provider, string expected
            yield return new object?[] { "-1.455E-20", -1.455E-20m, "-1455", 23, "j", null, "-1.455e-20" };
            yield return new object?[] { "-1.455E-20", -1.455E-20m, "-1455", 23, "j", new CultureInfo("de-DE"), "-1,455e-20" };
            yield return new object?[] { "-0.00001", -0.00001m, "-1", 5, "J", new CultureInfo("de-DE"), "-0,00001" };

            var customNegativeSignDecimalGroupSeparator = new NumberFormatInfo()
            {
                NegativeSign = "#",
                NumberDecimalSeparator = "~",
                NumberGroupSeparator = "*"
            };

            yield return new object?[] { "-0.00001", -0.00001m, "-1", 5, "J", customNegativeSignDecimalGroupSeparator, "#0~00001" };
            yield return new object?[] { "-123456789012345678901234567.89", -123456789012345678901234567.89m, "-12345678901234567890123456789", 2, "J", customNegativeSignDecimalGroupSeparator, "#123456789012345678901234567~89" };

            yield return new object?[] { "-1.455E-20", -1.455E-20m, "-1455", 23, "G", null, (-1.455E-20m).ToString(CultureInfo.InvariantCulture) };
            yield return new object?[] { "-1.455E-20", -1.455E-20m, "-1455", 23, "F", null, (-1.455E-20m).ToString("F", CultureInfo.InvariantCulture) };
            yield return new object?[] { "-1.455E-20", -1.455E-20m, "-1455", 23, "E", null, (-1.455E-20m).ToString("E", CultureInfo.InvariantCulture) };

            yield return new object?[] { "-1.455E-20", -1.455E-20m, "-1455", 23, "G", new CultureInfo("de-DE"), (-1.455E-20m).ToString(new CultureInfo("de-DE")) };
            yield return new object?[] { "-1.455E-20", -1.455E-20m, "-1455", 23, "F", new CultureInfo("de-DE"), (-1.455E-20m).ToString("F", new CultureInfo("de-DE")) };
            yield return new object?[] { "-1.455E-20", -1.455E-20m, "-1455", 23, "E", new CultureInfo("de-DE"), (-1.455E-20m).ToString("E", new CultureInfo("de-DE")) };


            // The following tests were automatically generated using Java 8 (see the attachment) so we get the exact string values to verify against.
            // Note the list contains values outside of the range of decimal. This same data can later be used for a formatter based on a BigDecimal type.

            //                 string value, decimal? literal, string unscaledValue, int scale, string? format, IFormatProvider? provider, string expected
            yield return new object?[] { "1234.000", 1234.000m, "1234000", 3, "J", null, "1234.000" };
            yield return new object?[] { "-123.4E-5", -123.4E-5m, "-1234", 6, "J", null, "-0.001234" };
            yield return new object?[] { "-1.455E-20", -1.455E-20m, "-1455", 23, "J", null, "-1.455E-20" };
            yield return new object?[] { "1233456.0000", 1233456.0000m, "12334560000", 4, "J", null, "1233456.0000" };
            yield return new object?[] { "0", 0m, "0", 0, "J", null, "0" };
            yield return new object?[] { "0.0", 0.0m, "0", 1, "J", null, "0.0" };
            yield return new object?[] { "0.00", 0.00m, "0", 2, "J", null, "0.00" };
            yield return new object?[] { "-0", -0m, "0", 0, "J", null, "0" };
            yield return new object?[] { "-0.0", -0.0m, "0", 1, "J", null, "0.0" };
            yield return new object?[] { "1", 1m, "1", 0, "J", null, "1" };
            yield return new object?[] { "-1", -1m, "-1", 0, "J", null, "-1" };
            yield return new object?[] { "10", 10m, "10", 0, "J", null, "10" };
            yield return new object?[] { "100", 100m, "100", 0, "J", null, "100" };
            yield return new object?[] { "1000", 1000m, "1000", 0, "J", null, "1000" };
            yield return new object?[] { "1000000", 1000000m, "1000000", 0, "J", null, "1000000" };
            yield return new object?[] { "10000000", 10000000m, "10000000", 0, "J", null, "10000000" };
            yield return new object?[] { "1.0", 1.0m, "10", 1, "J", null, "1.0" };
            yield return new object?[] { "1.00", 1.00m, "100", 2, "J", null, "1.00" };
            yield return new object?[] { "1.000", 1.000m, "1000", 3, "J", null, "1.000" };
            yield return new object?[] { "123.4500", 123.4500m, "1234500", 4, "J", null, "123.4500" };
            yield return new object?[] { "1000.0000", 1000.0000m, "10000000", 4, "J", null, "1000.0000" };
            yield return new object?[] { "0.00001", 0.00001m, "1", 5, "J", null, "0.00001" };
            yield return new object?[] { "0.000001", 0.000001m, "1", 6, "J", null, "0.000001" };
            yield return new object?[] { "0.0000001", 0.0000001m, "1", 7, "J", null, "1E-7" };
            yield return new object?[] { "0.00000001", 0.00000001m, "1", 8, "J", null, "1E-8" };
            yield return new object?[] { "0.1", 0.1m, "1", 1, "J", null, "0.1" };
            yield return new object?[] { "0.01", 0.01m, "1", 2, "J", null, "0.01" };
            yield return new object?[] { "0.001", 0.001m, "1", 3, "J", null, "0.001" };
            yield return new object?[] { "0.0001", 0.0001m, "1", 4, "J", null, "0.0001" };
            yield return new object?[] { "123.45", 123.45m, "12345", 2, "J", null, "123.45" };
            yield return new object?[] { "-123.45", -123.45m, "-12345", 2, "J", null, "-123.45" };
            yield return new object?[] { "999999.999999", 999999.999999m, "999999999999", 6, "J", null, "999999.999999" };
            yield return new object?[] { "9999999", 9999999m, "9999999", 0, "J", null, "9999999" };
            yield return new object?[] { "99999999", 99999999m, "99999999", 0, "J", null, "99999999" };
            yield return new object?[] { "79228162514264337593543950335", decimal.MaxValue, "79228162514264337593543950335", 0, "J", null, "79228162514264337593543950335" };
            yield return new object?[] { "-79228162514264337593543950335", decimal.MinValue, "-79228162514264337593543950335", 0, "J", null, "-79228162514264337593543950335" };
            yield return new object?[] { "7922816251426433759354395033.5", 7922816251426433759354395033.5m, "79228162514264337593543950335", 1, "J", null, "7922816251426433759354395033.5" };
            yield return new object?[] { "-7922816251426433759354395033.5", -7922816251426433759354395033.5m, "-79228162514264337593543950335", 1, "J", null, "-7922816251426433759354395033.5" };
            yield return new object?[] { "-0.1", -0.1m, "-1", 1, "J", null, "-0.1" };
            yield return new object?[] { "-0.01", -0.01m, "-1", 2, "J", null, "-0.01" };
            yield return new object?[] { "-0.001", -0.001m, "-1", 3, "J", null, "-0.001" };
            yield return new object?[] { "-0.0001", -0.0001m, "-1", 4, "J", null, "-0.0001" };
            yield return new object?[] { "-0.00001", -0.00001m, "-1", 5, "J", null, "-0.00001" };
            yield return new object?[] { "-0.000001", -0.000001m, "-1", 6, "J", null, "-0.000001" };
            yield return new object?[] { "-0.0000001", -0.0000001m, "-1", 7, "J", null, "-1E-7" };
            yield return new object?[] { "-0.00000001", -0.00000001m, "-1", 8, "J", null, "-1E-8" };
            yield return new object?[] { "0.000000001", 0.000000001m, "1", 9, "J", null, "1E-9" };
            yield return new object?[] { "-0.000000001", -0.000000001m, "-1", 9, "J", null, "-1E-9" };
            yield return new object?[] { "0.0000000001", 0.0000000001m, "1", 10, "J", null, "1E-10" };
            yield return new object?[] { "-0.0000000001", -0.0000000001m, "-1", 10, "J", null, "-1E-10" };
            yield return new object?[] { "0.00000000001", 0.00000000001m, "1", 11, "J", null, "1E-11" };
            yield return new object?[] { "-0.00000000001", -0.00000000001m, "-1", 11, "J", null, "-1E-11" };
            yield return new object?[] { "0.000000000001", 0.000000000001m, "1", 12, "J", null, "1E-12" };
            yield return new object?[] { "-0.000000000001", -0.000000000001m, "-1", 12, "J", null, "-1E-12" };
            yield return new object?[] { "0.0000000000001", 0.0000000000001m, "1", 13, "J", null, "1E-13" };
            yield return new object?[] { "-0.0000000000001", -0.0000000000001m, "-1", 13, "J", null, "-1E-13" };
            yield return new object?[] { "0.00000000000001", 0.00000000000001m, "1", 14, "J", null, "1E-14" };
            yield return new object?[] { "-0.00000000000001", -0.00000000000001m, "-1", 14, "J", null, "-1E-14" };
            yield return new object?[] { "0.000000000000001", 0.000000000000001m, "1", 15, "J", null, "1E-15" };
            yield return new object?[] { "-0.000000000000001", -0.000000000000001m, "-1", 15, "J", null, "-1E-15" };
            yield return new object?[] { "0.0000000000000001", 0.0000000000000001m, "1", 16, "J", null, "1E-16" };
            yield return new object?[] { "-0.0000000000000001", -0.0000000000000001m, "-1", 16, "J", null, "-1E-16" };
            yield return new object?[] { "0.00000000000000001", 0.00000000000000001m, "1", 17, "J", null, "1E-17" };
            yield return new object?[] { "-0.00000000000000001", -0.00000000000000001m, "-1", 17, "J", null, "-1E-17" };
            yield return new object?[] { "0.000000000000000001", 0.000000000000000001m, "1", 18, "J", null, "1E-18" };
            yield return new object?[] { "-0.000000000000000001", -0.000000000000000001m, "-1", 18, "J", null, "-1E-18" };
            yield return new object?[] { "0.0000000000000000001", 0.0000000000000000001m, "1", 19, "J", null, "1E-19" };
            yield return new object?[] { "-0.0000000000000000001", -0.0000000000000000001m, "-1", 19, "J", null, "-1E-19" };
            yield return new object?[] { "0.00000000000000000001", 0.00000000000000000001m, "1", 20, "J", null, "1E-20" };
            yield return new object?[] { "-0.00000000000000000001", -0.00000000000000000001m, "-1", 20, "J", null, "-1E-20" };
            yield return new object?[] { "0.000000000000000000001", 0.000000000000000000001m, "1", 21, "J", null, "1E-21" };
            yield return new object?[] { "-0.000000000000000000001", -0.000000000000000000001m, "-1", 21, "J", null, "-1E-21" };
            yield return new object?[] { "0.0000000000000000000001", 0.0000000000000000000001m, "1", 22, "J", null, "1E-22" };
            yield return new object?[] { "-0.0000000000000000000001", -0.0000000000000000000001m, "-1", 22, "J", null, "-1E-22" };
            yield return new object?[] { "0.00000000000000000000001", 0.00000000000000000000001m, "1", 23, "J", null, "1E-23" };
            yield return new object?[] { "-0.00000000000000000000001", -0.00000000000000000000001m, "-1", 23, "J", null, "-1E-23" };
            yield return new object?[] { "0.000000000000000000000001", 0.000000000000000000000001m, "1", 24, "J", null, "1E-24" };
            yield return new object?[] { "-0.000000000000000000000001", -0.000000000000000000000001m, "-1", 24, "J", null, "-1E-24" };
            yield return new object?[] { "0.0000000000000000000000001", 0.0000000000000000000000001m, "1", 25, "J", null, "1E-25" };
            yield return new object?[] { "-0.0000000000000000000000001", -0.0000000000000000000000001m, "-1", 25, "J", null, "-1E-25" };
            yield return new object?[] { "0.00000000000000000000000001", 0.00000000000000000000000001m, "1", 26, "J", null, "1E-26" };
            yield return new object?[] { "-0.00000000000000000000000001", -0.00000000000000000000000001m, "-1", 26, "J", null, "-1E-26" };
            yield return new object?[] { "0.000000000000000000000000001", 0.000000000000000000000000001m, "1", 27, "J", null, "1E-27" };
            yield return new object?[] { "-0.000000000000000000000000001", -0.000000000000000000000000001m, "-1", 27, "J", null, "-1E-27" };
            yield return new object?[] { "0.0000000000000000000000000001", 0.0000000000000000000000000001m, "1", 28, "J", null, "1E-28" };
            yield return new object?[] { "-0.0000000000000000000000000001", -0.0000000000000000000000000001m, "-1", 28, "J", null, "-1E-28" };
            yield return new object?[] { "2", 2m, "2", 0, "J", null, "2" };
            yield return new object?[] { "-2", -2m, "-2", 0, "J", null, "-2" };
            yield return new object?[] { "0.2", 0.2m, "2", 1, "J", null, "0.2" };
            yield return new object?[] { "-0.2", -0.2m, "-2", 1, "J", null, "-0.2" };
            yield return new object?[] { "0.02", 0.02m, "2", 2, "J", null, "0.02" };
            yield return new object?[] { "-0.02", -0.02m, "-2", 2, "J", null, "-0.02" };
            yield return new object?[] { "0.002", 0.002m, "2", 3, "J", null, "0.002" };
            yield return new object?[] { "-0.002", -0.002m, "-2", 3, "J", null, "-0.002" };
            yield return new object?[] { "0.0002", 0.0002m, "2", 4, "J", null, "0.0002" };
            yield return new object?[] { "-0.0002", -0.0002m, "-2", 4, "J", null, "-0.0002" };
            yield return new object?[] { "0.00002", 0.00002m, "2", 5, "J", null, "0.00002" };
            yield return new object?[] { "-0.00002", -0.00002m, "-2", 5, "J", null, "-0.00002" };
            yield return new object?[] { "0.000002", 0.000002m, "2", 6, "J", null, "0.000002" };
            yield return new object?[] { "-0.000002", -0.000002m, "-2", 6, "J", null, "-0.000002" };
            yield return new object?[] { "0.0000002", 0.0000002m, "2", 7, "J", null, "2E-7" };
            yield return new object?[] { "-0.0000002", -0.0000002m, "-2", 7, "J", null, "-2E-7" };
            yield return new object?[] { "0.00000002", 0.00000002m, "2", 8, "J", null, "2E-8" };
            yield return new object?[] { "-0.00000002", -0.00000002m, "-2", 8, "J", null, "-2E-8" };
            yield return new object?[] { "0.000000002", 0.000000002m, "2", 9, "J", null, "2E-9" };
            yield return new object?[] { "-0.000000002", -0.000000002m, "-2", 9, "J", null, "-2E-9" };
            yield return new object?[] { "0.0000000002", 0.0000000002m, "2", 10, "J", null, "2E-10" };
            yield return new object?[] { "-0.0000000002", -0.0000000002m, "-2", 10, "J", null, "-2E-10" };
            yield return new object?[] { "0.00000000002", 0.00000000002m, "2", 11, "J", null, "2E-11" };
            yield return new object?[] { "-0.00000000002", -0.00000000002m, "-2", 11, "J", null, "-2E-11" };
            yield return new object?[] { "0.000000000002", 0.000000000002m, "2", 12, "J", null, "2E-12" };
            yield return new object?[] { "-0.000000000002", -0.000000000002m, "-2", 12, "J", null, "-2E-12" };
            yield return new object?[] { "0.0000000000002", 0.0000000000002m, "2", 13, "J", null, "2E-13" };
            yield return new object?[] { "-0.0000000000002", -0.0000000000002m, "-2", 13, "J", null, "-2E-13" };
            yield return new object?[] { "0.00000000000002", 0.00000000000002m, "2", 14, "J", null, "2E-14" };
            yield return new object?[] { "-0.00000000000002", -0.00000000000002m, "-2", 14, "J", null, "-2E-14" };
            yield return new object?[] { "0.000000000000002", 0.000000000000002m, "2", 15, "J", null, "2E-15" };
            yield return new object?[] { "-0.000000000000002", -0.000000000000002m, "-2", 15, "J", null, "-2E-15" };
            yield return new object?[] { "0.0000000000000002", 0.0000000000000002m, "2", 16, "J", null, "2E-16" };
            yield return new object?[] { "-0.0000000000000002", -0.0000000000000002m, "-2", 16, "J", null, "-2E-16" };
            yield return new object?[] { "0.00000000000000002", 0.00000000000000002m, "2", 17, "J", null, "2E-17" };
            yield return new object?[] { "-0.00000000000000002", -0.00000000000000002m, "-2", 17, "J", null, "-2E-17" };
            yield return new object?[] { "0.000000000000000002", 0.000000000000000002m, "2", 18, "J", null, "2E-18" };
            yield return new object?[] { "-0.000000000000000002", -0.000000000000000002m, "-2", 18, "J", null, "-2E-18" };
            yield return new object?[] { "0.0000000000000000002", 0.0000000000000000002m, "2", 19, "J", null, "2E-19" };
            yield return new object?[] { "-0.0000000000000000002", -0.0000000000000000002m, "-2", 19, "J", null, "-2E-19" };
            yield return new object?[] { "0.00000000000000000002", 0.00000000000000000002m, "2", 20, "J", null, "2E-20" };
            yield return new object?[] { "-0.00000000000000000002", -0.00000000000000000002m, "-2", 20, "J", null, "-2E-20" };
            yield return new object?[] { "0.000000000000000000002", 0.000000000000000000002m, "2", 21, "J", null, "2E-21" };
            yield return new object?[] { "-0.000000000000000000002", -0.000000000000000000002m, "-2", 21, "J", null, "-2E-21" };
            yield return new object?[] { "0.0000000000000000000002", 0.0000000000000000000002m, "2", 22, "J", null, "2E-22" };
            yield return new object?[] { "-0.0000000000000000000002", -0.0000000000000000000002m, "-2", 22, "J", null, "-2E-22" };
            yield return new object?[] { "0.00000000000000000000002", 0.00000000000000000000002m, "2", 23, "J", null, "2E-23" };
            yield return new object?[] { "-0.00000000000000000000002", -0.00000000000000000000002m, "-2", 23, "J", null, "-2E-23" };
            yield return new object?[] { "0.000000000000000000000002", 0.000000000000000000000002m, "2", 24, "J", null, "2E-24" };
            yield return new object?[] { "-0.000000000000000000000002", -0.000000000000000000000002m, "-2", 24, "J", null, "-2E-24" };
            yield return new object?[] { "0.0000000000000000000000002", 0.0000000000000000000000002m, "2", 25, "J", null, "2E-25" };
            yield return new object?[] { "-0.0000000000000000000000002", -0.0000000000000000000000002m, "-2", 25, "J", null, "-2E-25" };
            yield return new object?[] { "0.00000000000000000000000002", 0.00000000000000000000000002m, "2", 26, "J", null, "2E-26" };
            yield return new object?[] { "-0.00000000000000000000000002", -0.00000000000000000000000002m, "-2", 26, "J", null, "-2E-26" };
            yield return new object?[] { "0.000000000000000000000000002", 0.000000000000000000000000002m, "2", 27, "J", null, "2E-27" };
            yield return new object?[] { "-0.000000000000000000000000002", -0.000000000000000000000000002m, "-2", 27, "J", null, "-2E-27" };
            yield return new object?[] { "0.0000000000000000000000000002", 0.0000000000000000000000000002m, "2", 28, "J", null, "2E-28" };
            yield return new object?[] { "-0.0000000000000000000000000002", -0.0000000000000000000000000002m, "-2", 28, "J", null, "-2E-28" };
            yield return new object?[] { "5", 5m, "5", 0, "J", null, "5" };
            yield return new object?[] { "-5", -5m, "-5", 0, "J", null, "-5" };
            yield return new object?[] { "0.5", 0.5m, "5", 1, "J", null, "0.5" };
            yield return new object?[] { "-0.5", -0.5m, "-5", 1, "J", null, "-0.5" };
            yield return new object?[] { "0.05", 0.05m, "5", 2, "J", null, "0.05" };
            yield return new object?[] { "-0.05", -0.05m, "-5", 2, "J", null, "-0.05" };
            yield return new object?[] { "0.005", 0.005m, "5", 3, "J", null, "0.005" };
            yield return new object?[] { "-0.005", -0.005m, "-5", 3, "J", null, "-0.005" };
            yield return new object?[] { "0.0005", 0.0005m, "5", 4, "J", null, "0.0005" };
            yield return new object?[] { "-0.0005", -0.0005m, "-5", 4, "J", null, "-0.0005" };
            yield return new object?[] { "0.00005", 0.00005m, "5", 5, "J", null, "0.00005" };
            yield return new object?[] { "-0.00005", -0.00005m, "-5", 5, "J", null, "-0.00005" };
            yield return new object?[] { "0.000005", 0.000005m, "5", 6, "J", null, "0.000005" };
            yield return new object?[] { "-0.000005", -0.000005m, "-5", 6, "J", null, "-0.000005" };
            yield return new object?[] { "0.0000005", 0.0000005m, "5", 7, "J", null, "5E-7" };
            yield return new object?[] { "-0.0000005", -0.0000005m, "-5", 7, "J", null, "-5E-7" };
            yield return new object?[] { "0.00000005", 0.00000005m, "5", 8, "J", null, "5E-8" };
            yield return new object?[] { "-0.00000005", -0.00000005m, "-5", 8, "J", null, "-5E-8" };
            yield return new object?[] { "0.000000005", 0.000000005m, "5", 9, "J", null, "5E-9" };
            yield return new object?[] { "-0.000000005", -0.000000005m, "-5", 9, "J", null, "-5E-9" };
            yield return new object?[] { "0.0000000005", 0.0000000005m, "5", 10, "J", null, "5E-10" };
            yield return new object?[] { "-0.0000000005", -0.0000000005m, "-5", 10, "J", null, "-5E-10" };
            yield return new object?[] { "0.00000000005", 0.00000000005m, "5", 11, "J", null, "5E-11" };
            yield return new object?[] { "-0.00000000005", -0.00000000005m, "-5", 11, "J", null, "-5E-11" };
            yield return new object?[] { "0.000000000005", 0.000000000005m, "5", 12, "J", null, "5E-12" };
            yield return new object?[] { "-0.000000000005", -0.000000000005m, "-5", 12, "J", null, "-5E-12" };
            yield return new object?[] { "0.0000000000005", 0.0000000000005m, "5", 13, "J", null, "5E-13" };
            yield return new object?[] { "-0.0000000000005", -0.0000000000005m, "-5", 13, "J", null, "-5E-13" };
            yield return new object?[] { "0.00000000000005", 0.00000000000005m, "5", 14, "J", null, "5E-14" };
            yield return new object?[] { "-0.00000000000005", -0.00000000000005m, "-5", 14, "J", null, "-5E-14" };
            yield return new object?[] { "0.000000000000005", 0.000000000000005m, "5", 15, "J", null, "5E-15" };
            yield return new object?[] { "-0.000000000000005", -0.000000000000005m, "-5", 15, "J", null, "-5E-15" };
            yield return new object?[] { "0.0000000000000005", 0.0000000000000005m, "5", 16, "J", null, "5E-16" };
            yield return new object?[] { "-0.0000000000000005", -0.0000000000000005m, "-5", 16, "J", null, "-5E-16" };
            yield return new object?[] { "0.00000000000000005", 0.00000000000000005m, "5", 17, "J", null, "5E-17" };
            yield return new object?[] { "-0.00000000000000005", -0.00000000000000005m, "-5", 17, "J", null, "-5E-17" };
            yield return new object?[] { "0.000000000000000005", 0.000000000000000005m, "5", 18, "J", null, "5E-18" };
            yield return new object?[] { "-0.000000000000000005", -0.000000000000000005m, "-5", 18, "J", null, "-5E-18" };
            yield return new object?[] { "0.0000000000000000005", 0.0000000000000000005m, "5", 19, "J", null, "5E-19" };
            yield return new object?[] { "-0.0000000000000000005", -0.0000000000000000005m, "-5", 19, "J", null, "-5E-19" };
            yield return new object?[] { "0.00000000000000000005", 0.00000000000000000005m, "5", 20, "J", null, "5E-20" };
            yield return new object?[] { "-0.00000000000000000005", -0.00000000000000000005m, "-5", 20, "J", null, "-5E-20" };
            yield return new object?[] { "0.000000000000000000005", 0.000000000000000000005m, "5", 21, "J", null, "5E-21" };
            yield return new object?[] { "-0.000000000000000000005", -0.000000000000000000005m, "-5", 21, "J", null, "-5E-21" };
            yield return new object?[] { "0.0000000000000000000005", 0.0000000000000000000005m, "5", 22, "J", null, "5E-22" };
            yield return new object?[] { "-0.0000000000000000000005", -0.0000000000000000000005m, "-5", 22, "J", null, "-5E-22" };
            yield return new object?[] { "0.00000000000000000000005", 0.00000000000000000000005m, "5", 23, "J", null, "5E-23" };
            yield return new object?[] { "-0.00000000000000000000005", -0.00000000000000000000005m, "-5", 23, "J", null, "-5E-23" };
            yield return new object?[] { "0.000000000000000000000005", 0.000000000000000000000005m, "5", 24, "J", null, "5E-24" };
            yield return new object?[] { "-0.000000000000000000000005", -0.000000000000000000000005m, "-5", 24, "J", null, "-5E-24" };
            yield return new object?[] { "0.0000000000000000000000005", 0.0000000000000000000000005m, "5", 25, "J", null, "5E-25" };
            yield return new object?[] { "-0.0000000000000000000000005", -0.0000000000000000000000005m, "-5", 25, "J", null, "-5E-25" };
            yield return new object?[] { "0.00000000000000000000000005", 0.00000000000000000000000005m, "5", 26, "J", null, "5E-26" };
            yield return new object?[] { "-0.00000000000000000000000005", -0.00000000000000000000000005m, "-5", 26, "J", null, "-5E-26" };
            yield return new object?[] { "0.000000000000000000000000005", 0.000000000000000000000000005m, "5", 27, "J", null, "5E-27" };
            yield return new object?[] { "-0.000000000000000000000000005", -0.000000000000000000000000005m, "-5", 27, "J", null, "-5E-27" };
            yield return new object?[] { "0.0000000000000000000000000005", 0.0000000000000000000000000005m, "5", 28, "J", null, "5E-28" };
            yield return new object?[] { "-0.0000000000000000000000000005", -0.0000000000000000000000000005m, "-5", 28, "J", null, "-5E-28" };
            yield return new object?[] { "9", 9m, "9", 0, "J", null, "9" };
            yield return new object?[] { "-9", -9m, "-9", 0, "J", null, "-9" };
            yield return new object?[] { "0.9", 0.9m, "9", 1, "J", null, "0.9" };
            yield return new object?[] { "-0.9", -0.9m, "-9", 1, "J", null, "-0.9" };
            yield return new object?[] { "0.09", 0.09m, "9", 2, "J", null, "0.09" };
            yield return new object?[] { "-0.09", -0.09m, "-9", 2, "J", null, "-0.09" };
            yield return new object?[] { "0.009", 0.009m, "9", 3, "J", null, "0.009" };
            yield return new object?[] { "-0.009", -0.009m, "-9", 3, "J", null, "-0.009" };
            yield return new object?[] { "0.0009", 0.0009m, "9", 4, "J", null, "0.0009" };
            yield return new object?[] { "-0.0009", -0.0009m, "-9", 4, "J", null, "-0.0009" };
            yield return new object?[] { "0.00009", 0.00009m, "9", 5, "J", null, "0.00009" };
            yield return new object?[] { "-0.00009", -0.00009m, "-9", 5, "J", null, "-0.00009" };
            yield return new object?[] { "0.000009", 0.000009m, "9", 6, "J", null, "0.000009" };
            yield return new object?[] { "-0.000009", -0.000009m, "-9", 6, "J", null, "-0.000009" };
            yield return new object?[] { "0.0000009", 0.0000009m, "9", 7, "J", null, "9E-7" };
            yield return new object?[] { "-0.0000009", -0.0000009m, "-9", 7, "J", null, "-9E-7" };
            yield return new object?[] { "0.00000009", 0.00000009m, "9", 8, "J", null, "9E-8" };
            yield return new object?[] { "-0.00000009", -0.00000009m, "-9", 8, "J", null, "-9E-8" };
            yield return new object?[] { "0.000000009", 0.000000009m, "9", 9, "J", null, "9E-9" };
            yield return new object?[] { "-0.000000009", -0.000000009m, "-9", 9, "J", null, "-9E-9" };
            yield return new object?[] { "0.0000000009", 0.0000000009m, "9", 10, "J", null, "9E-10" };
            yield return new object?[] { "-0.0000000009", -0.0000000009m, "-9", 10, "J", null, "-9E-10" };
            yield return new object?[] { "0.00000000009", 0.00000000009m, "9", 11, "J", null, "9E-11" };
            yield return new object?[] { "-0.00000000009", -0.00000000009m, "-9", 11, "J", null, "-9E-11" };
            yield return new object?[] { "0.000000000009", 0.000000000009m, "9", 12, "J", null, "9E-12" };
            yield return new object?[] { "-0.000000000009", -0.000000000009m, "-9", 12, "J", null, "-9E-12" };
            yield return new object?[] { "0.0000000000009", 0.0000000000009m, "9", 13, "J", null, "9E-13" };
            yield return new object?[] { "-0.0000000000009", -0.0000000000009m, "-9", 13, "J", null, "-9E-13" };
            yield return new object?[] { "0.00000000000009", 0.00000000000009m, "9", 14, "J", null, "9E-14" };
            yield return new object?[] { "-0.00000000000009", -0.00000000000009m, "-9", 14, "J", null, "-9E-14" };
            yield return new object?[] { "0.000000000000009", 0.000000000000009m, "9", 15, "J", null, "9E-15" };
            yield return new object?[] { "-0.000000000000009", -0.000000000000009m, "-9", 15, "J", null, "-9E-15" };
            yield return new object?[] { "0.0000000000000009", 0.0000000000000009m, "9", 16, "J", null, "9E-16" };
            yield return new object?[] { "-0.0000000000000009", -0.0000000000000009m, "-9", 16, "J", null, "-9E-16" };
            yield return new object?[] { "0.00000000000000009", 0.00000000000000009m, "9", 17, "J", null, "9E-17" };
            yield return new object?[] { "-0.00000000000000009", -0.00000000000000009m, "-9", 17, "J", null, "-9E-17" };
            yield return new object?[] { "0.000000000000000009", 0.000000000000000009m, "9", 18, "J", null, "9E-18" };
            yield return new object?[] { "-0.000000000000000009", -0.000000000000000009m, "-9", 18, "J", null, "-9E-18" };
            yield return new object?[] { "0.0000000000000000009", 0.0000000000000000009m, "9", 19, "J", null, "9E-19" };
            yield return new object?[] { "-0.0000000000000000009", -0.0000000000000000009m, "-9", 19, "J", null, "-9E-19" };
            yield return new object?[] { "0.00000000000000000009", 0.00000000000000000009m, "9", 20, "J", null, "9E-20" };
            yield return new object?[] { "-0.00000000000000000009", -0.00000000000000000009m, "-9", 20, "J", null, "-9E-20" };
            yield return new object?[] { "0.000000000000000000009", 0.000000000000000000009m, "9", 21, "J", null, "9E-21" };
            yield return new object?[] { "-0.000000000000000000009", -0.000000000000000000009m, "-9", 21, "J", null, "-9E-21" };
            yield return new object?[] { "0.0000000000000000000009", 0.0000000000000000000009m, "9", 22, "J", null, "9E-22" };
            yield return new object?[] { "-0.0000000000000000000009", -0.0000000000000000000009m, "-9", 22, "J", null, "-9E-22" };
            yield return new object?[] { "0.00000000000000000000009", 0.00000000000000000000009m, "9", 23, "J", null, "9E-23" };
            yield return new object?[] { "-0.00000000000000000000009", -0.00000000000000000000009m, "-9", 23, "J", null, "-9E-23" };
            yield return new object?[] { "0.000000000000000000000009", 0.000000000000000000000009m, "9", 24, "J", null, "9E-24" };
            yield return new object?[] { "-0.000000000000000000000009", -0.000000000000000000000009m, "-9", 24, "J", null, "-9E-24" };
            yield return new object?[] { "0.0000000000000000000000009", 0.0000000000000000000000009m, "9", 25, "J", null, "9E-25" };
            yield return new object?[] { "-0.0000000000000000000000009", -0.0000000000000000000000009m, "-9", 25, "J", null, "-9E-25" };
            yield return new object?[] { "0.00000000000000000000000009", 0.00000000000000000000000009m, "9", 26, "J", null, "9E-26" };
            yield return new object?[] { "-0.00000000000000000000000009", -0.00000000000000000000000009m, "-9", 26, "J", null, "-9E-26" };
            yield return new object?[] { "0.000000000000000000000000009", 0.000000000000000000000000009m, "9", 27, "J", null, "9E-27" };
            yield return new object?[] { "-0.000000000000000000000000009", -0.000000000000000000000000009m, "-9", 27, "J", null, "-9E-27" };
            yield return new object?[] { "0.0000000000000000000000000009", 0.0000000000000000000000000009m, "9", 28, "J", null, "9E-28" };
            yield return new object?[] { "-0.0000000000000000000000000009", -0.0000000000000000000000000009m, "-9", 28, "J", null, "-9E-28" };
            yield return new object?[] { "-10", -10m, "-10", 0, "J", null, "-10" };
            yield return new object?[] { "-1.0", -1.0m, "-10", 1, "J", null, "-1.0" };
            yield return new object?[] { "0.10", 0.10m, "10", 2, "J", null, "0.10" };
            yield return new object?[] { "-0.10", -0.10m, "-10", 2, "J", null, "-0.10" };
            yield return new object?[] { "0.010", 0.010m, "10", 3, "J", null, "0.010" };
            yield return new object?[] { "-0.010", -0.010m, "-10", 3, "J", null, "-0.010" };
            yield return new object?[] { "0.0010", 0.0010m, "10", 4, "J", null, "0.0010" };
            yield return new object?[] { "-0.0010", -0.0010m, "-10", 4, "J", null, "-0.0010" };
            yield return new object?[] { "0.00010", 0.00010m, "10", 5, "J", null, "0.00010" };
            yield return new object?[] { "-0.00010", -0.00010m, "-10", 5, "J", null, "-0.00010" };
            yield return new object?[] { "0.000010", 0.000010m, "10", 6, "J", null, "0.000010" };
            yield return new object?[] { "-0.000010", -0.000010m, "-10", 6, "J", null, "-0.000010" };
            yield return new object?[] { "0.0000010", 0.0000010m, "10", 7, "J", null, "0.0000010" };
            yield return new object?[] { "-0.0000010", -0.0000010m, "-10", 7, "J", null, "-0.0000010" };
            yield return new object?[] { "0.00000010", 0.00000010m, "10", 8, "J", null, "1.0E-7" };
            yield return new object?[] { "-0.00000010", -0.00000010m, "-10", 8, "J", null, "-1.0E-7" };
            yield return new object?[] { "0.000000010", 0.000000010m, "10", 9, "J", null, "1.0E-8" };
            yield return new object?[] { "-0.000000010", -0.000000010m, "-10", 9, "J", null, "-1.0E-8" };
            yield return new object?[] { "0.0000000010", 0.0000000010m, "10", 10, "J", null, "1.0E-9" };
            yield return new object?[] { "-0.0000000010", -0.0000000010m, "-10", 10, "J", null, "-1.0E-9" };
            yield return new object?[] { "0.00000000010", 0.00000000010m, "10", 11, "J", null, "1.0E-10" };
            yield return new object?[] { "-0.00000000010", -0.00000000010m, "-10", 11, "J", null, "-1.0E-10" };
            yield return new object?[] { "0.000000000010", 0.000000000010m, "10", 12, "J", null, "1.0E-11" };
            yield return new object?[] { "-0.000000000010", -0.000000000010m, "-10", 12, "J", null, "-1.0E-11" };
            yield return new object?[] { "0.0000000000010", 0.0000000000010m, "10", 13, "J", null, "1.0E-12" };
            yield return new object?[] { "-0.0000000000010", -0.0000000000010m, "-10", 13, "J", null, "-1.0E-12" };
            yield return new object?[] { "0.00000000000010", 0.00000000000010m, "10", 14, "J", null, "1.0E-13" };
            yield return new object?[] { "-0.00000000000010", -0.00000000000010m, "-10", 14, "J", null, "-1.0E-13" };
            yield return new object?[] { "0.000000000000010", 0.000000000000010m, "10", 15, "J", null, "1.0E-14" };
            yield return new object?[] { "-0.000000000000010", -0.000000000000010m, "-10", 15, "J", null, "-1.0E-14" };
            yield return new object?[] { "0.0000000000000010", 0.0000000000000010m, "10", 16, "J", null, "1.0E-15" };
            yield return new object?[] { "-0.0000000000000010", -0.0000000000000010m, "-10", 16, "J", null, "-1.0E-15" };
            yield return new object?[] { "0.00000000000000010", 0.00000000000000010m, "10", 17, "J", null, "1.0E-16" };
            yield return new object?[] { "-0.00000000000000010", -0.00000000000000010m, "-10", 17, "J", null, "-1.0E-16" };
            yield return new object?[] { "0.000000000000000010", 0.000000000000000010m, "10", 18, "J", null, "1.0E-17" };
            yield return new object?[] { "-0.000000000000000010", -0.000000000000000010m, "-10", 18, "J", null, "-1.0E-17" };
            yield return new object?[] { "0.0000000000000000010", 0.0000000000000000010m, "10", 19, "J", null, "1.0E-18" };
            yield return new object?[] { "-0.0000000000000000010", -0.0000000000000000010m, "-10", 19, "J", null, "-1.0E-18" };
            yield return new object?[] { "0.00000000000000000010", 0.00000000000000000010m, "10", 20, "J", null, "1.0E-19" };
            yield return new object?[] { "-0.00000000000000000010", -0.00000000000000000010m, "-10", 20, "J", null, "-1.0E-19" };
            yield return new object?[] { "0.000000000000000000010", 0.000000000000000000010m, "10", 21, "J", null, "1.0E-20" };
            yield return new object?[] { "-0.000000000000000000010", -0.000000000000000000010m, "-10", 21, "J", null, "-1.0E-20" };
            yield return new object?[] { "0.0000000000000000000010", 0.0000000000000000000010m, "10", 22, "J", null, "1.0E-21" };
            yield return new object?[] { "-0.0000000000000000000010", -0.0000000000000000000010m, "-10", 22, "J", null, "-1.0E-21" };
            yield return new object?[] { "0.00000000000000000000010", 0.00000000000000000000010m, "10", 23, "J", null, "1.0E-22" };
            yield return new object?[] { "-0.00000000000000000000010", -0.00000000000000000000010m, "-10", 23, "J", null, "-1.0E-22" };
            yield return new object?[] { "0.000000000000000000000010", 0.000000000000000000000010m, "10", 24, "J", null, "1.0E-23" };
            yield return new object?[] { "-0.000000000000000000000010", -0.000000000000000000000010m, "-10", 24, "J", null, "-1.0E-23" };
            yield return new object?[] { "0.0000000000000000000000010", 0.0000000000000000000000010m, "10", 25, "J", null, "1.0E-24" };
            yield return new object?[] { "-0.0000000000000000000000010", -0.0000000000000000000000010m, "-10", 25, "J", null, "-1.0E-24" };
            yield return new object?[] { "0.00000000000000000000000010", 0.00000000000000000000000010m, "10", 26, "J", null, "1.0E-25" };
            yield return new object?[] { "-0.00000000000000000000000010", -0.00000000000000000000000010m, "-10", 26, "J", null, "-1.0E-25" };
            yield return new object?[] { "0.000000000000000000000000010", 0.000000000000000000000000010m, "10", 27, "J", null, "1.0E-26" };
            yield return new object?[] { "-0.000000000000000000000000010", -0.000000000000000000000000010m, "-10", 27, "J", null, "-1.0E-26" };
            yield return new object?[] { "0.0000000000000000000000000010", 0.0000000000000000000000000010m, "10", 28, "J", null, "1.0E-27" };
            yield return new object?[] { "-0.0000000000000000000000000010", -0.0000000000000000000000000010m, "-10", 28, "J", null, "-1.0E-27" };
            yield return new object?[] { "11", 11m, "11", 0, "J", null, "11" };
            yield return new object?[] { "-11", -11m, "-11", 0, "J", null, "-11" };
            yield return new object?[] { "1.1", 1.1m, "11", 1, "J", null, "1.1" };
            yield return new object?[] { "-1.1", -1.1m, "-11", 1, "J", null, "-1.1" };
            yield return new object?[] { "0.11", 0.11m, "11", 2, "J", null, "0.11" };
            yield return new object?[] { "-0.11", -0.11m, "-11", 2, "J", null, "-0.11" };
            yield return new object?[] { "0.011", 0.011m, "11", 3, "J", null, "0.011" };
            yield return new object?[] { "-0.011", -0.011m, "-11", 3, "J", null, "-0.011" };
            yield return new object?[] { "0.0011", 0.0011m, "11", 4, "J", null, "0.0011" };
            yield return new object?[] { "-0.0011", -0.0011m, "-11", 4, "J", null, "-0.0011" };
            yield return new object?[] { "0.00011", 0.00011m, "11", 5, "J", null, "0.00011" };
            yield return new object?[] { "-0.00011", -0.00011m, "-11", 5, "J", null, "-0.00011" };
            yield return new object?[] { "0.000011", 0.000011m, "11", 6, "J", null, "0.000011" };
            yield return new object?[] { "-0.000011", -0.000011m, "-11", 6, "J", null, "-0.000011" };
            yield return new object?[] { "0.0000011", 0.0000011m, "11", 7, "J", null, "0.0000011" };
            yield return new object?[] { "-0.0000011", -0.0000011m, "-11", 7, "J", null, "-0.0000011" };
            yield return new object?[] { "0.00000011", 0.00000011m, "11", 8, "J", null, "1.1E-7" };
            yield return new object?[] { "-0.00000011", -0.00000011m, "-11", 8, "J", null, "-1.1E-7" };
            yield return new object?[] { "0.000000011", 0.000000011m, "11", 9, "J", null, "1.1E-8" };
            yield return new object?[] { "-0.000000011", -0.000000011m, "-11", 9, "J", null, "-1.1E-8" };
            yield return new object?[] { "0.0000000011", 0.0000000011m, "11", 10, "J", null, "1.1E-9" };
            yield return new object?[] { "-0.0000000011", -0.0000000011m, "-11", 10, "J", null, "-1.1E-9" };
            yield return new object?[] { "0.00000000011", 0.00000000011m, "11", 11, "J", null, "1.1E-10" };
            yield return new object?[] { "-0.00000000011", -0.00000000011m, "-11", 11, "J", null, "-1.1E-10" };
            yield return new object?[] { "0.000000000011", 0.000000000011m, "11", 12, "J", null, "1.1E-11" };
            yield return new object?[] { "-0.000000000011", -0.000000000011m, "-11", 12, "J", null, "-1.1E-11" };
            yield return new object?[] { "0.0000000000011", 0.0000000000011m, "11", 13, "J", null, "1.1E-12" };
            yield return new object?[] { "-0.0000000000011", -0.0000000000011m, "-11", 13, "J", null, "-1.1E-12" };
            yield return new object?[] { "0.00000000000011", 0.00000000000011m, "11", 14, "J", null, "1.1E-13" };
            yield return new object?[] { "-0.00000000000011", -0.00000000000011m, "-11", 14, "J", null, "-1.1E-13" };
            yield return new object?[] { "0.000000000000011", 0.000000000000011m, "11", 15, "J", null, "1.1E-14" };
            yield return new object?[] { "-0.000000000000011", -0.000000000000011m, "-11", 15, "J", null, "-1.1E-14" };
            yield return new object?[] { "0.0000000000000011", 0.0000000000000011m, "11", 16, "J", null, "1.1E-15" };
            yield return new object?[] { "-0.0000000000000011", -0.0000000000000011m, "-11", 16, "J", null, "-1.1E-15" };
            yield return new object?[] { "0.00000000000000011", 0.00000000000000011m, "11", 17, "J", null, "1.1E-16" };
            yield return new object?[] { "-0.00000000000000011", -0.00000000000000011m, "-11", 17, "J", null, "-1.1E-16" };
            yield return new object?[] { "0.000000000000000011", 0.000000000000000011m, "11", 18, "J", null, "1.1E-17" };
            yield return new object?[] { "-0.000000000000000011", -0.000000000000000011m, "-11", 18, "J", null, "-1.1E-17" };
            yield return new object?[] { "0.0000000000000000011", 0.0000000000000000011m, "11", 19, "J", null, "1.1E-18" };
            yield return new object?[] { "-0.0000000000000000011", -0.0000000000000000011m, "-11", 19, "J", null, "-1.1E-18" };
            yield return new object?[] { "0.00000000000000000011", 0.00000000000000000011m, "11", 20, "J", null, "1.1E-19" };
            yield return new object?[] { "-0.00000000000000000011", -0.00000000000000000011m, "-11", 20, "J", null, "-1.1E-19" };
            yield return new object?[] { "0.000000000000000000011", 0.000000000000000000011m, "11", 21, "J", null, "1.1E-20" };
            yield return new object?[] { "-0.000000000000000000011", -0.000000000000000000011m, "-11", 21, "J", null, "-1.1E-20" };
            yield return new object?[] { "0.0000000000000000000011", 0.0000000000000000000011m, "11", 22, "J", null, "1.1E-21" };
            yield return new object?[] { "-0.0000000000000000000011", -0.0000000000000000000011m, "-11", 22, "J", null, "-1.1E-21" };
            yield return new object?[] { "0.00000000000000000000011", 0.00000000000000000000011m, "11", 23, "J", null, "1.1E-22" };
            yield return new object?[] { "-0.00000000000000000000011", -0.00000000000000000000011m, "-11", 23, "J", null, "-1.1E-22" };
            yield return new object?[] { "0.000000000000000000000011", 0.000000000000000000000011m, "11", 24, "J", null, "1.1E-23" };
            yield return new object?[] { "-0.000000000000000000000011", -0.000000000000000000000011m, "-11", 24, "J", null, "-1.1E-23" };
            yield return new object?[] { "0.0000000000000000000000011", 0.0000000000000000000000011m, "11", 25, "J", null, "1.1E-24" };
            yield return new object?[] { "-0.0000000000000000000000011", -0.0000000000000000000000011m, "-11", 25, "J", null, "-1.1E-24" };
            yield return new object?[] { "0.00000000000000000000000011", 0.00000000000000000000000011m, "11", 26, "J", null, "1.1E-25" };
            yield return new object?[] { "-0.00000000000000000000000011", -0.00000000000000000000000011m, "-11", 26, "J", null, "-1.1E-25" };
            yield return new object?[] { "0.000000000000000000000000011", 0.000000000000000000000000011m, "11", 27, "J", null, "1.1E-26" };
            yield return new object?[] { "-0.000000000000000000000000011", -0.000000000000000000000000011m, "-11", 27, "J", null, "-1.1E-26" };
            yield return new object?[] { "0.0000000000000000000000000011", 0.0000000000000000000000000011m, "11", 28, "J", null, "1.1E-27" };
            yield return new object?[] { "-0.0000000000000000000000000011", -0.0000000000000000000000000011m, "-11", 28, "J", null, "-1.1E-27" };
            yield return new object?[] { "99", 99m, "99", 0, "J", null, "99" };
            yield return new object?[] { "-99", -99m, "-99", 0, "J", null, "-99" };
            yield return new object?[] { "9.9", 9.9m, "99", 1, "J", null, "9.9" };
            yield return new object?[] { "-9.9", -9.9m, "-99", 1, "J", null, "-9.9" };
            yield return new object?[] { "0.99", 0.99m, "99", 2, "J", null, "0.99" };
            yield return new object?[] { "-0.99", -0.99m, "-99", 2, "J", null, "-0.99" };
            yield return new object?[] { "0.099", 0.099m, "99", 3, "J", null, "0.099" };
            yield return new object?[] { "-0.099", -0.099m, "-99", 3, "J", null, "-0.099" };
            yield return new object?[] { "0.0099", 0.0099m, "99", 4, "J", null, "0.0099" };
            yield return new object?[] { "-0.0099", -0.0099m, "-99", 4, "J", null, "-0.0099" };
            yield return new object?[] { "0.00099", 0.00099m, "99", 5, "J", null, "0.00099" };
            yield return new object?[] { "-0.00099", -0.00099m, "-99", 5, "J", null, "-0.00099" };
            yield return new object?[] { "0.000099", 0.000099m, "99", 6, "J", null, "0.000099" };
            yield return new object?[] { "-0.000099", -0.000099m, "-99", 6, "J", null, "-0.000099" };
            yield return new object?[] { "0.0000099", 0.0000099m, "99", 7, "J", null, "0.0000099" };
            yield return new object?[] { "-0.0000099", -0.0000099m, "-99", 7, "J", null, "-0.0000099" };
            yield return new object?[] { "0.00000099", 0.00000099m, "99", 8, "J", null, "9.9E-7" };
            yield return new object?[] { "-0.00000099", -0.00000099m, "-99", 8, "J", null, "-9.9E-7" };
            yield return new object?[] { "0.000000099", 0.000000099m, "99", 9, "J", null, "9.9E-8" };
            yield return new object?[] { "-0.000000099", -0.000000099m, "-99", 9, "J", null, "-9.9E-8" };
            yield return new object?[] { "0.0000000099", 0.0000000099m, "99", 10, "J", null, "9.9E-9" };
            yield return new object?[] { "-0.0000000099", -0.0000000099m, "-99", 10, "J", null, "-9.9E-9" };
            yield return new object?[] { "0.00000000099", 0.00000000099m, "99", 11, "J", null, "9.9E-10" };
            yield return new object?[] { "-0.00000000099", -0.00000000099m, "-99", 11, "J", null, "-9.9E-10" };
            yield return new object?[] { "0.000000000099", 0.000000000099m, "99", 12, "J", null, "9.9E-11" };
            yield return new object?[] { "-0.000000000099", -0.000000000099m, "-99", 12, "J", null, "-9.9E-11" };
            yield return new object?[] { "0.0000000000099", 0.0000000000099m, "99", 13, "J", null, "9.9E-12" };
            yield return new object?[] { "-0.0000000000099", -0.0000000000099m, "-99", 13, "J", null, "-9.9E-12" };
            yield return new object?[] { "0.00000000000099", 0.00000000000099m, "99", 14, "J", null, "9.9E-13" };
            yield return new object?[] { "-0.00000000000099", -0.00000000000099m, "-99", 14, "J", null, "-9.9E-13" };
            yield return new object?[] { "0.000000000000099", 0.000000000000099m, "99", 15, "J", null, "9.9E-14" };
            yield return new object?[] { "-0.000000000000099", -0.000000000000099m, "-99", 15, "J", null, "-9.9E-14" };
            yield return new object?[] { "0.0000000000000099", 0.0000000000000099m, "99", 16, "J", null, "9.9E-15" };
            yield return new object?[] { "-0.0000000000000099", -0.0000000000000099m, "-99", 16, "J", null, "-9.9E-15" };
            yield return new object?[] { "0.00000000000000099", 0.00000000000000099m, "99", 17, "J", null, "9.9E-16" };
            yield return new object?[] { "-0.00000000000000099", -0.00000000000000099m, "-99", 17, "J", null, "-9.9E-16" };
            yield return new object?[] { "0.000000000000000099", 0.000000000000000099m, "99", 18, "J", null, "9.9E-17" };
            yield return new object?[] { "-0.000000000000000099", -0.000000000000000099m, "-99", 18, "J", null, "-9.9E-17" };
            yield return new object?[] { "0.0000000000000000099", 0.0000000000000000099m, "99", 19, "J", null, "9.9E-18" };
            yield return new object?[] { "-0.0000000000000000099", -0.0000000000000000099m, "-99", 19, "J", null, "-9.9E-18" };
            yield return new object?[] { "0.00000000000000000099", 0.00000000000000000099m, "99", 20, "J", null, "9.9E-19" };
            yield return new object?[] { "-0.00000000000000000099", -0.00000000000000000099m, "-99", 20, "J", null, "-9.9E-19" };
            yield return new object?[] { "0.000000000000000000099", 0.000000000000000000099m, "99", 21, "J", null, "9.9E-20" };
            yield return new object?[] { "-0.000000000000000000099", -0.000000000000000000099m, "-99", 21, "J", null, "-9.9E-20" };
            yield return new object?[] { "0.0000000000000000000099", 0.0000000000000000000099m, "99", 22, "J", null, "9.9E-21" };
            yield return new object?[] { "-0.0000000000000000000099", -0.0000000000000000000099m, "-99", 22, "J", null, "-9.9E-21" };
            yield return new object?[] { "0.00000000000000000000099", 0.00000000000000000000099m, "99", 23, "J", null, "9.9E-22" };
            yield return new object?[] { "-0.00000000000000000000099", -0.00000000000000000000099m, "-99", 23, "J", null, "-9.9E-22" };
            yield return new object?[] { "0.000000000000000000000099", 0.000000000000000000000099m, "99", 24, "J", null, "9.9E-23" };
            yield return new object?[] { "-0.000000000000000000000099", -0.000000000000000000000099m, "-99", 24, "J", null, "-9.9E-23" };
            yield return new object?[] { "0.0000000000000000000000099", 0.0000000000000000000000099m, "99", 25, "J", null, "9.9E-24" };
            yield return new object?[] { "-0.0000000000000000000000099", -0.0000000000000000000000099m, "-99", 25, "J", null, "-9.9E-24" };
            yield return new object?[] { "0.00000000000000000000000099", 0.00000000000000000000000099m, "99", 26, "J", null, "9.9E-25" };
            yield return new object?[] { "-0.00000000000000000000000099", -0.00000000000000000000000099m, "-99", 26, "J", null, "-9.9E-25" };
            yield return new object?[] { "0.000000000000000000000000099", 0.000000000000000000000000099m, "99", 27, "J", null, "9.9E-26" };
            yield return new object?[] { "-0.000000000000000000000000099", -0.000000000000000000000000099m, "-99", 27, "J", null, "-9.9E-26" };
            yield return new object?[] { "0.0000000000000000000000000099", 0.0000000000000000000000000099m, "99", 28, "J", null, "9.9E-27" };
            yield return new object?[] { "-0.0000000000000000000000000099", -0.0000000000000000000000000099m, "-99", 28, "J", null, "-9.9E-27" };
            yield return new object?[] { "-100", -100m, "-100", 0, "J", null, "-100" };
            yield return new object?[] { "10.0", 10.0m, "100", 1, "J", null, "10.0" };
            yield return new object?[] { "-10.0", -10.0m, "-100", 1, "J", null, "-10.0" };
            yield return new object?[] { "-1.00", -1.00m, "-100", 2, "J", null, "-1.00" };
            yield return new object?[] { "0.100", 0.100m, "100", 3, "J", null, "0.100" };
            yield return new object?[] { "-0.100", -0.100m, "-100", 3, "J", null, "-0.100" };
            yield return new object?[] { "0.0100", 0.0100m, "100", 4, "J", null, "0.0100" };
            yield return new object?[] { "-0.0100", -0.0100m, "-100", 4, "J", null, "-0.0100" };
            yield return new object?[] { "0.00100", 0.00100m, "100", 5, "J", null, "0.00100" };
            yield return new object?[] { "-0.00100", -0.00100m, "-100", 5, "J", null, "-0.00100" };
            yield return new object?[] { "0.000100", 0.000100m, "100", 6, "J", null, "0.000100" };
            yield return new object?[] { "-0.000100", -0.000100m, "-100", 6, "J", null, "-0.000100" };
            yield return new object?[] { "0.0000100", 0.0000100m, "100", 7, "J", null, "0.0000100" };
            yield return new object?[] { "-0.0000100", -0.0000100m, "-100", 7, "J", null, "-0.0000100" };
            yield return new object?[] { "0.00000100", 0.00000100m, "100", 8, "J", null, "0.00000100" };
            yield return new object?[] { "-0.00000100", -0.00000100m, "-100", 8, "J", null, "-0.00000100" };
            yield return new object?[] { "0.000000100", 0.000000100m, "100", 9, "J", null, "1.00E-7" };
            yield return new object?[] { "-0.000000100", -0.000000100m, "-100", 9, "J", null, "-1.00E-7" };
            yield return new object?[] { "0.0000000100", 0.0000000100m, "100", 10, "J", null, "1.00E-8" };
            yield return new object?[] { "-0.0000000100", -0.0000000100m, "-100", 10, "J", null, "-1.00E-8" };
            yield return new object?[] { "0.00000000100", 0.00000000100m, "100", 11, "J", null, "1.00E-9" };
            yield return new object?[] { "-0.00000000100", -0.00000000100m, "-100", 11, "J", null, "-1.00E-9" };
            yield return new object?[] { "0.000000000100", 0.000000000100m, "100", 12, "J", null, "1.00E-10" };
            yield return new object?[] { "-0.000000000100", -0.000000000100m, "-100", 12, "J", null, "-1.00E-10" };
            yield return new object?[] { "0.0000000000100", 0.0000000000100m, "100", 13, "J", null, "1.00E-11" };
            yield return new object?[] { "-0.0000000000100", -0.0000000000100m, "-100", 13, "J", null, "-1.00E-11" };
            yield return new object?[] { "0.00000000000100", 0.00000000000100m, "100", 14, "J", null, "1.00E-12" };
            yield return new object?[] { "-0.00000000000100", -0.00000000000100m, "-100", 14, "J", null, "-1.00E-12" };
            yield return new object?[] { "0.000000000000100", 0.000000000000100m, "100", 15, "J", null, "1.00E-13" };
            yield return new object?[] { "-0.000000000000100", -0.000000000000100m, "-100", 15, "J", null, "-1.00E-13" };
            yield return new object?[] { "0.0000000000000100", 0.0000000000000100m, "100", 16, "J", null, "1.00E-14" };
            yield return new object?[] { "-0.0000000000000100", -0.0000000000000100m, "-100", 16, "J", null, "-1.00E-14" };
            yield return new object?[] { "0.00000000000000100", 0.00000000000000100m, "100", 17, "J", null, "1.00E-15" };
            yield return new object?[] { "-0.00000000000000100", -0.00000000000000100m, "-100", 17, "J", null, "-1.00E-15" };
            yield return new object?[] { "0.000000000000000100", 0.000000000000000100m, "100", 18, "J", null, "1.00E-16" };
            yield return new object?[] { "-0.000000000000000100", -0.000000000000000100m, "-100", 18, "J", null, "-1.00E-16" };
            yield return new object?[] { "0.0000000000000000100", 0.0000000000000000100m, "100", 19, "J", null, "1.00E-17" };
            yield return new object?[] { "-0.0000000000000000100", -0.0000000000000000100m, "-100", 19, "J", null, "-1.00E-17" };
            yield return new object?[] { "0.00000000000000000100", 0.00000000000000000100m, "100", 20, "J", null, "1.00E-18" };
            yield return new object?[] { "-0.00000000000000000100", -0.00000000000000000100m, "-100", 20, "J", null, "-1.00E-18" };
            yield return new object?[] { "0.000000000000000000100", 0.000000000000000000100m, "100", 21, "J", null, "1.00E-19" };
            yield return new object?[] { "-0.000000000000000000100", -0.000000000000000000100m, "-100", 21, "J", null, "-1.00E-19" };
            yield return new object?[] { "0.0000000000000000000100", 0.0000000000000000000100m, "100", 22, "J", null, "1.00E-20" };
            yield return new object?[] { "-0.0000000000000000000100", -0.0000000000000000000100m, "-100", 22, "J", null, "-1.00E-20" };
            yield return new object?[] { "0.00000000000000000000100", 0.00000000000000000000100m, "100", 23, "J", null, "1.00E-21" };
            yield return new object?[] { "-0.00000000000000000000100", -0.00000000000000000000100m, "-100", 23, "J", null, "-1.00E-21" };
            yield return new object?[] { "0.000000000000000000000100", 0.000000000000000000000100m, "100", 24, "J", null, "1.00E-22" };
            yield return new object?[] { "-0.000000000000000000000100", -0.000000000000000000000100m, "-100", 24, "J", null, "-1.00E-22" };
            yield return new object?[] { "0.0000000000000000000000100", 0.0000000000000000000000100m, "100", 25, "J", null, "1.00E-23" };
            yield return new object?[] { "-0.0000000000000000000000100", -0.0000000000000000000000100m, "-100", 25, "J", null, "-1.00E-23" };
            yield return new object?[] { "0.00000000000000000000000100", 0.00000000000000000000000100m, "100", 26, "J", null, "1.00E-24" };
            yield return new object?[] { "-0.00000000000000000000000100", -0.00000000000000000000000100m, "-100", 26, "J", null, "-1.00E-24" };
            yield return new object?[] { "0.000000000000000000000000100", 0.000000000000000000000000100m, "100", 27, "J", null, "1.00E-25" };
            yield return new object?[] { "-0.000000000000000000000000100", -0.000000000000000000000000100m, "-100", 27, "J", null, "-1.00E-25" };
            yield return new object?[] { "0.0000000000000000000000000100", 0.0000000000000000000000000100m, "100", 28, "J", null, "1.00E-26" };
            yield return new object?[] { "-0.0000000000000000000000000100", -0.0000000000000000000000000100m, "-100", 28, "J", null, "-1.00E-26" };
            yield return new object?[] { "101", 101m, "101", 0, "J", null, "101" };
            yield return new object?[] { "-101", -101m, "-101", 0, "J", null, "-101" };
            yield return new object?[] { "10.1", 10.1m, "101", 1, "J", null, "10.1" };
            yield return new object?[] { "-10.1", -10.1m, "-101", 1, "J", null, "-10.1" };
            yield return new object?[] { "1.01", 1.01m, "101", 2, "J", null, "1.01" };
            yield return new object?[] { "-1.01", -1.01m, "-101", 2, "J", null, "-1.01" };
            yield return new object?[] { "0.101", 0.101m, "101", 3, "J", null, "0.101" };
            yield return new object?[] { "-0.101", -0.101m, "-101", 3, "J", null, "-0.101" };
            yield return new object?[] { "0.0101", 0.0101m, "101", 4, "J", null, "0.0101" };
            yield return new object?[] { "-0.0101", -0.0101m, "-101", 4, "J", null, "-0.0101" };
            yield return new object?[] { "0.00101", 0.00101m, "101", 5, "J", null, "0.00101" };
            yield return new object?[] { "-0.00101", -0.00101m, "-101", 5, "J", null, "-0.00101" };
            yield return new object?[] { "0.000101", 0.000101m, "101", 6, "J", null, "0.000101" };
            yield return new object?[] { "-0.000101", -0.000101m, "-101", 6, "J", null, "-0.000101" };
            yield return new object?[] { "0.0000101", 0.0000101m, "101", 7, "J", null, "0.0000101" };
            yield return new object?[] { "-0.0000101", -0.0000101m, "-101", 7, "J", null, "-0.0000101" };
            yield return new object?[] { "0.00000101", 0.00000101m, "101", 8, "J", null, "0.00000101" };
            yield return new object?[] { "-0.00000101", -0.00000101m, "-101", 8, "J", null, "-0.00000101" };
            yield return new object?[] { "0.000000101", 0.000000101m, "101", 9, "J", null, "1.01E-7" };
            yield return new object?[] { "-0.000000101", -0.000000101m, "-101", 9, "J", null, "-1.01E-7" };
            yield return new object?[] { "0.0000000101", 0.0000000101m, "101", 10, "J", null, "1.01E-8" };
            yield return new object?[] { "-0.0000000101", -0.0000000101m, "-101", 10, "J", null, "-1.01E-8" };
            yield return new object?[] { "0.00000000101", 0.00000000101m, "101", 11, "J", null, "1.01E-9" };
            yield return new object?[] { "-0.00000000101", -0.00000000101m, "-101", 11, "J", null, "-1.01E-9" };
            yield return new object?[] { "0.000000000101", 0.000000000101m, "101", 12, "J", null, "1.01E-10" };
            yield return new object?[] { "-0.000000000101", -0.000000000101m, "-101", 12, "J", null, "-1.01E-10" };
            yield return new object?[] { "0.0000000000101", 0.0000000000101m, "101", 13, "J", null, "1.01E-11" };
            yield return new object?[] { "-0.0000000000101", -0.0000000000101m, "-101", 13, "J", null, "-1.01E-11" };
            yield return new object?[] { "0.00000000000101", 0.00000000000101m, "101", 14, "J", null, "1.01E-12" };
            yield return new object?[] { "-0.00000000000101", -0.00000000000101m, "-101", 14, "J", null, "-1.01E-12" };
            yield return new object?[] { "0.000000000000101", 0.000000000000101m, "101", 15, "J", null, "1.01E-13" };
            yield return new object?[] { "-0.000000000000101", -0.000000000000101m, "-101", 15, "J", null, "-1.01E-13" };
            yield return new object?[] { "0.0000000000000101", 0.0000000000000101m, "101", 16, "J", null, "1.01E-14" };
            yield return new object?[] { "-0.0000000000000101", -0.0000000000000101m, "-101", 16, "J", null, "-1.01E-14" };
            yield return new object?[] { "0.00000000000000101", 0.00000000000000101m, "101", 17, "J", null, "1.01E-15" };
            yield return new object?[] { "-0.00000000000000101", -0.00000000000000101m, "-101", 17, "J", null, "-1.01E-15" };
            yield return new object?[] { "0.000000000000000101", 0.000000000000000101m, "101", 18, "J", null, "1.01E-16" };
            yield return new object?[] { "-0.000000000000000101", -0.000000000000000101m, "-101", 18, "J", null, "-1.01E-16" };
            yield return new object?[] { "0.0000000000000000101", 0.0000000000000000101m, "101", 19, "J", null, "1.01E-17" };
            yield return new object?[] { "-0.0000000000000000101", -0.0000000000000000101m, "-101", 19, "J", null, "-1.01E-17" };
            yield return new object?[] { "0.00000000000000000101", 0.00000000000000000101m, "101", 20, "J", null, "1.01E-18" };
            yield return new object?[] { "-0.00000000000000000101", -0.00000000000000000101m, "-101", 20, "J", null, "-1.01E-18" };
            yield return new object?[] { "0.000000000000000000101", 0.000000000000000000101m, "101", 21, "J", null, "1.01E-19" };
            yield return new object?[] { "-0.000000000000000000101", -0.000000000000000000101m, "-101", 21, "J", null, "-1.01E-19" };
            yield return new object?[] { "0.0000000000000000000101", 0.0000000000000000000101m, "101", 22, "J", null, "1.01E-20" };
            yield return new object?[] { "-0.0000000000000000000101", -0.0000000000000000000101m, "-101", 22, "J", null, "-1.01E-20" };
            yield return new object?[] { "0.00000000000000000000101", 0.00000000000000000000101m, "101", 23, "J", null, "1.01E-21" };
            yield return new object?[] { "-0.00000000000000000000101", -0.00000000000000000000101m, "-101", 23, "J", null, "-1.01E-21" };
            yield return new object?[] { "0.000000000000000000000101", 0.000000000000000000000101m, "101", 24, "J", null, "1.01E-22" };
            yield return new object?[] { "-0.000000000000000000000101", -0.000000000000000000000101m, "-101", 24, "J", null, "-1.01E-22" };
            yield return new object?[] { "0.0000000000000000000000101", 0.0000000000000000000000101m, "101", 25, "J", null, "1.01E-23" };
            yield return new object?[] { "-0.0000000000000000000000101", -0.0000000000000000000000101m, "-101", 25, "J", null, "-1.01E-23" };
            yield return new object?[] { "0.00000000000000000000000101", 0.00000000000000000000000101m, "101", 26, "J", null, "1.01E-24" };
            yield return new object?[] { "-0.00000000000000000000000101", -0.00000000000000000000000101m, "-101", 26, "J", null, "-1.01E-24" };
            yield return new object?[] { "0.000000000000000000000000101", 0.000000000000000000000000101m, "101", 27, "J", null, "1.01E-25" };
            yield return new object?[] { "-0.000000000000000000000000101", -0.000000000000000000000000101m, "-101", 27, "J", null, "-1.01E-25" };
            yield return new object?[] { "0.0000000000000000000000000101", 0.0000000000000000000000000101m, "101", 28, "J", null, "1.01E-26" };
            yield return new object?[] { "-0.0000000000000000000000000101", -0.0000000000000000000000000101m, "-101", 28, "J", null, "-1.01E-26" };
            yield return new object?[] { "999", 999m, "999", 0, "J", null, "999" };
            yield return new object?[] { "-999", -999m, "-999", 0, "J", null, "-999" };
            yield return new object?[] { "99.9", 99.9m, "999", 1, "J", null, "99.9" };
            yield return new object?[] { "-99.9", -99.9m, "-999", 1, "J", null, "-99.9" };
            yield return new object?[] { "9.99", 9.99m, "999", 2, "J", null, "9.99" };
            yield return new object?[] { "-9.99", -9.99m, "-999", 2, "J", null, "-9.99" };
            yield return new object?[] { "0.999", 0.999m, "999", 3, "J", null, "0.999" };
            yield return new object?[] { "-0.999", -0.999m, "-999", 3, "J", null, "-0.999" };
            yield return new object?[] { "0.0999", 0.0999m, "999", 4, "J", null, "0.0999" };
            yield return new object?[] { "-0.0999", -0.0999m, "-999", 4, "J", null, "-0.0999" };
            yield return new object?[] { "0.00999", 0.00999m, "999", 5, "J", null, "0.00999" };
            yield return new object?[] { "-0.00999", -0.00999m, "-999", 5, "J", null, "-0.00999" };
            yield return new object?[] { "0.000999", 0.000999m, "999", 6, "J", null, "0.000999" };
            yield return new object?[] { "-0.000999", -0.000999m, "-999", 6, "J", null, "-0.000999" };
            yield return new object?[] { "0.0000999", 0.0000999m, "999", 7, "J", null, "0.0000999" };
            yield return new object?[] { "-0.0000999", -0.0000999m, "-999", 7, "J", null, "-0.0000999" };
            yield return new object?[] { "0.00000999", 0.00000999m, "999", 8, "J", null, "0.00000999" };
            yield return new object?[] { "-0.00000999", -0.00000999m, "-999", 8, "J", null, "-0.00000999" };
            yield return new object?[] { "0.000000999", 0.000000999m, "999", 9, "J", null, "9.99E-7" };
            yield return new object?[] { "-0.000000999", -0.000000999m, "-999", 9, "J", null, "-9.99E-7" };
            yield return new object?[] { "0.0000000999", 0.0000000999m, "999", 10, "J", null, "9.99E-8" };
            yield return new object?[] { "-0.0000000999", -0.0000000999m, "-999", 10, "J", null, "-9.99E-8" };
            yield return new object?[] { "0.00000000999", 0.00000000999m, "999", 11, "J", null, "9.99E-9" };
            yield return new object?[] { "-0.00000000999", -0.00000000999m, "-999", 11, "J", null, "-9.99E-9" };
            yield return new object?[] { "0.000000000999", 0.000000000999m, "999", 12, "J", null, "9.99E-10" };
            yield return new object?[] { "-0.000000000999", -0.000000000999m, "-999", 12, "J", null, "-9.99E-10" };
            yield return new object?[] { "0.0000000000999", 0.0000000000999m, "999", 13, "J", null, "9.99E-11" };
            yield return new object?[] { "-0.0000000000999", -0.0000000000999m, "-999", 13, "J", null, "-9.99E-11" };
            yield return new object?[] { "0.00000000000999", 0.00000000000999m, "999", 14, "J", null, "9.99E-12" };
            yield return new object?[] { "-0.00000000000999", -0.00000000000999m, "-999", 14, "J", null, "-9.99E-12" };
            yield return new object?[] { "0.000000000000999", 0.000000000000999m, "999", 15, "J", null, "9.99E-13" };
            yield return new object?[] { "-0.000000000000999", -0.000000000000999m, "-999", 15, "J", null, "-9.99E-13" };
            yield return new object?[] { "0.0000000000000999", 0.0000000000000999m, "999", 16, "J", null, "9.99E-14" };
            yield return new object?[] { "-0.0000000000000999", -0.0000000000000999m, "-999", 16, "J", null, "-9.99E-14" };
            yield return new object?[] { "0.00000000000000999", 0.00000000000000999m, "999", 17, "J", null, "9.99E-15" };
            yield return new object?[] { "-0.00000000000000999", -0.00000000000000999m, "-999", 17, "J", null, "-9.99E-15" };
            yield return new object?[] { "0.000000000000000999", 0.000000000000000999m, "999", 18, "J", null, "9.99E-16" };
            yield return new object?[] { "-0.000000000000000999", -0.000000000000000999m, "-999", 18, "J", null, "-9.99E-16" };
            yield return new object?[] { "0.0000000000000000999", 0.0000000000000000999m, "999", 19, "J", null, "9.99E-17" };
            yield return new object?[] { "-0.0000000000000000999", -0.0000000000000000999m, "-999", 19, "J", null, "-9.99E-17" };
            yield return new object?[] { "0.00000000000000000999", 0.00000000000000000999m, "999", 20, "J", null, "9.99E-18" };
            yield return new object?[] { "-0.00000000000000000999", -0.00000000000000000999m, "-999", 20, "J", null, "-9.99E-18" };
            yield return new object?[] { "0.000000000000000000999", 0.000000000000000000999m, "999", 21, "J", null, "9.99E-19" };
            yield return new object?[] { "-0.000000000000000000999", -0.000000000000000000999m, "-999", 21, "J", null, "-9.99E-19" };
            yield return new object?[] { "0.0000000000000000000999", 0.0000000000000000000999m, "999", 22, "J", null, "9.99E-20" };
            yield return new object?[] { "-0.0000000000000000000999", -0.0000000000000000000999m, "-999", 22, "J", null, "-9.99E-20" };
            yield return new object?[] { "0.00000000000000000000999", 0.00000000000000000000999m, "999", 23, "J", null, "9.99E-21" };
            yield return new object?[] { "-0.00000000000000000000999", -0.00000000000000000000999m, "-999", 23, "J", null, "-9.99E-21" };
            yield return new object?[] { "0.000000000000000000000999", 0.000000000000000000000999m, "999", 24, "J", null, "9.99E-22" };
            yield return new object?[] { "-0.000000000000000000000999", -0.000000000000000000000999m, "-999", 24, "J", null, "-9.99E-22" };
            yield return new object?[] { "0.0000000000000000000000999", 0.0000000000000000000000999m, "999", 25, "J", null, "9.99E-23" };
            yield return new object?[] { "-0.0000000000000000000000999", -0.0000000000000000000000999m, "-999", 25, "J", null, "-9.99E-23" };
            yield return new object?[] { "0.00000000000000000000000999", 0.00000000000000000000000999m, "999", 26, "J", null, "9.99E-24" };
            yield return new object?[] { "-0.00000000000000000000000999", -0.00000000000000000000000999m, "-999", 26, "J", null, "-9.99E-24" };
            yield return new object?[] { "0.000000000000000000000000999", 0.000000000000000000000000999m, "999", 27, "J", null, "9.99E-25" };
            yield return new object?[] { "-0.000000000000000000000000999", -0.000000000000000000000000999m, "-999", 27, "J", null, "-9.99E-25" };
            yield return new object?[] { "0.0000000000000000000000000999", 0.0000000000000000000000000999m, "999", 28, "J", null, "9.99E-26" };
            yield return new object?[] { "-0.0000000000000000000000000999", -0.0000000000000000000000000999m, "-999", 28, "J", null, "-9.99E-26" };
            yield return new object?[] { "-1000", -1000m, "-1000", 0, "J", null, "-1000" };
            yield return new object?[] { "100.0", 100.0m, "1000", 1, "J", null, "100.0" };
            yield return new object?[] { "-100.0", -100.0m, "-1000", 1, "J", null, "-100.0" };
            yield return new object?[] { "10.00", 10.00m, "1000", 2, "J", null, "10.00" };
            yield return new object?[] { "-10.00", -10.00m, "-1000", 2, "J", null, "-10.00" };
            yield return new object?[] { "-1.000", -1.000m, "-1000", 3, "J", null, "-1.000" };
            yield return new object?[] { "0.1000", 0.1000m, "1000", 4, "J", null, "0.1000" };
            yield return new object?[] { "-0.1000", -0.1000m, "-1000", 4, "J", null, "-0.1000" };
            yield return new object?[] { "0.01000", 0.01000m, "1000", 5, "J", null, "0.01000" };
            yield return new object?[] { "-0.01000", -0.01000m, "-1000", 5, "J", null, "-0.01000" };
            yield return new object?[] { "0.001000", 0.001000m, "1000", 6, "J", null, "0.001000" };
            yield return new object?[] { "-0.001000", -0.001000m, "-1000", 6, "J", null, "-0.001000" };
            yield return new object?[] { "0.0001000", 0.0001000m, "1000", 7, "J", null, "0.0001000" };
            yield return new object?[] { "-0.0001000", -0.0001000m, "-1000", 7, "J", null, "-0.0001000" };
            yield return new object?[] { "0.00001000", 0.00001000m, "1000", 8, "J", null, "0.00001000" };
            yield return new object?[] { "-0.00001000", -0.00001000m, "-1000", 8, "J", null, "-0.00001000" };
            yield return new object?[] { "0.000001000", 0.000001000m, "1000", 9, "J", null, "0.000001000" };
            yield return new object?[] { "-0.000001000", -0.000001000m, "-1000", 9, "J", null, "-0.000001000" };
            yield return new object?[] { "0.0000001000", 0.0000001000m, "1000", 10, "J", null, "1.000E-7" };
            yield return new object?[] { "-0.0000001000", -0.0000001000m, "-1000", 10, "J", null, "-1.000E-7" };
            yield return new object?[] { "0.00000001000", 0.00000001000m, "1000", 11, "J", null, "1.000E-8" };
            yield return new object?[] { "-0.00000001000", -0.00000001000m, "-1000", 11, "J", null, "-1.000E-8" };
            yield return new object?[] { "0.000000001000", 0.000000001000m, "1000", 12, "J", null, "1.000E-9" };
            yield return new object?[] { "-0.000000001000", -0.000000001000m, "-1000", 12, "J", null, "-1.000E-9" };
            yield return new object?[] { "0.0000000001000", 0.0000000001000m, "1000", 13, "J", null, "1.000E-10" };
            yield return new object?[] { "-0.0000000001000", -0.0000000001000m, "-1000", 13, "J", null, "-1.000E-10" };
            yield return new object?[] { "0.00000000001000", 0.00000000001000m, "1000", 14, "J", null, "1.000E-11" };
            yield return new object?[] { "-0.00000000001000", -0.00000000001000m, "-1000", 14, "J", null, "-1.000E-11" };
            yield return new object?[] { "0.000000000001000", 0.000000000001000m, "1000", 15, "J", null, "1.000E-12" };
            yield return new object?[] { "-0.000000000001000", -0.000000000001000m, "-1000", 15, "J", null, "-1.000E-12" };
            yield return new object?[] { "0.0000000000001000", 0.0000000000001000m, "1000", 16, "J", null, "1.000E-13" };
            yield return new object?[] { "-0.0000000000001000", -0.0000000000001000m, "-1000", 16, "J", null, "-1.000E-13" };
            yield return new object?[] { "0.00000000000001000", 0.00000000000001000m, "1000", 17, "J", null, "1.000E-14" };
            yield return new object?[] { "-0.00000000000001000", -0.00000000000001000m, "-1000", 17, "J", null, "-1.000E-14" };
            yield return new object?[] { "0.000000000000001000", 0.000000000000001000m, "1000", 18, "J", null, "1.000E-15" };
            yield return new object?[] { "-0.000000000000001000", -0.000000000000001000m, "-1000", 18, "J", null, "-1.000E-15" };
            yield return new object?[] { "0.0000000000000001000", 0.0000000000000001000m, "1000", 19, "J", null, "1.000E-16" };
            yield return new object?[] { "-0.0000000000000001000", -0.0000000000000001000m, "-1000", 19, "J", null, "-1.000E-16" };
            yield return new object?[] { "0.00000000000000001000", 0.00000000000000001000m, "1000", 20, "J", null, "1.000E-17" };
            yield return new object?[] { "-0.00000000000000001000", -0.00000000000000001000m, "-1000", 20, "J", null, "-1.000E-17" };
            yield return new object?[] { "0.000000000000000001000", 0.000000000000000001000m, "1000", 21, "J", null, "1.000E-18" };
            yield return new object?[] { "-0.000000000000000001000", -0.000000000000000001000m, "-1000", 21, "J", null, "-1.000E-18" };
            yield return new object?[] { "0.0000000000000000001000", 0.0000000000000000001000m, "1000", 22, "J", null, "1.000E-19" };
            yield return new object?[] { "-0.0000000000000000001000", -0.0000000000000000001000m, "-1000", 22, "J", null, "-1.000E-19" };
            yield return new object?[] { "0.00000000000000000001000", 0.00000000000000000001000m, "1000", 23, "J", null, "1.000E-20" };
            yield return new object?[] { "-0.00000000000000000001000", -0.00000000000000000001000m, "-1000", 23, "J", null, "-1.000E-20" };
            yield return new object?[] { "0.000000000000000000001000", 0.000000000000000000001000m, "1000", 24, "J", null, "1.000E-21" };
            yield return new object?[] { "-0.000000000000000000001000", -0.000000000000000000001000m, "-1000", 24, "J", null, "-1.000E-21" };
            yield return new object?[] { "0.0000000000000000000001000", 0.0000000000000000000001000m, "1000", 25, "J", null, "1.000E-22" };
            yield return new object?[] { "-0.0000000000000000000001000", -0.0000000000000000000001000m, "-1000", 25, "J", null, "-1.000E-22" };
            yield return new object?[] { "0.00000000000000000000001000", 0.00000000000000000000001000m, "1000", 26, "J", null, "1.000E-23" };
            yield return new object?[] { "-0.00000000000000000000001000", -0.00000000000000000000001000m, "-1000", 26, "J", null, "-1.000E-23" };
            yield return new object?[] { "0.000000000000000000000001000", 0.000000000000000000000001000m, "1000", 27, "J", null, "1.000E-24" };
            yield return new object?[] { "-0.000000000000000000000001000", -0.000000000000000000000001000m, "-1000", 27, "J", null, "-1.000E-24" };
            yield return new object?[] { "0.0000000000000000000000001000", 0.0000000000000000000000001000m, "1000", 28, "J", null, "1.000E-25" };
            yield return new object?[] { "-0.0000000000000000000000001000", -0.0000000000000000000000001000m, "-1000", 28, "J", null, "-1.000E-25" };
            yield return new object?[] { "1001", 1001m, "1001", 0, "J", null, "1001" };
            yield return new object?[] { "-1001", -1001m, "-1001", 0, "J", null, "-1001" };
            yield return new object?[] { "100.1", 100.1m, "1001", 1, "J", null, "100.1" };
            yield return new object?[] { "-100.1", -100.1m, "-1001", 1, "J", null, "-100.1" };
            yield return new object?[] { "10.01", 10.01m, "1001", 2, "J", null, "10.01" };
            yield return new object?[] { "-10.01", -10.01m, "-1001", 2, "J", null, "-10.01" };
            yield return new object?[] { "1.001", 1.001m, "1001", 3, "J", null, "1.001" };
            yield return new object?[] { "-1.001", -1.001m, "-1001", 3, "J", null, "-1.001" };
            yield return new object?[] { "0.1001", 0.1001m, "1001", 4, "J", null, "0.1001" };
            yield return new object?[] { "-0.1001", -0.1001m, "-1001", 4, "J", null, "-0.1001" };
            yield return new object?[] { "0.01001", 0.01001m, "1001", 5, "J", null, "0.01001" };
            yield return new object?[] { "-0.01001", -0.01001m, "-1001", 5, "J", null, "-0.01001" };
            yield return new object?[] { "0.001001", 0.001001m, "1001", 6, "J", null, "0.001001" };
            yield return new object?[] { "-0.001001", -0.001001m, "-1001", 6, "J", null, "-0.001001" };
            yield return new object?[] { "0.0001001", 0.0001001m, "1001", 7, "J", null, "0.0001001" };
            yield return new object?[] { "-0.0001001", -0.0001001m, "-1001", 7, "J", null, "-0.0001001" };
            yield return new object?[] { "0.00001001", 0.00001001m, "1001", 8, "J", null, "0.00001001" };
            yield return new object?[] { "-0.00001001", -0.00001001m, "-1001", 8, "J", null, "-0.00001001" };
            yield return new object?[] { "0.000001001", 0.000001001m, "1001", 9, "J", null, "0.000001001" };
            yield return new object?[] { "-0.000001001", -0.000001001m, "-1001", 9, "J", null, "-0.000001001" };
            yield return new object?[] { "0.0000001001", 0.0000001001m, "1001", 10, "J", null, "1.001E-7" };
            yield return new object?[] { "-0.0000001001", -0.0000001001m, "-1001", 10, "J", null, "-1.001E-7" };
            yield return new object?[] { "0.00000001001", 0.00000001001m, "1001", 11, "J", null, "1.001E-8" };
            yield return new object?[] { "-0.00000001001", -0.00000001001m, "-1001", 11, "J", null, "-1.001E-8" };
            yield return new object?[] { "0.000000001001", 0.000000001001m, "1001", 12, "J", null, "1.001E-9" };
            yield return new object?[] { "-0.000000001001", -0.000000001001m, "-1001", 12, "J", null, "-1.001E-9" };
            yield return new object?[] { "0.0000000001001", 0.0000000001001m, "1001", 13, "J", null, "1.001E-10" };
            yield return new object?[] { "-0.0000000001001", -0.0000000001001m, "-1001", 13, "J", null, "-1.001E-10" };
            yield return new object?[] { "0.00000000001001", 0.00000000001001m, "1001", 14, "J", null, "1.001E-11" };
            yield return new object?[] { "-0.00000000001001", -0.00000000001001m, "-1001", 14, "J", null, "-1.001E-11" };
            yield return new object?[] { "0.000000000001001", 0.000000000001001m, "1001", 15, "J", null, "1.001E-12" };
            yield return new object?[] { "-0.000000000001001", -0.000000000001001m, "-1001", 15, "J", null, "-1.001E-12" };
            yield return new object?[] { "0.0000000000001001", 0.0000000000001001m, "1001", 16, "J", null, "1.001E-13" };
            yield return new object?[] { "-0.0000000000001001", -0.0000000000001001m, "-1001", 16, "J", null, "-1.001E-13" };
            yield return new object?[] { "0.00000000000001001", 0.00000000000001001m, "1001", 17, "J", null, "1.001E-14" };
            yield return new object?[] { "-0.00000000000001001", -0.00000000000001001m, "-1001", 17, "J", null, "-1.001E-14" };
            yield return new object?[] { "0.000000000000001001", 0.000000000000001001m, "1001", 18, "J", null, "1.001E-15" };
            yield return new object?[] { "-0.000000000000001001", -0.000000000000001001m, "-1001", 18, "J", null, "-1.001E-15" };
            yield return new object?[] { "0.0000000000000001001", 0.0000000000000001001m, "1001", 19, "J", null, "1.001E-16" };
            yield return new object?[] { "-0.0000000000000001001", -0.0000000000000001001m, "-1001", 19, "J", null, "-1.001E-16" };
            yield return new object?[] { "0.00000000000000001001", 0.00000000000000001001m, "1001", 20, "J", null, "1.001E-17" };
            yield return new object?[] { "-0.00000000000000001001", -0.00000000000000001001m, "-1001", 20, "J", null, "-1.001E-17" };
            yield return new object?[] { "0.000000000000000001001", 0.000000000000000001001m, "1001", 21, "J", null, "1.001E-18" };
            yield return new object?[] { "-0.000000000000000001001", -0.000000000000000001001m, "-1001", 21, "J", null, "-1.001E-18" };
            yield return new object?[] { "0.0000000000000000001001", 0.0000000000000000001001m, "1001", 22, "J", null, "1.001E-19" };
            yield return new object?[] { "-0.0000000000000000001001", -0.0000000000000000001001m, "-1001", 22, "J", null, "-1.001E-19" };
            yield return new object?[] { "0.00000000000000000001001", 0.00000000000000000001001m, "1001", 23, "J", null, "1.001E-20" };
            yield return new object?[] { "-0.00000000000000000001001", -0.00000000000000000001001m, "-1001", 23, "J", null, "-1.001E-20" };
            yield return new object?[] { "0.000000000000000000001001", 0.000000000000000000001001m, "1001", 24, "J", null, "1.001E-21" };
            yield return new object?[] { "-0.000000000000000000001001", -0.000000000000000000001001m, "-1001", 24, "J", null, "-1.001E-21" };
            yield return new object?[] { "0.0000000000000000000001001", 0.0000000000000000000001001m, "1001", 25, "J", null, "1.001E-22" };
            yield return new object?[] { "-0.0000000000000000000001001", -0.0000000000000000000001001m, "-1001", 25, "J", null, "-1.001E-22" };
            yield return new object?[] { "0.00000000000000000000001001", 0.00000000000000000000001001m, "1001", 26, "J", null, "1.001E-23" };
            yield return new object?[] { "-0.00000000000000000000001001", -0.00000000000000000000001001m, "-1001", 26, "J", null, "-1.001E-23" };
            yield return new object?[] { "0.000000000000000000000001001", 0.000000000000000000000001001m, "1001", 27, "J", null, "1.001E-24" };
            yield return new object?[] { "-0.000000000000000000000001001", -0.000000000000000000000001001m, "-1001", 27, "J", null, "-1.001E-24" };
            yield return new object?[] { "0.0000000000000000000000001001", 0.0000000000000000000000001001m, "1001", 28, "J", null, "1.001E-25" };
            yield return new object?[] { "-0.0000000000000000000000001001", -0.0000000000000000000000001001m, "-1001", 28, "J", null, "-1.001E-25" };
            yield return new object?[] { "12345", 12345m, "12345", 0, "J", null, "12345" };
            yield return new object?[] { "-12345", -12345m, "-12345", 0, "J", null, "-12345" };
            yield return new object?[] { "1234.5", 1234.5m, "12345", 1, "J", null, "1234.5" };
            yield return new object?[] { "-1234.5", -1234.5m, "-12345", 1, "J", null, "-1234.5" };
            yield return new object?[] { "12.345", 12.345m, "12345", 3, "J", null, "12.345" };
            yield return new object?[] { "-12.345", -12.345m, "-12345", 3, "J", null, "-12.345" };
            yield return new object?[] { "1.2345", 1.2345m, "12345", 4, "J", null, "1.2345" };
            yield return new object?[] { "-1.2345", -1.2345m, "-12345", 4, "J", null, "-1.2345" };
            yield return new object?[] { "0.12345", 0.12345m, "12345", 5, "J", null, "0.12345" };
            yield return new object?[] { "-0.12345", -0.12345m, "-12345", 5, "J", null, "-0.12345" };
            yield return new object?[] { "0.012345", 0.012345m, "12345", 6, "J", null, "0.012345" };
            yield return new object?[] { "-0.012345", -0.012345m, "-12345", 6, "J", null, "-0.012345" };
            yield return new object?[] { "0.0012345", 0.0012345m, "12345", 7, "J", null, "0.0012345" };
            yield return new object?[] { "-0.0012345", -0.0012345m, "-12345", 7, "J", null, "-0.0012345" };
            yield return new object?[] { "0.00012345", 0.00012345m, "12345", 8, "J", null, "0.00012345" };
            yield return new object?[] { "-0.00012345", -0.00012345m, "-12345", 8, "J", null, "-0.00012345" };
            yield return new object?[] { "0.000012345", 0.000012345m, "12345", 9, "J", null, "0.000012345" };
            yield return new object?[] { "-0.000012345", -0.000012345m, "-12345", 9, "J", null, "-0.000012345" };
            yield return new object?[] { "0.0000012345", 0.0000012345m, "12345", 10, "J", null, "0.0000012345" };
            yield return new object?[] { "-0.0000012345", -0.0000012345m, "-12345", 10, "J", null, "-0.0000012345" };
            yield return new object?[] { "0.00000012345", 0.00000012345m, "12345", 11, "J", null, "1.2345E-7" };
            yield return new object?[] { "-0.00000012345", -0.00000012345m, "-12345", 11, "J", null, "-1.2345E-7" };
            yield return new object?[] { "0.000000012345", 0.000000012345m, "12345", 12, "J", null, "1.2345E-8" };
            yield return new object?[] { "-0.000000012345", -0.000000012345m, "-12345", 12, "J", null, "-1.2345E-8" };
            yield return new object?[] { "0.0000000012345", 0.0000000012345m, "12345", 13, "J", null, "1.2345E-9" };
            yield return new object?[] { "-0.0000000012345", -0.0000000012345m, "-12345", 13, "J", null, "-1.2345E-9" };
            yield return new object?[] { "0.00000000012345", 0.00000000012345m, "12345", 14, "J", null, "1.2345E-10" };
            yield return new object?[] { "-0.00000000012345", -0.00000000012345m, "-12345", 14, "J", null, "-1.2345E-10" };
            yield return new object?[] { "0.000000000012345", 0.000000000012345m, "12345", 15, "J", null, "1.2345E-11" };
            yield return new object?[] { "-0.000000000012345", -0.000000000012345m, "-12345", 15, "J", null, "-1.2345E-11" };
            yield return new object?[] { "0.0000000000012345", 0.0000000000012345m, "12345", 16, "J", null, "1.2345E-12" };
            yield return new object?[] { "-0.0000000000012345", -0.0000000000012345m, "-12345", 16, "J", null, "-1.2345E-12" };
            yield return new object?[] { "0.00000000000012345", 0.00000000000012345m, "12345", 17, "J", null, "1.2345E-13" };
            yield return new object?[] { "-0.00000000000012345", -0.00000000000012345m, "-12345", 17, "J", null, "-1.2345E-13" };
            yield return new object?[] { "0.000000000000012345", 0.000000000000012345m, "12345", 18, "J", null, "1.2345E-14" };
            yield return new object?[] { "-0.000000000000012345", -0.000000000000012345m, "-12345", 18, "J", null, "-1.2345E-14" };
            yield return new object?[] { "0.0000000000000012345", 0.0000000000000012345m, "12345", 19, "J", null, "1.2345E-15" };
            yield return new object?[] { "-0.0000000000000012345", -0.0000000000000012345m, "-12345", 19, "J", null, "-1.2345E-15" };
            yield return new object?[] { "0.00000000000000012345", 0.00000000000000012345m, "12345", 20, "J", null, "1.2345E-16" };
            yield return new object?[] { "-0.00000000000000012345", -0.00000000000000012345m, "-12345", 20, "J", null, "-1.2345E-16" };
            yield return new object?[] { "0.000000000000000012345", 0.000000000000000012345m, "12345", 21, "J", null, "1.2345E-17" };
            yield return new object?[] { "-0.000000000000000012345", -0.000000000000000012345m, "-12345", 21, "J", null, "-1.2345E-17" };
            yield return new object?[] { "0.0000000000000000012345", 0.0000000000000000012345m, "12345", 22, "J", null, "1.2345E-18" };
            yield return new object?[] { "-0.0000000000000000012345", -0.0000000000000000012345m, "-12345", 22, "J", null, "-1.2345E-18" };
            yield return new object?[] { "0.00000000000000000012345", 0.00000000000000000012345m, "12345", 23, "J", null, "1.2345E-19" };
            yield return new object?[] { "-0.00000000000000000012345", -0.00000000000000000012345m, "-12345", 23, "J", null, "-1.2345E-19" };
            yield return new object?[] { "0.000000000000000000012345", 0.000000000000000000012345m, "12345", 24, "J", null, "1.2345E-20" };
            yield return new object?[] { "-0.000000000000000000012345", -0.000000000000000000012345m, "-12345", 24, "J", null, "-1.2345E-20" };
            yield return new object?[] { "0.0000000000000000000012345", 0.0000000000000000000012345m, "12345", 25, "J", null, "1.2345E-21" };
            yield return new object?[] { "-0.0000000000000000000012345", -0.0000000000000000000012345m, "-12345", 25, "J", null, "-1.2345E-21" };
            yield return new object?[] { "0.00000000000000000000012345", 0.00000000000000000000012345m, "12345", 26, "J", null, "1.2345E-22" };
            yield return new object?[] { "-0.00000000000000000000012345", -0.00000000000000000000012345m, "-12345", 26, "J", null, "-1.2345E-22" };
            yield return new object?[] { "0.000000000000000000000012345", 0.000000000000000000000012345m, "12345", 27, "J", null, "1.2345E-23" };
            yield return new object?[] { "-0.000000000000000000000012345", -0.000000000000000000000012345m, "-12345", 27, "J", null, "-1.2345E-23" };
            yield return new object?[] { "0.0000000000000000000000012345", 0.0000000000000000000000012345m, "12345", 28, "J", null, "1.2345E-24" };
            yield return new object?[] { "-0.0000000000000000000000012345", -0.0000000000000000000000012345m, "-12345", 28, "J", null, "-1.2345E-24" };
            yield return new object?[] { "99999", 99999m, "99999", 0, "J", null, "99999" };
            yield return new object?[] { "-99999", -99999m, "-99999", 0, "J", null, "-99999" };
            yield return new object?[] { "9999.9", 9999.9m, "99999", 1, "J", null, "9999.9" };
            yield return new object?[] { "-9999.9", -9999.9m, "-99999", 1, "J", null, "-9999.9" };
            yield return new object?[] { "999.99", 999.99m, "99999", 2, "J", null, "999.99" };
            yield return new object?[] { "-999.99", -999.99m, "-99999", 2, "J", null, "-999.99" };
            yield return new object?[] { "99.999", 99.999m, "99999", 3, "J", null, "99.999" };
            yield return new object?[] { "-99.999", -99.999m, "-99999", 3, "J", null, "-99.999" };
            yield return new object?[] { "9.9999", 9.9999m, "99999", 4, "J", null, "9.9999" };
            yield return new object?[] { "-9.9999", -9.9999m, "-99999", 4, "J", null, "-9.9999" };
            yield return new object?[] { "0.99999", 0.99999m, "99999", 5, "J", null, "0.99999" };
            yield return new object?[] { "-0.99999", -0.99999m, "-99999", 5, "J", null, "-0.99999" };
            yield return new object?[] { "0.099999", 0.099999m, "99999", 6, "J", null, "0.099999" };
            yield return new object?[] { "-0.099999", -0.099999m, "-99999", 6, "J", null, "-0.099999" };
            yield return new object?[] { "0.0099999", 0.0099999m, "99999", 7, "J", null, "0.0099999" };
            yield return new object?[] { "-0.0099999", -0.0099999m, "-99999", 7, "J", null, "-0.0099999" };
            yield return new object?[] { "0.00099999", 0.00099999m, "99999", 8, "J", null, "0.00099999" };
            yield return new object?[] { "-0.00099999", -0.00099999m, "-99999", 8, "J", null, "-0.00099999" };
            yield return new object?[] { "0.000099999", 0.000099999m, "99999", 9, "J", null, "0.000099999" };
            yield return new object?[] { "-0.000099999", -0.000099999m, "-99999", 9, "J", null, "-0.000099999" };
            yield return new object?[] { "0.0000099999", 0.0000099999m, "99999", 10, "J", null, "0.0000099999" };
            yield return new object?[] { "-0.0000099999", -0.0000099999m, "-99999", 10, "J", null, "-0.0000099999" };
            yield return new object?[] { "0.00000099999", 0.00000099999m, "99999", 11, "J", null, "9.9999E-7" };
            yield return new object?[] { "-0.00000099999", -0.00000099999m, "-99999", 11, "J", null, "-9.9999E-7" };
            yield return new object?[] { "0.000000099999", 0.000000099999m, "99999", 12, "J", null, "9.9999E-8" };
            yield return new object?[] { "-0.000000099999", -0.000000099999m, "-99999", 12, "J", null, "-9.9999E-8" };
            yield return new object?[] { "0.0000000099999", 0.0000000099999m, "99999", 13, "J", null, "9.9999E-9" };
            yield return new object?[] { "-0.0000000099999", -0.0000000099999m, "-99999", 13, "J", null, "-9.9999E-9" };
            yield return new object?[] { "0.00000000099999", 0.00000000099999m, "99999", 14, "J", null, "9.9999E-10" };
            yield return new object?[] { "-0.00000000099999", -0.00000000099999m, "-99999", 14, "J", null, "-9.9999E-10" };
            yield return new object?[] { "0.000000000099999", 0.000000000099999m, "99999", 15, "J", null, "9.9999E-11" };
            yield return new object?[] { "-0.000000000099999", -0.000000000099999m, "-99999", 15, "J", null, "-9.9999E-11" };
            yield return new object?[] { "0.0000000000099999", 0.0000000000099999m, "99999", 16, "J", null, "9.9999E-12" };
            yield return new object?[] { "-0.0000000000099999", -0.0000000000099999m, "-99999", 16, "J", null, "-9.9999E-12" };
            yield return new object?[] { "0.00000000000099999", 0.00000000000099999m, "99999", 17, "J", null, "9.9999E-13" };
            yield return new object?[] { "-0.00000000000099999", -0.00000000000099999m, "-99999", 17, "J", null, "-9.9999E-13" };
            yield return new object?[] { "0.000000000000099999", 0.000000000000099999m, "99999", 18, "J", null, "9.9999E-14" };
            yield return new object?[] { "-0.000000000000099999", -0.000000000000099999m, "-99999", 18, "J", null, "-9.9999E-14" };
            yield return new object?[] { "0.0000000000000099999", 0.0000000000000099999m, "99999", 19, "J", null, "9.9999E-15" };
            yield return new object?[] { "-0.0000000000000099999", -0.0000000000000099999m, "-99999", 19, "J", null, "-9.9999E-15" };
            yield return new object?[] { "0.00000000000000099999", 0.00000000000000099999m, "99999", 20, "J", null, "9.9999E-16" };
            yield return new object?[] { "-0.00000000000000099999", -0.00000000000000099999m, "-99999", 20, "J", null, "-9.9999E-16" };
            yield return new object?[] { "0.000000000000000099999", 0.000000000000000099999m, "99999", 21, "J", null, "9.9999E-17" };
            yield return new object?[] { "-0.000000000000000099999", -0.000000000000000099999m, "-99999", 21, "J", null, "-9.9999E-17" };
            yield return new object?[] { "0.0000000000000000099999", 0.0000000000000000099999m, "99999", 22, "J", null, "9.9999E-18" };
            yield return new object?[] { "-0.0000000000000000099999", -0.0000000000000000099999m, "-99999", 22, "J", null, "-9.9999E-18" };
            yield return new object?[] { "0.00000000000000000099999", 0.00000000000000000099999m, "99999", 23, "J", null, "9.9999E-19" };
            yield return new object?[] { "-0.00000000000000000099999", -0.00000000000000000099999m, "-99999", 23, "J", null, "-9.9999E-19" };
            yield return new object?[] { "0.000000000000000000099999", 0.000000000000000000099999m, "99999", 24, "J", null, "9.9999E-20" };
            yield return new object?[] { "-0.000000000000000000099999", -0.000000000000000000099999m, "-99999", 24, "J", null, "-9.9999E-20" };
            yield return new object?[] { "0.0000000000000000000099999", 0.0000000000000000000099999m, "99999", 25, "J", null, "9.9999E-21" };
            yield return new object?[] { "-0.0000000000000000000099999", -0.0000000000000000000099999m, "-99999", 25, "J", null, "-9.9999E-21" };
            yield return new object?[] { "0.00000000000000000000099999", 0.00000000000000000000099999m, "99999", 26, "J", null, "9.9999E-22" };
            yield return new object?[] { "-0.00000000000000000000099999", -0.00000000000000000000099999m, "-99999", 26, "J", null, "-9.9999E-22" };
            yield return new object?[] { "0.000000000000000000000099999", 0.000000000000000000000099999m, "99999", 27, "J", null, "9.9999E-23" };
            yield return new object?[] { "-0.000000000000000000000099999", -0.000000000000000000000099999m, "-99999", 27, "J", null, "-9.9999E-23" };
            yield return new object?[] { "0.0000000000000000000000099999", 0.0000000000000000000000099999m, "99999", 28, "J", null, "9.9999E-24" };
            yield return new object?[] { "-0.0000000000000000000000099999", -0.0000000000000000000000099999m, "-99999", 28, "J", null, "-9.9999E-24" };
            yield return new object?[] { "123456789", 123456789m, "123456789", 0, "J", null, "123456789" };
            yield return new object?[] { "-123456789", -123456789m, "-123456789", 0, "J", null, "-123456789" };
            yield return new object?[] { "12345678.9", 12345678.9m, "123456789", 1, "J", null, "12345678.9" };
            yield return new object?[] { "-12345678.9", -12345678.9m, "-123456789", 1, "J", null, "-12345678.9" };
            yield return new object?[] { "1234567.89", 1234567.89m, "123456789", 2, "J", null, "1234567.89" };
            yield return new object?[] { "-1234567.89", -1234567.89m, "-123456789", 2, "J", null, "-1234567.89" };
            yield return new object?[] { "123456.789", 123456.789m, "123456789", 3, "J", null, "123456.789" };
            yield return new object?[] { "-123456.789", -123456.789m, "-123456789", 3, "J", null, "-123456.789" };
            yield return new object?[] { "12345.6789", 12345.6789m, "123456789", 4, "J", null, "12345.6789" };
            yield return new object?[] { "-12345.6789", -12345.6789m, "-123456789", 4, "J", null, "-12345.6789" };
            yield return new object?[] { "1234.56789", 1234.56789m, "123456789", 5, "J", null, "1234.56789" };
            yield return new object?[] { "-1234.56789", -1234.56789m, "-123456789", 5, "J", null, "-1234.56789" };
            yield return new object?[] { "123.456789", 123.456789m, "123456789", 6, "J", null, "123.456789" };
            yield return new object?[] { "-123.456789", -123.456789m, "-123456789", 6, "J", null, "-123.456789" };
            yield return new object?[] { "12.3456789", 12.3456789m, "123456789", 7, "J", null, "12.3456789" };
            yield return new object?[] { "-12.3456789", -12.3456789m, "-123456789", 7, "J", null, "-12.3456789" };
            yield return new object?[] { "1.23456789", 1.23456789m, "123456789", 8, "J", null, "1.23456789" };
            yield return new object?[] { "-1.23456789", -1.23456789m, "-123456789", 8, "J", null, "-1.23456789" };
            yield return new object?[] { "0.123456789", 0.123456789m, "123456789", 9, "J", null, "0.123456789" };
            yield return new object?[] { "-0.123456789", -0.123456789m, "-123456789", 9, "J", null, "-0.123456789" };
            yield return new object?[] { "0.0123456789", 0.0123456789m, "123456789", 10, "J", null, "0.0123456789" };
            yield return new object?[] { "-0.0123456789", -0.0123456789m, "-123456789", 10, "J", null, "-0.0123456789" };
            yield return new object?[] { "0.00123456789", 0.00123456789m, "123456789", 11, "J", null, "0.00123456789" };
            yield return new object?[] { "-0.00123456789", -0.00123456789m, "-123456789", 11, "J", null, "-0.00123456789" };
            yield return new object?[] { "0.000123456789", 0.000123456789m, "123456789", 12, "J", null, "0.000123456789" };
            yield return new object?[] { "-0.000123456789", -0.000123456789m, "-123456789", 12, "J", null, "-0.000123456789" };
            yield return new object?[] { "0.0000123456789", 0.0000123456789m, "123456789", 13, "J", null, "0.0000123456789" };
            yield return new object?[] { "-0.0000123456789", -0.0000123456789m, "-123456789", 13, "J", null, "-0.0000123456789" };
            yield return new object?[] { "0.00000123456789", 0.00000123456789m, "123456789", 14, "J", null, "0.00000123456789" };
            yield return new object?[] { "-0.00000123456789", -0.00000123456789m, "-123456789", 14, "J", null, "-0.00000123456789" };
            yield return new object?[] { "0.000000123456789", 0.000000123456789m, "123456789", 15, "J", null, "1.23456789E-7" };
            yield return new object?[] { "-0.000000123456789", -0.000000123456789m, "-123456789", 15, "J", null, "-1.23456789E-7" };
            yield return new object?[] { "0.0000000123456789", 0.0000000123456789m, "123456789", 16, "J", null, "1.23456789E-8" };
            yield return new object?[] { "-0.0000000123456789", -0.0000000123456789m, "-123456789", 16, "J", null, "-1.23456789E-8" };
            yield return new object?[] { "0.00000000123456789", 0.00000000123456789m, "123456789", 17, "J", null, "1.23456789E-9" };
            yield return new object?[] { "-0.00000000123456789", -0.00000000123456789m, "-123456789", 17, "J", null, "-1.23456789E-9" };
            yield return new object?[] { "0.000000000123456789", 0.000000000123456789m, "123456789", 18, "J", null, "1.23456789E-10" };
            yield return new object?[] { "-0.000000000123456789", -0.000000000123456789m, "-123456789", 18, "J", null, "-1.23456789E-10" };
            yield return new object?[] { "0.0000000000123456789", 0.0000000000123456789m, "123456789", 19, "J", null, "1.23456789E-11" };
            yield return new object?[] { "-0.0000000000123456789", -0.0000000000123456789m, "-123456789", 19, "J", null, "-1.23456789E-11" };
            yield return new object?[] { "0.00000000000123456789", 0.00000000000123456789m, "123456789", 20, "J", null, "1.23456789E-12" };
            yield return new object?[] { "-0.00000000000123456789", -0.00000000000123456789m, "-123456789", 20, "J", null, "-1.23456789E-12" };
            yield return new object?[] { "0.000000000000123456789", 0.000000000000123456789m, "123456789", 21, "J", null, "1.23456789E-13" };
            yield return new object?[] { "-0.000000000000123456789", -0.000000000000123456789m, "-123456789", 21, "J", null, "-1.23456789E-13" };
            yield return new object?[] { "0.0000000000000123456789", 0.0000000000000123456789m, "123456789", 22, "J", null, "1.23456789E-14" };
            yield return new object?[] { "-0.0000000000000123456789", -0.0000000000000123456789m, "-123456789", 22, "J", null, "-1.23456789E-14" };
            yield return new object?[] { "0.00000000000000123456789", 0.00000000000000123456789m, "123456789", 23, "J", null, "1.23456789E-15" };
            yield return new object?[] { "-0.00000000000000123456789", -0.00000000000000123456789m, "-123456789", 23, "J", null, "-1.23456789E-15" };
            yield return new object?[] { "0.000000000000000123456789", 0.000000000000000123456789m, "123456789", 24, "J", null, "1.23456789E-16" };
            yield return new object?[] { "-0.000000000000000123456789", -0.000000000000000123456789m, "-123456789", 24, "J", null, "-1.23456789E-16" };
            yield return new object?[] { "0.0000000000000000123456789", 0.0000000000000000123456789m, "123456789", 25, "J", null, "1.23456789E-17" };
            yield return new object?[] { "-0.0000000000000000123456789", -0.0000000000000000123456789m, "-123456789", 25, "J", null, "-1.23456789E-17" };
            yield return new object?[] { "0.00000000000000000123456789", 0.00000000000000000123456789m, "123456789", 26, "J", null, "1.23456789E-18" };
            yield return new object?[] { "-0.00000000000000000123456789", -0.00000000000000000123456789m, "-123456789", 26, "J", null, "-1.23456789E-18" };
            yield return new object?[] { "0.000000000000000000123456789", 0.000000000000000000123456789m, "123456789", 27, "J", null, "1.23456789E-19" };
            yield return new object?[] { "-0.000000000000000000123456789", -0.000000000000000000123456789m, "-123456789", 27, "J", null, "-1.23456789E-19" };
            yield return new object?[] { "0.0000000000000000000123456789", 0.0000000000000000000123456789m, "123456789", 28, "J", null, "1.23456789E-20" };
            yield return new object?[] { "-0.0000000000000000000123456789", -0.0000000000000000000123456789m, "-123456789", 28, "J", null, "-1.23456789E-20" };
            yield return new object?[] { "999999999", 999999999m, "999999999", 0, "J", null, "999999999" };
            yield return new object?[] { "-999999999", -999999999m, "-999999999", 0, "J", null, "-999999999" };
            yield return new object?[] { "99999999.9", 99999999.9m, "999999999", 1, "J", null, "99999999.9" };
            yield return new object?[] { "-99999999.9", -99999999.9m, "-999999999", 1, "J", null, "-99999999.9" };
            yield return new object?[] { "9999999.99", 9999999.99m, "999999999", 2, "J", null, "9999999.99" };
            yield return new object?[] { "-9999999.99", -9999999.99m, "-999999999", 2, "J", null, "-9999999.99" };
            yield return new object?[] { "999999.999", 999999.999m, "999999999", 3, "J", null, "999999.999" };
            yield return new object?[] { "-999999.999", -999999.999m, "-999999999", 3, "J", null, "-999999.999" };
            yield return new object?[] { "99999.9999", 99999.9999m, "999999999", 4, "J", null, "99999.9999" };
            yield return new object?[] { "-99999.9999", -99999.9999m, "-999999999", 4, "J", null, "-99999.9999" };
            yield return new object?[] { "9999.99999", 9999.99999m, "999999999", 5, "J", null, "9999.99999" };
            yield return new object?[] { "-9999.99999", -9999.99999m, "-999999999", 5, "J", null, "-9999.99999" };
            yield return new object?[] { "999.999999", 999.999999m, "999999999", 6, "J", null, "999.999999" };
            yield return new object?[] { "-999.999999", -999.999999m, "-999999999", 6, "J", null, "-999.999999" };
            yield return new object?[] { "99.9999999", 99.9999999m, "999999999", 7, "J", null, "99.9999999" };
            yield return new object?[] { "-99.9999999", -99.9999999m, "-999999999", 7, "J", null, "-99.9999999" };
            yield return new object?[] { "9.99999999", 9.99999999m, "999999999", 8, "J", null, "9.99999999" };
            yield return new object?[] { "-9.99999999", -9.99999999m, "-999999999", 8, "J", null, "-9.99999999" };
            yield return new object?[] { "0.999999999", 0.999999999m, "999999999", 9, "J", null, "0.999999999" };
            yield return new object?[] { "-0.999999999", -0.999999999m, "-999999999", 9, "J", null, "-0.999999999" };
            yield return new object?[] { "0.0999999999", 0.0999999999m, "999999999", 10, "J", null, "0.0999999999" };
            yield return new object?[] { "-0.0999999999", -0.0999999999m, "-999999999", 10, "J", null, "-0.0999999999" };
            yield return new object?[] { "0.00999999999", 0.00999999999m, "999999999", 11, "J", null, "0.00999999999" };
            yield return new object?[] { "-0.00999999999", -0.00999999999m, "-999999999", 11, "J", null, "-0.00999999999" };
            yield return new object?[] { "0.000999999999", 0.000999999999m, "999999999", 12, "J", null, "0.000999999999" };
            yield return new object?[] { "-0.000999999999", -0.000999999999m, "-999999999", 12, "J", null, "-0.000999999999" };
            yield return new object?[] { "0.0000999999999", 0.0000999999999m, "999999999", 13, "J", null, "0.0000999999999" };
            yield return new object?[] { "-0.0000999999999", -0.0000999999999m, "-999999999", 13, "J", null, "-0.0000999999999" };
            yield return new object?[] { "0.00000999999999", 0.00000999999999m, "999999999", 14, "J", null, "0.00000999999999" };
            yield return new object?[] { "-0.00000999999999", -0.00000999999999m, "-999999999", 14, "J", null, "-0.00000999999999" };
            yield return new object?[] { "0.000000999999999", 0.000000999999999m, "999999999", 15, "J", null, "9.99999999E-7" };
            yield return new object?[] { "-0.000000999999999", -0.000000999999999m, "-999999999", 15, "J", null, "-9.99999999E-7" };
            yield return new object?[] { "0.0000000999999999", 0.0000000999999999m, "999999999", 16, "J", null, "9.99999999E-8" };
            yield return new object?[] { "-0.0000000999999999", -0.0000000999999999m, "-999999999", 16, "J", null, "-9.99999999E-8" };
            yield return new object?[] { "0.00000000999999999", 0.00000000999999999m, "999999999", 17, "J", null, "9.99999999E-9" };
            yield return new object?[] { "-0.00000000999999999", -0.00000000999999999m, "-999999999", 17, "J", null, "-9.99999999E-9" };
            yield return new object?[] { "0.000000000999999999", 0.000000000999999999m, "999999999", 18, "J", null, "9.99999999E-10" };
            yield return new object?[] { "-0.000000000999999999", -0.000000000999999999m, "-999999999", 18, "J", null, "-9.99999999E-10" };
            yield return new object?[] { "0.0000000000999999999", 0.0000000000999999999m, "999999999", 19, "J", null, "9.99999999E-11" };
            yield return new object?[] { "-0.0000000000999999999", -0.0000000000999999999m, "-999999999", 19, "J", null, "-9.99999999E-11" };
            yield return new object?[] { "0.00000000000999999999", 0.00000000000999999999m, "999999999", 20, "J", null, "9.99999999E-12" };
            yield return new object?[] { "-0.00000000000999999999", -0.00000000000999999999m, "-999999999", 20, "J", null, "-9.99999999E-12" };
            yield return new object?[] { "0.000000000000999999999", 0.000000000000999999999m, "999999999", 21, "J", null, "9.99999999E-13" };
            yield return new object?[] { "-0.000000000000999999999", -0.000000000000999999999m, "-999999999", 21, "J", null, "-9.99999999E-13" };
            yield return new object?[] { "0.0000000000000999999999", 0.0000000000000999999999m, "999999999", 22, "J", null, "9.99999999E-14" };
            yield return new object?[] { "-0.0000000000000999999999", -0.0000000000000999999999m, "-999999999", 22, "J", null, "-9.99999999E-14" };
            yield return new object?[] { "0.00000000000000999999999", 0.00000000000000999999999m, "999999999", 23, "J", null, "9.99999999E-15" };
            yield return new object?[] { "-0.00000000000000999999999", -0.00000000000000999999999m, "-999999999", 23, "J", null, "-9.99999999E-15" };
            yield return new object?[] { "0.000000000000000999999999", 0.000000000000000999999999m, "999999999", 24, "J", null, "9.99999999E-16" };
            yield return new object?[] { "-0.000000000000000999999999", -0.000000000000000999999999m, "-999999999", 24, "J", null, "-9.99999999E-16" };
            yield return new object?[] { "0.0000000000000000999999999", 0.0000000000000000999999999m, "999999999", 25, "J", null, "9.99999999E-17" };
            yield return new object?[] { "-0.0000000000000000999999999", -0.0000000000000000999999999m, "-999999999", 25, "J", null, "-9.99999999E-17" };
            yield return new object?[] { "0.00000000000000000999999999", 0.00000000000000000999999999m, "999999999", 26, "J", null, "9.99999999E-18" };
            yield return new object?[] { "-0.00000000000000000999999999", -0.00000000000000000999999999m, "-999999999", 26, "J", null, "-9.99999999E-18" };
            yield return new object?[] { "0.000000000000000000999999999", 0.000000000000000000999999999m, "999999999", 27, "J", null, "9.99999999E-19" };
            yield return new object?[] { "-0.000000000000000000999999999", -0.000000000000000000999999999m, "-999999999", 27, "J", null, "-9.99999999E-19" };
            yield return new object?[] { "0.0000000000000000000999999999", 0.0000000000000000000999999999m, "999999999", 28, "J", null, "9.99999999E-20" };
            yield return new object?[] { "-0.0000000000000000000999999999", -0.0000000000000000000999999999m, "-999999999", 28, "J", null, "-9.99999999E-20" };
            yield return new object?[] { "1234567890123456789", 1234567890123456789m, "1234567890123456789", 0, "J", null, "1234567890123456789" };
            yield return new object?[] { "-1234567890123456789", -1234567890123456789m, "-1234567890123456789", 0, "J", null, "-1234567890123456789" };
            yield return new object?[] { "123456789012345678.9", 123456789012345678.9m, "1234567890123456789", 1, "J", null, "123456789012345678.9" };
            yield return new object?[] { "-123456789012345678.9", -123456789012345678.9m, "-1234567890123456789", 1, "J", null, "-123456789012345678.9" };
            yield return new object?[] { "12345678901234567.89", 12345678901234567.89m, "1234567890123456789", 2, "J", null, "12345678901234567.89" };
            yield return new object?[] { "-12345678901234567.89", -12345678901234567.89m, "-1234567890123456789", 2, "J", null, "-12345678901234567.89" };
            yield return new object?[] { "1234567890123456.789", 1234567890123456.789m, "1234567890123456789", 3, "J", null, "1234567890123456.789" };
            yield return new object?[] { "-1234567890123456.789", -1234567890123456.789m, "-1234567890123456789", 3, "J", null, "-1234567890123456.789" };
            yield return new object?[] { "123456789012345.6789", 123456789012345.6789m, "1234567890123456789", 4, "J", null, "123456789012345.6789" };
            yield return new object?[] { "-123456789012345.6789", -123456789012345.6789m, "-1234567890123456789", 4, "J", null, "-123456789012345.6789" };
            yield return new object?[] { "12345678901234.56789", 12345678901234.56789m, "1234567890123456789", 5, "J", null, "12345678901234.56789" };
            yield return new object?[] { "-12345678901234.56789", -12345678901234.56789m, "-1234567890123456789", 5, "J", null, "-12345678901234.56789" };
            yield return new object?[] { "1234567890123.456789", 1234567890123.456789m, "1234567890123456789", 6, "J", null, "1234567890123.456789" };
            yield return new object?[] { "-1234567890123.456789", -1234567890123.456789m, "-1234567890123456789", 6, "J", null, "-1234567890123.456789" };
            yield return new object?[] { "123456789012.3456789", 123456789012.3456789m, "1234567890123456789", 7, "J", null, "123456789012.3456789" };
            yield return new object?[] { "-123456789012.3456789", -123456789012.3456789m, "-1234567890123456789", 7, "J", null, "-123456789012.3456789" };
            yield return new object?[] { "12345678901.23456789", 12345678901.23456789m, "1234567890123456789", 8, "J", null, "12345678901.23456789" };
            yield return new object?[] { "-12345678901.23456789", -12345678901.23456789m, "-1234567890123456789", 8, "J", null, "-12345678901.23456789" };
            yield return new object?[] { "1234567890.123456789", 1234567890.123456789m, "1234567890123456789", 9, "J", null, "1234567890.123456789" };
            yield return new object?[] { "-1234567890.123456789", -1234567890.123456789m, "-1234567890123456789", 9, "J", null, "-1234567890.123456789" };
            yield return new object?[] { "123456789.0123456789", 123456789.0123456789m, "1234567890123456789", 10, "J", null, "123456789.0123456789" };
            yield return new object?[] { "-123456789.0123456789", -123456789.0123456789m, "-1234567890123456789", 10, "J", null, "-123456789.0123456789" };
            yield return new object?[] { "12345678.90123456789", 12345678.90123456789m, "1234567890123456789", 11, "J", null, "12345678.90123456789" };
            yield return new object?[] { "-12345678.90123456789", -12345678.90123456789m, "-1234567890123456789", 11, "J", null, "-12345678.90123456789" };
            yield return new object?[] { "1234567.890123456789", 1234567.890123456789m, "1234567890123456789", 12, "J", null, "1234567.890123456789" };
            yield return new object?[] { "-1234567.890123456789", -1234567.890123456789m, "-1234567890123456789", 12, "J", null, "-1234567.890123456789" };
            yield return new object?[] { "123456.7890123456789", 123456.7890123456789m, "1234567890123456789", 13, "J", null, "123456.7890123456789" };
            yield return new object?[] { "-123456.7890123456789", -123456.7890123456789m, "-1234567890123456789", 13, "J", null, "-123456.7890123456789" };
            yield return new object?[] { "12345.67890123456789", 12345.67890123456789m, "1234567890123456789", 14, "J", null, "12345.67890123456789" };
            yield return new object?[] { "-12345.67890123456789", -12345.67890123456789m, "-1234567890123456789", 14, "J", null, "-12345.67890123456789" };
            yield return new object?[] { "1234.567890123456789", 1234.567890123456789m, "1234567890123456789", 15, "J", null, "1234.567890123456789" };
            yield return new object?[] { "-1234.567890123456789", -1234.567890123456789m, "-1234567890123456789", 15, "J", null, "-1234.567890123456789" };
            yield return new object?[] { "123.4567890123456789", 123.4567890123456789m, "1234567890123456789", 16, "J", null, "123.4567890123456789" };
            yield return new object?[] { "-123.4567890123456789", -123.4567890123456789m, "-1234567890123456789", 16, "J", null, "-123.4567890123456789" };
            yield return new object?[] { "12.34567890123456789", 12.34567890123456789m, "1234567890123456789", 17, "J", null, "12.34567890123456789" };
            yield return new object?[] { "-12.34567890123456789", -12.34567890123456789m, "-1234567890123456789", 17, "J", null, "-12.34567890123456789" };
            yield return new object?[] { "1.234567890123456789", 1.234567890123456789m, "1234567890123456789", 18, "J", null, "1.234567890123456789" };
            yield return new object?[] { "-1.234567890123456789", -1.234567890123456789m, "-1234567890123456789", 18, "J", null, "-1.234567890123456789" };
            yield return new object?[] { "0.1234567890123456789", 0.1234567890123456789m, "1234567890123456789", 19, "J", null, "0.1234567890123456789" };
            yield return new object?[] { "-0.1234567890123456789", -0.1234567890123456789m, "-1234567890123456789", 19, "J", null, "-0.1234567890123456789" };
            yield return new object?[] { "0.01234567890123456789", 0.01234567890123456789m, "1234567890123456789", 20, "J", null, "0.01234567890123456789" };
            yield return new object?[] { "-0.01234567890123456789", -0.01234567890123456789m, "-1234567890123456789", 20, "J", null, "-0.01234567890123456789" };
            yield return new object?[] { "0.001234567890123456789", 0.001234567890123456789m, "1234567890123456789", 21, "J", null, "0.001234567890123456789" };
            yield return new object?[] { "-0.001234567890123456789", -0.001234567890123456789m, "-1234567890123456789", 21, "J", null, "-0.001234567890123456789" };
            yield return new object?[] { "0.0001234567890123456789", 0.0001234567890123456789m, "1234567890123456789", 22, "J", null, "0.0001234567890123456789" };
            yield return new object?[] { "-0.0001234567890123456789", -0.0001234567890123456789m, "-1234567890123456789", 22, "J", null, "-0.0001234567890123456789" };
            yield return new object?[] { "0.00001234567890123456789", 0.00001234567890123456789m, "1234567890123456789", 23, "J", null, "0.00001234567890123456789" };
            yield return new object?[] { "-0.00001234567890123456789", -0.00001234567890123456789m, "-1234567890123456789", 23, "J", null, "-0.00001234567890123456789" };
            yield return new object?[] { "0.000001234567890123456789", 0.000001234567890123456789m, "1234567890123456789", 24, "J", null, "0.000001234567890123456789" };
            yield return new object?[] { "-0.000001234567890123456789", -0.000001234567890123456789m, "-1234567890123456789", 24, "J", null, "-0.000001234567890123456789" };
            yield return new object?[] { "0.0000001234567890123456789", 0.0000001234567890123456789m, "1234567890123456789", 25, "J", null, "1.234567890123456789E-7" };
            yield return new object?[] { "-0.0000001234567890123456789", -0.0000001234567890123456789m, "-1234567890123456789", 25, "J", null, "-1.234567890123456789E-7" };
            yield return new object?[] { "0.00000001234567890123456789", 0.00000001234567890123456789m, "1234567890123456789", 26, "J", null, "1.234567890123456789E-8" };
            yield return new object?[] { "-0.00000001234567890123456789", -0.00000001234567890123456789m, "-1234567890123456789", 26, "J", null, "-1.234567890123456789E-8" };
            yield return new object?[] { "0.000000001234567890123456789", 0.000000001234567890123456789m, "1234567890123456789", 27, "J", null, "1.234567890123456789E-9" };
            yield return new object?[] { "-0.000000001234567890123456789", -0.000000001234567890123456789m, "-1234567890123456789", 27, "J", null, "-1.234567890123456789E-9" };
            yield return new object?[] { "0.0000000001234567890123456789", 0.0000000001234567890123456789m, "1234567890123456789", 28, "J", null, "1.234567890123456789E-10" };
            yield return new object?[] { "-0.0000000001234567890123456789", -0.0000000001234567890123456789m, "-1234567890123456789", 28, "J", null, "-1.234567890123456789E-10" };
            yield return new object?[] { "9999999999999999999", 9999999999999999999m, "9999999999999999999", 0, "J", null, "9999999999999999999" };
            yield return new object?[] { "-9999999999999999999", -9999999999999999999m, "-9999999999999999999", 0, "J", null, "-9999999999999999999" };
            yield return new object?[] { "999999999999999999.9", 999999999999999999.9m, "9999999999999999999", 1, "J", null, "999999999999999999.9" };
            yield return new object?[] { "-999999999999999999.9", -999999999999999999.9m, "-9999999999999999999", 1, "J", null, "-999999999999999999.9" };
            yield return new object?[] { "99999999999999999.99", 99999999999999999.99m, "9999999999999999999", 2, "J", null, "99999999999999999.99" };
            yield return new object?[] { "-99999999999999999.99", -99999999999999999.99m, "-9999999999999999999", 2, "J", null, "-99999999999999999.99" };
            yield return new object?[] { "9999999999999999.999", 9999999999999999.999m, "9999999999999999999", 3, "J", null, "9999999999999999.999" };
            yield return new object?[] { "-9999999999999999.999", -9999999999999999.999m, "-9999999999999999999", 3, "J", null, "-9999999999999999.999" };
            yield return new object?[] { "999999999999999.9999", 999999999999999.9999m, "9999999999999999999", 4, "J", null, "999999999999999.9999" };
            yield return new object?[] { "-999999999999999.9999", -999999999999999.9999m, "-9999999999999999999", 4, "J", null, "-999999999999999.9999" };
            yield return new object?[] { "99999999999999.99999", 99999999999999.99999m, "9999999999999999999", 5, "J", null, "99999999999999.99999" };
            yield return new object?[] { "-99999999999999.99999", -99999999999999.99999m, "-9999999999999999999", 5, "J", null, "-99999999999999.99999" };
            yield return new object?[] { "9999999999999.999999", 9999999999999.999999m, "9999999999999999999", 6, "J", null, "9999999999999.999999" };
            yield return new object?[] { "-9999999999999.999999", -9999999999999.999999m, "-9999999999999999999", 6, "J", null, "-9999999999999.999999" };
            yield return new object?[] { "999999999999.9999999", 999999999999.9999999m, "9999999999999999999", 7, "J", null, "999999999999.9999999" };
            yield return new object?[] { "-999999999999.9999999", -999999999999.9999999m, "-9999999999999999999", 7, "J", null, "-999999999999.9999999" };
            yield return new object?[] { "99999999999.99999999", 99999999999.99999999m, "9999999999999999999", 8, "J", null, "99999999999.99999999" };
            yield return new object?[] { "-99999999999.99999999", -99999999999.99999999m, "-9999999999999999999", 8, "J", null, "-99999999999.99999999" };
            yield return new object?[] { "9999999999.999999999", 9999999999.999999999m, "9999999999999999999", 9, "J", null, "9999999999.999999999" };
            yield return new object?[] { "-9999999999.999999999", -9999999999.999999999m, "-9999999999999999999", 9, "J", null, "-9999999999.999999999" };
            yield return new object?[] { "999999999.9999999999", 999999999.9999999999m, "9999999999999999999", 10, "J", null, "999999999.9999999999" };
            yield return new object?[] { "-999999999.9999999999", -999999999.9999999999m, "-9999999999999999999", 10, "J", null, "-999999999.9999999999" };
            yield return new object?[] { "99999999.99999999999", 99999999.99999999999m, "9999999999999999999", 11, "J", null, "99999999.99999999999" };
            yield return new object?[] { "-99999999.99999999999", -99999999.99999999999m, "-9999999999999999999", 11, "J", null, "-99999999.99999999999" };
            yield return new object?[] { "9999999.999999999999", 9999999.999999999999m, "9999999999999999999", 12, "J", null, "9999999.999999999999" };
            yield return new object?[] { "-9999999.999999999999", -9999999.999999999999m, "-9999999999999999999", 12, "J", null, "-9999999.999999999999" };
            yield return new object?[] { "999999.9999999999999", 999999.9999999999999m, "9999999999999999999", 13, "J", null, "999999.9999999999999" };
            yield return new object?[] { "-999999.9999999999999", -999999.9999999999999m, "-9999999999999999999", 13, "J", null, "-999999.9999999999999" };
            yield return new object?[] { "99999.99999999999999", 99999.99999999999999m, "9999999999999999999", 14, "J", null, "99999.99999999999999" };
            yield return new object?[] { "-99999.99999999999999", -99999.99999999999999m, "-9999999999999999999", 14, "J", null, "-99999.99999999999999" };
            yield return new object?[] { "9999.999999999999999", 9999.999999999999999m, "9999999999999999999", 15, "J", null, "9999.999999999999999" };
            yield return new object?[] { "-9999.999999999999999", -9999.999999999999999m, "-9999999999999999999", 15, "J", null, "-9999.999999999999999" };
            yield return new object?[] { "999.9999999999999999", 999.9999999999999999m, "9999999999999999999", 16, "J", null, "999.9999999999999999" };
            yield return new object?[] { "-999.9999999999999999", -999.9999999999999999m, "-9999999999999999999", 16, "J", null, "-999.9999999999999999" };
            yield return new object?[] { "99.99999999999999999", 99.99999999999999999m, "9999999999999999999", 17, "J", null, "99.99999999999999999" };
            yield return new object?[] { "-99.99999999999999999", -99.99999999999999999m, "-9999999999999999999", 17, "J", null, "-99.99999999999999999" };
            yield return new object?[] { "9.999999999999999999", 9.999999999999999999m, "9999999999999999999", 18, "J", null, "9.999999999999999999" };
            yield return new object?[] { "-9.999999999999999999", -9.999999999999999999m, "-9999999999999999999", 18, "J", null, "-9.999999999999999999" };
            yield return new object?[] { "0.9999999999999999999", 0.9999999999999999999m, "9999999999999999999", 19, "J", null, "0.9999999999999999999" };
            yield return new object?[] { "-0.9999999999999999999", -0.9999999999999999999m, "-9999999999999999999", 19, "J", null, "-0.9999999999999999999" };
            yield return new object?[] { "0.09999999999999999999", 0.09999999999999999999m, "9999999999999999999", 20, "J", null, "0.09999999999999999999" };
            yield return new object?[] { "-0.09999999999999999999", -0.09999999999999999999m, "-9999999999999999999", 20, "J", null, "-0.09999999999999999999" };
            yield return new object?[] { "0.009999999999999999999", 0.009999999999999999999m, "9999999999999999999", 21, "J", null, "0.009999999999999999999" };
            yield return new object?[] { "-0.009999999999999999999", -0.009999999999999999999m, "-9999999999999999999", 21, "J", null, "-0.009999999999999999999" };
            yield return new object?[] { "0.0009999999999999999999", 0.0009999999999999999999m, "9999999999999999999", 22, "J", null, "0.0009999999999999999999" };
            yield return new object?[] { "-0.0009999999999999999999", -0.0009999999999999999999m, "-9999999999999999999", 22, "J", null, "-0.0009999999999999999999" };
            yield return new object?[] { "0.00009999999999999999999", 0.00009999999999999999999m, "9999999999999999999", 23, "J", null, "0.00009999999999999999999" };
            yield return new object?[] { "-0.00009999999999999999999", -0.00009999999999999999999m, "-9999999999999999999", 23, "J", null, "-0.00009999999999999999999" };
            yield return new object?[] { "0.000009999999999999999999", 0.000009999999999999999999m, "9999999999999999999", 24, "J", null, "0.000009999999999999999999" };
            yield return new object?[] { "-0.000009999999999999999999", -0.000009999999999999999999m, "-9999999999999999999", 24, "J", null, "-0.000009999999999999999999" };
            yield return new object?[] { "0.0000009999999999999999999", 0.0000009999999999999999999m, "9999999999999999999", 25, "J", null, "9.999999999999999999E-7" };
            yield return new object?[] { "-0.0000009999999999999999999", -0.0000009999999999999999999m, "-9999999999999999999", 25, "J", null, "-9.999999999999999999E-7" };
            yield return new object?[] { "0.00000009999999999999999999", 0.00000009999999999999999999m, "9999999999999999999", 26, "J", null, "9.999999999999999999E-8" };
            yield return new object?[] { "-0.00000009999999999999999999", -0.00000009999999999999999999m, "-9999999999999999999", 26, "J", null, "-9.999999999999999999E-8" };
            yield return new object?[] { "0.000000009999999999999999999", 0.000000009999999999999999999m, "9999999999999999999", 27, "J", null, "9.999999999999999999E-9" };
            yield return new object?[] { "-0.000000009999999999999999999", -0.000000009999999999999999999m, "-9999999999999999999", 27, "J", null, "-9.999999999999999999E-9" };
            yield return new object?[] { "0.0000000009999999999999999999", 0.0000000009999999999999999999m, "9999999999999999999", 28, "J", null, "9.999999999999999999E-10" };
            yield return new object?[] { "-0.0000000009999999999999999999", -0.0000000009999999999999999999m, "-9999999999999999999", 28, "J", null, "-9.999999999999999999E-10" };
            yield return new object?[] { "12345678901234567890123456789", 12345678901234567890123456789m, "12345678901234567890123456789", 0, "J", null, "12345678901234567890123456789" };
            yield return new object?[] { "-12345678901234567890123456789", -12345678901234567890123456789m, "-12345678901234567890123456789", 0, "J", null, "-12345678901234567890123456789" };
            yield return new object?[] { "1234567890123456789012345678.9", 1234567890123456789012345678.9m, "12345678901234567890123456789", 1, "J", null, "1234567890123456789012345678.9" };
            yield return new object?[] { "-1234567890123456789012345678.9", -1234567890123456789012345678.9m, "-12345678901234567890123456789", 1, "J", null, "-1234567890123456789012345678.9" };
            yield return new object?[] { "123456789012345678901234567.89", 123456789012345678901234567.89m, "12345678901234567890123456789", 2, "J", null, "123456789012345678901234567.89" };
            yield return new object?[] { "-123456789012345678901234567.89", -123456789012345678901234567.89m, "-12345678901234567890123456789", 2, "J", null, "-123456789012345678901234567.89" };
            yield return new object?[] { "12345678901234567890123456.789", 12345678901234567890123456.789m, "12345678901234567890123456789", 3, "J", null, "12345678901234567890123456.789" };
            yield return new object?[] { "-12345678901234567890123456.789", -12345678901234567890123456.789m, "-12345678901234567890123456789", 3, "J", null, "-12345678901234567890123456.789" };
            yield return new object?[] { "1234567890123456789012345.6789", 1234567890123456789012345.6789m, "12345678901234567890123456789", 4, "J", null, "1234567890123456789012345.6789" };
            yield return new object?[] { "-1234567890123456789012345.6789", -1234567890123456789012345.6789m, "-12345678901234567890123456789", 4, "J", null, "-1234567890123456789012345.6789" };
            yield return new object?[] { "123456789012345678901234.56789", 123456789012345678901234.56789m, "12345678901234567890123456789", 5, "J", null, "123456789012345678901234.56789" };
            yield return new object?[] { "-123456789012345678901234.56789", -123456789012345678901234.56789m, "-12345678901234567890123456789", 5, "J", null, "-123456789012345678901234.56789" };
            yield return new object?[] { "12345678901234567890123.456789", 12345678901234567890123.456789m, "12345678901234567890123456789", 6, "J", null, "12345678901234567890123.456789" };
            yield return new object?[] { "-12345678901234567890123.456789", -12345678901234567890123.456789m, "-12345678901234567890123456789", 6, "J", null, "-12345678901234567890123.456789" };
            yield return new object?[] { "1234567890123456789012.3456789", 1234567890123456789012.3456789m, "12345678901234567890123456789", 7, "J", null, "1234567890123456789012.3456789" };
            yield return new object?[] { "-1234567890123456789012.3456789", -1234567890123456789012.3456789m, "-12345678901234567890123456789", 7, "J", null, "-1234567890123456789012.3456789" };
            yield return new object?[] { "123456789012345678901.23456789", 123456789012345678901.23456789m, "12345678901234567890123456789", 8, "J", null, "123456789012345678901.23456789" };
            yield return new object?[] { "-123456789012345678901.23456789", -123456789012345678901.23456789m, "-12345678901234567890123456789", 8, "J", null, "-123456789012345678901.23456789" };
            yield return new object?[] { "12345678901234567890.123456789", 12345678901234567890.123456789m, "12345678901234567890123456789", 9, "J", null, "12345678901234567890.123456789" };
            yield return new object?[] { "-12345678901234567890.123456789", -12345678901234567890.123456789m, "-12345678901234567890123456789", 9, "J", null, "-12345678901234567890.123456789" };
            yield return new object?[] { "1234567890123456789.0123456789", 1234567890123456789.0123456789m, "12345678901234567890123456789", 10, "J", null, "1234567890123456789.0123456789" };
            yield return new object?[] { "-1234567890123456789.0123456789", -1234567890123456789.0123456789m, "-12345678901234567890123456789", 10, "J", null, "-1234567890123456789.0123456789" };
            yield return new object?[] { "123456789012345678.90123456789", 123456789012345678.90123456789m, "12345678901234567890123456789", 11, "J", null, "123456789012345678.90123456789" };
            yield return new object?[] { "-123456789012345678.90123456789", -123456789012345678.90123456789m, "-12345678901234567890123456789", 11, "J", null, "-123456789012345678.90123456789" };
            yield return new object?[] { "12345678901234567.890123456789", 12345678901234567.890123456789m, "12345678901234567890123456789", 12, "J", null, "12345678901234567.890123456789" };
            yield return new object?[] { "-12345678901234567.890123456789", -12345678901234567.890123456789m, "-12345678901234567890123456789", 12, "J", null, "-12345678901234567.890123456789" };
            yield return new object?[] { "1234567890123456.7890123456789", 1234567890123456.7890123456789m, "12345678901234567890123456789", 13, "J", null, "1234567890123456.7890123456789" };
            yield return new object?[] { "-1234567890123456.7890123456789", -1234567890123456.7890123456789m, "-12345678901234567890123456789", 13, "J", null, "-1234567890123456.7890123456789" };
            yield return new object?[] { "123456789012345.67890123456789", 123456789012345.67890123456789m, "12345678901234567890123456789", 14, "J", null, "123456789012345.67890123456789" };
            yield return new object?[] { "-123456789012345.67890123456789", -123456789012345.67890123456789m, "-12345678901234567890123456789", 14, "J", null, "-123456789012345.67890123456789" };
            yield return new object?[] { "12345678901234.567890123456789", 12345678901234.567890123456789m, "12345678901234567890123456789", 15, "J", null, "12345678901234.567890123456789" };
            yield return new object?[] { "-12345678901234.567890123456789", -12345678901234.567890123456789m, "-12345678901234567890123456789", 15, "J", null, "-12345678901234.567890123456789" };
            yield return new object?[] { "1234567890123.4567890123456789", 1234567890123.4567890123456789m, "12345678901234567890123456789", 16, "J", null, "1234567890123.4567890123456789" };
            yield return new object?[] { "-1234567890123.4567890123456789", -1234567890123.4567890123456789m, "-12345678901234567890123456789", 16, "J", null, "-1234567890123.4567890123456789" };
            yield return new object?[] { "123456789012.34567890123456789", 123456789012.34567890123456789m, "12345678901234567890123456789", 17, "J", null, "123456789012.34567890123456789" };
            yield return new object?[] { "-123456789012.34567890123456789", -123456789012.34567890123456789m, "-12345678901234567890123456789", 17, "J", null, "-123456789012.34567890123456789" };
            yield return new object?[] { "12345678901.234567890123456789", 12345678901.234567890123456789m, "12345678901234567890123456789", 18, "J", null, "12345678901.234567890123456789" };
            yield return new object?[] { "-12345678901.234567890123456789", -12345678901.234567890123456789m, "-12345678901234567890123456789", 18, "J", null, "-12345678901.234567890123456789" };
            yield return new object?[] { "1234567890.1234567890123456789", 1234567890.1234567890123456789m, "12345678901234567890123456789", 19, "J", null, "1234567890.1234567890123456789" };
            yield return new object?[] { "-1234567890.1234567890123456789", -1234567890.1234567890123456789m, "-12345678901234567890123456789", 19, "J", null, "-1234567890.1234567890123456789" };
            yield return new object?[] { "123456789.01234567890123456789", 123456789.01234567890123456789m, "12345678901234567890123456789", 20, "J", null, "123456789.01234567890123456789" };
            yield return new object?[] { "-123456789.01234567890123456789", -123456789.01234567890123456789m, "-12345678901234567890123456789", 20, "J", null, "-123456789.01234567890123456789" };
            yield return new object?[] { "12345678.901234567890123456789", 12345678.901234567890123456789m, "12345678901234567890123456789", 21, "J", null, "12345678.901234567890123456789" };
            yield return new object?[] { "-12345678.901234567890123456789", -12345678.901234567890123456789m, "-12345678901234567890123456789", 21, "J", null, "-12345678.901234567890123456789" };
            yield return new object?[] { "1234567.8901234567890123456789", 1234567.8901234567890123456789m, "12345678901234567890123456789", 22, "J", null, "1234567.8901234567890123456789" };
            yield return new object?[] { "-1234567.8901234567890123456789", -1234567.8901234567890123456789m, "-12345678901234567890123456789", 22, "J", null, "-1234567.8901234567890123456789" };
            yield return new object?[] { "123456.78901234567890123456789", 123456.78901234567890123456789m, "12345678901234567890123456789", 23, "J", null, "123456.78901234567890123456789" };
            yield return new object?[] { "-123456.78901234567890123456789", -123456.78901234567890123456789m, "-12345678901234567890123456789", 23, "J", null, "-123456.78901234567890123456789" };
            yield return new object?[] { "12345.678901234567890123456789", 12345.678901234567890123456789m, "12345678901234567890123456789", 24, "J", null, "12345.678901234567890123456789" };
            yield return new object?[] { "-12345.678901234567890123456789", -12345.678901234567890123456789m, "-12345678901234567890123456789", 24, "J", null, "-12345.678901234567890123456789" };
            yield return new object?[] { "1234.5678901234567890123456789", 1234.5678901234567890123456789m, "12345678901234567890123456789", 25, "J", null, "1234.5678901234567890123456789" };
            yield return new object?[] { "-1234.5678901234567890123456789", -1234.5678901234567890123456789m, "-12345678901234567890123456789", 25, "J", null, "-1234.5678901234567890123456789" };
            yield return new object?[] { "123.45678901234567890123456789", 123.45678901234567890123456789m, "12345678901234567890123456789", 26, "J", null, "123.45678901234567890123456789" };
            yield return new object?[] { "-123.45678901234567890123456789", -123.45678901234567890123456789m, "-12345678901234567890123456789", 26, "J", null, "-123.45678901234567890123456789" };
            yield return new object?[] { "12.345678901234567890123456789", 12.345678901234567890123456789m, "12345678901234567890123456789", 27, "J", null, "12.345678901234567890123456789" };
            yield return new object?[] { "-12.345678901234567890123456789", -12.345678901234567890123456789m, "-12345678901234567890123456789", 27, "J", null, "-12.345678901234567890123456789" };
            yield return new object?[] { "1.2345678901234567890123456789", 1.2345678901234567890123456789m, "12345678901234567890123456789", 28, "J", null, "1.2345678901234567890123456789" };
            yield return new object?[] { "-1.2345678901234567890123456789", -1.2345678901234567890123456789m, "-12345678901234567890123456789", 28, "J", null, "-1.2345678901234567890123456789" };
            yield return new object?[] { "99999999999999999999999999999", null, "99999999999999999999999999999", 0, "J", null, "99999999999999999999999999999" };
            yield return new object?[] { "-99999999999999999999999999999", null, "-99999999999999999999999999999", 0, "J", null, "-99999999999999999999999999999" };
            yield return new object?[] { "9999999999999999999999999999.9", null, "99999999999999999999999999999", 1, "J", null, "9999999999999999999999999999.9" };
            yield return new object?[] { "-9999999999999999999999999999.9", null, "-99999999999999999999999999999", 1, "J", null, "-9999999999999999999999999999.9" };
            yield return new object?[] { "999999999999999999999999999.99", null, "99999999999999999999999999999", 2, "J", null, "999999999999999999999999999.99" };
            yield return new object?[] { "-999999999999999999999999999.99", null, "-99999999999999999999999999999", 2, "J", null, "-999999999999999999999999999.99" };
            yield return new object?[] { "99999999999999999999999999.999", null, "99999999999999999999999999999", 3, "J", null, "99999999999999999999999999.999" };
            yield return new object?[] { "-99999999999999999999999999.999", null, "-99999999999999999999999999999", 3, "J", null, "-99999999999999999999999999.999" };
            yield return new object?[] { "9999999999999999999999999.9999", null, "99999999999999999999999999999", 4, "J", null, "9999999999999999999999999.9999" };
            yield return new object?[] { "-9999999999999999999999999.9999", null, "-99999999999999999999999999999", 4, "J", null, "-9999999999999999999999999.9999" };
            yield return new object?[] { "999999999999999999999999.99999", null, "99999999999999999999999999999", 5, "J", null, "999999999999999999999999.99999" };
            yield return new object?[] { "-999999999999999999999999.99999", null, "-99999999999999999999999999999", 5, "J", null, "-999999999999999999999999.99999" };
            yield return new object?[] { "99999999999999999999999.999999", null, "99999999999999999999999999999", 6, "J", null, "99999999999999999999999.999999" };
            yield return new object?[] { "-99999999999999999999999.999999", null, "-99999999999999999999999999999", 6, "J", null, "-99999999999999999999999.999999" };
            yield return new object?[] { "9999999999999999999999.9999999", null, "99999999999999999999999999999", 7, "J", null, "9999999999999999999999.9999999" };
            yield return new object?[] { "-9999999999999999999999.9999999", null, "-99999999999999999999999999999", 7, "J", null, "-9999999999999999999999.9999999" };
            yield return new object?[] { "999999999999999999999.99999999", null, "99999999999999999999999999999", 8, "J", null, "999999999999999999999.99999999" };
            yield return new object?[] { "-999999999999999999999.99999999", null, "-99999999999999999999999999999", 8, "J", null, "-999999999999999999999.99999999" };
            yield return new object?[] { "99999999999999999999.999999999", null, "99999999999999999999999999999", 9, "J", null, "99999999999999999999.999999999" };
            yield return new object?[] { "-99999999999999999999.999999999", null, "-99999999999999999999999999999", 9, "J", null, "-99999999999999999999.999999999" };
            yield return new object?[] { "9999999999999999999.9999999999", null, "99999999999999999999999999999", 10, "J", null, "9999999999999999999.9999999999" };
            yield return new object?[] { "-9999999999999999999.9999999999", null, "-99999999999999999999999999999", 10, "J", null, "-9999999999999999999.9999999999" };
            yield return new object?[] { "999999999999999999.99999999999", null, "99999999999999999999999999999", 11, "J", null, "999999999999999999.99999999999" };
            yield return new object?[] { "-999999999999999999.99999999999", null, "-99999999999999999999999999999", 11, "J", null, "-999999999999999999.99999999999" };
            yield return new object?[] { "99999999999999999.999999999999", null, "99999999999999999999999999999", 12, "J", null, "99999999999999999.999999999999" };
            yield return new object?[] { "-99999999999999999.999999999999", null, "-99999999999999999999999999999", 12, "J", null, "-99999999999999999.999999999999" };
            yield return new object?[] { "9999999999999999.9999999999999", null, "99999999999999999999999999999", 13, "J", null, "9999999999999999.9999999999999" };
            yield return new object?[] { "-9999999999999999.9999999999999", null, "-99999999999999999999999999999", 13, "J", null, "-9999999999999999.9999999999999" };
            yield return new object?[] { "999999999999999.99999999999999", null, "99999999999999999999999999999", 14, "J", null, "999999999999999.99999999999999" };
            yield return new object?[] { "-999999999999999.99999999999999", null, "-99999999999999999999999999999", 14, "J", null, "-999999999999999.99999999999999" };
            yield return new object?[] { "99999999999999.999999999999999", null, "99999999999999999999999999999", 15, "J", null, "99999999999999.999999999999999" };
            yield return new object?[] { "-99999999999999.999999999999999", null, "-99999999999999999999999999999", 15, "J", null, "-99999999999999.999999999999999" };
            yield return new object?[] { "9999999999999.9999999999999999", null, "99999999999999999999999999999", 16, "J", null, "9999999999999.9999999999999999" };
            yield return new object?[] { "-9999999999999.9999999999999999", null, "-99999999999999999999999999999", 16, "J", null, "-9999999999999.9999999999999999" };
            yield return new object?[] { "999999999999.99999999999999999", null, "99999999999999999999999999999", 17, "J", null, "999999999999.99999999999999999" };
            yield return new object?[] { "-999999999999.99999999999999999", null, "-99999999999999999999999999999", 17, "J", null, "-999999999999.99999999999999999" };
            yield return new object?[] { "99999999999.999999999999999999", null, "99999999999999999999999999999", 18, "J", null, "99999999999.999999999999999999" };
            yield return new object?[] { "-99999999999.999999999999999999", null, "-99999999999999999999999999999", 18, "J", null, "-99999999999.999999999999999999" };
            yield return new object?[] { "9999999999.9999999999999999999", null, "99999999999999999999999999999", 19, "J", null, "9999999999.9999999999999999999" };
            yield return new object?[] { "-9999999999.9999999999999999999", null, "-99999999999999999999999999999", 19, "J", null, "-9999999999.9999999999999999999" };
            yield return new object?[] { "999999999.99999999999999999999", null, "99999999999999999999999999999", 20, "J", null, "999999999.99999999999999999999" };
            yield return new object?[] { "-999999999.99999999999999999999", null, "-99999999999999999999999999999", 20, "J", null, "-999999999.99999999999999999999" };
            yield return new object?[] { "99999999.999999999999999999999", null, "99999999999999999999999999999", 21, "J", null, "99999999.999999999999999999999" };
            yield return new object?[] { "-99999999.999999999999999999999", null, "-99999999999999999999999999999", 21, "J", null, "-99999999.999999999999999999999" };
            yield return new object?[] { "9999999.9999999999999999999999", null, "99999999999999999999999999999", 22, "J", null, "9999999.9999999999999999999999" };
            yield return new object?[] { "-9999999.9999999999999999999999", null, "-99999999999999999999999999999", 22, "J", null, "-9999999.9999999999999999999999" };
            yield return new object?[] { "999999.99999999999999999999999", null, "99999999999999999999999999999", 23, "J", null, "999999.99999999999999999999999" };
            yield return new object?[] { "-999999.99999999999999999999999", null, "-99999999999999999999999999999", 23, "J", null, "-999999.99999999999999999999999" };
            yield return new object?[] { "99999.999999999999999999999999", null, "99999999999999999999999999999", 24, "J", null, "99999.999999999999999999999999" };
            yield return new object?[] { "-99999.999999999999999999999999", null, "-99999999999999999999999999999", 24, "J", null, "-99999.999999999999999999999999" };
            yield return new object?[] { "9999.9999999999999999999999999", null, "99999999999999999999999999999", 25, "J", null, "9999.9999999999999999999999999" };
            yield return new object?[] { "-9999.9999999999999999999999999", null, "-99999999999999999999999999999", 25, "J", null, "-9999.9999999999999999999999999" };
            yield return new object?[] { "999.99999999999999999999999999", null, "99999999999999999999999999999", 26, "J", null, "999.99999999999999999999999999" };
            yield return new object?[] { "-999.99999999999999999999999999", null, "-99999999999999999999999999999", 26, "J", null, "-999.99999999999999999999999999" };
            yield return new object?[] { "99.999999999999999999999999999", null, "99999999999999999999999999999", 27, "J", null, "99.999999999999999999999999999" };
            yield return new object?[] { "-99.999999999999999999999999999", null, "-99999999999999999999999999999", 27, "J", null, "-99.999999999999999999999999999" };
            yield return new object?[] { "9.9999999999999999999999999999", null, "99999999999999999999999999999", 28, "J", null, "9.9999999999999999999999999999" };
            yield return new object?[] { "-9.9999999999999999999999999999", null, "-99999999999999999999999999999", 28, "J", null, "-9.9999999999999999999999999999" };

        }


        [Test]
        public static void Test_TryFormat()
        {
            using (new CultureContext(CultureInfo.InvariantCulture))
            {
                foreach (object?[] testdata in ToString_TestData())
                {
                    TryFormat((string)testdata[0]!, (decimal?)testdata[1], (string)testdata[2]!, (int)testdata[3]!, (string?)testdata[4], (IFormatProvider?)testdata[5], (string)testdata[6]!);
                }
            }
        }

        private static void TryFormat(string value, decimal? decimalValue, string unscaledValue, int scale, string? format, IFormatProvider? provider, string expected)
        {
            if (decimalValue.HasValue)
            {
                Span<char> buffer = stackalloc char[128];
                assertTrue(DotNetNumber.TryFormatDecimal(decimalValue.Value, format, provider, buffer, out int charsWritten));

                string actual = buffer.Slice(0, charsWritten).ToString();
                if (expected != actual)
                {
                    Assert.Fail(BuildFailureMessage(value, decimalValue.Value, unscaledValue, scale, format, provider, expected, actual));
                }
            }
        }


        [Test]
        public static void Test_ToString()
        {
            using (new CultureContext(CultureInfo.InvariantCulture))
            {
                foreach (object?[] testdata in ToString_TestData())
                {
                    ToString((string)testdata[0]!, (decimal?)testdata[1], (string)testdata[2]!, (int)testdata[3]!, (string?)testdata[4], (IFormatProvider?)testdata[5], (string)testdata[6]!);
                }
            }
        }

        private static void ToString(string value, decimal? decimalValue, string unscaledValue, int scale, string? format, IFormatProvider? provider, string expected)
        {
            if (decimalValue.HasValue)
            {
                Span<char> buffer = stackalloc char[128];
                string actual = DotNetNumber.FormatDecimal(decimalValue.Value, format, provider);

                if (expected != actual)
                {
                    Assert.Fail(BuildFailureMessage(value, decimalValue.Value, unscaledValue, scale, format, provider, expected, actual));
                }
            }
        }

        private static string DecimalLiteral(decimal value)
        {
            return value.ToString(CultureInfo.InvariantCulture) + "m";
        }

        private static string BuildFailureMessage(
            string value,
            decimal decimalValue,
            string unscaledValue,
            int scale,
            string? format,
            IFormatProvider? provider,
            string expected,
            string actual)
        {
            var sb = new StringBuilder();

            sb.AppendLine($"ToString test failed.");
            sb.AppendLine();

            sb.Append("value          = \"").Append(value).AppendLine("\"");
            sb.Append("decimalValue   = ").Append(DecimalLiteral(decimalValue)).AppendLine();
            sb.Append("unscaledValue  = \"").Append(unscaledValue).AppendLine("\"");
            sb.Append("scale          = ").Append(scale).AppendLine();
            sb.Append("format         = ").Append(format ?? "<null>").AppendLine();

            sb.Append("provider       = ");
            if (provider is null)
                sb.Append("<null>").AppendLine();
            else
                sb.Append(provider.GetType().Name).Append(": ").Append(provider.ToString()).AppendLine();
            sb.AppendLine();

            sb.Append("Expected: \"").Append(expected).AppendLine("\"");
            sb.Append("But was:  \"").Append(actual).AppendLine("\"");

            return sb.ToString();
        }

        // Our decimal formatter relies on the underlying layout of the decimal data type
        // to read the raw binary data. GetBits() is another way to do this, but it allocates temporary
        // heap. This test ensures the layout remains stable and will fail if the decimal format ever changes.
        [Test]
        public void TestDecimalLayout()
        {
            decimal value = 123.45m;

            //DecimalData data = new DecimalData { Value = value };
            DecimalData data = Unsafe.As<decimal, DecimalData>(ref value);

            int[] bits = decimal.GetBits(value);

            assertEquals(bits[0], unchecked((int)data.Low));
            assertEquals(bits[1], unchecked((int)data.Mid));
            assertEquals(bits[2], unchecked((int)data.High));
            assertEquals(bits[3], data.Flags);
        }
    }
}
