using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using StringBuffer = System.Text.StringBuilder;

namespace J2N
{
    /// <summary>
    /// Extension methods that turn any <see cref="IDictionary{String, String}"/> into a set of
    /// properties that can read from or be written to the Java <c>.properties</c> file format.
    /// </summary>
    public static class PropertyExtensions
    {
        static PropertyExtensions()
        {
#if NETSTANDARD
            // Support for iso-8859-1 encoding. See: https://docs.microsoft.com/en-us/dotnet/api/system.text.codepagesencodingprovider?view=netcore-2.0
            var encodingProvider = CodePagesEncodingProvider.Instance;
            System.Text.Encoding.RegisterProvider(encodingProvider);
#endif
        }


        /// <summary>
        /// Retrieves the value of a property from the current dictionary
        /// as <see cref="bool"/>. If the value cannot be cast to <see cref="bool"/>, returns <c>false</c>.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The named property value, or <c>false</c> if it can't be found.</returns>
        public static bool GetPropertyAsBoolean(this IDictionary<string, string> properties, string name)
        {
            return GetPropertyAsBoolean(properties, name, false);
        }

        /// <summary>
        /// Retrieves the value of a property from the current dictionary as <see cref="bool"/>, 
        /// with a default value if it doesn't exist or the value cannot be cast to a <see cref="bool"/>.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The value to use if the property does not exist
        /// or the value cannot be cast to <see cref="bool"/>.</param>
        /// <returns>The named property value, or <paramref name="defaultValue"/> if it can't be found or cannot be converted to a <see cref="bool"/>.</returns>
        public static bool GetPropertyAsBoolean(this IDictionary<string, string> properties, string name, bool defaultValue)
        {
            return GetProperty(properties, name, defaultValue,
                (stringValue) =>
                {
                    return bool.TryParse(stringValue, out bool value) ? value : defaultValue;
                }
            );
        }

        /// <summary>
        /// Retrieves the value of an environment variable from the current process
        /// as <see cref="int"/>. If the value cannot be converted to <see cref="int"/> using the ambient culture, returns <c>0</c>.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The named property value, or <c>0</c> if it can't be found or cannot be
        /// converted to a <see cref="int"/> using the ambient culture.</returns>
        public static int GetPropertyAsInt32(this IDictionary<string, string> properties, string name)
        {
            return GetPropertyAsInt32(properties, name, 0);
        }

        /// <summary>
        /// Retrieves the value of an environment variable from the current process
        /// as <see cref="int"/>. If the value cannot be converted to <see cref="int"/> using <paramref name="provider"/>, returns <c>0</c>.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="provider">An object that provides culture-specific formatting information about the value.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>The named property value, or <c>0</c> if it can't be found or cannot be
        /// converted to a <see cref="int"/> using <paramref name="provider"/>.</returns>
        public static int GetPropertyAsInt32(this IDictionary<string, string> properties, IFormatProvider provider, string name)
        {
            return GetPropertyAsInt32(properties, provider, name, 0);
        }

        /// <summary>
        /// Retrieves the value of an environment variable from the current process as <see cref="int"/>, 
        /// with a default value if it doens't exist, the caller doesn't have permission to read the value, 
        /// or the value cannot be converted to <see cref="int"/> using the ambient culture.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The value to use if property does not exist or the value cannot
        /// be converted to <see cref="int"/> using the ambient culture.</param>
        /// <returns>The named property value, or <paramref name="defaultValue"/> if it can't be found or cannot be
        /// converted to a <see cref="int"/> using the ambient culture.</returns>
        public static int GetPropertyAsInt32(this IDictionary<string, string> properties, string name, int defaultValue)
        {
            return GetProperty(properties, name, defaultValue,
                (stringValue) =>
                {
                    return int.TryParse(stringValue, NumberStyles.Integer, NumberFormatInfo.CurrentInfo, out int value) ? value : defaultValue;
                }
            );
        }

        /// <summary>
        /// Retrieves the value of an environment variable from the current process as <see cref="int"/>, 
        /// with a default value if it doens't exist, the caller doesn't have permission to read the value, 
        /// or the value cannot be converted to <see cref="int"/> using <paramref name="provider"/>.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="provider">An object that provides culture-specific formatting information about the value.</param>
        /// <param name="name">The name of the property.</param>
        /// <param name="defaultValue">The value to use if property does not exist or the value cannot
        /// be converted to <see cref="int"/> using <paramref name="provider"/>.</param>
        /// <returns>The named property value, or <paramref name="defaultValue"/> if it can't be found or cannot be
        /// converted to a <see cref="int"/> using <paramref name="provider"/>.</returns>
        public static int GetPropertyAsInt32(this IDictionary<string, string> properties, IFormatProvider provider, string name, int defaultValue)
        {
            return GetProperty(properties, name, defaultValue,
                (stringValue) =>
                {
                    return int.TryParse(stringValue, NumberStyles.Integer, NumberFormatInfo.GetInstance(provider), out int value) ? value : defaultValue;
                }
            );
        }

