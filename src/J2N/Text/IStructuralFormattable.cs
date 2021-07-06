#region Copyright 2019-2021 by Shad Storhaug, Licensed under the Apache License, Version 2.0
/*Licensed to the Apache Software Foundation (ASF) under one or more
 *  contributor license agreements.  See the NOTICE file distributed with
 *  this work for additional information regarding copyright ownership.
 *  The ASF licenses this file to You under the Apache License, Version 2.0
 *  (the "License"); you may not use this file except in compliance with
 *  the License.  You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 *Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */
#endregion

using System;


namespace J2N.Text
{
    /// <summary>
    /// Provides functionality to format the value of an collection into a string representation.
    /// </summary>
    internal interface IStructuralFormattable : IFormattable
    {
        /// <summary>
        /// Formats the value of the current instance using the specified format.
        /// </summary>
        /// <param name="format">
        /// The format to use. -or- A null reference (Nothing in Visual Basic) to use the
        /// default format defined for the type of the <see cref="System.IFormattable"/> implementation.</param>
        /// <param name="formatProvider">
        /// The provider to use to format the value. -or- A null reference (Nothing in Visual
        /// Basic) to obtain the numeric format information from the current locale setting
        /// of the operating system.</param>
        /// <returns>The value of the current instance in the specified format.</returns>
        new string ToString(string? format, IFormatProvider? formatProvider);
    }
}
