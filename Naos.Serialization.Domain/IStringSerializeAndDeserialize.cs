// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IStringSerializeAndDeserialize.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;

    /// <summary>
    /// Interface to serialize and deserialize to and from a string.
    /// </summary>
    public interface IStringSerializeAndDeserialize : IStringSerialize, IStringDeserialize
    {
    }

    /// <summary>
    /// Interface to serialize to a string.
    /// </summary>
    public interface IStringSerialize : IHaveSerializationKind, IHaveConfigurationType
    {
        /// <summary>
        /// Serializes an object into a string.
        /// </summary>
        /// <param name="objectToSerialize">Object to serialize.</param>
        /// <returns>Serialized object into a string.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "Spelling/name is correct.")]
        string SerializeToString(object objectToSerialize);
    }

    /// <summary>
    /// Interface to deserialize from a string.
    /// </summary>
    public interface IStringDeserialize : IHaveSerializationKind, IHaveConfigurationType
    {
        /// <summary>
        /// Deserializes the string into an object.
        /// </summary>
        /// <param name="serializedString">String to deserialize.</param>
        /// <typeparam name="T">Type of object to deserialize.</typeparam>
        /// <returns>Deserialized string into object of specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "Spelling/name is correct.")]
        T Deserialize<T>(string serializedString);

        /// <summary>
        /// Deserializes the string into an object.
        /// </summary>
        /// <param name="serializedString">String to deserialize.</param>
        /// <param name="type">Type to deserialize into.</param>
        /// <returns>Deserialized string into object of specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "Spelling/name is correct.")]
        object Deserialize(string serializedString, Type type);
    }
}