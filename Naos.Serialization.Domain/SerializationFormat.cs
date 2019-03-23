// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationFormat.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    /// <summary>
    /// Representation of serialization.
    /// </summary>
    public enum SerializationFormat
    {
        /// <summary>
        /// Invalid default option.
        /// </summary>
        Invalid,

        /// <summary>
        /// String representation.
        /// </summary>
        String,

        /// <summary>
        /// Binary representation.
        /// </summary>
        Binary,
    }
}