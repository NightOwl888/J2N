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

using System;

namespace J2N.Text
{
    public sealed class SpanCopyableCharSequence : ICharSequence, ISpanCopyable<char>
    {
        private readonly ReadOnlyMemory<char> memory;

        public SpanCopyableCharSequence(ReadOnlyMemory<char> memory)
        {
            this.memory = memory;
        }

        public bool HasValue => true;

        public int Length => memory.Length;

        public char this[int index] => memory.Span[index];

        public void CopyTo(int sourceIndex, Span<char> destination, int count)
        {
            memory.Span.Slice(sourceIndex, count)
                .CopyTo(destination);
        }

        public ICharSequence Subsequence(int startIndex, int length)
            => new CharArrayCharSequence(memory.ToArray());

        public override string ToString() => memory.Span.ToString();
    }
}