        private static T GetProperty<T>(IDictionary<string, string> properties, string key, T defaultValue, Func<string, T> conversionFunction)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            if (properties.TryGetValue(key, out string setting))
                return string.IsNullOrEmpty(setting)
                    ? defaultValue
                    : conversionFunction(setting);

            return defaultValue;
        }


        /// <summary>
        /// Searches for the property with the specified name. If the property is not
        /// found, <c>null</c> is returned.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="name">The name of the property to find.</param>
        /// <returns>The named property value, or <c>null</c> if it can't be found.</returns>
        public static string GetProperty(this IDictionary<string, string> properties, string name)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            return properties.TryGetValue(name, out string value) ? value : null;
        }

        /// <summary>
        /// Searches for the property with the specified name. If the property is not
        /// found, <paramref name="defaultValue"/> is returned.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="name">The name of the property to find.</param>
        /// <param name="defaultValue"></param>
        /// <returns>The named property value, or <paramref name="defaultValue"/> if it can't be found.</returns>
        public static string GetProperty(this IDictionary<string, string> properties, string name, string defaultValue)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));

            return properties.TryGetValue(name, out string value) ? value : defaultValue;
        }

        /// <summary>
        /// Reads a property list (key and element pairs) from the input
        /// byte stream. The input stream is in a simple line-oriented
        /// format as specified in <see cref="LoadProperties(IDictionary{string, string}, TextReader)"/>
        /// and is assumed to use
        /// the ISO 8859-1 character encoding; that is each byte is one Latin1
        /// character. Characters not in Latin1, and certain special characters,
        /// are represented in keys and elements using Unicode escapes as defined in
        /// section 3.3 of <i>The Java&#8482; Language Specification</i>.
        /// <para/>
        /// The specified <paramref name="input"/> stream remains open after this method returns.
        /// <para/>
        /// The file format is compatible with Java properties, so they can easily be written or consumed by .NET.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="input">The input stream.</param>
        /// <exception cref="IOException">If an error occurred while reading from the input stream.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="properties"/> or <paramref name="input"/> is <c>null</c>.</exception>
        public static void LoadProperties(this IDictionary<string, string> properties, Stream input)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            if (input == null)
                throw new ArgumentNullException(nameof(input));

            lock (properties.GetSyncRoot())
            {
                LoadProperties(properties, new LineReader(input));
            }
        }


        /// <summary>
        /// Reads a property list (key and element pairs) from the input
        /// <see cref="TextReader"/> in a simple line-oriented format.
        /// <para/>
        /// The file format is compatible with Java properties, so they can easily be written or consumed by .NET.
        /// <para/>
        /// Properties are processed in terms of lines. There are two
        /// kinds of line, <i>natural lines</i> and <i>logical lines</i>.
        /// A natural line is defined as a line of
        /// characters that is terminated either by a set of line terminator
        /// characters (<c>\n</c> or <c>\r</c> or <c>\r\n</c>)
        /// or by the end of the stream. A natural line may be either a blank line,
        /// a comment line, or hold all or some of a key-element pair. A logical
        /// line holds all the data of a key-element pair, which may be spread
        /// out across several adjacent natural lines by escaping
        /// the line terminator sequence with a backslash character
        /// <c>\</c>.  Note that a comment line cannot be extended
        /// in this manner; every natural line that is a comment must have
        /// its own comment indicator, as described below. Lines are read from
        /// input until the end of the stream is reached.
        ///
        /// <para/>
        /// A natural line that contains only white space characters is
        /// considered blank and is ignored.  A comment line has an ASCII
        /// <c>'#'</c> or <c>'!'</c> as its first non-white
        /// space character; comment lines are also ignored and do not
        /// encode key-element information.  In addition to line
        /// terminators, this format considers the characters space
        /// (<c>' '</c>, <c>'&#92;u0020'</c>), tab
        /// (<c>'\t'</c>, <c>'&#92;u0009'</c>), and form feed
        /// (<c>'\f'</c>, <c>'&#92;u000C'</c>) to be white
        /// space.
        ///
        /// <para/>
        /// If a logical line is spread across several natural lines, the
        /// backslash escaping the line terminator sequence, the line
        /// terminator sequence, and any white space at the start of the
        /// following line have no affect on the key or element values.
        /// The remainder of the discussion of key and element parsing
        /// (when loading) will assume all the characters constituting
        /// the key and element appear on a single natural line after
        /// line continuation characters have been removed.  Note that
        /// it is <i>not</i> sufficient to only examine the character
        /// preceding a line terminator sequence to decide if the line
        /// terminator is escaped; there must be an odd number of
        /// contiguous backslashes for the line terminator to be escaped.
        /// Since the input is processed from left to right, a
        /// non-zero even number of 2<i>n</i> contiguous backslashes
        /// before a line terminator (or elsewhere) encodes <i>n</i>
        /// backslashes after escape processing.
        ///
        /// <para/>
        /// The key contains all of the characters in the line starting
        /// with the first non-white space character and up to, but not
        /// including, the first unescaped <c>'='</c>,
        /// <c>':'</c>, or white space character other than a line
        /// terminator. All of these key termination characters may be
        /// included in the key by escaping them with a preceding backslash
        /// character; for example,<para/>
        ///
        /// <code>\:\=</code><para/>
        ///
        /// would be the two-character key <c>":="</c>.  Line
        /// terminator characters can be included using <c>\r</c> and
        /// <c>\n</c> escape sequences.  Any white space after the
        /// key is skipped; if the first non-white space character after
        /// the key is <c>'='</c> or <c>':'</c>, then it is
        /// ignored and any white space characters after it are also
        /// skipped.  All remaining characters on the line become part of
        /// the associated element string; if there are no remaining
        /// characters, the element is the empty string
        /// <c>&quot;&quot;</c>.  Once the raw character sequences
        /// constituting the key and element are identified, escape
        /// processing is performed as described above.
        ///
        /// <para/>
        /// As an example, each of the following three lines specifies the key
        /// <c>"Truth"</c> and the associated element value
        /// <c>"Beauty"</c>:
        /// <para/>
        /// <code>
        /// Truth = Beauty
        ///  Truth:Beauty
        /// Truth                    :Beauty
        /// </code>
        /// As another example, the following three lines specify a single
        /// property:
        /// <para/>
        /// <code>
        /// fruits                           apple, banana, pear, \
        ///                                  cantaloupe, watermelon, \
        ///                                  kiwi, mango
        /// </code>
        /// The key is <c>"fruits"</c> and the associated element is:
        /// <para/>
        /// <code>"apple, banana, pear, cantaloupe, watermelon, kiwi, mango"</code>
        /// Note that a space appears before each <c>\</c> so that a space
        /// will appear after each comma in the final result; the <c>\</c>,
        /// line terminator, and leading white space on the continuation line are
        /// merely discarded and are <i>not</i> replaced by one or more other
        /// characters.
        /// <para/>
        /// As a third example, the line:
        /// <para/>
        /// <code>cheeses
        /// </code>
        /// specifies that the key is <c>"cheeses"</c> and the associated
        /// element is the empty string <c>""</c>.<para/>
        /// <para/>
        ///
        /// <a name="unicodeescapes"></a>
        /// Characters in keys and elements can be represented in escape
        /// sequences similar to those used for character and string literals
        /// (see sections 3.3 and 3.10.6 of
        /// <i>The Java&#8482; Language Specification</i>).
        ///
        /// The differences from the character escape sequences and Unicode
        /// escapes used for characters and strings are:
        ///
        /// <list type="bullet">
        /// <item><description>Octal escapes are not recognized.</description></item>
        ///
        /// <item><description>The character sequence <c>\b</c> does <i>not</i>
        /// represent a backspace character.</description></item>
        ///
        /// <item><description>The method does not treat a backslash character,
        /// <c>\</c>, before a non-valid escape character as an
        /// error; the backslash is silently dropped.  For example, in a
        /// Java string the sequence <c>"\z"</c> would cause a
        /// compile time error.  In contrast, this method silently drops
        /// the backslash.  Therefore, this method treats the two character
        /// sequence <c>"\b"</c> as equivalent to the single
        /// character <c>'b'</c>.</description></item>
        ///
        /// <item><description>Escapes are not necessary for single and double quotes;
        /// however, by the rule above, single and double quote characters
        /// preceded by a backslash still yield single and double quote
        /// characters, respectively.</description></item>
        ///
        /// <item><description>Only a single 'u' character is allowed in a Uniocde escape
        /// sequence.</description></item>
        ///
        /// </list>
        /// <para/>
        /// The specified stream remains open after this method returns.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="reader">The input <see cref="TextReader"/>.</param>
        /// <exception cref="IOException">If an error occurs when reading from the
        /// input stream.</exception>
        /// <exception cref="ArgumentException">If a malformed Unicode escape
        /// appears in the input.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="properties"/> or <paramref name="reader"/> is <c>null</c>.</exception>
        public static void LoadProperties(this IDictionary<string, string> properties, TextReader reader)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            if (reader == null)
                throw new ArgumentNullException(nameof(reader));

            lock (properties.GetSyncRoot())
            {
                LoadProperties(properties, new LineReader(reader));
            }
        }

        /// <summary>
        /// Writes this property list (key and element pairs) in this
        /// <see cref="IDictionary{String, String}"/> table to the <see cref="TextWriter"/>
        /// in a format suitable for using the <see cref="LoadProperties(IDictionary{string, string}, Stream)"/>
        /// method.
        /// <para/>
        /// The file format is compatible with Java properties, so they can easily be written or consumed by .NET.
        /// <para/>
        /// This method outputs the properties keys and values in
        /// the same format as specified in
        /// <see cref="SaveProperties(IDictionary{string, string}, TextWriter)"/>,
        /// with the following differences:
        /// <para/>
        /// The stream is written using the ISO 8859-1 character encoding.
        /// <para/>
        /// Characters not in Latin-1 in the comments are written as
        /// <c>&#92;u</c><i>xxxx</i> for their appropriate unicode
        /// hexadecimal value <i>xxxx</i>.
        /// <para/>
        /// Characters less than <c>&#92;u0020</c> and characters greater
        /// than <code>&#92;u007E</code> in property keys or values are written
        /// as <c>&#92;u</c><i>xxxx</i> for the appropriate hexadecimal
        /// value <i>xxxx</i>.
        /// <para/>
        /// After the entries have been written, the output stream is flushed.
        /// The output stream remains open after this method returns.
        /// <para/>
        /// An attempt will be tried (but not guaranteed) to be thread-safe.
        /// If the <see cref="IDictionary{String, String}"/> implements <see cref="ICollection"/>
        /// and its <see cref="ICollection.IsSynchronized"/> property returns <c>true</c>,
        /// a lock will be placed on its <see cref="ICollection.SyncRoot"/> property.
        /// If the <see cref="IDictionary{String, String}"/> does not implement <see cref="ICollection"/>
        /// or its <see cref="ICollection.IsSynchronized"/> property returns <c>false</c>,
        /// a lock will be placed on the <paramref name="properties"/> object.
        /// Either way, it is up to the implementation of the <see cref="IDictionary{String, String}"/>
        /// and/or its calling code to respect thread safety.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="output">A <see cref="Stream"/> to write to.</param>
        /// <exception cref="IOException">If writing this property list to the specified
        /// output stream throws an <see cref="IOException"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="properties"/> or
        /// <paramref name="output"/> is <c>null</c>.</exception>
        public static void SaveProperties(this IDictionary<string, string> properties, Stream output)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            if (output == null)
                throw new ArgumentNullException(nameof(output));

            using (var sw = new StreamWriter(output, Encoding.GetEncoding("iso-8859-1"), 1024, true))
                Store0(properties, sw, null, true);
        }

        /// <summary>
        /// Writes this property list (key and element pairs) in this
        /// <see cref="IDictionary{String, String}"/> table to the <see cref="TextWriter"/>
        /// in a format suitable for using the <see cref="LoadProperties(IDictionary{string, string}, Stream)"/>
        /// method.
        /// <para/>
        /// The file format is compatible with Java properties, so they can easily be written or consumed by .NET.
        /// <para/>
        /// This method outputs the comments, properties keys and values in
        /// the same format as specified in
        /// <see cref="SaveProperties(IDictionary{string, string}, TextWriter, string)"/>,
        /// with the following differences:
        /// <para/>
        /// The stream is written using the ISO 8859-1 character encoding.
        /// <para/>
        /// Characters not in Latin-1 in the comments are written as
        /// <c>&#92;u</c><i>xxxx</i> for their appropriate unicode
        /// hexadecimal value <i>xxxx</i>.
        /// <para/>
        /// Characters less than <c>&#92;u0020</c> and characters greater
        /// than <code>&#92;u007E</code> in property keys or values are written
        /// as <c>&#92;u</c><i>xxxx</i> for the appropriate hexadecimal
        /// value <i>xxxx</i>.
        /// <para/>
        /// After the entries have been written, the output stream is flushed.
        /// The output stream remains open after this method returns.
        /// <para/>
        /// An attempt will be tried (but not guaranteed) to be thread-safe.
        /// If the <see cref="IDictionary{String, String}"/> implements <see cref="ICollection"/>
        /// and its <see cref="ICollection.IsSynchronized"/> property returns <c>true</c>,
        /// a lock will be placed on its <see cref="ICollection.SyncRoot"/> property.
        /// If the <see cref="IDictionary{String, String}"/> does not implement <see cref="ICollection"/>
        /// or its <see cref="ICollection.IsSynchronized"/> property returns <c>false</c>,
        /// a lock will be placed on the <paramref name="properties"/> object.
        /// Either way, it is up to the implementation of the <see cref="IDictionary{String, String}"/>
        /// and/or its calling code to respect thread safety.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="output">A <see cref="Stream"/> to write to.</param>
        /// <param name="comments">A description of the property list.</param>
        /// <exception cref="IOException">If writing this property list to the specified
        /// output stream throws an <see cref="IOException"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="properties"/>, 
        /// <paramref name="output"/>, or <paramref name="comments"/> is <c>null</c>.</exception>
        public static void SaveProperties(this IDictionary<string, string> properties, Stream output, string comments)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            if (output == null)
                throw new ArgumentNullException(nameof(output));
            if (comments == null)
                throw new ArgumentNullException(nameof(comments));

            using (var sw = new StreamWriter(output, Encoding.GetEncoding("iso-8859-1"), 1024, true))
                Store0(properties, sw, comments, true);
        }

        /// <summary>
        /// Writes this property list (key and element pairs) in this
        /// <see cref="IDictionary{String, String}"/> table to the <see cref="TextWriter"/>
        /// in a format suitable for using the <see cref="PropertyExtensions.LoadProperties(IDictionary{string, string}, TextReader)"/>
        /// method.
        /// <para/>
        /// The file format is compatible with Java properties, so they can easily be written or consumed by .NET.
        /// <para/>
        /// A comment line is always written, consisting of an ASCII
        /// <c>#</c> character, the current date and time (as if produced
        /// by <c>DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)</c>
        /// for the current time, and a line separator as generated by the <see cref="TextWriter"/>.
        /// <para/>
        /// Then every entry in this <paramref name="properties"/> dictionary is
        /// written out, one per line. For each entry the key string is
        /// written, then an ASCII <c>=</c>, then the associated
        /// element string. For the key, all space characters are
        /// written with a preceding <c>\</c> character.  For the
        /// element, leading space characters, but not embedded or trailing
        /// space characters, are written with a preceding <c>\</c>
        /// character. The key and element characters <c>#</c>,
        /// <c>!</c>, <c>=</c>, and <c>:</c> are written
        /// with a preceding backslash to ensure that they are properly loaded.
        /// <para/>
        /// After the entries have been written, the output stream is flushed.
        /// The output stream remains open after this method returns.
        /// <para/>
        /// An attempt will be tried (but not guaranteed) to be thread-safe.
        /// If the <see cref="IDictionary{String, String}"/> implements <see cref="ICollection"/>
        /// and its <see cref="ICollection.IsSynchronized"/> property returns <c>true</c>,
        /// a lock will be placed on its <see cref="ICollection.SyncRoot"/> property.
        /// If the <see cref="IDictionary{String, String}"/> does not implement <see cref="ICollection"/>
        /// or its <see cref="ICollection.IsSynchronized"/> property returns <c>false</c>,
        /// a lock will be placed on the <paramref name="properties"/> object.
        /// Either way, it is up to the implementation of the <see cref="IDictionary{String, String}"/>
        /// and/or its calling code to respect thread safety.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="writer">A <see cref="TextWriter"/>.</param>
        /// <exception cref="IOException">If writing this property list to the specified
        /// output stream throws an <see cref="IOException"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="properties"/> or
        /// <paramref name="writer"/> is <c>null</c>.</exception>
        public static void SaveProperties(this IDictionary<string, string> properties, TextWriter writer)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));

            Store0(properties, writer,
               null,
               false);
        }

        /// <summary>
        /// Writes this property list (key and element pairs) in this
        /// <see cref="IDictionary{String, String}"/> table to the <see cref="TextWriter"/>
        /// in a format suitable for using the <see cref="PropertyExtensions.LoadProperties(IDictionary{string, string}, TextReader)"/>
        /// method.
        /// <para/>
        /// The file format is compatible with Java properties, so they can easily be written or consumed by .NET.
        /// <para/>
        /// For the <paramref name="comments"/> argument, an ASCII <c>#</c>
        /// character, the comments string, and a line separater are first written to the output stream.
        /// Thus, the <paramref name="comments"/> can serve as an identifying comment. Any one of a line 
        /// feed ('\n'), a carriage return ('\r'), or a carriage return followed immediately by a line feed
        /// in comments is replaced by a line separator generated by the <see cref="TextWriter"/>
        /// and if the next character in comments is not character <c>#</c> or
        /// character <c>!</c> then an ASCII <c>#</c> is written out
        /// after that line separator.
        /// <para/>
        /// Next, a comment line is always written, consisting of an ASCII
        /// <c>#</c> character, the current date and time (as if produced
        /// by <c>DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)</c>
        /// for the current time, and a line separator as generated by the <see cref="TextWriter"/>.
        /// <para/>
        /// Then every entry in this <paramref name="properties"/> dictionary is
        /// written out, one per line. For each entry the key string is
        /// written, then an ASCII <c>=</c>, then the associated
        /// element string. For the key, all space characters are
        /// written with a preceding <c>\</c> character.  For the
        /// element, leading space characters, but not embedded or trailing
        /// space characters, are written with a preceding <c>\</c>
        /// character. The key and element characters <c>#</c>,
        /// <c>!</c>, <c>=</c>, and <c>:</c> are written
        /// with a preceding backslash to ensure that they are properly loaded.
        /// <para/>
        /// After the entries have been written, the output stream is flushed.
        /// The output stream remains open after this method returns.
        /// <para/>
        /// An attempt will be tried (but not guaranteed) to be thread-safe.
        /// If the <see cref="IDictionary{String, String}"/> implements <see cref="ICollection"/>
        /// and its <see cref="ICollection.IsSynchronized"/> property returns <c>true</c>,
        /// a lock will be placed on its <see cref="ICollection.SyncRoot"/> property.
        /// If the <see cref="IDictionary{String, String}"/> does not implement <see cref="ICollection"/>
        /// or its <see cref="ICollection.IsSynchronized"/> property returns <c>false</c>,
        /// a lock will be placed on the <paramref name="properties"/> object.
        /// Either way, it is up to the implementation of the <see cref="IDictionary{String, String}"/>
        /// and/or its calling code to respect thread safety.
        /// </summary>
        /// <param name="properties">A <see cref="IDictionary{String, String}"/> containing
        /// key value pairs that correspond to property names and values.</param>
        /// <param name="writer">A <see cref="TextWriter"/>.</param>
        /// <param name="comments">A description of the property list.</param>
        /// <exception cref="IOException">If writing this property list to the specified
        /// output stream throws an <see cref="IOException"/>.</exception>
        /// <exception cref="ArgumentNullException">If <paramref name="properties"/>, 
        /// <paramref name="writer"/>, or <paramref name="comments"/> is <c>null</c>.</exception>
        public static void SaveProperties(this IDictionary<string, string> properties, TextWriter writer, string comments)
        {
            if (properties == null)
                throw new ArgumentNullException(nameof(properties));
            if (writer == null)
                throw new ArgumentNullException(nameof(writer));
            if (comments == null)
                throw new ArgumentNullException(nameof(comments));

            Store0(properties, writer,
               comments,
               false);
        }

        private static object GetSyncRoot(this IDictionary<string, string> dictionary)
        {
            if (dictionary is ICollection collection && collection.IsSynchronized)
                return collection.SyncRoot;
            else
                return dictionary;
        }

        private static void LoadProperties(IDictionary<string, string> properties, LineReader lr)
        {
            char[] convtBuf = new char[1024];
            int limit;
            int keyLen;
            int valueStart;
            char c;
            bool hasSep;
            bool precedingBackslash;

            while ((limit = lr.ReadLine()) >= 0)
            {
                c = (char)0;
                keyLen = 0;
                valueStart = limit;
                hasSep = false;

                //System.out.println("line=<" + new String(lineBuf, 0, limit) + ">");
                precedingBackslash = false;
                while (keyLen < limit)
                {
                    c = lr.lineBuf[keyLen];
                    //need check if escaped.
                    if ((c == '=' || c == ':') && !precedingBackslash)
                    {
                        valueStart = keyLen + 1;
                        hasSep = true;
                        break;
                    }
                    else if ((c == ' ' || c == '\t' || c == '\f') && !precedingBackslash)
                    {
                        valueStart = keyLen + 1;
                        break;
                    }
                    if (c == '\\')
                    {
                        precedingBackslash = !precedingBackslash;
                    }
                    else
                    {
                        precedingBackslash = false;
                    }
                    keyLen++;
                }
                while (valueStart < limit)
                {
                    c = lr.lineBuf[valueStart];
                    if (c != ' ' && c != '\t' && c != '\f')
                    {
                        if (!hasSep && (c == '=' || c == ':'))
                        {
                            hasSep = true;
                        }
                        else
                        {
                            break;
                        }
                    }
                    valueStart++;
                }
                string key = LoadConvert(lr.lineBuf, 0, keyLen, convtBuf);
                string value = LoadConvert(lr.lineBuf, valueStart, limit - valueStart, convtBuf);
                properties[key] = value;
            }
        }

        /// <summary>
        /// Read in a "logical line" from an Stream/TextReader, skip all comment
        /// and blank lines and filter out those leading whitespace characters
        /// (\u0020, \u0009 and \u000c) from the beginning of a "natural line".
        /// Method returns the char length of the "logical line" and stores
        /// the line in "lineBuf".
        /// </summary>
        private class LineReader
        {
            public LineReader(Stream inStream)
            {
                this.inStream = inStream ?? throw new ArgumentNullException(nameof(inStream));
                inByteBuf = new byte[8192];
            }

            public LineReader(TextReader reader)
            {
                this.reader = reader ?? throw new ArgumentNullException(nameof(reader));
                inCharBuf = new char[8192];
            }

            internal byte[] inByteBuf;
            internal char[] inCharBuf;
            internal char[] lineBuf = new char[1024];
            internal int inLimit = 0;
            internal int inOff = 0;
            internal Stream inStream;
            internal TextReader reader;

            internal int ReadLine()
            {
                int len = 0;
                char c = (char)0;

                bool skipWhiteSpace = true;
                bool isCommentLine = false;
                bool isNewLine = true;
                bool appendedLineBegin = false;
                bool precedingBackslash = false;
                bool skipLF = false;

                while (true)
                {
                    if (inOff >= inLimit)
                    {
                        inLimit = (inStream == null) ? reader.Read(inCharBuf, 0, inCharBuf.Length)
                                                  : inStream.Read(inByteBuf, 0, inByteBuf.Length);
                        inOff = 0;
                        if (inLimit <= 0)
                        {
                            if (len == 0 || isCommentLine)
                            {
                                return -1;
                            }
                            return len;
                        }
                    }
                    if (inStream != null)
                    {
                        //The line below is equivalent to calling a
                        //ISO8859-1 decoder.
                        c = (char)(0xff & inByteBuf[inOff++]);
                    }
                    else
                    {
                        c = inCharBuf[inOff++];
                    }
                    if (skipLF)
                    {
                        skipLF = false;
                        if (c == '\n')
                        {
                            continue;
                        }
                    }
                    if (skipWhiteSpace)
                    {
                        if (c == ' ' || c == '\t' || c == '\f')
                        {
                            continue;
                        }
                        if (!appendedLineBegin && (c == '\r' || c == '\n'))
                        {
                            continue;
                        }
                        skipWhiteSpace = false;
                        appendedLineBegin = false;
                    }
                    if (isNewLine)
                    {
                        isNewLine = false;
                        if (c == '#' || c == '!')
                        {
                            isCommentLine = true;
                            continue;
                        }
                    }

                    if (c != '\n' && c != '\r')
                    {
                        lineBuf[len++] = c;
                        if (len == lineBuf.Length)
                        {
                            int newLength = lineBuf.Length * 2;
                            if (newLength < 0)
                            {
                                newLength = int.MaxValue;
                            }
                            char[] buf = new char[newLength];
                            System.Array.Copy(lineBuf, 0, buf, 0, lineBuf.Length);
                            lineBuf = buf;
                        }
                        //flip the preceding backslash flag
                        if (c == '\\')
                        {
                            precedingBackslash = !precedingBackslash;
                        }
                        else
                        {
                            precedingBackslash = false;
                        }
                    }
                    else
                    {
                        // reached EOL
                        if (isCommentLine || len == 0)
                        {
                            isCommentLine = false;
                            isNewLine = true;
                            skipWhiteSpace = true;
                            len = 0;
                            continue;
                        }
                        if (inOff >= inLimit)
                        {
                            inLimit = (inStream == null)
                                      ? reader.Read(inCharBuf, 0, inCharBuf.Length)
                                      : inStream.Read(inByteBuf, 0, inByteBuf.Length);
                            inOff = 0;
                            if (inLimit <= 0)
                            {
                                return len;
                            }
                        }
                        if (precedingBackslash)
                        {
                            len -= 1;
                            //skip the leading whitespace characters in following line
                            skipWhiteSpace = true;
                            appendedLineBegin = true;
                            precedingBackslash = false;
                            if (c == '\r')
                            {
                                skipLF = true;
                            }
                        }
                        else
                        {
                            return len;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts encoded &#92;uxxxx to unicode chars
        /// and changes special saved chars to their original forms.
        /// </summary>
        private static string LoadConvert(char[] input, int off, int len, char[] convtBuf)
        {
            if (convtBuf.Length < len)
            {
                int newLen = len * 2;
                if (newLen < 0)
                {
                    newLen = int.MaxValue;
                }
                convtBuf = new char[newLen];
            }
            char aChar;
            char[] output = convtBuf;
            int outLen = 0;
            int end = off + len;

            while (off < end)
            {
                aChar = input[off++];
                if (aChar == '\\')
                {
                    aChar = input[off++];
                    if (aChar == 'u')
                    {
                        // Read the xxxx
                        int value = 0;
                        for (int i = 0; i < 4; i++)
                        {
                            aChar = input[off++];
                            switch (aChar)
                            {
                                case '0':
                                case '1':
                                case '2':
                                case '3':
                                case '4':
                                case '5':
                                case '6':
                                case '7':
                                case '8':
                                case '9':
                                    value = (value << 4) + aChar - '0';
                                    break;
                                case 'a':
                                case 'b':
                                case 'c':
                                case 'd':
                                case 'e':
                                case 'f':
                                    value = (value << 4) + 10 + aChar - 'a';
                                    break;
                                case 'A':
                                case 'B':
                                case 'C':
                                case 'D':
                                case 'E':
                                case 'F':
                                    value = (value << 4) + 10 + aChar - 'A';
                                    break;
                                default:
                                    throw new ArgumentException(
                                                 "Malformed \\uxxxx encoding.");
                            }
                        }
                        output[outLen++] = (char)value;
                    }
                    else
                    {
                        if (aChar == 't') aChar = '\t';
                        else if (aChar == 'r') aChar = '\r';
                        else if (aChar == 'n') aChar = '\n';
                        else if (aChar == 'f') aChar = '\f';
                        output[outLen++] = aChar;
                    }
                }
                else
                {
                    output[outLen++] = aChar;
                }
            }
            return new string(output, 0, outLen);
        }

        /// <summary>
        /// Converts unicodes to encoded &#92;uxxxx and escapes
        /// special characters with a preceding slash.
        /// </summary>
        private static string SaveConvert(string theString,
                                   bool escapeSpace,
                                   bool escapeUnicode)
        {
            int len = theString.Length;
            int bufLen = len * 2;
            if (bufLen < 0)
            {
                bufLen = int.MaxValue;
            }
            StringBuffer outBuffer = new StringBuffer(bufLen);

            for (int x = 0; x < len; x++)
            {
                char aChar = theString[x];
                // Handle common case first, selecting largest block that
                // avoids the specials below
                if ((aChar > 61) && (aChar < 127))
                {
                    if (aChar == '\\')
                    {
                        outBuffer.Append('\\'); outBuffer.Append('\\');
                        continue;
                    }
                    outBuffer.Append(aChar);
                    continue;
                }
                switch (aChar)
                {
                    case ' ':
                        if (x == 0 || escapeSpace)
                            outBuffer.Append('\\');
                        outBuffer.Append(' ');
                        break;
                    case '\t':
                        outBuffer.Append('\\'); outBuffer.Append('t');
                        break;
                    case '\n':
                        outBuffer.Append('\\'); outBuffer.Append('n');
                        break;
                    case '\r':
                        outBuffer.Append('\\'); outBuffer.Append('r');
                        break;
                    case '\f':
                        outBuffer.Append('\\'); outBuffer.Append('f');
                        break;
                    case '=': // Fall through
                    case ':': // Fall through
                    case '#': // Fall through
                    case '!':
                        outBuffer.Append('\\'); outBuffer.Append(aChar);
                        break;
                    default:
                        if (((aChar < 0x0020) || (aChar > 0x007e)) & escapeUnicode)
                        {
                            outBuffer.Append('\\');
                            outBuffer.Append('u');
                            outBuffer.Append(ToHex((aChar >> 12) & 0xF));
                            outBuffer.Append(ToHex((aChar >> 8) & 0xF));
                            outBuffer.Append(ToHex((aChar >> 4) & 0xF));
                            outBuffer.Append(ToHex(aChar & 0xF));
                        }
                        else
                        {
                            outBuffer.Append(aChar);
                        }
                        break;
                }
            }
            return outBuffer.ToString();
        }

        private static void WriteComments(TextWriter bw, string comments)
        {
            bw.Write("#");
            int len = comments.Length;
            int current = 0;
            int last = 0;
            char[] uu = new char[6];
            uu[0] = '\\';
            uu[1] = 'u';
            while (current < len)
            {
                char c = comments[current];
                if (c > '\u00ff' || c == '\n' || c == '\r')
                {
                    if (last != current)
                        bw.Write(comments.Substring(last, current - last)); // end - start
                    if (c > '\u00ff')
                    {
                        uu[2] = ToHex((c >> 12) & 0xf);
                        uu[3] = ToHex((c >> 8) & 0xf);
                        uu[4] = ToHex((c >> 4) & 0xf);
                        uu[5] = ToHex(c & 0xf);
                        bw.Write(new string(uu));
                    }
                    else
                    {
                        bw.WriteLine();
                        if (c == '\r' &&
                            current != len - 1 &&
                            comments[current + 1] == '\n')
                        {
                            current++;
                        }
                        if (current == len - 1 ||
                            (comments[current + 1] != '#' &&
                            comments[current + 1] != '!'))
                            bw.Write("#");
                    }
                    last = current + 1;
                }
                current++;
            }
            if (last != current)
                bw.Write(comments.Substring(last, current - last)); // end - start
            bw.WriteLine();
        }

        private static void Store0(IDictionary<string, string> properties, TextWriter bw, string comments, bool escUnicode)
        {
            if (comments != null)
            {
                WriteComments(bw, comments);
            }
            bw.Write("#");
            bw.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            lock (properties.GetSyncRoot())
            {
                foreach (var prop in properties)
                {
                    string key = prop.Key;
                    string val = prop.Value;
                    key = SaveConvert(key, true, escUnicode);
                    /* No need to escape embedded and trailing spaces for value, hence
                     * pass false to flag.
                     */
                    val = SaveConvert(val, false, escUnicode);
                    bw.Write(key + "=" + val);
                    bw.WriteLine();
                }
            }
            bw.Flush();
        }

        /// <summary>
        /// Convert a nibble to a hex character.
        /// </summary>
        /// <param name="nibble">The nibble to convert.</param>
        private static char ToHex(int nibble)
        {
            return hexDigit[(nibble & 0xF)];
        }

        /// <summary>A table of hex digits</summary>
        private static readonly char[] hexDigit = {
            '0','1','2','3','4','5','6','7','8','9','A','B','C','D','E','F'
        };
    }
}
