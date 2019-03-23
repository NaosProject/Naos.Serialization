// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationFormat.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    /// <summary>
    /// Format of serialization.
    /// </summary>
    public enum SerializationFormat
    {
        /// <summary>
        /// Invalid default option.
        /// </summary>
        Invalid,

        /// <summary>
        /// String format.
        /// </summary>
        String,

        /// <summary>
        /// Binary format.
        /// </summary>
        Binary,
    }
}