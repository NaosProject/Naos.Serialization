// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnregisteredTypeAttemptException.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Runtime.Serialization;
    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Exception for trying to perform an operation on a type that was never registered and a <see cref="UnregisteredTypeEncounteredStrategy" /> value of <see cref="UnregisteredTypeEncounteredStrategy.Throw" />.
    /// </summary>
    [Serializable]
    public class UnregisteredTypeAttemptException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnregisteredTypeAttemptException"/> class.
        /// </summary>
        public UnregisteredTypeAttemptException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnregisteredTypeAttemptException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        public UnregisteredTypeAttemptException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnregisteredTypeAttemptException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        /// <param name="typeAttempted">The type that an operation was attempted on.</param>
        public UnregisteredTypeAttemptException(string message, Type typeAttempted)
            : base(message)
        {
            new { typeAttempted }.Must().NotBeNull();

            this.TypeAttempted = typeAttempted;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnregisteredTypeAttemptException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public UnregisteredTypeAttemptException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UnregisteredTypeAttemptException"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Reading context.</param>
        protected UnregisteredTypeAttemptException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the type that an operation was attempted on.
        /// </summary>
        public Type TypeAttempted { get; private set; }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(this.TypeAttempted), this.TypeAttempted);
        }
    }
}