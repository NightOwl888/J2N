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

namespace J2N.Text
{
    /// <summary>
    /// Extensions to <see cref="SynchronizedTextBuilder"/>.
    /// </summary>
    public static partial class SynchronizedTextBuilderExtensions
    {
        #region AsCharSequence

        /// <summary>
        /// Convenience method to wrap a string in a <see cref="SynchronizedTextBuilderCharSequence"/>
        /// so a <see cref="SynchronizedTextBuilder"/> can be used as <see cref="ICharSequence"/>.
        /// </summary>
        public static SynchronizedTextBuilderCharSequence AsCharSequence(this SynchronizedTextBuilder? text)
        {
            return new SynchronizedTextBuilderCharSequence(text);
        }

        #endregion AsCharSequence
    }
}
