using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
#if FEATURE_IMMUTABLEARRAY
using System.Collections.Immutable;
#endif

namespace J2N
{
    [TestFixture]
    public class TestGenericType : TestCase
    {
        #region IsStructuralEquatable Tests

        /**
         * @tests GenericType<T>#IsStructuralEquatable
         */
        [Test]
        public void Test_IsStructuralEquatable_Array()
        {
            // Arrays implement IStructuralEquatable
            assertTrue(GenericType<int[]>.IsStructuralEquatable);
            assertTrue(GenericType<string[]>.IsStructuralEquatable);
            assertTrue(GenericType<object[]>.IsStructuralEquatable);
        }

        /**
         * @tests GenericType<T>#IsStructuralEquatable
         */
        [Test]
        public void Test_IsStructuralEquatable_Tuple()
        {
            // Tuples implement IStructuralEquatable
            assertTrue(GenericType<Tuple<int, string>>.IsStructuralEquatable);
            assertTrue(GenericType<Tuple<int>>.IsStructuralEquatable);
        }

        /**
         * @tests GenericType<T>#IsStructuralEquatable
         */
        [Test]
        public void Test_IsStructuralEquatable_ValueTuple()
        {
            // ValueTuples implement IStructuralEquatable
            assertTrue(GenericType<ValueTuple<int, string>>.IsStructuralEquatable);
            assertTrue(GenericType<ValueTuple<int>>.IsStructuralEquatable);
        }

        /**
         * @tests GenericType<T>#IsStructuralEquatable
         */
        [Test]
        public void Test_IsStructuralEquatable_PrimitiveTypes()
        {
            // Primitive types do NOT implement IStructuralEquatable
            assertFalse(GenericType<int>.IsStructuralEquatable);
            assertFalse(GenericType<string>.IsStructuralEquatable);
            assertFalse(GenericType<bool>.IsStructuralEquatable);
            assertFalse(GenericType<double>.IsStructuralEquatable);
        }

        /**
         * @tests GenericType<T>#IsStructuralEquatable
         */
        [Test]
        public void Test_IsStructuralEquatable_Collections()
        {
            // Collections do NOT implement IStructuralEquatable
            assertFalse(GenericType<List<int>>.IsStructuralEquatable);
            assertFalse(GenericType<Dictionary<string, int>>.IsStructuralEquatable);
            assertFalse(GenericType<HashSet<string>>.IsStructuralEquatable);
        }

        #endregion

        #region IsCollection Tests - Non-Generic Collections

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_ArrayList()
        {
            assertTrue(GenericType<ArrayList>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_Hashtable()
        {
            assertTrue(GenericType<Hashtable>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_Queue()
        {
            assertTrue(GenericType<Queue>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_Stack()
        {
            assertTrue(GenericType<Stack>.IsCollection);
        }

        #endregion

        #region IsCollection Tests - Generic Collections

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_ListOfInt()
        {
            assertTrue(GenericType<List<int>>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_ListOfString()
        {
            assertTrue(GenericType<List<string>>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_HashSetOfString()
        {
            assertTrue(GenericType<HashSet<string>>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_HashSetOfInt()
        {
            assertTrue(GenericType<HashSet<int>>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_DictionaryStringString()
        {
            // Dictionary<T, T> where both types are same (string, string)
            assertTrue(GenericType<Dictionary<string, string>>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_DictionaryIntInt()
        {
            // Dictionary<T, T> where both types are same (int, int)
            assertTrue(GenericType<Dictionary<int, int>>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_QueueOfString()
        {
            assertTrue(GenericType<Queue<string>>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_StackOfInt()
        {
            assertTrue(GenericType<Stack<int>>.IsCollection);
        }

        #endregion

        #region IsCollection Tests - Arrays

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_IntArray()
        {
            assertTrue(GenericType<int[]>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_StringArray()
        {
            assertTrue(GenericType<string[]>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_ObjectArray()
        {
            assertTrue(GenericType<object[]>.IsCollection);
        }

        #endregion

#if FEATURE_IREADONLYCOLLECTIONS

        #region IsCollection Tests - ReadOnly Collections

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_IReadOnlyListOfInt()
        {
            assertTrue(GenericType<IReadOnlyList<int>>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_IReadOnlyCollectionOfString()
        {
            assertTrue(GenericType<IReadOnlyCollection<string>>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_IReadOnlyDictionaryStringString()
        {
            assertTrue(GenericType<IReadOnlyDictionary<string, string>>.IsCollection);
        }

        #endregion

#endif

#if FEATURE_READONLYSET

        #region IsCollection Tests - ReadOnly Set

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_IReadOnlySetOfInt()
        {
            assertTrue(GenericType<IReadOnlySet<int>>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_IReadOnlySetOfString()
        {
            assertTrue(GenericType<IReadOnlySet<string>>.IsCollection);
        }

        #endregion

#endif

        #region IsCollection Tests - Negative Cases

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_Int()
        {
            assertFalse(GenericType<int>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_String()
        {
            assertFalse(GenericType<string>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_Bool()
        {
            assertFalse(GenericType<bool>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_Double()
        {
            assertFalse(GenericType<double>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_DateTime()
        {
            assertFalse(GenericType<DateTime>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_Guid()
        {
            assertFalse(GenericType<Guid>.IsCollection);
        }

        #endregion

#if FEATURE_IMMUTABLEARRAY

        #region IsCollection Tests - ImmutableArray

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_ImmutableArrayOfInt()
        {
            assertTrue(GenericType<ImmutableArray<int>>.IsCollection);
        }

        /**
         * @tests GenericType<T>#IsCollection
         */
        [Test]
        public void Test_IsCollection_ImmutableArrayOfString()
        {
            assertTrue(GenericType<ImmutableArray<string>>.IsCollection);
        }

        #endregion

#endif

        #region Bug Fix Verification Tests

        /**
         * @tests GenericType<T>#IsCollection
         *
         * Verifies that non-generic types can be checked for IsCollection
         * without crashing, even with FEATURE_IMMUTABLEARRAY enabled.
         */
        [Test]
        public void Test_IsCollection_NonGenericTypesShouldNotCrash()
        {
            // These should all return false without throwing exceptions
            assertFalse(GenericType<int>.IsCollection);
            assertFalse(GenericType<string>.IsCollection);
            assertFalse(GenericType<object>.IsCollection);
        }

        #endregion
    }
}
