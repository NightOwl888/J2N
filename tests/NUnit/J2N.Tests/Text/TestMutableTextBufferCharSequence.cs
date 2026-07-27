#region Copyright 2019-2026 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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

using J2N.Buffers;
using NUnit.Framework;
using System;
using System.Text;
#nullable enable

namespace J2N.Text
{
    internal class TestMutableTextBufferCharSequence : CharSequenceTestBase<MutableTextBufferCharSequence>
    {
        public override void SetUp()
        {
            base.SetUp();

            target = new MutableTextBufferCharSequence(new MutableTextBuffer(ArrayAllocator<char>.Default).Initialize(CharArray1));
            nullTarget = new MutableTextBufferCharSequence(null);
            equalTarget = new MutableTextBufferCharSequence(new MutableTextBuffer(ArrayAllocator<char>.Default).Initialize(CharArray1));
            unequalTarget = new MutableTextBufferCharSequence(new MutableTextBuffer(ArrayAllocator<char>.Default).Initialize(CharArray2));
            emptyTarget = new MutableTextBufferCharSequence(new MutableTextBuffer(ArrayAllocator<char>.Default).Initialize());
        }

        protected override int CompareToString(MutableTextBufferCharSequence target, string? value) => target.CompareTo(value);
        protected override int CompareToCharArray(MutableTextBufferCharSequence target, char[]? value) => target.CompareTo(value);
        protected override int CompareToStringBuilder(MutableTextBufferCharSequence target, StringBuilder? value) => target.CompareTo(value);
        protected override int CompareToReadOnlySpan(MutableTextBufferCharSequence target, ReadOnlySpan<char> value) => target.CompareTo(value);
        protected override int CompareToObject(MutableTextBufferCharSequence target, object? value) => target.CompareTo(value);

        protected override bool EqualsString(MutableTextBufferCharSequence target, string? value) => target.Equals(value);
        protected override bool EqualsCharArray(MutableTextBufferCharSequence target, char[]? value) => target.Equals(value);
        protected override bool EqualsStringBuilder(MutableTextBufferCharSequence target, StringBuilder? value) => target.Equals(value);
        protected override bool EqualsReadOnlySpan(MutableTextBufferCharSequence target, ReadOnlySpan<char> value) => target.Equals(value);

        [Test]
        public void TestValue()
        {
            Assert.IsNotNull(target.Value);
            Assert.AreEqual(String1, target.Value!.ToString());

            Assert.IsNull(nullTarget.Value);
        }

        //[Test]
        //public virtual void TestEqualityOperators()
        //{
        //    Assert.IsTrue(target == equalTarget);
        //    Assert.IsTrue(equalTarget == target);

        //    Assert.IsFalse(target == unequalTarget);
        //    Assert.IsFalse(unequalTarget == target);

        //    Assert.IsTrue(target == CharArray1);
        //    Assert.IsTrue(CharArray1 == target);

        //    Assert.IsFalse(target == CharArray2);
        //    Assert.IsFalse(CharArray2 == target);

        //    Assert.IsTrue(nullTarget == (char[])null);
        //    Assert.IsTrue((char[])null == nullTarget);


        //    Assert.IsFalse(target != equalTarget);
        //    Assert.IsFalse(equalTarget != target);

        //    Assert.IsTrue(target != unequalTarget);
        //    Assert.IsTrue(unequalTarget != target);

        //    Assert.IsTrue(target != CharArray2);
        //    Assert.IsTrue(CharArray2 != target);

        //    Assert.IsFalse(target != CharArray1);
        //    Assert.IsFalse(CharArray1 != target);

        //    Assert.IsFalse(nullTarget != (char[])null);
        //    Assert.IsFalse((char[])null != nullTarget);
        //}
    }
}
