// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializeToString.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    /// <summary>
    /// Interface to serialize itself to a string.
    /// </summary>
    public interface ISerializeToString
    {
        /// <summary>
        /// Serialize to a string.
        /// </summary>
        /// <returns>String version of object.</returns>
        string SerializeToString();
    }
}