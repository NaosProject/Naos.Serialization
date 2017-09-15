// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializeAndDeserializeThings.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;

    /// <summary>
    /// Interface to work with compression.
    /// </summary>
    public interface ISerializeAndDeserializeThings
    {
        /// <summary>
        /// Serializes an object into a byte array.
        /// </summary>
        /// <param name="objectToSerialize">Object to serialize.</param>
        /// <returns>Serialized object into a byte array.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "Spelling/name is correct.")]
        byte[] Serialize(object objectToSerialize);

        /// <summary>
        /// Deserializes the byte array into an object.
        /// </summary>
        /// <param name="serializedBytes">Byte array to deserialize.</param>
        /// <typeparam name="T">Type of object to deserialize.</typeparam>
        /// <returns>Deserialized bytes into object of specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "Spelling/name is correct.")]
        T Deserialize<T>(byte[] serializedBytes);

        /// <summary>
        /// Deserializes the byte array into an object.
        /// </summary>
        /// <param name="serializedBytes">Byte array to deserialize.</param>
        /// <param name="type">Type to deserialize into.</param>
        /// <returns>Deserialized bytes into object of specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "Spelling/name is correct.")]
        object Deserialize(byte[] serializedBytes, Type type);
    }
}