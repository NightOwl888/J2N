using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Collections.Generic
{
    public class TestEqualityComparer : TestCase
    {
        [Test]
        public void TestLoadNullableValueTypes()
        {
            assertEquals(typeof(NullableDoubleComparer), EqualityComparer<double?>.Default.GetType());
            assertEquals(typeof(NullableSingleComparer), EqualityComparer<float?>.Default.GetType());

            assertEquals(typeof(DoubleComparer), EqualityComparer<double>.Default.GetType());
            assertEquals(typeof(SingleComparer), EqualityComparer<float>.Default.GetType());
        }
    }
}
