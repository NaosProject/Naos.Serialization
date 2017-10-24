﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationKind.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
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
    }
}