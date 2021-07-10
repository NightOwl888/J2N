#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
/*  Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;

namespace J2N
{
    public class TestTime : TestCase
    {
        public override void SetUp()
        {
            base.SetUp();
            setDelays();
        }


        public static long SHORT_DELAY_MS;
        public static long SMALL_DELAY_MS;
        public static long MEDIUM_DELAY_MS;
        public static long LONG_DELAY_MS;

        /**
         * Returns the shortest timed delay. This could
         * be reimplemented to use for example a Property.
         */
        protected long getShortDelay()
        {
            return 50;
        }


        /**
         * Sets delays as multiples of SHORT_DELAY.
         */
        protected void setDelays()
        {
            SHORT_DELAY_MS = getShortDelay();
            SMALL_DELAY_MS = SHORT_DELAY_MS * 5;
            MEDIUM_DELAY_MS = SHORT_DELAY_MS * 10;
            LONG_DELAY_MS = SHORT_DELAY_MS * 50;
        }

        /**
         * fail with message "Unexpected exception"
         */
        public void unexpectedException()
        {
            fail("Unexpected exception");
        }


        /** 
         * Worst case rounding for millisecs; set for 60 cycle millis clock.
         * This value might need to be changed os JVMs with coarser
         *  System.currentTimeMillis clocks.
         */
        static readonly long MILLIS_ROUND = 17;

        /**
         * Nanos between readings of millis is no longer than millis (plus
         * possible rounding).
         * This shows only that nano timing not (much) worse than milli.
         */
        [Test]
        public void TestNanoTime1()
        {
            try
            {
                long m1 = Time.CurrentTimeMilliseconds();
                Thread.Sleep(1);
                long n1 = Time.NanoTime();
                Thread.Sleep(TimeSpan.FromMilliseconds(SHORT_DELAY_MS));
                long n2 = Time.NanoTime();
                Thread.Sleep(1);
                long m2 = Time.CurrentTimeMilliseconds();
                long millis = m2 - m1;
                long nanos = n2 - n1;
                assertTrue(nanos >= 0);
                long nanosAsMillis = nanos / 1000000;
                assertTrue(nanosAsMillis <= millis + MILLIS_ROUND);
            }
            catch (ThreadInterruptedException ie)
            {
                unexpectedException();
            }
        }

        /**
         * Millis between readings of nanos is less than nanos, adjusting
         * for rounding.
         * This shows only that nano timing not (much) worse than milli.
         */
        [Test]
        public void TestNanoTime2()
        {
            try
            {
                long n1 = Time.NanoTime();
                Thread.Sleep(1);
                long m1 = Time.CurrentTimeMilliseconds();
                Thread.Sleep(TimeSpan.FromMilliseconds(SHORT_DELAY_MS));
                long m2 = Time.CurrentTimeMilliseconds();
                Thread.Sleep(1);
                long n2 = Time.NanoTime();
                long millis = m2 - m1;
                long nanos = n2 - n1;

                assertTrue(nanos >= 0);
                long nanosAsMillis = nanos / 1000000;
                assertTrue(millis <= nanosAsMillis + MILLIS_ROUND);
            }
            catch (ThreadInterruptedException ie)
            {
                unexpectedException();
            }
        }

        /**
         * @tests java.lang.System#currentTimeMillis()
         */
        [Test]
        public void Test_currentTimeMillis()
        {
            // Test for method long java.lang.System.currentTimeMillis()
            long firstRead = Time.CurrentTimeMilliseconds();
            try
            {
                Thread.Sleep(150);
            }
            catch (ThreadInterruptedException e)
            {
            }
            long secondRead = Time.CurrentTimeMilliseconds();
            assertTrue("Incorrect times returned: " + firstRead + ", "
                            + secondRead, firstRead < secondRead);
        }

        
        /**
         * @tests java.util.Date#getTime()
         */
        [Test]
        public void Test_getTime()
        {
            // Test for method long java.util.Date.getTime()
            DateTime d1 = Time.UnixEpoch; //new Date(0);
            DateTime d2 = Time.UnixEpoch + TimeSpan.FromMilliseconds(1900000); //new Date(1900000);
            assertEquals("Returned incorrect time", 1900000, d2.GetMillisecondsSinceUnixEpoch());
            assertEquals("Returned incorrect time", 0, d1.GetMillisecondsSinceUnixEpoch());
        }

        /**
         * @tests java.util.Date#getTime()
         */
        [Test]
        public void Test_GetTimeSpanSinceUnixEpoch()
        {
            // Test for method long java.util.Date.getTime()
            DateTime d1 = Time.UnixEpoch; //new Date(0);
            DateTime d2 = Time.UnixEpoch + TimeSpan.FromMilliseconds(1900000); //new Date(1900000);
            assertEquals("Returned incorrect time", 1900000d, d2.GetTimeSpanSinceUnixEpoch().TotalMilliseconds, 0d);
            assertEquals("Returned incorrect time", 0d, d1.GetTimeSpanSinceUnixEpoch().TotalMilliseconds, 0d);
        }
    }
}
