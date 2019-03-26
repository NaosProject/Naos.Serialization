// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationKind.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;

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

        /// <summary>
        /// Wrapper to honor protocol using provided <see cref="Func{TResult}" />'s.
        /// </summary>
        LambdaBacked,
    }
}