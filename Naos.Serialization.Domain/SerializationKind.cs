﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationKind.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    /// <summary>
    /// Format of serialization.
    /// </summary>
    public enum SerializationKind
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
}