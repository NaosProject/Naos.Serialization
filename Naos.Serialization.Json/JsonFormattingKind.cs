// --------------------------------------------------------------------------------------------------------------------
// <copyright file="JsonFormattingKind.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    /// <summary>
    /// Kind of serialization to use.
    /// </summary>
    public enum JsonFormattingKind
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
        /// Invalid option.
        /// </summary>
        Invalid,
    }
}