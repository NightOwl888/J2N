﻿#region Copyright 2010 by Apache Harmony, Licensed under the Apache License, Version 2.0
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
using System.Globalization;
using System.IO;
using System.Text;


namespace J2N.IO
{
    /// <summary>
    /// Parses a stream into a set of defined tokens, one at a time. The different
    /// types of tokens that can be found are numbers, identifiers, quoted strings,
    /// and different comment styles. The class can be used for limited processing
    /// of source code of programming languages like Java, although it is nowhere
    /// near a full parser.
    /// </summary>
    public class StreamTokenizer
    {
        /// <summary>
        /// Contains a number if the current token is a number 
        /// (<see cref="TokenType"/> == <see cref="TokenType_Number"/>).
        /// </summary>
        public double NumberValue { get; set; }

        /// <summary>
        /// Contains a string if the current token is a word 
        /// (<see cref="TokenType"/> == <see cref="TokenType_Word"/>).
        /// </summary>
        public string? StringValue { get; set; }

        /// <summary>
        /// The constant representing the end of the stream.
        /// </summary>
        public const int TokenType_EndOfStream = -1;

        /// <summary>
        /// The constant representing the end of the line.
        /// </summary>
        public const int TokenType_EndOfLine = '\n';

        /// <summary>
        /// The constant representing a number token.
        /// </summary>
        public const int TokenType_Number = -2;

        /// <summary>
        /// The constant representing a word token.
        /// </summary>
        public const int TokenType_Word = -3;

        /// <summary>
        /// Internal representation of unknown state.
        /// </summary>
        private const int TokenType_Unknown = -4;

        /// <summary>
        /// After calling <see cref="NextToken()"/>, <see cref="TokenType"/> contains the type of
        /// token that has been read. When a single character is read, its value
        /// converted to an integer is stored in <see cref="TokenType"/>. For a quoted string,
        /// the value is the quoted character. Otherwise, its value is one of the
        /// following:
        /// <list type="bullet">
        ///     <item><description><see cref="TokenType_Word"/> - the token is a word.</description></item>
        ///     <item><description><see cref="TokenType_Number"/> - the token is a number.</description></item>
        ///     <item><description><see cref="TokenType_EndOfLine"/> - the end of line has been reached. Depends on
        ///     whether <see cref="EndOfLineIsSignificant"/> is <c>true</c>.</description></item>
        ///     <item><description><see cref="TokenType_EndOfStream"/> - the end of the stream has been reached.</description></item>
        /// </list>
        /// </summary>
        public int TokenType { get; private set; } = TokenType_Unknown;

        /// <summary>
        /// Internal character meanings, 0 implies TOKEN_ORDINARY
        /// </summary>
        private readonly byte[] tokenTypes = new byte[256];

        private const byte Token_Comment = 1;
        private const byte Token_Quote = 2;
        private const byte Token_White = 4;
        private const byte Token_Word = 8;
        private const byte Token_Digit = 16;

        private bool pushBackToken;
        private bool lastCr;

        /// <summary>One of these will have the stream</summary>
        private readonly Stream? inStream;
        private readonly TextReader? inReader;
        private int peekChar = -2;

        /// <summary>
        /// Private constructor to initialize the default values according to the
        /// specification.
        /// </summary>
        private StreamTokenizer()
        {
            //
            // Initialize the default state per specification. All byte values 'A'
            // through 'Z', 'a' through 'z', and '\u00A0' through '\u00FF' are
            // considered to be alphabetic.
            //
            WordChars('A', 'Z');
            WordChars('a', 'z');
            WordChars(160, 255);
            //
            // All byte values '\u0000' through '\u0020' are considered to be white
            // space.
            //
            WhitespaceChars(0, 32);
            //
            // '/' is a comment character. Single quote '\'' and double quote '"'
            // are string quote characters.
            //
            CommentChar('/');
            QuoteChar('"');
            QuoteChar('\'');
            //
            // Numbers are parsed.
            //
            ParseNumbers();
            //
            // Ends of lines are treated as white space, not as separate tokens.
            // C-style and C++-style comments are not recognized. These are the
            // defaults and are not needed in constructor.
            //
        }

