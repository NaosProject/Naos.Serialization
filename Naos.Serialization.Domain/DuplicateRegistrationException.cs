// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DuplicateRegistrationException.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Exception for trying to register a type more than once.
    /// </summary>
    [Serializable]
    public class DuplicateRegistrationException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateRegistrationException"/> class.
        /// </summary>
        public DuplicateRegistrationException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateRegistrationException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        public DuplicateRegistrationException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateRegistrationException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        /// <param name="typesAttempted">The types that were attempting to be registered that are already handled.</param>
        public DuplicateRegistrationException(string message, ICollection<Type> typesAttempted)
            : base(message)
        {
            new { typesAttempted }.Must().NotBeNull().And().NotBeEmptyEnumerable();

            this.TypesAttempted = typesAttempted.ToList();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateRegistrationException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public DuplicateRegistrationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DuplicateRegistrationException"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Reading context.</param>
        protected DuplicateRegistrationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the type that an operation was attempted on.
        /// </summary>
        public IReadOnlyCollection<Type> TypesAttempted { get; private set; }

        /// <inheritdoc />
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue(nameof(this.TypesAttempted), this.TypesAttempted);
        }
    }
}