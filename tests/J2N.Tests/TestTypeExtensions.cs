using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace J2N
{
    [TestFixture]
    public class TestTypeExtensions
    {
        [Test]
        public void TestImplementsGenericInterface()
        {
            Assert.IsTrue(typeof(List<string>).ImplementsGenericInterface(typeof(IList<>)));
            Assert.IsFalse(typeof(List<string>).ImplementsGenericInterface(typeof(IDictionary<,>)));

            Assert.IsFalse(typeof(Dictionary<string, string>).ImplementsGenericInterface(typeof(IList<>)));
            Assert.IsTrue(typeof(Dictionary<string, string>).ImplementsGenericInterface(typeof(IDictionary<,>)));
        }
    }
}
