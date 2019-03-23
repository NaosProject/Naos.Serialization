// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationDirection.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    /// <summary>
    /// Direction of serialization.
    /// </summary>
    public enum SerializationDirection
    {
        /// <summary>
        /// Unknown direction.
        /// </summary>
        Unknown,

        /// <summary>
        /// Serializing object.
        /// </summary>
        Serialize,

        /// <summary>
        /// Deserializing object.
        /// </summary>
        Deserialize,
    }
}