        /// <summary>
        /// Constructs a new <see cref="StreamTokenizer"/> with <paramref name="input"/> as source input
        /// stream. This constructor is deprecated; instead, the constructor that
        /// takes a <see cref="TextReader"/> as an arugment should be used.
        /// </summary>
        /// <param name="input">the source stream from which to parse tokens.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="input"/> is <c>null</c>.</exception>
        [Obsolete("Use StreamTokenizer(TextReader)")]
        public StreamTokenizer(Stream input)
            : this() // Calls private constructor
        {
            inStream = input ?? throw new ArgumentNullException(nameof(input));
        }

        /// <summary>
        /// Constructs a new <see cref="StreamTokenizer"/> with <paramref name="reader"/> as source reader.
        /// The tokenizer's initial state is as follows:
        /// <list type="bullet">
        ///     <item><description>All byte values 'A' through 'Z', 'a' through 'z', and '&#92;u00A0' through '&#92;u00FF' are considered to be alphabetic.</description></item>
        ///     <item><description>All byte values '&#92;u0000' through '&#92;u0020' are considered to be white space. '/' is a comment character.</description></item>
        ///     <item><description>Single quote '\'' and double quote '"' are string quote characters.</description></item>
        ///     <item><description>Numbers are parsed.</description></item>
        ///     <item><description>End of lines are considered to be white space rather than separate tokens.</description></item>
        ///     <item><description>C-style and C++-style comments are not recognized.</description></item>
        /// </list>
        /// </summary>
        /// <param name="reader">The source text reader from which to parse tokens.</param>
        public StreamTokenizer(TextReader reader)
            : this() // Calls private constructor
        {
            inReader = reader ?? throw new ArgumentNullException(nameof(reader));
        }

        /// <summary>
        /// Specifies that the character <paramref name="ch"/> shall be treated as a comment
        /// character.
        /// </summary>
        /// <param name="ch">The character to be considered a comment character.</param>
        public virtual void CommentChar(int ch)
        {
            if (0 <= ch && ch < tokenTypes.Length)
            {
                tokenTypes[ch] = Token_Comment;
            }
        }

        /// <summary>
        /// Specifies whether the end of a line is significant and should be returned
        /// as <see cref="TokenType_EndOfStream"/> in <see cref="TokenType"/> by this tokenizer.
        /// <c>true</c> if EOL is significant, <c>false</c> otherwise.
        /// </summary>
        public virtual bool EndOfLineIsSignificant { get; set; }

        /// <summary>
        /// Gets the current line number.
        /// </summary>
        public virtual int LineNumber { get; private set; } = 1;

        /// <summary>
        /// Specifies whether word tokens should be converted to lower case when they
        /// are stored in <see cref="StringValue"/>. <c>true</c> if <see cref="StringValue"/>
        /// should be converted to lower case, <c>false</c> otherwise.
        /// </summary>
        public virtual bool LowerCaseMode { get; set; }

