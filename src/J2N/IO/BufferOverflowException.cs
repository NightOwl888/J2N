using System;
#nullable enable

namespace J2N.IO
{
    /// <summary>
    /// A <see cref="BufferOverflowException"/> is thrown when elements are written
    /// to a buffer but there is not enough remaining space in the buffer.
    /// </summary>
    // It is no longer good practice to use binary serialization. 
    // See: https://github.com/dotnet/corefx/issues/23584#issuecomment-325724568
#if FEATURE_SERIALIZABLE_EXCEPTIONS
    [Serializable]
#endif
    public sealed class BufferOverflowException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="BufferOverflowException"/>.
        /// </summary>
        public BufferOverflowException()
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="BufferOverflowException"/>
        /// with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public BufferOverflowException(string message) : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="BufferOverflowException"/>
        /// with a specified error message and a reference to the inner exception
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (<c>Nothing</c> in Visual Basic) if no inner exception is specified.</param>
        public BufferOverflowException(string message, Exception innerException) : base(message, innerException)
        { }

#if FEATURE_SERIALIZABLE_EXCEPTIONS
        /// <summary>
        /// Initializes a new instance of this class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        private BufferOverflowException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
#endif
    }
}
