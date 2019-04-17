// --------------------------------------------------------------------------------------------------------------------
// <copyright file="UnregisteredTypeEncounteredStrategy.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    /// <summary>
    /// Format of serialization.
    /// </summary>
    public enum UnregisteredTypeEncounteredStrategy
    {
        /// <summary>
        /// Default will use <see cref="Throw" /> if a <see cref="SerializationConfigurationBase" /> is provided, otherwise <see cref="Attempt" /> will be used.
        /// </summary>
        Default,

        /// <summary>
        /// Attempt an operation on an object without prior registration.
        /// </summary>
        Attempt,

        /// <summary>
        /// Throw if an operation is attempted on an object without prior registration.
        /// </summary>
        Throw,
    }
}