using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace J2N
{
    [TestFixture]
    public class TestTypeInfoExtensions
    {
        [Test]
        public void TestImplementsGenericInterface()
        {
            Assert.IsTrue(typeof(List<string>).GetTypeInfo().ImplementsGenericInterface(typeof(IList<>)));
            Assert.IsFalse(typeof(List<string>).GetTypeInfo().ImplementsGenericInterface(typeof(IDictionary<,>)));

            Assert.IsFalse(typeof(Dictionary<string, string>).GetTypeInfo().ImplementsGenericInterface(typeof(IList<>)));
            Assert.IsTrue(typeof(Dictionary<string, string>).GetTypeInfo().ImplementsGenericInterface(typeof(IDictionary<,>)));
        }
    }
}
