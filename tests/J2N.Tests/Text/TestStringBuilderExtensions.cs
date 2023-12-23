using J2N.Globalization;
using NUnit.Framework;
using System;
using System.Buffers;
using System.Globalization;
using System.Text;

namespace J2N.Text
{
    public class TestStringBuilderExtensions : TestCase
    {
        // String larger than 16384 bytes
        private const string LargeUnicodeString = "⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛⽄\u2fda⼃⾮⾵\u2fde⼒⼱⾠⽚⽕⽆⾭⾕⼇⼂⽖⽋⽲\u2fd8⿄⽁⼄⼽⾸⼉⽤⾲⼡\u2fdb⼱⼈⽥⾰⽬⼤⿃⽞⽪⽗⼟⾃⼪⾔⾏⼼\u2fdb⼩⼘⼷⾪⼲⾛⾫⾊⼃⿕⾥⿕⾫⽹⽀⼐⾤⼩⽍⿀\u2fdd⼩⿂⼞\u2fd7⿁⼚⾹⽁⼖⽐⾎⽻⼍⼻⾚⿊⼰\u2fdf⽌⾚⼥⽨⼯⼞⽩⾞⾽⾿⽳⽥⽫⽁⽛";

        private static readonly string s_chunkSplitSource = new string('a', 30);

        private static StringBuilder StringBuilderWithMultipleChunks() => new StringBuilder(20).Append(s_chunkSplitSource);

        [Test]
        public void TestAppend()
        {
            StringBuilder target = new StringBuilder("This is a test");
            ICharSequence string1 = ". Text to add.".AsCharSequence();
            ICharSequence string2 = " Some more text to add.".AsCharSequence();

            // NOTE: We must use the label, or the compiler will choose the
            // Append(object) overload. That overload works, but is not as efficient
            // as this for CharArrayCharSequence.
            target.Append(charSequence: string1);
            Assert.AreEqual("This is a test. Text to add.", target.ToString());

            target.Append(string2, 6, string2.Length - 6);
            Assert.AreEqual("This is a test. Text to add.more text to add.", target.ToString());
        }

        [Test]
        public virtual void TestAppendCodePointBmp()
        {
            var sb = new StringBuilder("foo bar");
            int codePoint = 97; // a

            sb.AppendCodePoint(codePoint);

            Assert.AreEqual("foo bara", sb.ToString());
        }

        [Test]
        public virtual void TestAppendCodePointUnicode()
        {
            var sb = new StringBuilder("foo bar");
            int codePoint = 3594; // ช

            sb.AppendCodePoint(codePoint);

            Assert.AreEqual("foo barช", sb.ToString());
        }

        [Test]
        public virtual void TestAppendCodePointUTF16Surrogates()
        {
            var sb = new StringBuilder("foo bar");
            int codePoint = 176129; // '\uD86C', '\uDC01' (𫀁)

            sb.AppendCodePoint(codePoint);

            Assert.AreEqual("foo bar𫀁", sb.ToString());
        }

        [Test]
        public virtual void TestAppendCodePointTooHigh()
        {
            var sb = new StringBuilder("foo bar");
            int codePoint = Character.MaxCodePoint + 1;

            Assert.Throws<ArgumentException>(() => sb.AppendCodePoint(codePoint));
        }

        [Test]
        public virtual void TestAppendCodePointTooLow()
        {
            var sb = new StringBuilder("foo bar");
            int codePoint = Character.MinCodePoint - 1;

            Assert.Throws<ArgumentException>(() => sb.AppendCodePoint(codePoint));
        }

        [Test]
        public void TestCompareToOrdinal()
        {
            StringBuilder target = null;
            string compareTo = "Alpine";

            Assert.Greater(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Greater(0, target.CompareToOrdinal(compareTo));
            Assert.Greater(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Greater(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            target = new StringBuilder("Alpha");

            Assert.Greater(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Greater(0, target.CompareToOrdinal(compareTo));
            Assert.Greater(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Greater(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Greater(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            compareTo = "Alpha";

            Assert.AreEqual(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.AreEqual(0, target.CompareToOrdinal(compareTo));
            Assert.AreEqual(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));

            compareTo = "Alp";

            Assert.Less(0, target.CompareToOrdinal(compareTo.ToCharArray()));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilder(compareTo)));
            Assert.Less(0, target.CompareToOrdinal(compareTo));
            Assert.Less(0, target.CompareToOrdinal(new CharArrayCharSequence(compareTo.ToCharArray())));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilderCharSequence(new StringBuilder(compareTo))));
            Assert.Less(0, target.CompareToOrdinal(new StringCharSequence(compareTo)));


            Assert.Less(0, target.CompareToOrdinal((char[])null));
            Assert.Less(0, target.CompareToOrdinal((StringBuilder)null));
            Assert.Less(0, target.CompareToOrdinal((string)null));
            Assert.Less(0, target.CompareToOrdinal(new CharArrayCharSequence(null)));
            Assert.Less(0, target.CompareToOrdinal(new StringBuilderCharSequence(null)));
            Assert.Less(0, target.CompareToOrdinal(new StringCharSequence(null)));

            target = null;

            Assert.AreEqual(0, target.CompareToOrdinal((char[])null));
            Assert.AreEqual(0, target.CompareToOrdinal((StringBuilder)null));
            Assert.AreEqual(0, target.CompareToOrdinal((string)null));
            Assert.AreEqual(0, target.CompareToOrdinal(new CharArrayCharSequence(null)));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringBuilderCharSequence(null)));
            Assert.AreEqual(0, target.CompareToOrdinal(new StringCharSequence(null)));
        }

        [Test]
        public virtual void TestReverse()
        {
            var sb = new StringBuilder("foo 𝌆 bar𫀁mañana");

            sb.Reverse();

            Assert.AreEqual("anañam𫀁rab 𝌆 oof", sb.ToString());
        }

        [Test]
        public void TestSubsequence()
        {
            StringBuilder target = new StringBuilder("This is a test");

            Assert.AreEqual("This is a test", target.Subsequence(0, target.Length));
            Assert.AreEqual("is a", target.Subsequence(5, 4));
            Assert.AreEqual("", target.Subsequence(4, 0));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(-1, 10));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(3, -2));
            Assert.Throws<ArgumentOutOfRangeException>(() => target.Subsequence(target.Length, 1));

