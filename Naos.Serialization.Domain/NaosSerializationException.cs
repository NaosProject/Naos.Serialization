// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosSerializationException.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception for issues in <see cref="Naos.Serialization"/>.
    /// </summary>
    [Serializable]
    public class NaosSerializationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NaosSerializationException"/> class.
        /// </summary>
        public NaosSerializationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NaosSerializationException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        public NaosSerializationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NaosSerializationException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public NaosSerializationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NaosSerializationException"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Reading context.</param>
        protected NaosSerializationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}