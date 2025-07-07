// Source: https://github.com/apache/harmony/blob/trunk/classlib/support/src/test/java/tests/support/Support_MapTest2.java

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace J2N.Collections
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    public class Support_MapTest2 : TestCase
    {
        IDictionary<string, string> map;

        public Support_MapTest2(IDictionary<string, string> m)
        {
            map = m;
            if (map.Count > 0)
            {
                fail("Map must be empty");
            }
        }


        public void RunTest()
        {
            try
            {
                map["one"] = "1";
                assertEquals("size should be one", 1, map.Count);
                map.Clear();
                assertEquals("size should be zero", 0, map.Count);
                assertTrue("Should not have entries", !map.Any());
                assertTrue("Should not have keys", !map.Keys.Any());
                assertTrue("Should not have values", !map.Values.Any());
            }
            catch (InvalidOperationException e)
            {
            }

            try
            {
                map["one"] = "1";
                assertEquals("size should be one", 1, map.Count);
                map.Remove("one");
                assertEquals("size should be zero", 0, map.Count);
                assertTrue("Should not have entries", !map.Any());
                assertTrue("Should not have keys", !map.Keys.Any());
                assertTrue("Should not have values", !map.Values.Any());
            }
            catch (InvalidOperationException e)
            {
            }
        }
    }
}
