// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBagConfigurationException.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.PropertyBag
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Exception for issues configuring Property Bag.
    /// </summary>
    [Serializable]
    public class PropertyBagConfigurationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBagConfigurationException"/> class.
        /// </summary>
        public PropertyBagConfigurationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBagConfigurationException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        public PropertyBagConfigurationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBagConfigurationException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public PropertyBagConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyBagConfigurationException"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Reading context.</param>
        protected PropertyBagConfigurationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
