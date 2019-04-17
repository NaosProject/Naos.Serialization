// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegistrationDetails.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;

    /// <summary>
    /// Exception for issues in <see cref="Naos.Serialization"/>.
    /// </summary>
    public class RegistrationDetails
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegistrationDetails"/> class.
        /// </summary>
        /// <param name="registeringType">The type that performed the registration.</param>
        public RegistrationDetails(Type registeringType)
        {
            this.RegisteringType = registeringType;
        }

        /// <summary>
        /// Gets the type that performed the registration.
        /// </summary>
        public Type RegisteringType { get; private set; }
    }
}