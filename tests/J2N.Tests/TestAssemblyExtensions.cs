using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace J2N
{
    public class TestAssemblyExtensions : TestCase
    {
        /**
         * @tests java.lang.Class#getResource(java.lang.String)
         */
        [Test]
        public void Test_getResourceLjava_lang_String()
        {
            string name = "test_resource.txt";
            string res = GetType().GetTypeInfo().Assembly.FindResource(GetType(), name);
            assertNotNull(res);
        }

        /**
         * @tests java.lang.Class#getResourceAsStream(java.lang.String)
         */
        [Test]
        public void Test_getResourceAsStreamLjava_lang_String()
        {
            string name = "test_resource.txt";
            using (Stream stream = GetType().GetTypeInfo().Assembly.FindAndGetManifestResourceStream(GetType(), name))
                assertNotNull("the file " + name + " can not be found in this directory", stream);

            String nameBadURI = "org/apache/harmony/luni/tests/test_resource.txt";
            using (Stream stream2 = GetType().GetTypeInfo().Assembly.FindAndGetManifestResourceStream(GetType(), nameBadURI))
                assertNull("the file " + nameBadURI + " should not be found in this directory", stream2);

            //Stream str = Object.class.getResourceAsStream("Class.class");
            //assertNotNull("java.lang.Object couldn't find its class with getResource...", str);

            //assertTrue("Cannot read single byte", str.read() != -1);
            //assertEquals("Cannot read multiple bytes", 5, str.read(new byte[5]));
            //str.close();

            using (Stream str2 = GetType().GetTypeInfo().Assembly.FindAndGetManifestResourceStream(GetType(), "test_resource.txt"))
            {
                assertNotNull("Can't find resource", str2);
                assertTrue("Cannot read single byte", str2.ReadByte() != -1);
                assertEquals("Cannot read multiple bytes", 5, str2.Read(new byte[5], 0, 5));
            }
        }
    }
}
