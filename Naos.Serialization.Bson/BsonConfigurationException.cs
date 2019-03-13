// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BsonConfigurationException.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception for issues configuring BSON.
    /// </summary>
    [Serializable]
    public class BsonConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BsonConfigurationException"/> class.
        /// </summary>
        public BsonConfigurationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonConfigurationException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        public BsonConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonConfigurationException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public BsonConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BsonConfigurationException"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Reading context.</param>
        protected BsonConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
