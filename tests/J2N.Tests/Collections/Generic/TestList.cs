using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace J2N.Collections.Generic
{
    public class TestList : TestCase
    {
        //[Test]
        //public void TestSerialize1()
        //{
        //    var intList = new List<int> { 1, 2, 3, 4, 5 };
        //    var stringList = new List<string> { "one", "two", "three", "four", "five" };

        //    var formatter = new BinaryFormatter();

        //    using (Stream stream = File.Open(@"F:\legacy-list-int.bin", FileMode.OpenOrCreate))
        //        formatter.Serialize(stream, intList);

        //    using (Stream stream = File.Open(@"F:\legacy-list-string.bin", FileMode.OpenOrCreate))
        //        formatter.Serialize(stream, stringList);
        //}

        [Test]
        public void TestDeserializeLegacy()
        {
            List<int> intList;
            List<string> stringList;

            var formatter = new BinaryFormatter();

            //using (Stream stream = File.Open(@"F:\legacy-list-int.bin", FileMode.Open))
            //    intList = (List<int>)formatter.Deserialize(stream);
            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("legacy-list-int.bin"))
                intList = (List<int>)formatter.Deserialize(stream);

            assertEquals(5, intList.Count);
            assertEquals(5, intList._version);

            //using (Stream stream = File.Open(@"F:\legacy-list-string.bin", FileMode.Open))
            //    stringList = (List<string>)formatter.Deserialize(stream);
            using (Stream stream = this.GetType().FindAndGetManifestResourceStream("legacy-list-string.bin"))
                stringList = (List<string>)formatter.Deserialize(stream);

            assertEquals(5, stringList.Count);
            assertEquals(5, stringList._version);
        }

    }
}