            StringBuilder emptyTarget = new StringBuilder();

            Assert.Throws<ArgumentOutOfRangeException>(() => emptyTarget.Subsequence(0, 1));

            StringBuilder nullTarget = null;

            Assert.IsFalse(nullTarget.Subsequence(6, 10).HasValue); // Null target will always return null subsequence
        }

        [Test]
        public void TestAsCharSequence()
        {
            StringBuilder target = new StringBuilder("This is a test");

            var result = target.AsCharSequence();

            Assert.IsNotNull(result);
            Assert.AreEqual(typeof(StringBuilderCharSequence), result.GetType());
        }

        #region Apache Harmony Tests

        /**
         * @tests java.lang.StringBuilder.delete(int, int)
         */
        [Test]
        public void Test_DeleteII()
        {
            string fixture = "0123456789";
            StringBuilder sb = new StringBuilder(fixture);
            assertSame(sb, sb.Delete(0, 0 - 0)); // J2N: Corrected 2nd parameter
            assertEquals(fixture, sb.ToString());
            assertSame(sb, sb.Delete(5, 5 - 5)); // J2N: Corrected 2nd parameter
            assertEquals(fixture, sb.ToString());
            assertSame(sb, sb.Delete(0, 1 - 0)); // J2N: Corrected 2nd parameter
            assertEquals("123456789", sb.ToString());
            assertEquals(9, sb.Length);
            assertSame(sb, sb.Delete(0, sb.Length - 0)); // J2N: Corrected 2nd parameter
            assertEquals("", sb.ToString());
            assertEquals(0, sb.Length);

            sb = new StringBuilder(fixture);
            assertSame(sb, sb.Delete(0, 11 - 0)); // J2N: Corrected 2nd parameter
            assertEquals("", sb.ToString());
            assertEquals(0, sb.Length);

            try
            {
                new StringBuilder(fixture).Delete(-1, 2 - -1); // J2N: Corrected 2nd parameter
                fail("no SIOOBE, negative start");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                new StringBuilder(fixture).Delete(11, 12 - 11); // J2N: Corrected 2nd parameter
                fail("no SIOOBE, start too far");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                new StringBuilder(fixture).Delete(13, 12 - 13); // J2N: Corrected 2nd parameter
                fail("no SIOOBE, start larger than end");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            // HARMONY 6212
            sb = new StringBuilder();
            sb.Append("abcde");
            String str = sb.ToString();
            sb.Delete(0, sb.Length - 0); // J2N: Corrected 2nd parameter
            sb.Append("YY");
            assertEquals("abcde", str);
            assertEquals("YY", sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.replace(int, int, String)'
         */
        [Test]
        public void Test_ReplaceIILjava_lang_String()
        {
            string fixture = "0000";
            StringBuilder sb = new StringBuilder(fixture);
            assertSame(sb, sb.Replace(1, 3 - 1, "11")); // J2N; Corrected 2nd parameter
            assertEquals("0110", sb.ToString());
            assertEquals(4, sb.Length);

            sb = new StringBuilder(fixture);
            assertSame(sb, sb.Replace(1, 2 - 1, "11")); // J2N; Corrected 2nd parameter
            assertEquals("01100", sb.ToString());
            assertEquals(5, sb.Length);

            sb = new StringBuilder(fixture);
            assertSame(sb, sb.Replace(4, 5 - 4, "11")); // J2N; Corrected 2nd parameter
            assertEquals("000011", sb.ToString());
            assertEquals(6, sb.Length);

            sb = new StringBuilder(fixture);
            assertSame(sb, sb.Replace(4, 6 - 4, "11")); // J2N; Corrected 2nd parameter
            assertEquals("000011", sb.ToString());
            assertEquals(6, sb.Length);

            // FIXME Undocumented NPE in Sun's JRE 5.0_5
            try
            {
                sb.Replace(1, 2 - 1, null); // J2N; Corrected 2nd parameter
                fail("No NPE");
            }
            catch (ArgumentNullException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuilder(fixture);
                sb.Replace(-1, 2 - -1, "11"); // J2N; Corrected 2nd parameter
                fail("No SIOOBE, negative start");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuilder(fixture);
                sb.Replace(5, 2 - 5, "11"); // J2N; Corrected 2nd parameter
                fail("No SIOOBE, start > length");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            try
            {
                sb = new StringBuilder(fixture);
                sb.Replace(3, 2 - 3, "11"); // J2N; Corrected 2nd parameter
                fail("No SIOOBE, start > end");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // Expected
            }

            // Regression for HARMONY-348
            StringBuilder buffer = new StringBuilder("1234567");
            buffer.Replace(2, 6 - 2, "XXX"); // J2N; Corrected 2nd parameter
            assertEquals("12XXX7", buffer.ToString());
        }

        private void reverseTest(String org, String rev, String back)
        {
            // create non-shared StringBuilder
            StringBuilder sb = new StringBuilder(org);
            sb.Reverse();
            String reversed = sb.ToString();
            assertEquals(rev, reversed);
            // create non-shared StringBuilder
            sb = new StringBuilder(reversed);
            sb.Reverse();
            reversed = sb.ToString();
            assertEquals(back, reversed);

            // test algorithm when StringBuilder is shared
            sb = new StringBuilder(org);
            String copy = sb.ToString();
            assertEquals(org, copy);
            sb.Reverse();
            reversed = sb.ToString();
            assertEquals(rev, reversed);
            sb = new StringBuilder(reversed);
            copy = sb.ToString();
            assertEquals(rev, copy);
            sb.Reverse();
            reversed = sb.ToString();
            assertEquals(back, reversed);
        }

        /**
         * @tests java.lang.StringBuilder.reverse()
         */
        [Test]
        public void Test_Reverse()
        {
            String fixture = "0123456789";
            StringBuilder sb = new StringBuilder(fixture);
            assertSame(sb, sb.Reverse());
            assertEquals("9876543210", sb.ToString());

            sb = new StringBuilder("012345678");
            assertSame(sb, sb.Reverse());
            assertEquals("876543210", sb.ToString());

            sb.Length = (1);
            assertSame(sb, sb.Reverse());
            assertEquals("8", sb.ToString());

            sb.Length = (0);
            assertSame(sb, sb.Reverse());
            assertEquals("", sb.ToString());

            String str;
            str = "a";
            reverseTest(str, str, str);

            str = "ab";
            reverseTest(str, "ba", str);

            str = "abcdef";
            reverseTest(str, "fedcba", str);

            str = "abcdefg";
            reverseTest(str, "gfedcba", str);

            str = "\ud800\udc00";
            reverseTest(str, str, str);

            str = "\udc00\ud800";
            reverseTest(str, "\ud800\udc00", "\ud800\udc00");

            str = "a\ud800\udc00";
            reverseTest(str, "\ud800\udc00a", str);

            str = "ab\ud800\udc00";
            reverseTest(str, "\ud800\udc00ba", str);

            str = "abc\ud800\udc00";
            reverseTest(str, "\ud800\udc00cba", str);

            str = "\ud800\udc00\udc01\ud801\ud802\udc02";
            reverseTest(str, "\ud802\udc02\ud801\udc01\ud800\udc00",
                    "\ud800\udc00\ud801\udc01\ud802\udc02");

            str = "\ud800\udc00\ud801\udc01\ud802\udc02";
            reverseTest(str, "\ud802\udc02\ud801\udc01\ud800\udc00", str);

            str = "\ud800\udc00\udc01\ud801a";
            reverseTest(str, "a\ud801\udc01\ud800\udc00",
                    "\ud800\udc00\ud801\udc01a");

            str = "a\ud800\udc00\ud801\udc01";
            reverseTest(str, "\ud801\udc01\ud800\udc00a", str);

            str = "\ud800\udc00\udc01\ud801ab";
            reverseTest(str, "ba\ud801\udc01\ud800\udc00",
                    "\ud800\udc00\ud801\udc01ab");

            str = "ab\ud800\udc00\ud801\udc01";
            reverseTest(str, "\ud801\udc01\ud800\udc00ba", str);

            str = "\ud800\udc00\ud801\udc01";
            reverseTest(str, "\ud801\udc01\ud800\udc00", str);

            str = "a\ud800\udc00z\ud801\udc01";
            reverseTest(str, "\ud801\udc01z\ud800\udc00a", str);

            str = "a\ud800\udc00bz\ud801\udc01";
            reverseTest(str, "\ud801\udc01zb\ud800\udc00a", str);

            str = "abc\ud802\udc02\ud801\udc01\ud800\udc00";
            reverseTest(str, "\ud800\udc00\ud801\udc01\ud802\udc02cba", str);

            str = "abcd\ud802\udc02\ud801\udc01\ud800\udc00";
            reverseTest(str, "\ud800\udc00\ud801\udc01\ud802\udc02dcba", str);
        }

        /**
         * @tests java.lang.StringBuilder.codePointCount(int, int)
         */
        [Test]
        public void Test_CodePointCountII()
        {
            assertEquals(1, new StringBuilder("\uD800\uDC00").CodePointCount(0, 2));
            assertEquals(1, new StringBuilder("\uD800\uDC01").CodePointCount(0, 2));
            assertEquals(1, new StringBuilder("\uD801\uDC01").CodePointCount(0, 2));
            assertEquals(1, new StringBuilder("\uDBFF\uDFFF").CodePointCount(0, 2));

            assertEquals(3, new StringBuilder("a\uD800\uDC00b").CodePointCount(0, 4));
            assertEquals(4, new StringBuilder("a\uD800\uDC00b\uD800").CodePointCount(0, 5));

            StringBuilder sb = new StringBuilder();
            sb.Append("abc");
            try
            {
                sb.CodePointCount(-1, 2);
                fail("No IOOBE for negative begin index.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.CodePointCount(0, 4);
                fail("No IOOBE for end index that's too large.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.CodePointCount(3, 2);
                fail("No IOOBE for begin index larger than end index.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }
        }

        /**
         * @tests java.lang.StringBuilder.codePointAt(int)
         */
        [Test]
        public void Test_CodePointAtI()
        {
            StringBuilder sb = new StringBuilder("abc");
            assertEquals('a', sb.CodePointAt(0));
            assertEquals('b', sb.CodePointAt(1));
            assertEquals('c', sb.CodePointAt(2));

            sb = new StringBuilder("\uD800\uDC00");
            assertEquals(0x10000, sb.CodePointAt(0));
            assertEquals('\uDC00', sb.CodePointAt(1));

            sb = new StringBuilder();
            sb.Append("abc");
            try
            {
                sb.CodePointAt(-1);
                fail("No IOOBE on negative index.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.CodePointAt(sb.Length);
                fail("No IOOBE on index equal to length.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }

            try
            {
                sb.CodePointAt(sb.Length + 1);
                fail("No IOOBE on index greater than length.");
            }
            catch (ArgumentOutOfRangeException e)
            {

            }
        }

        private class MyCharSequence : ICharSequence
        {
            public string Value { get; }
            public MyCharSequence(string value)
            {
                Value = value;
            }

            public char this[int index] => Value[index];

            public bool HasValue => Value != null;

            public int Length => Value.Length;

            public ICharSequence Subsequence(int startIndex, int length)
            {
                return Value.Substring(startIndex, length).AsCharSequence();
            }

            public override string ToString()
            {
                return Value;
            }
        }


        /**
         * @tests java.lang.StringBuilder.append(CharSequence)
         */
        [Test]
        public void Test_Append_ICharSequence()
        {
            StringBuilder sb = new StringBuilder();
            assertSame(sb, sb.Append(charSequence: "ab".AsCharSequence())); // J2N: charSequence label is required to get to this overload over object overload. See https://stackoverflow.com/a/26885473
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(charSequence: "cd".AsCharSequence()));
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(charSequence: (ICharSequence)null));
            // assertEquals("null", sb.ToString());
            assertEquals("", sb.ToString()); // J2N: Changed the behavior to be a no-op rather than appending the string "null"
        }

        /**
         * @tests java.lang.StringBuilder.append(CharSequence)
         */
        [Test]
        public void Test_Append_ICharSequence_Custom()
        {
            StringBuilder sb = new StringBuilder();
            assertSame(sb, sb.Append(charSequence: new MyCharSequence("ab")));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(charSequence: new MyCharSequence("cd")));
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(charSequence: (ICharSequence)null));
            // assertEquals("null", sb.ToString());
            assertEquals("", sb.ToString()); // J2N: Changed the behavior to be a no-op rather than appending the string "null"

            // J2N: Check long strings that will be appended in chunks in certain cases
            string longString1 = new string('a', 2000) + new string('b', 200) + new string('c', 699) + new string('d', 3000);
            string longString2 = new string('z', 1000) + new string('y', 550) + new string('r', 6999) + new string('f', 300);

            sb.Length = (0);
            sb.Append(charSequence: new MyCharSequence(longString1));
            assertEquals(longString1, sb.ToString());
            sb.Length = (0);
            sb.Append(charSequence: new MyCharSequence(longString2));
            assertEquals(longString2, sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.append(CharSequence, int, int)
         */
        [Test]
        public void Test_Append_ICharSequence_Int32_Int32()
        {
            StringBuilder sb = new StringBuilder();
            assertSame(sb, sb.Append("ab".AsCharSequence(), 0, 2 - 0)); // J2N: corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append("cd".AsCharSequence(), 0, 2 - 0)); // J2N: corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append("abcd".AsCharSequence(), 0, 2 - 0)); // J2N: corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append("abcd".AsCharSequence(), 2, 4 - 2)); // J2N: corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            try
            {
                assertSame(sb, sb.Append((ICharSequence)null, 0, 2)); // J2N: Changed the behavior to throw an exception (to match .NET Core 3.0's Append(StringBuilder,int,int) overload) rather than appending the string "null"
                fail("no NPE");
            }
            catch (ArgumentNullException e)
            {
                // Expected
            }
            //assertEquals("nu", sb.ToString());
            assertEquals("", sb.ToString());


            // J2N: Check long strings that will be appended in chunks in certain cases
            string longString1 = new string('a', 2000) + new string('b', 200) + new string('c', 699) + new string('d', 3000);
            string longString2 = new string('z', 1000) + new string('y', 550) + new string('r', 6999) + new string('f', 300);

            sb.Length = (0);
            sb.Append(new MyCharSequence(longString1), 0, longString1.Length);
            assertEquals(longString1, sb.ToString());
            sb.Length = (0);
            sb.Append(new MyCharSequence(longString2), 0, longString2.Length);
            assertEquals(longString2, sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.append(CharSequence, int, int)
         */
        [Test]
        public void Test_Append_ICharSequence_Int32_Int32_Custom()
        {
            StringBuilder sb = new StringBuilder();
            assertSame(sb, sb.Append(new MyCharSequence("ab"), 0, 2 - 0)); // J2N: corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(new MyCharSequence("cd"), 0, 2 - 0)); // J2N: corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(new MyCharSequence("abcd"), 0, 2 - 0)); // J2N: corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(new MyCharSequence("abcd"), 2, 4 - 2)); // J2N: corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            try
            {
                assertSame(sb, sb.Append((ICharSequence)null, 0, 2)); // J2N: Changed the behavior to throw an exception (to match .NET Core 3.0's Append(StringBuilder,int,int) overload) rather than appending the string "null"
                fail("no NPE");
            }
            catch (ArgumentNullException e)
            {
                // Expected
            }
            //assertEquals("nu", sb.ToString());
            assertEquals("", sb.ToString());


            // J2N: Check long strings that will be appended in chunks on certain TFMs
            string longString1 = new string('a', 2000) + new string('b', 200) + new string('c', 699) + new string('d', 3000);
            string longString2 = new string('z', 1000) + new string('y', 550) + new string('r', 6999) + new string('f', 300);

            sb.Length = (0);
            sb.Append(new MyCharSequence(longString1), 0, longString1.Length);
            assertEquals(longString1, sb.ToString());
            sb.Length = (0);
            sb.Append(new MyCharSequence(longString2), 1000, 550 + 6999);
            assertEquals(new string('y', 550) + new string('r', 6999), sb.ToString());
        }


        /**
         * @tests java.lang.StringBuilder.append(CharSequence)
         */
        [Test]
        public void Test_Append_StringBuilder()
        {
            StringBuilder sb = new StringBuilder();
            assertSame(sb, sb.Append(new StringBuilder("ab")));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append(new StringBuilder("cd")));
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append((StringBuilder)null));
            // assertEquals("null", sb.ToString());
            assertEquals("", sb.ToString()); // J2N: Changed the behavior to be a no-op rather than appending the string "null"

            // J2N: Check long strings that will be appended in chunks on certain TFMs
            string longString1 = new string('a', 2000) + new string('b', 200) + new string('c', 699) + new string('d', 3000);
            string longString2 = new string('z', 1000) + new string('y', 550) + new string('r', 6999) + new string('f', 300);

            sb.Length = (0);
            sb.Append(new StringBuilder(longString1));
            assertEquals(longString1, sb.ToString());
            sb.Length = (0);
            sb.Append(new StringBuilder(longString2), 1000, 550 + 6999);
            assertEquals(new string('y', 550) + new string('r', 6999), sb.ToString());
        }

        /**
         * @tests java.lang.StringBuilder.append(CharSequence, int, int)
         */
        [Test]
        public void Test_Append_StringBuilder_Int32_Int32()
        {
            StringBuilder sb = new StringBuilder();
            assertSame(sb, sb.Append(new StringBuilder("ab"), 0, 2 - 0)); // J2N: corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length=(0);
            assertSame(sb, sb.Append(new StringBuilder("cd"), 0, 2 - 0)); // J2N: corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length=(0);
            assertSame(sb, sb.Append(new StringBuilder("abcd"), 0, 2 - 0)); // J2N: corrected 3rd parameter
            assertEquals("ab", sb.ToString());
            sb.Length=(0);
            assertSame(sb, sb.Append(new StringBuilder("abcd"), 2, 4 - 2)); // J2N: corrected 3rd parameter
            assertEquals("cd", sb.ToString());
            sb.Length=(0);
            try
            {
                assertSame(sb, sb.Append((StringBuilder)null, 0, 2)); // J2N: Changed the behavior to throw an exception (to match .NET Core 3.0) rather than appending the string "null"
                fail("no NPE");
            }
            catch (ArgumentNullException e)
            {
                // Expected
            }
            //assertEquals("nu", sb.ToString());
            assertEquals("", sb.ToString());

            // J2N: Check long strings that will be appended in chunks on certain TFMs
            string longString1 = new string('a', 2000) + new string('b', 200) + new string('c', 699) + new string('d', 3000);
            string longString2 = new string('z', 1000) + new string('y', 550) + new string('r', 6999) + new string('f', 300);

            sb.Length = (0);
            sb.Append(new StringBuilder(longString1), 0, longString1.Length);
            assertEquals(longString1, sb.ToString());
            sb.Length = (0);
            sb.Append(new StringBuilder(longString2), 1000, 550 + 6999);
            assertEquals(new string('y', 550) + new string('r', 6999), sb.ToString());
        }

#if FEATURE_SPAN

        [Test]
        public void Test_Append_ReadOnlySpan() // J2N Specific to cover missing overload in older .NET TFMs
        {
            StringBuilder sb = new StringBuilder();
            assertSame(sb, sb.Append("ab".AsSpan()));
            assertEquals("ab", sb.ToString());
            sb.Length = (0);
            assertSame(sb, sb.Append("cd".AsSpan()));
            assertEquals("cd", sb.ToString());
            sb.Length = (0);
            assertEquals("", sb.ToString());

            // J2N: Check long strings that will be appended in chunks on certain TFMs
            string longString1 = new string('a', 2000) + new string('b', 200) + new string('c', 699) + new string('d', 3000);
            string longString2 = new string('z', 1000) + new string('y', 550) + new string('r', 6999) + new string('f', 300);

            sb.Length = (0);
            sb.Append(longString1.AsSpan());
            assertEquals(longString1, sb.ToString());
            sb.Length = (0);
            sb.Append(longString2.AsSpan());
            assertEquals(longString2, sb.ToString());
        }

#endif

        /**
         * @tests java.lang.StringBuilder.indexOf(String)
         */
        [Test]
        public void Test_IndexOfLSystem_String()
        {
            using (var context = new CultureContext("en-US"))
            {
                String fixture = "0123456789";
                StringBuilder sb = new StringBuilder(fixture);
                assertEquals(0, sb.IndexOf("0", StringComparison.Ordinal));
                assertEquals(0, sb.IndexOf("012", StringComparison.Ordinal));
                assertEquals(-1, sb.IndexOf("02", StringComparison.Ordinal));
                assertEquals(8, sb.IndexOf("89", StringComparison.Ordinal));

                try
                {
                    sb.IndexOf((string)null, StringComparison.Ordinal);
                    fail("no NPE");
                }
                catch (ArgumentNullException e)
                {
                    // Expected
                }
            }
        }

        /**
         * @tests java.lang.StringBuilder.indexOf(String, int)
         */
        [Test]
        public void Test_IndexOfStringInt()
        {
            using (var context = new CultureContext("en-US"))
            {
                String fixture = "0123456789";
                StringBuilder sb = new StringBuilder(fixture);
                assertEquals(0, sb.IndexOf("0", StringComparison.Ordinal));
                assertEquals(0, sb.IndexOf("012", StringComparison.Ordinal));
                assertEquals(-1, sb.IndexOf("02", StringComparison.Ordinal));
                assertEquals(8, sb.IndexOf("89", StringComparison.Ordinal));

                assertEquals(0, sb.IndexOf("0", StringComparison.Ordinal), 0);
                assertEquals(0, sb.IndexOf("012", StringComparison.Ordinal), 0);
                assertEquals(-1, sb.IndexOf("02", StringComparison.Ordinal), 0);
                assertEquals(8, sb.IndexOf("89", StringComparison.Ordinal), 0);

                assertEquals(-1, sb.IndexOf("0", StringComparison.Ordinal), 5);
                assertEquals(-1, sb.IndexOf("012", StringComparison.Ordinal), 5);
                assertEquals(-1, sb.IndexOf("02", StringComparison.Ordinal), 0);
                assertEquals(8, sb.IndexOf("89", StringComparison.Ordinal), 5);

                try
                {
                    sb.IndexOf((string)null, 0, StringComparison.Ordinal);
                    fail("no NPE");
                }
                catch (ArgumentNullException e)
                {
                    // Expected
                }
            }
        }

        /**
         * @tests java.lang.StringBuilder.lastIndexOf(String)
         */
        [Test]
        public void Test_LastIndexOfLjava_lang_String()
        {
            using (var context = new CultureContext("en-US"))
            {
                string fixture = "0123456789";
                StringBuilder sb = new StringBuilder(fixture);
                assertEquals(0, sb.LastIndexOf("0", StringComparison.Ordinal));
                assertEquals(0, sb.LastIndexOf("012", StringComparison.Ordinal));
                assertEquals(-1, sb.LastIndexOf("02", StringComparison.Ordinal));
                assertEquals(8, sb.LastIndexOf("89", StringComparison.Ordinal));

                try
                {
                    sb.LastIndexOf((string)null, StringComparison.Ordinal);
                    fail("no NPE");
                }
                catch (ArgumentNullException e)
                {
                    // Expected
                }
            }
        }

        /**
         * @tests java.lang.StringBuilder.lastIndexOf(String, int)
         */
        [Test]
        public void Test_LastIndexOfLjava_lang_StringI()
        {
            using (var context = new CultureContext("en-US"))
            {
                string fixture = "0123456789";
                StringBuilder sb = new StringBuilder(fixture);
                assertEquals(0, sb.LastIndexOf("0", StringComparison.Ordinal));
                assertEquals(0, sb.LastIndexOf("012", StringComparison.Ordinal));
                assertEquals(-1, sb.LastIndexOf("02", StringComparison.Ordinal));
                assertEquals(8, sb.LastIndexOf("89", StringComparison.Ordinal));

                assertEquals(0, sb.LastIndexOf("0", StringComparison.Ordinal), 0);
                assertEquals(0, sb.LastIndexOf("012", StringComparison.Ordinal), 0);
                assertEquals(-1, sb.LastIndexOf("02", StringComparison.Ordinal), 0);
                assertEquals(8, sb.LastIndexOf("89", StringComparison.Ordinal), 0);

                assertEquals(-1, sb.LastIndexOf("0", StringComparison.Ordinal), 5);
                assertEquals(-1, sb.LastIndexOf("012", StringComparison.Ordinal), 5);
                assertEquals(-1, sb.LastIndexOf("02", StringComparison.Ordinal), 0);
                assertEquals(8, sb.LastIndexOf("89", StringComparison.Ordinal), 5);

                try
                {
                    sb.LastIndexOf((string)null, 0, StringComparison.Ordinal);
                    fail("no NPE");
                }
                catch (ArgumentNullException e)
                {
                    // Expected
                }
            }
        }

        #endregion Apache Harmony Tests

        [Test]
        public void Test_IndexOf_String_CultureSensitivity()
        {
            string fixture = "ዬ፡ዶጶቝአሄኢቌጕኬ\u124fቖኋዘዻ፡ሆገኅጬሷ\u135cቔቿ፺ዃጫቭዄ";
            string searchFor = "ሄኢቌጕኬ\u124fቖኋዘዻ";
            StringBuilder sb = new StringBuilder(fixture);

            using (var context = new CultureContext("ru-MD"))
            {
                assertEquals(fixture.IndexOf(searchFor, StringComparison.CurrentCulture), sb.IndexOf(searchFor, StringComparison.CurrentCulture));
                assertEquals(6, sb.IndexOf(searchFor, StringComparison.Ordinal));
                assertEquals(6, sb.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase));
            }
        }

        [Test]
        public void Test_IndexOf_String_CultureSensitivity_LargeString()
        {
            string fixture = LargeUnicodeString + "ዬ፡ዶጶቝአሄኢቌጕኬ\u124fቖኋዘዻ፡ሆገኅጬሷ\u135cቔቿ፺ዃጫቭዄ";
            string searchFor = "ሄኢቌጕኬ\u124fቖኋዘዻ";
            StringBuilder sb = new StringBuilder(fixture);

            using (var context = new CultureContext("ru-MD"))
            {
                assertEquals(fixture.IndexOf(searchFor, StringComparison.CurrentCulture), sb.IndexOf(searchFor, StringComparison.CurrentCulture));
                assertEquals(LargeUnicodeString.Length + 6, sb.IndexOf(searchFor, StringComparison.Ordinal));
                assertEquals(LargeUnicodeString.Length + 6, sb.IndexOf(searchFor, StringComparison.OrdinalIgnoreCase));
            }
        }

        [Test]
        public void Test_IndexOf_String_Int32_CultureSensitivity()
        {
            string fixture = "ዬ፡ዶጶቝአሄኢቌጕኬ\u124fቖኋዘዻ፡ሆገኅጬሷ\u135cቔቿ፺ዃጫቭዄ";
            string searchFor = "ሄኢቌጕኬ\u124fቖኋዘዻ";
            StringBuilder sb = new StringBuilder(fixture);

            using (var context = new CultureContext("ru-MD"))
            {
                assertEquals(fixture.IndexOf(searchFor, 4, StringComparison.CurrentCulture), sb.IndexOf(searchFor, 4, StringComparison.CurrentCulture));
                assertEquals(6, sb.IndexOf(searchFor, 4, StringComparison.Ordinal));
                assertEquals(6, sb.IndexOf(searchFor, 4, StringComparison.OrdinalIgnoreCase));
            }
        }

        [Test]
        public void Test_IndexOf_String_Int32_CultureSensitivity_LargeString()
        {
            string fixture = LargeUnicodeString + "ዬ፡ዶጶቝአሄኢቌጕኬ\u124fቖኋዘዻ፡ሆገኅጬሷ\u135cቔቿ፺ዃጫቭዄ";
            string searchFor = "ሄኢቌጕኬ\u124fቖኋዘዻ";
            StringBuilder sb = new StringBuilder(fixture);

            using (var context = new CultureContext("ru-MD"))
            {
                assertEquals(fixture.IndexOf(searchFor, 4, StringComparison.CurrentCulture), sb.IndexOf(searchFor, 4, StringComparison.CurrentCulture));
                assertEquals(LargeUnicodeString.Length + 6, sb.IndexOf(searchFor, 4, StringComparison.Ordinal));
                assertEquals(LargeUnicodeString.Length + 6, sb.IndexOf(searchFor, 4, StringComparison.OrdinalIgnoreCase));
            }
        }


        [Test]
        public void Test_LastIndexOf_String_CultureSensitivity()
        {
            string fixture = "ዬ፡ዶጶቝአሄኢቌጕኬ\u124fቖኋዘዻ፡ሆገኅጬሷ\u135cቔቿ፺ዃጫቭዄ";
            string searchFor = "ሄኢቌጕኬ\u124fቖኋዘዻ";
            StringBuilder sb = new StringBuilder(fixture);

            using (var context = new CultureContext("ru-MD"))
            {
                assertEquals(fixture.LastIndexOf(searchFor, StringComparison.CurrentCulture), sb.LastIndexOf(searchFor, StringComparison.CurrentCulture));
                assertEquals(6, sb.LastIndexOf(searchFor, StringComparison.Ordinal));
                assertEquals(6, sb.LastIndexOf(searchFor, StringComparison.OrdinalIgnoreCase));
            }
        }

        [Test]
        public void Test_LastIndexOf_String_CultureSensitivity_LargeString()
        {
            string fixture = "ዬ፡ዶጶቝአሄኢቌጕኬ\u124fቖኋዘዻ፡ሆገኅጬሷ\u135cቔቿ፺ዃጫቭዄ" + LargeUnicodeString;
            string searchFor = "ሄኢቌጕኬ\u124fቖኋዘዻ";
            StringBuilder sb = new StringBuilder(fixture);

            using (var context = new CultureContext("ru-MD"))
            {
                assertEquals(fixture.LastIndexOf(searchFor, StringComparison.CurrentCulture), sb.LastIndexOf(searchFor, StringComparison.CurrentCulture));
                assertEquals(6, sb.LastIndexOf(searchFor, StringComparison.Ordinal));
                assertEquals(6, sb.LastIndexOf(searchFor, StringComparison.OrdinalIgnoreCase));
            }
        }

        [Test]
        public void Test_LastIndexOf_String_Int32_CultureSensitivity()
        {
            string fixture = "ዬ፡ዶጶቝአሄኢቌጕኬ\u124fቖኋዘዻ፡ሆገኅጬሷ\u135cቔቿ፺ዃጫቭዄ";
            string searchFor = "ሄኢቌጕኬ\u124fቖኋዘዻ";
            StringBuilder sb = new StringBuilder(fixture);

            using (var context = new CultureContext("ru-MD"))
            {
                assertEquals(fixture.LastIndexOf(searchFor, 20, StringComparison.CurrentCulture), sb.LastIndexOf(searchFor, 20, StringComparison.CurrentCulture));
                assertEquals(6, sb.LastIndexOf(searchFor, 20, StringComparison.Ordinal));
                assertEquals(6, sb.LastIndexOf(searchFor, 20, StringComparison.OrdinalIgnoreCase));
            }
        }

        [Test]
        public void Test_LastIndexOf_String_Int32_CultureSensitivity_LargeString()
        {
            string fixture = "ዬ፡ዶጶቝአሄኢቌጕኬ\u124fቖኋዘዻ፡ሆገኅጬሷ\u135cቔቿ፺ዃጫቭዄ" + LargeUnicodeString;
            string searchFor = "ሄኢቌጕኬ\u124fቖኋዘዻ";
            StringBuilder sb = new StringBuilder(fixture);

            using (var context = new CultureContext("ru-MD"))
            {
                assertEquals(fixture.LastIndexOf(searchFor, LargeUnicodeString.Length - 20, StringComparison.CurrentCulture), sb.LastIndexOf(searchFor, LargeUnicodeString.Length - 20, StringComparison.CurrentCulture));
                assertEquals(6, sb.LastIndexOf(searchFor, LargeUnicodeString.Length - 20, StringComparison.Ordinal));
                assertEquals(6, sb.LastIndexOf(searchFor, LargeUnicodeString.Length - 20, StringComparison.OrdinalIgnoreCase));
            }
        }

#if FEATURE_SPAN

        [Test]
        [TestCase("Hello", 0, new char[] { '\0', '\0', '\0', '\0', '\0' }, 5, new char[] { 'H', 'e', 'l', 'l', 'o' })]
        [TestCase("Hello", 0, new char[] { '\0', '\0', '\0', '\0' }, 4, new char[] { 'H', 'e', 'l', 'l' })]
        [TestCase("Hello", 1, new char[] { '\0', '\0', '\0', '\0', '\0' }, 4, new char[] { 'e', 'l', 'l', 'o', '\0' })]
        public static void Test_CopyTo_CharSpan(string value, int sourceIndex, char[] destination, int count, char[] expected)
        {
            var builder = new StringBuilder(value);
            builder.CopyTo(sourceIndex, new Span<char>(destination), count);
            Assert.AreEqual(expected, destination);
        }

        [Test]
        public static void Test_CopyTo_CharSpan_StringBuilderWithMultipleChunks()
        {
            StringBuilder builder = StringBuilderWithMultipleChunks();
            char[] destination = new char[builder.Length];
            builder.CopyTo(0, new Span<char>(destination), destination.Length);
            Assert.AreEqual(s_chunkSplitSource.ToCharArray(), destination);
        }

        [Test]
        public static void Test_CopyTo_CharSpan_Invalid()
        {
            var builder = new StringBuilder("Hello");

            Assert.Throws<ArgumentOutOfRangeException>(() => builder.CopyTo(-1, new Span<char>(new char[10]), 0), "sourceIndex"); // Source index < 0
            Assert.Throws<ArgumentOutOfRangeException>(() => builder.CopyTo(6, new Span<char>(new char[10]), 0), "sourceIndex"); // Source index > builder.Length

            Assert.Throws<ArgumentOutOfRangeException>(() => builder.CopyTo(0, new Span<char>(new char[10]), -1), "count"); // Count < 0

            Assert.Throws<ArgumentException>(() => builder.CopyTo(5, new Span<char>(new char[10]), 1)); // Source index + count > builder.Length
            Assert.Throws<ArgumentException>(() => builder.CopyTo(4, new Span<char>(new char[10]), 2)); // Source index + count > builder.Length

            Assert.Throws<ArgumentException>(() => builder.CopyTo(0, new Span<char>(new char[10]), 11)); // count > destinationArray.Length
        }
#endif
    }
}