        /// <summary>
        /// Parses the next token from this tokenizer's source stream or reader. The
        /// type of the token is stored in the <see cref="TokenType"/> field, additional
        /// information may be stored in the <see cref="NumberValue"/> or <see cref="StringValue"/> fields.
        /// </summary>
        /// <returns>The value of <see cref="TokenType"/>.</returns>
        /// <exception cref="IOException">If an I/O error occurs while parsing the next token.</exception>
        public virtual int NextToken()
        {
            if (pushBackToken)
            {
                pushBackToken = false;
                if (TokenType != TokenType_Unknown)
                {
                    return TokenType;
                }
            }
            StringValue = null; // Always reset sval to null
            int currentChar = peekChar == -2 ? Read() : peekChar;

            if (lastCr && currentChar == '\n')
            {
                lastCr = false;
                currentChar = Read();
            }
            if (currentChar == -1)
            {
                return (TokenType = TokenType_EndOfStream);
            }

            byte currentType = currentChar > 255 ? Token_Word
                    : tokenTypes[currentChar];
            while ((currentType & Token_White) != 0)
            {
                //
                // Skip over white space until we hit a new line or a real token
                //
                if (currentChar == '\r')
                {
                    LineNumber++;
                    if (EndOfLineIsSignificant)
                    {
                        lastCr = true;
                        peekChar = -2;
                        return (TokenType = TokenType_EndOfLine);
                    }
                    if ((currentChar = Read()) == '\n')
                    {
                        currentChar = Read();
                    }
                }
                else if (currentChar == '\n')
                {
                    LineNumber++;
                    if (EndOfLineIsSignificant)
                    {
                        peekChar = -2;
                        return (TokenType = TokenType_EndOfLine);
                    }
                    currentChar = Read();
                }
                else
                {
                    // Advance over this white space character and try again.
                    currentChar = Read();
                }
                if (currentChar == -1)
                {
                    return (TokenType = TokenType_EndOfStream);
                }
                currentType = currentChar > 255 ? Token_Word
                        : tokenTypes[currentChar];
            }

            //
            // Check for digits before checking for words since digits can be
            // contained within words.
            //
            if ((currentType & Token_Digit) != 0)
            {
                StringBuilder digits = new StringBuilder(20);
                bool haveDecimal = false, checkJustNegative = currentChar == '-';
                while (true)
                {
                    if (currentChar == '.')
                    {
                        haveDecimal = true;
                    }
                    digits.Append((char)currentChar);
                    currentChar = Read();
                    if ((currentChar < '0' || currentChar > '9')
                            && (haveDecimal || currentChar != '.'))
                    {
                        break;
                    }
                }
                peekChar = currentChar;
                if (checkJustNegative && digits.Length == 1)
                {
                    // Didn't get any other digits other than '-'
                    return (TokenType = '-');
                }
                if (double.TryParse(digits.ToString(), NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
                    NumberValue = value;
                else
                    NumberValue = 0; // Unsure what to do, will write test.

                return (TokenType = TokenType_Number);
            }
            // Check for words
            if ((currentType & Token_Word) != 0)
            {
                StringBuilder word = new StringBuilder(20);
                while (true)
                {
                    word.Append((char)currentChar);
                    currentChar = Read();
                    if (currentChar == -1
                            || (currentChar < 256 && (tokenTypes[currentChar] & (Token_Word | Token_Digit)) == 0))
                    {
                        break;
                    }
                }
                peekChar = currentChar;
                StringValue = LowerCaseMode ? word.ToString().ToLowerInvariant() : word
                       .ToString();
                return (TokenType = TokenType_Word);
            }
            // Check for quoted character
            if (currentType == Token_Quote)
            {
                int matchQuote = currentChar;
                StringBuilder quoteString = new StringBuilder();
                int peekOne = Read();
                while (peekOne >= 0 && peekOne != matchQuote && peekOne != '\r'
                        && peekOne != '\n')
                {
                    bool readPeek = true;
                    if (peekOne == '\\')
                    {
                        int c1 = Read();
                        // Check for quoted octal IE: \377
                        if (c1 <= '7' && c1 >= '0')
                        {
                            int digitValue = c1 - '0';
                            c1 = Read();
                            if (c1 > '7' || c1 < '0')
                            {
                                readPeek = false;
                            }
                            else
                            {
                                digitValue = digitValue * 8 + (c1 - '0');
                                c1 = Read();
                                // limit the digit value to a byte
                                if (digitValue > 037 || c1 > '7' || c1 < '0')
                                {
                                    readPeek = false;
                                }
                                else
                                {
                                    digitValue = digitValue * 8 + (c1 - '0');
                                }
                            }
                            if (!readPeek)
                            {
                                // We've consumed one to many
                                quoteString.Append((char)digitValue);
                                peekOne = c1;
                            }
                            else
                            {
                                peekOne = digitValue;
                            }
                        }
                        else
                        {
                            switch (c1)
                            {
                                case 'a':
                                    peekOne = 0x7;
                                    break;
                                case 'b':
                                    peekOne = 0x8;
                                    break;
                                case 'f':
                                    peekOne = 0xc;
                                    break;
                                case 'n':
                                    peekOne = 0xA;
                                    break;
                                case 'r':
                                    peekOne = 0xD;
                                    break;
                                case 't':
                                    peekOne = 0x9;
                                    break;
                                case 'v':
                                    peekOne = 0xB;
                                    break;
                                default:
                                    peekOne = c1;
                                    break;
                            }
                        }
                    }
                    if (readPeek)
                    {
                        quoteString.Append((char)peekOne);
                        peekOne = Read();
                    }
                }
                if (peekOne == matchQuote)
                {
                    peekOne = Read();
                }
                peekChar = peekOne;
                TokenType = matchQuote;
                StringValue = quoteString.ToString();
                return TokenType;
            }
            // Do comments, both "//" and "/*stuff*/"
            if (currentChar == '/' && (SlashSlashComments || SlashStarComments))
            {
                if ((currentChar = Read()) == '*' && SlashStarComments)
                {
                    int peekOne = Read();
                    while (true)
                    {
                        currentChar = peekOne;
                        peekOne = Read();
                        if (currentChar == -1)
                        {
                            peekChar = -1;
                            return (TokenType = TokenType_EndOfStream);
                        }
                        if (currentChar == '\r')
                        {
                            if (peekOne == '\n')
                            {
                                peekOne = Read();
                            }
                            LineNumber++;
                        }
                        else if (currentChar == '\n')
                        {
                            LineNumber++;
                        }
                        else if (currentChar == '*' && peekOne == '/')
                        {
                            peekChar = Read();
                            return NextToken();
                        }
                    }
                }
                else if (currentChar == '/' && SlashSlashComments)
                {
                    // Skip to EOF or new line then return the next token
                    while ((currentChar = Read()) >= 0 && currentChar != '\r'
                            && currentChar != '\n')
                    {
                        // Intentionally empty
                    }
                    peekChar = currentChar;
                    return NextToken();
                }
                else if (currentType != Token_Comment)
                {
                    // Was just a slash by itself
                    peekChar = currentChar;
                    return (TokenType = '/');
                }
            }
            // Check for comment character
            if (currentType == Token_Comment)
            {
                // Skip to EOF or new line then return the next token
                while ((currentChar = Read()) >= 0 && currentChar != '\r'
                        && currentChar != '\n')
                {
                    // Intentionally empty
                }
                peekChar = currentChar;
                return NextToken();
            }

            peekChar = Read();
            return (TokenType = currentChar);
        }

        /// <summary>
        /// Specifies that the character <paramref name="ch"/> shall be treated as an ordinary
        /// character by this tokenizer. That is, it has no special meaning as a
        /// comment character, word component, white space, string delimiter or
        /// number.
        /// </summary>
        /// <param name="ch">The character to be considered an ordinary character.</param>
        public virtual void OrdinaryChar(int ch)
        {
            if (0 <= ch && ch < tokenTypes.Length)
            {
                tokenTypes[ch] = 0;
            }
        }

        /// <summary>
        /// Specifies that the characters in the range from <paramref name="low"/> to <paramref name="hi"/>
        /// shall be treated as an ordinary character by this tokenizer. That is,
        /// they have no special meaning as a comment character, word component,
        /// white space, string delimiter or number.
        /// </summary>
        /// <param name="low">The first character in the range of ordinary characters.</param>
        /// <param name="hi">The last character in the range of ordinary characters.</param>
        public virtual void OrdinaryChars(int low, int hi)
        {
            if (low < 0)
            {
                low = 0;
            }
            if (hi > tokenTypes.Length)
            {
                hi = tokenTypes.Length - 1;
            }
            for (int i = low; i <= hi; i++)
            {
                tokenTypes[i] = 0;
            }
        }

        /// <summary>
        /// Specifies that this tokenizer shall parse numbers.
        /// </summary>
        public virtual void ParseNumbers()
        {
            for (int i = '0'; i <= '9'; i++)
            {
                tokenTypes[i] |= Token_Digit;
            }
            tokenTypes['.'] |= Token_Digit;
            tokenTypes['-'] |= Token_Digit;
        }

        /// <summary>
        /// Indicates that the current token should be pushed back and returned again
        /// the next time <see cref="NextToken()"/> is called.
        /// </summary>
        public virtual void PushBack()
        {
            pushBackToken = true;
        }

        /// <summary>
        /// Specifies that the character <paramref name="ch"/> shall be treated as a quote
        /// character.
        /// </summary>
        /// <param name="ch">The character to be considered a quote character.</param>
        public virtual void QuoteChar(int ch)
        {
            if (0 <= ch && ch < tokenTypes.Length)
            {
                tokenTypes[ch] = Token_Quote;
            }
        }

        private int Read()
        {
            // Call the read for the appropriate stream
            if (inStream is null)
            {
                return inReader!.Read();
            }
            return inStream.ReadByte();
        }

        /// <summary>
        /// Specifies that all characters shall be treated as ordinary characters.
        /// </summary>
        public virtual void ResetSyntax()
        {
            for (int i = 0; i < 256; i++)
            {
                tokenTypes[i] = 0;
            }
        }

        /// <summary>
        /// Specifies whether "slash-slash" (C++-style) comments shall be recognized.
        /// This kind of comment ends at the end of the line.
        /// <c>true</c> if <c>//</c> should be recognized as the start
        /// of a comment, <c>false</c> otherwise.
        /// </summary>
        public virtual bool SlashSlashComments { get; set; }

        /// <summary>
        /// Specifies whether "slash-star" (C-style) comments shall be recognized.
        /// Slash-star comments cannot be nested and end when a star-slash
        /// combination is found.
        /// <c>true</c> if <c>/*</c> should be recognized as the start
        /// of a comment, <c>false</c> otherwise.
        /// </summary>
        public virtual bool SlashStarComments { get; set; }

        /// <summary>
        /// Returns the state of this tokenizer in a readable format.
        /// </summary>
        /// <returns>The current state of this tokenizer.</returns>
        public override string ToString()
        {
            // Values determined through experimentation
            StringBuilder result = new StringBuilder();
            result.Append("Token["); //$NON-NLS-1$
            switch (TokenType)
            {
                case TokenType_EndOfStream:
                    result.Append("EOF"); //$NON-NLS-1$
                    break;
                case TokenType_EndOfLine:
                    result.Append("EOL"); //$NON-NLS-1$
                    break;
                case TokenType_Number:
                    result.Append("n="); //$NON-NLS-1$
                    result.Append(NumberValue);
                    break;
                case TokenType_Word:
                    result.Append(StringValue);
                    break;
                default:
                    if (TokenType == TokenType_Unknown || tokenTypes[TokenType] == Token_Quote)
                    {
                        result.Append(StringValue);
                    }
                    else
                    {
                        result.Append('\'');
                        result.Append((char)TokenType);
                        result.Append('\'');
                    }
                    break;
            }
            result.Append("], line "); //$NON-NLS-1$
            result.Append(LineNumber);
            return result.ToString();
        }

        /// <summary>
        /// Specifies that the characters in the range from <paramref name="low"/> to <paramref name="hi"/>
        /// shall be treated as whitespace characters by this tokenizer.
        /// </summary>
        /// <param name="low">The first character in the range of whitespace characters.</param>
        /// <param name="hi">The last character in the range of whitespace characters.</param>
        public virtual void WhitespaceChars(int low, int hi)
        {
            if (low < 0)
            {
                low = 0;
            }
            if (hi > tokenTypes.Length)
            {
                hi = tokenTypes.Length - 1;
            }
            for (int i = low; i <= hi; i++)
            {
                tokenTypes[i] = Token_White;
            }
        }

        /// <summary>
        /// Specifies that the characters in the range from <paramref name="low"/> to <paramref name="hi"/>
        /// shall be treated as word characters by this tokenizer. A word consists of
        /// a word character followed by zero or more word or number characters.
        /// </summary>
        /// <param name="low">The first character in the range of word characters.</param>
        /// <param name="hi">The last character in the range of word characters.</param>
        public virtual void WordChars(int low, int hi)
        {
            if (low < 0)
            {
                low = 0;
            }
            if (hi > tokenTypes.Length)
            {
                hi = tokenTypes.Length - 1;
            }
            for (int i = low; i <= hi; i++)
            {
                tokenTypes[i] |= Token_Word;
            }
        }
    }
}
