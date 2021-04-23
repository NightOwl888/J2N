using System;

namespace J2N.Text
{
    /// <summary>
    /// A <see cref="ParseException"/> is thrown when elements are written
    /// to a buffer but there is not enough remaining space in the buffer.
    /// </summary>
    // It is no longer good practice to use binary serialization. 
    // See: https://github.com/dotnet/corefx/issues/23584#issuecomment-325724568
#if FEATURE_SERIALIZABLE_EXCEPTIONS
    [Serializable]
#endif
    public class ParseException : Exception
    {
#if FEATURE_SERIALIZABLE_EXCEPTIONS
        // names for serialization
        private const string ErrorOffsetName = "ErrorOffset"; // Do not rename (binary serialization)
#endif

        private readonly int errorOffset;

        /// <summary>
        /// Initializes a new instance of <see cref="ParseException"/>
        /// with the specified <paramref name="message"/> and <paramref name="errorOffset"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="errorOffset">The position where the error is found while parsing.</param>
        public ParseException(string message, int errorOffset) : base(message)
        {
            this.errorOffset = errorOffset;
        }

        /// <summary>
        /// Initializes a new instance of <see cref="ParseException"/>
        /// with the specified <paramref name="message"/>, <paramref name="errorOffset"/> and <paramref name="innerException"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="errorOffset">The position where the error is found while parsing.</param>
        /// <param name="innerException">The original cause of this parse exception.</param>
        public ParseException(string message, int errorOffset, Exception innerException) : base(message, innerException)
        {
            this.errorOffset = errorOffset;
        }

        // J2N: For testing purposes
        internal ParseException()
        {
        }

        // J2N: For testing purposes
        internal ParseException(string message) : base(message)
        {
        }

        // J2N: For testing purposes
        internal ParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

#if FEATURE_SERIALIZABLE_EXCEPTIONS
        /// <summary>
        /// Initializes a new instance of this class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        private ParseException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
            errorOffset = info.GetInt32(ErrorOffsetName);
        }

        /// <summary>
        /// Sets the <see cref="System.Runtime.Serialization.SerializationInfo"/> with information about the exception.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        public override void GetObjectData(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
        {
            if (info is null)
                throw new ArgumentNullException(nameof(info));

            info.AddValue(ErrorOffsetName, errorOffset);

            base.GetObjectData(info, context);
        }
#endif

        /// <summary>
        /// Returns the position where the error was found.
        /// </summary>
        public int ErrorOffset => errorOffset;
    }
}
