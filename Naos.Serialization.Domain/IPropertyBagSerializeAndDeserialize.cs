// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IPropertyBagSerializeAndDeserialize.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Interface to serialize and deserialize to and from a string.
    /// </summary>
    public interface IPropertyBagSerializeAndDeserialize : IPropertyBagSerialize, IPropertyBagDeserialize
    {
    }

    /// <summary>
    /// Interface to serialize to a string.
    /// </summary>
    public interface IPropertyBagSerialize : IHaveSerializationKind, IHaveConfigurationType
    {
        /// <summary>
        /// Serializes an object into a string.
        /// </summary>
        /// <param name="objectToSerialize">Object to serialize.</param>
        /// <returns>Serialized object into a string.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "object", Justification = "Name/spelling is correct.")]
        IReadOnlyDictionary<string, string> SerializeToPropertyBag(object objectToSerialize);
    }

    /// <summary>
    /// Interface to deserialize from a string.
    /// </summary>
    public interface IPropertyBagDeserialize : IHaveSerializationKind, IHaveConfigurationType
    {
        /// <summary>
        /// Deserializes the Property Bag into an object.
        /// </summary>
        /// <param name="serializedPropertyBag">Property Bag to deserialize.</param>
        /// <typeparam name="T">Type of object to deserialize.</typeparam>
        /// <returns>Deserialized Property Bag into object of specified type.</returns>
        T Deserialize<T>(IReadOnlyDictionary<string, string> serializedPropertyBag);

        /// <summary>
        /// Deserializes the Property Bag into an object.
        /// </summary>
        /// <param name="serializedPropertyBag">Property Bag to deserialize.</param>
        /// <param name="type">Type to deserialize into.</param>
        /// <returns>Deserialized Property Bag into object of specified type.</returns>
        object Deserialize(IReadOnlyDictionary<string, string> serializedPropertyBag, Type type);
    }
}