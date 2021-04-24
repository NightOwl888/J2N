using System;


namespace J2N.IO
{
    /// <summary>
    /// A <see cref="ReadOnlyBufferException"/> is thrown when some write operation is
    /// called on a read-only buffer.
    /// </summary>
    // It is no longer good practice to use binary serialization. 
    // See: https://github.com/dotnet/corefx/issues/23584#issuecomment-325724568
#if FEATURE_SERIALIZABLE_EXCEPTIONS
    [Serializable]
#endif
    public sealed class ReadOnlyBufferException : NotSupportedException
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ReadOnlyBufferException"/>.
        /// </summary>
        public ReadOnlyBufferException()
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="ReadOnlyBufferException"/>
        /// with the specified <paramref name="message"/>.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        public ReadOnlyBufferException(string message) : base(message)
        { }

        /// <summary>
        /// Initializes a new instance of <see cref="ReadOnlyBufferException"/>
        /// with a specified error message and a reference to the inner exception
        /// that is the cause of this exception.
        /// </summary>
        /// <param name="message">The message that describes the error.</param>
        /// <param name="innerException">The exception that is the cause of the current exception, or a null reference
        /// (<c>Nothing</c> in Visual Basic) if no inner exception is specified.</param>
        public ReadOnlyBufferException(string message, Exception innerException) : base(message, innerException)
        { }

#if FEATURE_SERIALIZABLE_EXCEPTIONS
        /// <summary>
        /// Initializes a new instance of this class with serialized data.
        /// </summary>
        /// <param name="info">The <see cref="System.Runtime.Serialization.SerializationInfo"/> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="System.Runtime.Serialization.StreamingContext"/> that contains contextual information about the source or destination.</param>
        private ReadOnlyBufferException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        { }
#endif
    }
}
