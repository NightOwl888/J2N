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
using System.Text.RegularExpressions;


namespace J2N.Text
{
    /// <summary>
    /// Extensions to <see cref="T:string[]"/>.
    /// </summary>
    public static class StringArrayExtensions
    {
        /// <summary>
        /// Removes <c>null</c> or empty elements from the end of a string array.
        /// <para/>
        /// Usage Note: In Java, the <c>Split()</c> methods of the <c>java.util.regex.Pattern</c> class
        /// trim empty entries from the end of the array automatically. This method can be used
        /// on the result of either the static or instance <see cref="Regex.Split(string)"/>
        /// overloads to remove the empty entries from the end of the array, but leave them
        /// in the beginning and middle of the array.
        /// <code>
        /// string[] split = Regex.Split("  The string with spaces.   ", "\\s+");
        /// 
        /// // split : { "", "", "The", "string", "with", "spaces", "", "", "" }
        /// 
        /// string[] splitAndTrimmed = split.TrimEnd();
        /// 
        /// // splitAndTrimmed : { "", "", "The", "string", "with", "spaces" }
        /// </code>
        /// The methods can be safely combined to produce the results seamlessly
        /// <c>string[] split = Regex.Split("  The string with spaces.   ", "\\s+").TrimEnd();</c>
        /// <para/>
        /// A <c>null</c> array <paramref name="input"/> will return a <c>null</c> array.
        /// </summary>
        /// <param name="input">This string array.</param>
        /// <returns>The array with any null or empty elements removed from the end.</returns>
        public static string?[]? TrimEnd(this string?[]? input)
        {
            if (input is null)
                return null;
            int end;
            int inputLength = input.Length;
            for (end = inputLength; end > 0; end--)
            {
                if (!string.IsNullOrEmpty(input[end - 1]))
                    break;
            }
            if (end < inputLength)
            {
                string[] result = new string[end];
                if (end > 0)
                    Array.Copy(input, result, end);
                return result;
            }
            return input;
        }
    }
}
