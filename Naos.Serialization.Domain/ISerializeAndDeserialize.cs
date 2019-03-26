// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ISerializeAndDeserialize.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;

    /// <summary>
    /// Interface to serialize and deserialize to and from a byte array or string.
    /// </summary>
    public interface ISerializeAndDeserialize : ISerialize, IDeserialize, IStringSerializeAndDeserialize, IBinarySerializeAndDeserialize
    {
    }

    /// <summary>
    /// Interface to serialize to and from a byte array or string.
    /// </summary>
    public interface ISerialize : IStringSerialize, IBinarySerialize, IHaveKind
    {
    }

    /// <summary>
    /// Interface to deserialize to and from a byte array or string.
    /// </summary>
    public interface IDeserialize : IStringDeserialize, IBinaryDeserialize, IHaveKind
    {
    }

    /// <summary>
    /// Interface to expose the <see cref="Type" /> of configuration.
    /// </summary>
    public interface IHaveConfigurationType
    {
        /// <summary>
        /// Gets the <see cref="Type" /> of configuration.
        /// </summary>
        Type ConfigurationType { get; }
    }

    /// <summary>
    /// Interface to expose the <see cref="SerializationKind" /> of serializer.
    /// </summary>
    public interface IHaveKind
    {
        /// <summary>
        /// Gets the <see cref="SerializationKind" /> of the serializer.
        /// </summary>
        SerializationKind Kind { get; }
    }
}