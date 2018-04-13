// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializeToString.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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