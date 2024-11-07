using NUnit.Framework;

namespace J2N.Text
{
    public class TestStringArrayExtensions : TestCase
    {
        [Test]
        public void TestTrimEnd()
        {
            // Zero length
            CollectionAssert.AreEqual(new string[0], new string[0].TrimEnd());
            CollectionAssert.AreEqual(new string[0], new string[] { "" }.TrimEnd());
            CollectionAssert.AreEqual(new string[0], new string[] { "", "" }.TrimEnd());

            CollectionAssert.AreEqual(new string[0], new string[] { "", "", null }.TrimEnd());
            CollectionAssert.AreEqual(new string[0], new string[] { null }.TrimEnd());
            CollectionAssert.AreEqual(new string[0], new string[] { null, null }.TrimEnd());

            // End trimming
            CollectionAssert.AreEqual(new string[] { "foo" }, new string[] { "foo", "" }.TrimEnd());
            CollectionAssert.AreEqual(new string[] { "foo" }, new string[] { "foo", "", "" }.TrimEnd());
            CollectionAssert.AreEqual(new string[] { "foo" }, new string[] { "foo", "", "", "" }.TrimEnd());

            CollectionAssert.AreEqual(new string[] { "foo", "bar" }, new string[] { "foo", "bar", "" }.TrimEnd());
            CollectionAssert.AreEqual(new string[] { "foo", "bar" }, new string[] { "foo", "bar", "", "" }.TrimEnd());
            CollectionAssert.AreEqual(new string[] { "foo", "bar" }, new string[] { "foo", "bar", "", "", "" }.TrimEnd());

            // End trimming without trimming beginning or middle entries
            CollectionAssert.AreEqual(new string[] { "", "foo" }, new string[] { "", "foo" }.TrimEnd());
            CollectionAssert.AreEqual(new string[] { "", "foo", "bar" }, new string[] { "", "foo", "bar" }.TrimEnd());
            CollectionAssert.AreEqual(new string[] { "foo", "", "bar" }, new string[] { "foo", "", "bar" }.TrimEnd());

            CollectionAssert.AreEqual(new string[] { "", "foo" }, new string[] { "", "foo", "" }.TrimEnd());
            CollectionAssert.AreEqual(new string[] { "", "foo", "bar" }, new string[] { "", "foo", "bar", "" }.TrimEnd());
            CollectionAssert.AreEqual(new string[] { "foo", "", "bar" }, new string[] { "foo", "", "bar", "" }.TrimEnd());

            CollectionAssert.AreEqual(new string[] { "", "foo" }, new string[] { "", "foo", "", "" }.TrimEnd());
            CollectionAssert.AreEqual(new string[] { "", "foo", "bar" }, new string[] { "", "foo", "bar", "", "" }.TrimEnd());
            CollectionAssert.AreEqual(new string[] { "foo", "", "bar" }, new string[] { "foo", "", "bar", "", "" }.TrimEnd());
        }
    }
}
