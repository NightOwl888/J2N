#if FEATURE_TYPEINFO

using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace J2N
{
    [TestFixture]
    public class TestTypeInfoExtensions : TestCase
    {
        [Test]
        public void TestImplementsGenericInterface()
        {
            assertTrue(typeof(List<string>).GetTypeInfo().ImplementsGenericInterface(typeof(IList<>)));
            assertFalse(typeof(List<string>).GetTypeInfo().ImplementsGenericInterface(typeof(IDictionary<,>)));

            assertFalse(typeof(Dictionary<string, string>).GetTypeInfo().ImplementsGenericInterface(typeof(IList<>)));
            assertTrue(typeof(Dictionary<string, string>).GetTypeInfo().ImplementsGenericInterface(typeof(IDictionary<,>)));
        }
    }
}
#endif
