#region Copyright 2019-2025 by Shad Storhaug, Licensed under the Apache License, Version 2.0
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

namespace J2N
{
    /// <summary>
    /// Specifies the format to use when converting a boolean to a string.
    /// </summary>
    public enum BooleanFormat
    {
        /// <summary>
        /// Use lowercase "true" and "false". This is standard in Java.
        /// </summary>
        Lowercase,
        /// <summary>
        /// Use titlecase "True" and "False". This is standard in .NET.
        /// </summary>
        TitleCase
    }
}
