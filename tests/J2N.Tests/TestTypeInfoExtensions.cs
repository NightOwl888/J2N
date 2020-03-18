using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace J2N
{
    [TestFixture]
    public class TestTypeInfoExtensions : TestCase
    {
        [Test]
        public void TestImplementsGenericInterface()
        {
#if NET40
            assertTrue(typeof(List<string>).GetType().ImplementsGenericInterface(typeof(IList<>)));
            assertFalse(typeof(List<string>).GetType().ImplementsGenericInterface(typeof(IDictionary<,>)));

            assertFalse(typeof(Dictionary<string, string>).GetType().ImplementsGenericInterface(typeof(IList<>)));
            assertTrue(typeof(Dictionary<string, string>).GetType().ImplementsGenericInterface(typeof(IDictionary<,>)));
#else
            assertTrue(typeof(List<string>).GetTypeInfo().ImplementsGenericInterface(typeof(IList<>)));
            assertFalse(typeof(List<string>).GetTypeInfo().ImplementsGenericInterface(typeof(IDictionary<,>)));

            assertFalse(typeof(Dictionary<string, string>).GetTypeInfo().ImplementsGenericInterface(typeof(IList<>)));
            assertTrue(typeof(Dictionary<string, string>).GetTypeInfo().ImplementsGenericInterface(typeof(IDictionary<,>)));
#endif
        }
    }
}
