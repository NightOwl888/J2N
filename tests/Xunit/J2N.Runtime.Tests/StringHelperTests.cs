using J2N.TestUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace J2N.Tests
{
    public class StringHelperTests
    {
        /// <summary>
        /// This test data is
        /// 
        /// 1) confirmed unequal between the original form and uppercase
        /// 2) tailored from a specific set of code points
        /// 3) contains both surrogate pairs and individual code points
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> OrdinalIgnoreCaseTestData()
        {
            yield return new object[] { "\uFB00\u267A\u08CC\u0D3A\u0BD2\uD801\uDC01\u01CB\uFB01\u11DD\u2C65" };
            yield return new object[] { "\u1C47\u236C\u139F\u01C8\u2C65\uD801\uDCA4\u2C65\uD801\uDCA2\uD801\uDC8D\u0EDA\uD83A\uDD10" };
            yield return new object[] { "\u1DF1\uD801\uDC81\u226E\uFB02\u0201\uD801\uDC80\u01ED\u2355\u121B\u226C\uD801\uDC9F\u0D33\u0131\uD801\uDCAA\u1A91\u0450\u01FA\uD806\uDCA5" };
            yield return new object[] { "\u0DBC\uD801\uDC85\uD801\uDC9D\uD806\uDCA8\u065A\u094E\uFB01\uD801\uDC9E\u1F4F\u0B1A\u2666\uD806\uDCB8\u0DF0\u043A\u086C\uFB02\uD806\uDCA0\u1677" };
            yield return new object[] { "\u265E\u1536\u2B00\u090E\u1E2B\uD83A\uDD57\uD801\uDC23\uD83A\uDD44\uD806\uDCC2\u2C65\uD83A\uDD00\u2C65\u0DD3\u2C65\u0FC0\uD801\uDC1D\u2933" };
            yield return new object[] { "\u0A4A\u2A67\u0CBB\u26E5\u0B99\uFB01\u043A\u01CB\uD801\uDC47\u1E0E\u0C54\u1E2B\uD801\uDC02\u1207\u0572\u275C" };
            yield return new object[] { "\u1E2B\u1612\u09BA\uD806\uDCDA\u0201\uD806\uDCA6\u2814\uFB01\u2CB8\uFB02\uD801\uDC96\uFB00\u01C5\uD801\uDC03\uD801\uDC83\uD806\uDCE0\uD806\uDCC0" };
            yield return new object[] { "\u03A1\uD83A\uDD3B\u05B0\u220B\uFB01\u2B2E\uFB02\uFB02\uD801\uDC8A" };
            yield return new object[] { "\uD801\uDC06\u0201\u0FAC\u01C5\uD801\uDC9C\u1058\uD801\uDC28" };
            yield return new object[] { "\u2DA2\u01C5\u0815\uD801\uDCA9\uD801\uDCA0\u0F5A\u0131\uD801\uDC0F\u18E1\u2602\uD801\uDC3C\u0EED\u2A7D\u2D29" };
            yield return new object[] { "\u1BB1\uD801\uDC3D\uFB02\u1E81\uFB02\u2AF9\u2E60\uD83A\uDD03\uD801\uDC39\u289F\u01C8\uD801\uDC2A\u2C65" };
            yield return new object[] { "\u1A82\u0131\u2837\u2C65\u0CED\uFB01\u28FE\u0C00\u01C8\u0610\u0A58\u24E1\u2311\u04E7\uD801\uDC20\u06C4\u0201\u01CB\uD801\uDC33\u187B\u24D8\u0659\u0FA0" };
            yield return new object[] { "\uFB00\u0DDF\uD801\uDC30\u157F\u0131\u2010\u01C8\u0FE2\u2465\u1511" };
            yield return new object[] { "\u14EB\u043A\uD806\uDCAF\uFB00\u26D2\uD801\uDC17\u146F\u2F15\u1B44\uD801\uDC8A\u0AAB\uD83A\uDD57\uD801\uDC8A" };
            yield return new object[] { "\u2084\u1E2B\u03D1\u2ED1\uD801\uDC92\uD83A\uDD25\u04E7\uD83A\uDD03\u0963\u0E51\u0AC8\uD801\uDC05\u0CB8\u155B\u0AC4" };
            yield return new object[] { "\uD801\uDC05\uD806\uDCB2\u05AF\u2B9E\u017C\u094B\u1C46\uD801\uDC95\uD83A\uDD25\u0CB3\u06B0\u0CFD\u01CB\u22D7\u25B8" };
            yield return new object[] { "\u043A\u16C1\u1E06\u1476\u0CC3\u03D1\u09A6\u083D\u0201\u27EA\u01C8\u1737\uD801\uDC8E\u0724\u0C2C\uD801\uDC28\uD801\uDCAB" };
            yield return new object[] { "\u26A2\uFB02\u2FEF\u2B28\u0457\u1C76\u1E75\u2DCC\u1603\uD801\uDC01\u04E7\u0E52\u0196\u0A1E\uD83A\uDD22\u043A\uFB02\u0488\u275A\u1AE5\u0C07" };
            yield return new object[] { "\u0706\u133D\uFB01\u297C\u28C6\u17F9\u01C8\uD83A\uDD3F\u15E6\uD801\uDC1B\u13B9\u07AA\u2330" };
            yield return new object[] { "\u2C65\uD801\uDC2A\u1896\uD806\uDCAB\u278A\uD801\uDC93\uD801\uDCA0\uD801\uDCA7\uFB01\uFB02\u03D1\u0AFC\uD83A\uDD1F\u03D1" };
            yield return new object[] { "\uD801\uDC93\u2DC2\u1C79\u01C5\u1733\u2ACB\uFB02\uD801\uDC87\uD83A\uDD03\u0DC5\u04E7\u2E26\u0201\u03D1\u0D9E" };
            yield return new object[] { "\u03D1\u0131\uD806\uDCB7\u043A\uD801\uDC96\uD801\uDC8D\u2C65\uFB01\uD806\uDCE9\u043A\u1E2B\u1397\u1E2B\u2A82\uD83A\uDD10\u10F8\uD806\uDCEB\u2D18" };
            yield return new object[] { "\u01C8\u0AFB\u2B77\u01CB\u01CB\u12C2\uD801\uDC40\u0182\u28EE\uFB01\u1C8F\u04E7\u01C5\u03F3" };
            yield return new object[] { "\uFB01\u1470\u01C5\uD801\uDC15\uD83A\uDD41\u09A2\u1DE0\u1C4F\u1C51\u05CE\u29B4\uD806\uDCA7\u2AAE" };
            yield return new object[] { "\uD801\uDC9F\u1D03\u2674\u087C\u2C65\u14AB\uD801\uDC0D\u13EC\uD806\uDCBA\u08BA\uD806\uDCA0\u01CB" };
            yield return new object[] { "\uD806\uDCD0\uD83A\uDD0C\u0292\u01C5\u276A\u1E2B\uD801\uDC49\u119A\u1017\u20CA\uD801\uDC0B\uD801\uDC0D\u1CF6\u04DD\u2050\u2E83\uD806\uDCB5\u078A" };
            yield return new object[] { "\u2AEE\u0A3C\u2855\uD83A\uDD1E\u2DDF\u23B1\u0767\u01CB\u13D9\u0EFE\u23CC\u0874\u1FF2\u1E2B\u0201\uD801\uDC0C\u1762" };
            yield return new object[] { "\u29BD\uD83A\uDD06\uD801\uDC0A\uD83A\uDD4E\u108C\u18B8\u043A\uD83A\uDD0E\u0131\u0963" };
            yield return new object[] { "\uD801\uDC35\u1496\u2FB7\u29A3\u294D\u0131\u01CB\u1D8F\u27E1\u0F71\u043A\u081D" };
            yield return new object[] { "\u02D4\uD801\uDC48\u03D1\u22BB\u04E7\uD83A\uDD21\u0919\uD801\uDC33\u03D1\u0201\uD806\uDCB3\u10AC\u0C6B\u0360\u1914\u2DE2\uD806\uDCF1\u2916\uFFFD" };
            yield return new object[] { "\uFB02\uD801\uDC0A\u07CF\u0503\u266E\u29A0\u225C\uFB01\u174F\u04E7\u1358\u29A0\u2316\uFFFD" };
            yield return new object[] { "\u16F2\u04E7\uD801\uDC8E\u1D22\u04AE\uD83A\uDD57\u2C65\u0201\u0114\u0299\u2F48\uD806\uDCF0\u043A\u2AD4\uFB01\u0C18\uD806\uDCB3" };
            yield return new object[] { "\u1A87\u2A2C\uD801\uDC21\u2314\u1234\u0661\u03D1\u19BC\u08A0\u01CB\u07E8\uD806\uDCF8\u1CBE\u20E4\u2469" };
            yield return new object[] { "\u2F89\uD801\uDC0C\u01CB\u2EF9\u04E7\u11F9\u29A9\u0694\u1948\u0731\u0172\uD801\uDC89\uD801\uDCA7\u0330\uD806\uDCD8\uD801\uDC3C\u1250\u1513\u01CB\u27DA" };
            yield return new object[] { "\uD801\uDC09\u234D\u02FF\uFB00\u2F16\u01C8\u272A\uD801\uDC14\u0131\uD83A\uDD21\u2BB0\uD801\uDCA5\uFB02\u1777" };
            yield return new object[] { "\u2A42\uD801\uDC47\u04A6\u1E2B\u1694\uD806\uDCB1\uD83A\uDD5C\u2C65\uD83A\uDD17\u260F\uD801\uDC24\u0D8B\uFB01\u189F" };
            yield return new object[] { "\u04ED\u0FB0\u043A\u2692\u016E\u11FD\u0201\u28C8\uFB00\u0182\u1AD0\uD801\uDC9F\u1A3F\uD83A\uDD20\u1800" };
            yield return new object[] { "\u04E7\uD801\uDC9F\u0AD4\uD83A\uDD20\u2B00\uFB00\uD806\uDCC8\u0477\uD801\uDCA2\uD83A\uDD09\u0E56\u1A35\u1B67\u1E2B\u01EE\u1DB9\uFFFD" };
            yield return new object[] { "\u263E\uFB00\u19E7\u03D1\u1AEE\uD83A\uDD03\uD801\uDCAF\u0DBD\u213D\uD83A\uDD0F\u13E8" };
            yield return new object[] { "\u25CF\u04D7\u1323\u0201\u2C2F\uFB02\u06B8\u01C5\u0DF2\u04E7\uD801\uDC99\u030C\u2C65\uFB00\u1EA7\uFFFD" };
            yield return new object[] { "\u03D1\u01CB\u01CB\u0445\uD801\uDC8D\u1C70\uD801\uDC49\u05DF" };
            yield return new object[] { "\uFB00\uD801\uDC4D\u15E4\uD806\uDCE1\u14CD\u25E6\uD83A\uDD4D\uFB01\u2A13\u2C65\u2CB8\uD801\uDC01\uD83A\uDD35\u03E0\u01C5" };
            yield return new object[] { "\u08A1\uD83A\uDD38\uD801\uDC1E\uD83A\uDD34\u0D34\uD801\uDCAA\u2C65\u2A5F\uD83A\uDD09" };
            yield return new object[] { "\u152D\uFB01\uFB01\uD806\uDCB3\u04E7\u2484\u0131\u2E8A\u0201\u23A0\uD83A\uDD4E\uD806\uDCF9\u151B\u0191\u03EF\u0706\u0201" };
            yield return new object[] { "\uD806\uDCD8\u01C8\u0201\u06DD\u1E2B\uFB01\uFB00\uFB02\uD806\uDCC1\uD801\uDCAE\u0591\uFFFD" };
            yield return new object[] { "\uFB01\u1871\u2935\u12FE\u1F10\u20BC\u1AD2\u091A\uFB00\u2A73\u1F81\u180B\u2A7D\u1B01\u2C43\u03D1\uD801\uDC4D\u01C5" };
            yield return new object[] { "\uD801\uDC89\u0BCF\u07A0\u2051\u0568\u295D\u093C\u1F51\u080C\uD806\uDCE8\u0131\u2687\uD801\uDC89\u1C5B\u1E96\u197A\u211B\u1E2B" };
            yield return new object[] { "\uD801\uDC11\u1699\u0131\uD801\uDC38\uD801\uDC80\uFB02\u0CEE\u03D1\uD801\uDCA6\uD83A\uDD1E\uD83A\uDD1D\u01C8\u1E2B\u03D1\u01C8\u084C\u0FFC\u2EC4\uFFFD" };
            yield return new object[] { "\u175D\u0151\uD806\uDCA1\u21AF\u1C83\u01C5\u043A\u0EDC\uD806\uDCFE" };
            yield return new object[] { "\u065A\uD83A\uDD09\u29C0\u1019\uD801\uDC38\u01C8\uD806\uDCB1\uD806\uDCC9\u2C65\uD801\uDC17" };
            yield return new object[] { "\u032E\u20E7\u292E\uD83A\uDD51\uD801\uDC81\uFB02\u043A\uD801\uDC2E\u129F\uD83A\uDD04\u0131\u01C5\uD801\uDC9D\u01C8" };
            yield return new object[] { "\uFB00\u1F30\u0841\uD801\uDCA7\u04E7\u0201\u0E8A\u0536\uD806\uDCE7\u0201\uD83A\uDD35\u25D4\u17CC\uD806\uDCAF" };
            yield return new object[] { "\u2D80\uD806\uDCE6\uD801\uDC85\u1BF5\u20C1\u2B09\u0D8A\uD801\uDC88\u2D19\uD83A\uDD35\u04C4" };
            yield return new object[] { "\uD806\uDCFC\uD801\uDC89\u178E\u2E43\u04E7\u0552\u0E03\u1D47\uD806\uDCFE\uD801\uDC81\u01C5\uD801\uDC91\u01CB\u1F9F\u0366\uFFFD" };
            yield return new object[] { "\uD801\uDC21\u0F45\uFB02\u01C5\uD801\uDC1F\u1874\uD806\uDCA8\uD83A\uDD14\u2271\uFFFD" };
            yield return new object[] { "\uD806\uDCB1\u03D1\uD801\uDC8F\u145D\u1371\uFB00\u04E7\u0593\uD806\uDCED\uFFFD" };
            yield return new object[] { "\u0131\u1709\u267C\uD801\uDC85\u0C14\u2C65\u01CB\u01C8\u04A7\u01CB\u0201\u15EA\u0E50\u2EB7\uD801\uDC2B\u0201\u043A\uD83A\uDD04\u0DC9\u0131\u2F1C\uFFFD" };
            yield return new object[] { "\u092C\u04F3\u0D2E\uFB01\u021E\u192B\uD801\uDCA2\u217C\u1618\u1E2B\u25A4\u16C6\u1CDD\u03D1\u0201\uD801\uDC89\uFFFD" };
            yield return new object[] { "\uFB00\u1360\uD83A\uDD07\uFB00\u1064\u043A\uD83A\uDD58\uFB01\uD83A\uDD3B\u01C8\u01CB\u2BFC\u0B57\uFFFD" };
            yield return new object[] { "\u01CB\u0510\u04E7\u1C6A\uD801\uDC3F\u2E91\u21A6\uD801\uDC82\u2798\u2877\u03D1\u215E" };
            yield return new object[] { "\u1866\u172B\u27D3\u03C7\u0FFB\u0564\uD801\uDC92\uD83A\uDD53\uD806\uDCB9\u0201\u0CF7\uFFFD" };
            yield return new object[] { "\u1710\u04E7\u0131\u2C65\u2C02\u02A3\u01FD\u2C65\u0414\uD801\uDC2E\u043A\u2DEF\u01CB\uD801\uDC11" };
            yield return new object[] { "\u03D1\u03FB\u22F2\u1732\uD801\uDCA5\uD83A\uDD24\u1D05\uFB02\uD801\uDC8E\uD806\uDCED\uD801\uDC21\uD83A\uDD20" };
            yield return new object[] { "\uD801\uDC92\u2A43\u2E00\uD83A\uDD15\u2E4D\u26FC\u10D8\uD801\uDC98\uD801\uDCA2\u0BA1\u1895\u1ECE\u0F27\uD806\uDCA7\u101E\u1B47" };
            yield return new object[] { "\u1D34\u01CB\u046F\u10FA\u0103\u289F\u0201\uD83A\uDD20\u1858\uFB01\uD801\uDC19\u18FA\u027C\u0CBD\uD83A\uDD4A\u159F" };
            yield return new object[] { "\uD806\uDCB4\u1998\u20C0\u041E\u0A4D\u03D1\u043A\u1448\uD806\uDCCA\uD83A\uDD0A\u086E\u1632\u1925\u040F\uD806\uDCD0\u214E\u1FBB\u1460\u0E08\uFFFD" };
            yield return new object[] { "\u1FBA\uFB01\u131F\u2FC7\uD801\uDC9A\u01C8\u0367\u04E7\uD83A\uDD4C\uD806\uDCDE\u03C6\u2433\uD806\uDCB1\u1012\u13F0\u2676\u1DC2\u01C8" };
            yield return new object[] { "\u03F8\u0201\uD83A\uDD4B\uD806\uDCF9\u0228\u0F93\u0131\uD83A\uDD4B\u2203\uD801\uDCA0\u2C65\uFFFD" };
            yield return new object[] { "\u1302\u04E7\u1CA5\uD801\uDCA4\u2E03\u07A7\uD83A\uDD1C\u0201\u2650\uFB00\uD801\uDC38\uD83A\uDD36\u254B\u2376" };
            yield return new object[] { "\u03D1\uD83A\uDD0C\u1EF1\u0380\u0131\u2541\uFB02\u2CCE\uD801\uDC27" };
            yield return new object[] { "\uD83A\uDD09\u01C8\uD83A\uDD50\u2EFD\u03D1\u0496\u1E2B\uD801\uDC28\u0A2A\u01C8\uD801\uDCAC\uD83A\uDD47" };
            yield return new object[] { "\uD801\uDC8B\u1E8A\u04E7\u167F\uD83A\uDD49\u0652\u1659\u029F\u130D\u112D\u1B31\uD801\uDC8D\u20B2\u2B6F\u04F5\u1C37\u2F11\u1E2B\u0675\uD806\uDCB1\u1E2B" };
            yield return new object[] { "\u12DD\u2F6F\uD83A\uDD23\uD801\uDC41\u12F6\uFB02\uD801\uDC39\uD83A\uDD41\u2C65\uD801\uDCA5\u06BD\uFFFD" };
            yield return new object[] { "\uD83A\uDD56\u15AC\u2466\u05D6\u06C8\u25C2\uD83A\uDD14\u1DF9\uFB01\u09A8\u16B5\u0201\uD801\uDC3A\u059A" };
            yield return new object[] { "\uD801\uDC89\u1100\u0131\uD83A\uDD50\u2E39\u017B\u097E\u12B7\u01C8\u2C24\u0F15\u1E2B" };
            yield return new object[] { "\uD801\uDC99\u11DD\u2939\u2C65\u043A\u1F80\u2453\u2C2F\uD83A\uDD35\u01C5\uD801\uDC1C\uD801\uDC90\uD806\uDCF6\uD83A\uDD33\u076B" };
            yield return new object[] { "\u1515\uD801\uDC3D\u0201\u18DC\u0572\u0131\u2B5F\uD83A\uDD40\u047D\uD801\uDCA9\u2C65\u2A53\uFB00" };
            yield return new object[] { "\uFB01\u16DF\u2E9E\uD806\uDCB8\u08BA\u28F1\u1189\u04E7\u0112\u0131\u2B44\u0201\u1B85\u01C5\u0957\u2D64\u17AA\u01C5\u2B07\u2911\uD801\uDC8E\u25D9\u2E68" };
            yield return new object[] { "\uD806\uDCAB\u01C8\u0D19\u1AFA\uD801\uDC93\uD83A\uDD22\uD801\uDC9B\uD801\uDC85\u2592\u0451\uD806\uDCCA\u082A\uD83A\uDD39\uD801\uDC1C\u01CB" };
            yield return new object[] { "\u0B90\uD83A\uDD15\uD806\uDCD7\uD801\uDC4D\u0B37\uD806\uDCE3\u24C1\u0201\uD801\uDCA7\uFFFD" };
            yield return new object[] { "\u2CC8\u0164\uD806\uDCB7\u09EA\u1F77\u138A\u239D\u04E7\u18AE\u2C65\u01C8\u1CC6\u1D51\u043A\uD83A\uDD39\uFB01" };
            yield return new object[] { "\u2267\u01CB\uFB02\u1202\u0200\uFB01\u1BE6\u1E2B\u2B20\u01FC\u0EC6\u1E2B\uD83A\uDD15\uD83A\uDD57\u04E7\uD83A\uDD08\u1031\u13C1" };
            yield return new object[] { "\u212C\u1AA3\uD83A\uDD03\uD801\uDC4C\u01CB\uD83A\uDD5B\uD806\uDCDD\uFFFD" };
            yield return new object[] { "\u2E1C\u2B21\uD806\uDCCE\u06A6\u185E\u067D\u2C65\u02A7\uD83A\uDD4B\uFB00\u11BA" };
            yield return new object[] { "\u2A1F\u2D61\u20AB\u1E2B\u060B\u1C2B\uD806\uDCBC\u1F92\u144E\u01C8\u2A3D\u2E04\u2512\uD83A\uDD05\u0CCF\u2E6E\u299E\u1E9D\uD801\uDC14\uD801\uDCAE" };
            yield return new object[] { "\u01C5\u1DD2\u1994\u0EFE\u043A\u2FE0\u2F5B\uFB01\u15FB\u119A\u2E6E\u25AE\uD801\uDC01\uD801\uDCAD\uD801\uDCAB\uFB02\uFB01\u0795" };
            yield return new object[] { "\u2DD0\uD806\uDCA1\u276D\u113E\u043A\u27CB\u08AF\u0131\u03D1\u189F\u013F\uFB01\uD801\uDC41\u0131\u04E7\uFFFD" };
            yield return new object[] { "\u26A3\u1B76\uD801\uDC83\u01C8\u192D\uD83A\uDD53\u01CB\uD806\uDCA2\uFB02\uFB01\u2584\u01C5\uD801\uDC4B\u23DE\u01C5" };
            yield return new object[] { "\u04E7\u09B7\u2C65\u1B5C\uD83A\uDD4C\u097A\u043A\u24C4\u04E7\u0DBE\u0AC9\u140E\u2466\u224E\uFB00\uD801\uDC8F" };
            yield return new object[] { "\uD83A\uDD22\u2157\u01C8\u1E46\u072D\uD801\uDCA9\uFB00\u1FAE\u0E04\uD801\uDC42\u10F2\u01AC\u0FF8\u01C5\uD83A\uDD15\u02BC\uD801\uDC37" };
            yield return new object[] { "\u140D\u26F3\uD801\uDC96\u01C8\u0201\u1732\u2C65\u0131\u1B94\u0170\u0B50\uD801\uDC9E\u0449\u0296\uD806\uDCFC\uD806\uDCF4" };
            yield return new object[] { "\u2C65\u0F04\u0B11\u0201\u01F1\u22CD\u027E\u1C5A\u1E2C\u09BC\u04E7\u27AC\uD801\uDC30\u1A6D\u0A26\uD806\uDCE1" };
            yield return new object[] { "\u01C5\u02A6\u06C2\uD801\uDC9D\u117A\uD801\uDC2C\u0A2C\u2C24\u155C\u29F0\uFB01\u1DAB\uD806\uDCBC\u15C7\uFB02\u01CB\u0724\u0840\u01CB\u01BE\u0131\u1D4D" };
            yield return new object[] { "\u2932\uFB01\u0131\u2E1A\u0E82\u08C4\u234D\u1928\uD801\uDC87\uFB01\u0B71\u25C7\uD806\uDCCD\u1315\uD806\uDCBC\u1A97\uFFFD" };
            yield return new object[] { "\u2B6D\u11AB\u0F90\u01C5\u043A\uD806\uDCDC\u158D\u05E0\u0EE2" };
            yield return new object[] { "\u11A3\uD801\uDC96\u0FBB\u0921\u03D1\u01C5\uD801\uDC17\u04E7\u1691" };
            yield return new object[] { "\u1572\u0201\u286B\uFB01\u15B7\u05EB\u1568\u1474\u2AFE\u2FD4\u1E2B\u0131\uD801\uDC12\uD801\uDC3A\u2C16\u139F\u2C30\uD801\uDC3A" };
            yield return new object[] { "\u0131\uD806\uDCCE\uFB00\u1957\u19AA\uD801\uDC24\uD806\uDCC6\uFB00\u1DB9\u2298\u01C8\u0D9B\uD83A\uDD45\u043A\u01CB\u0B35\u043A\u1130\uD801\uDCAC\uFFFD" };
            yield return new object[] { "\uD83A\uDD0D\u0716\uD801\uDC1D\u0EB1\uD801\uDC36\u14DF\uFB00\u0C21\uD801\uDCA1\uD806\uDCAA\u156A\uD801\uDCA0\uD83A\uDD1C\u2B06\uD806\uDCB3\uFB00" };
            yield return new object[] { "\uD801\uDC4A\u0A7D\u1D5D\u0364\u2C65\u0534\u2049\u1DC2\uFFFD" };
        }

        [Theory] // J2N specific
        [MemberData(nameof(OrdinalIgnoreCaseTestData))]
        public void GetNonRandomizedHashCodeOrdinalIgnoreCase_ReturnsSameHashCodeForCasedString(string original)
        {
            var comparer = System.StringComparer.OrdinalIgnoreCase;

            var upper = original.ToUpperInvariant();
            var lower = original.ToLowerInvariant();

            // These hash code values do not necessarily match the BCL, but should all consistently match
            // each other regardless of string/span or original/upper/lower case.
            int j2nHashCodeOriginalString = StringHelper.GetNonRandomizedHashCodeOrdinalIgnoreCase(original);
            int j2nHashCodeOriginalSpan = StringHelper.GetNonRandomizedHashCodeOrdinalIgnoreCase(original.AsSpan());

            Assert.Equal(j2nHashCodeOriginalString, j2nHashCodeOriginalSpan);

            int j2nHashCodeUpperString = StringHelper.GetNonRandomizedHashCodeOrdinalIgnoreCase(upper);
            int j2nHashCodeUpperSpan = StringHelper.GetNonRandomizedHashCodeOrdinalIgnoreCase(upper.AsSpan());

            Assert.Equal(j2nHashCodeUpperString, j2nHashCodeUpperSpan);

            int j2nHashCodeLowerString = StringHelper.GetNonRandomizedHashCodeOrdinalIgnoreCase(lower);
            int j2nHashCodeLowerSpan = StringHelper.GetNonRandomizedHashCodeOrdinalIgnoreCase(lower.AsSpan());

            Assert.Equal(j2nHashCodeLowerString, j2nHashCodeLowerSpan);

            // Ensure cased strings match
            Assert.Equal(j2nHashCodeOriginalString, j2nHashCodeUpperString);
            Assert.Equal(j2nHashCodeOriginalString, j2nHashCodeLowerString);
        }

        [Theory] // J2N specific
        [MemberData(nameof(OrdinalIgnoreCaseTestData))]
        public void GetHashCodeOrdinalIgnoreCase_ReturnsSameHashCodeForCasedString(string original)
        {
            var comparer = System.StringComparer.OrdinalIgnoreCase;

            var upper = original.ToUpperInvariant();
            var lower = original.ToLowerInvariant();

            // These hash code values do not necessarily match the BCL, but should all consistently match
            // each other regardless of string/span or original/upper/lower case.
            int j2nHashCodeOriginalString = StringHelper.GetHashCodeOrdinalIgnoreCase(original);
            int j2nHashCodeOriginalSpan = StringHelper.GetHashCodeOrdinalIgnoreCase(original.AsSpan());

            Assert.Equal(j2nHashCodeOriginalString, j2nHashCodeOriginalSpan);

            int j2nHashCodeUpperString = StringHelper.GetHashCodeOrdinalIgnoreCase(upper);
            int j2nHashCodeUpperSpan = StringHelper.GetHashCodeOrdinalIgnoreCase(upper.AsSpan());

            Assert.Equal(j2nHashCodeUpperString, j2nHashCodeUpperSpan);

            int j2nHashCodeLowerString = StringHelper.GetHashCodeOrdinalIgnoreCase(lower);
            int j2nHashCodeLowerSpan = StringHelper.GetHashCodeOrdinalIgnoreCase(lower.AsSpan());

            Assert.Equal(j2nHashCodeLowerString, j2nHashCodeLowerSpan);

            // Ensure cased strings match
            Assert.Equal(j2nHashCodeOriginalString, j2nHashCodeUpperString);
            Assert.Equal(j2nHashCodeOriginalString, j2nHashCodeLowerString);
        }

        [Theory] // J2N specific
        [MemberData(nameof(OrdinalIgnoreCaseTestData))]
        public void GetNonRandomizedHashCode_ReturnsSameHashCodeForStringAndSpanOverloads(string original)
        {
            var comparer = System.StringComparer.OrdinalIgnoreCase;

            var upper = original.ToUpperInvariant();
            var lower = original.ToLowerInvariant();

            // These hash code values do not necessarily match the BCL, but should all consistently match
            // each other regardless of string/span.
            int j2nHashCodeOriginalString = StringHelper.GetNonRandomizedHashCode(original);
            int j2nHashCodeOriginalSpan = StringHelper.GetNonRandomizedHashCode(original.AsSpan());

            Assert.Equal(j2nHashCodeOriginalString, j2nHashCodeOriginalSpan);

            int j2nHashCodeUpperString = StringHelper.GetNonRandomizedHashCode(upper);
            int j2nHashCodeUpperSpan = StringHelper.GetNonRandomizedHashCode(upper.AsSpan());

            Assert.Equal(j2nHashCodeUpperString, j2nHashCodeUpperSpan);

            int j2nHashCodeLowerString = StringHelper.GetNonRandomizedHashCode(lower);
            int j2nHashCodeLowerSpan = StringHelper.GetNonRandomizedHashCode(lower.AsSpan());

            Assert.Equal(j2nHashCodeLowerString, j2nHashCodeLowerSpan);

            // Matching hash codes (collisions) are fine here, because Equals() will be used to tie-break
        }

        [Theory] // J2N specific
        [MemberData(nameof(OrdinalIgnoreCaseTestData))]
        public void GetHashCode_ReturnsSameHashCodeForStringAndSpanOverloads(string original)
        {
            var comparer = System.StringComparer.OrdinalIgnoreCase;

            var upper = original.ToUpperInvariant();
            var lower = original.ToLowerInvariant();

            // These hash code values do not necessarily match the BCL, but should all consistently match
            // each other regardless of string/span.
            int j2nHashCodeOriginalString = StringHelper.GetHashCode(original);
            int j2nHashCodeOriginalSpan = StringHelper.GetHashCode(original.AsSpan());

            Assert.Equal(j2nHashCodeOriginalString, j2nHashCodeOriginalSpan);

            int j2nHashCodeUpperString = StringHelper.GetHashCode(upper);
            int j2nHashCodeUpperSpan = StringHelper.GetHashCode(upper.AsSpan());

            Assert.Equal(j2nHashCodeUpperString, j2nHashCodeUpperSpan);

            int j2nHashCodeLowerString = StringHelper.GetHashCode(lower);
            int j2nHashCodeLowerSpan = StringHelper.GetHashCode(lower.AsSpan());

            Assert.Equal(j2nHashCodeLowerString, j2nHashCodeLowerSpan);

            // Matching hash codes (collisions) are fine here, because Equals() will be used to tie-break
        }
    }
}