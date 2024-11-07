using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace J2N.Collections.Concurrent
{
    #region Copyright 2012-2014 by Roger Knapp, Licensed under the Apache License, Version 2.0
    /* Licensed under the Apache License, Version 2.0 (the "License");
     * you may not use this file except in compliance with the License.
     * You may obtain a copy of the License at
     * 
     *   http://www.apache.org/licenses/LICENSE-2.0
     * 
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */
    #endregion

    [TestFixture]
    public class TestLurchTableThreading
    {
        private const int MAXTHREADS = 8;
        private const int COUNT = 1000;
        static LurchTable<HashCollider, T> CreateMap<T>()
        {
            var ht = new LurchTable<HashCollider, T>(COUNT, LurchTableOrder.Access);
            return ht;
        }

        private static void Parallel<T>(int loopCount, T[] args, Action<T> task)
        {
            var timer = Stopwatch.StartNew();
            int[] ready = new[] { 0 };
            ManualResetEvent start = new ManualResetEvent(false);
            int nthreads = Math.Min(MAXTHREADS, args.Length);
            var threads = new Thread[nthreads];
            for (int i = 0; i < threads.Length; i++)
            {
                threads[i] = new Thread((ithread) =>
                {
                    Interlocked.Increment(ref ready[0]);
                    start.WaitOne();
                    for (int loop = 0; loop < loopCount; loop++)
                        for (int ix = (int)ithread; ix < args.Length; ix += nthreads)
                            task(args[ix]);
                });
            }

            int threadIx = 0;
            foreach (var t in threads)
                t.Start(threadIx++);

            while (Interlocked.CompareExchange(ref ready[0], 0, 0) < nthreads)
                Thread.Sleep(0);

            start.Set();

            foreach (var t in threads)
                t.Join();

            Trace.TraceInformation("Execution time: {0}", timer.Elapsed);
        }

        [Test]
        public void TestHashCollision()
        {
            HashCollider id1 = new HashCollider(Guid.NewGuid().GetHashCode(), 0);
            HashCollider id2 = NextHashCollision(Guid.NewGuid().GetHashCode(), ref id1);

            Assert.AreNotEqual(id1, id2);
            Assert.AreEqual(id1.GetHashCode(), id2.GetHashCode());
        }

        [Test]
        public void TestLimitedInsert()
        {
            var Map = new LurchTable<HashCollider, bool>(LurchTableOrder.Access, 1000);
            var ids = CreateSample(0, 1000000, 0);

            Parallel(1, ids,
                     id =>
                     {
                         bool test;
                         Assert.IsTrue(Map.TryAdd(id, true));
                         Map.TryGetValue(id, out test);
                     });

            Assert.AreEqual(1000, Map.Count);
        }

        [Test]
        public void TestInsert()
        {
            var Map = CreateMap<bool>();
            var ids = CreateSample(0, COUNT, 4);

            bool test;
            Parallel(1, ids, id => { Assert.IsTrue(Map.TryAdd(id, true)); });

            Assert.AreEqual(ids.Length, Map.Count);
            foreach (var id in ids)
                Assert.IsTrue(Map.TryGetValue(id, out test) && test);
        }

        [Test]
        public void TestDelete()
        {
            var Map = CreateMap<bool>();
            var ids = CreateSample(0, COUNT, 4);
            foreach (var id in ids)
                Assert.IsTrue(Map.TryAdd(id, true));

            bool test;
            Parallel(1, ids, id => { Assert.IsTrue(Map.Remove(id)); });

            Assert.AreEqual(0, Map.Count);
            foreach (var id in ids)
                Assert.IsTrue(!Map.TryGetValue(id, out test));
        }

        [Test]
        public void TestInsertDelete()
        {
            var Map = CreateMap<bool>();
            var ids = CreateSample(0, COUNT, 4);

            bool test;
            Parallel(100, ids, id => { Assert.IsTrue(Map.TryAdd(id, true)); Assert.IsTrue(Map.Remove(id)); });

            Assert.AreEqual(0, Map.Count);
            foreach (var id in ids)
                Assert.IsTrue(!Map.TryGetValue(id, out test));
        }

        [Test]
        public void TestInsertUpdateDelete()
        {
            var Map = CreateMap<bool>();
            var ids = CreateSample(0, COUNT, 4);

            bool test;
            Parallel(100, ids, id => { Assert.IsTrue(Map.TryAdd(id, true)); Assert.IsTrue(Map.TryUpdate(id, false, true)); Assert.IsTrue(Map.Remove(id)); });

            Assert.AreEqual(0, Map.Count);
            foreach (var id in ids)
                Assert.IsTrue(!Map.TryGetValue(id, out test));
        }

        [Test]
        public void CompareTest()
        {
            const int size = 1000000;
            int reps = 3;
            Stopwatch timer;

            IDictionary<HashCollider, TestValue> dict = new ConcurrentDictionary<HashCollider, TestValue>(new Dictionary<HashCollider, TestValue>(size));
            IDictionary<HashCollider, TestValue> test = new LurchTable<HashCollider, TestValue>(size);

            for (int rep = 0; rep < reps; rep++)
            {
                var sample = CreateSample(0, size, 1);

                timer = Stopwatch.StartNew();
                Parallel(1, sample, item => dict.Add(item, new TestValue { Id = item, Count = rep }));
                Trace.TraceInformation("Dict Add: {0}", timer.Elapsed);

                timer = Stopwatch.StartNew();
                Parallel(1, sample, item => test.Add(item, new TestValue { Id = item, Count = rep }));
                Trace.TraceInformation("Test Add: {0}", timer.Elapsed);

                timer = Stopwatch.StartNew();
                Parallel(1, sample, item => dict[item] = new TestValue { Id = item, Count = rep });
                Trace.TraceInformation("Dict Update: {0}", timer.Elapsed);

                timer = Stopwatch.StartNew();
                Parallel(1, sample, item => test[item] = new TestValue { Id = item, Count = rep });
                Trace.TraceInformation("Test Update: {0}", timer.Elapsed);

                timer = Stopwatch.StartNew();
                Parallel(1, sample, item => dict.Remove(item));
                Trace.TraceInformation("Dict Rem: {0}", timer.Elapsed);
                Assert.AreEqual(0, dict.Count);

                timer = Stopwatch.StartNew();
                Parallel(1, sample, item => test.Remove(item));
                Trace.TraceInformation("Test Rem: {0}", timer.Elapsed);

                test.Clear();
                dict.Clear();

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        struct TestValue
        {
            public HashCollider Id;
            public int Count;
        };

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1067:Override Object.Equals(object) when implementing IEquatable<T>", Justification = "by design")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0079:Remove unnecessary suppression", Justification = "CA1067 doesn't fire on all target frameworks")]
        internal struct HashCollider : IEquatable<HashCollider>
        {
            public int Value;
            public int CollisionValue;

            public HashCollider(int value, int collisionValue)
            {
                this.Value = value;
                this.CollisionValue = collisionValue;
            }

            public bool Equals(HashCollider other)
            {
                return Value.Equals(other.Value);
            }

            public override int GetHashCode()
            {
                if (CollisionValue > 0)
                {
                    return CollisionValue;
                }
                return Value;
            }
        }

        #region Hash Collision Generator

        // J2N: Refactored this so it doesn't depend on the underlying implementation of Guid, which varies
        // depending on the platform it is running on.

        private static int hashCollision = 1000000000;

        internal static HashCollider[] CreateSample(int seed, int size, double collisions)
        {
            var sample = new HashCollider[size];
            int count = 0, collis = 0, uix = 0;
            for (int i = 0; i < size; i++)
            {
                if (collis >= count * collisions)
                {
                    sample[uix = i] = new HashCollider(value: i, collisionValue: 0);
                    count++;
                }
                else
                {
                    sample[i] = NextHashCollision(value: i, collideWith: ref sample[uix]);
                    collis++;
                }
            }
            return sample;
        }

        internal static HashCollider NextHashCollision(int value, ref HashCollider collideWith)
        {
            int collisionValue = Interlocked.Increment(ref hashCollision);
            collideWith.CollisionValue = collisionValue;
            var result = new HashCollider(value, collisionValue);
            Assert.AreEqual(collideWith.GetHashCode(), result.GetHashCode());
            return result;
        }

        #endregion
    }
}

