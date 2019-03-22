// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationKind.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;

    /// <summary>
    /// Kind of serialization to use.
    /// </summary>
    public enum SerializationKind
    {
        /// <summary>
        /// Default option.
        /// </summary>
        Default,

        /// <summary>
        /// Compact option.
        /// </summary>
        Compact,

        /// <summary>
        /// Minimal option.
        /// </summary>
        Minimal,

        /// <summary>
        /// Custom option.
        /// </summary>
        Custom,

        /// <summary>
        /// Invalid option.
        /// </summary>
        Invalid,
    }

    /// <summary>
    /// Represenation of serialization.
    /// </summary>
    public enum SerializationRepresentation
    {
        /// <summary>
        /// Invalid default option.
        /// </summary>
        Invalid,

        /// <summary>
        /// String represenation.
        /// </summary>
        String,

        /// <summary>
        /// Binary represenation.
        /// </summary>
        Binary,
    }

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
        /// JSON format.
        /// </summary>
        Json,

        /// <summary>
        /// BSON format.
        /// </summary>
        Bson,

        /// <summary>
        /// Property bag (Dictionary{string, string} format.
        /// </summary>
        PropertyBag,
    }

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

    /// <summary>
    /// Directions of serialization.
    /// </summary>
    [Flags]
    public enum SerializationDirections
    {
        /// <summary>
        /// No direction.
        /// </summary>
        None = 0,

        /// <summary>
        /// Serializing direction.
        /// </summary>
        Serialize = 1,

        /// <summary>
        /// Deserializing direction.
        /// </summary>
        Deserialize = 2,

        /// <summary>
        /// Both directions.
        /// </summary>
        Both = Serialize | Deserialize,
    }
}