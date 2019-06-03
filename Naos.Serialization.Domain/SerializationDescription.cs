// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationDescription.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Naos.Compression.Domain;
    using OBeautifulCode.Collection.Recipes;
    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.Type;
    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Model object to desribe a serializer so you can persist and share the definition and rehydrate the serializer later.
    /// </summary>
    public class SerializationDescription : IEquatable<SerializationDescription>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationDescription"/> class.
        /// </summary>
        /// <param name="serializationKind">The <see cref="SerializationKind" /> to serialize into.</param>
        /// <param name="serializationFormat">The <see cref="SerializationFormat" /> to serialize into.</param>
        /// <param name="compressionKind">Optional <see cref="CompressionKind" /> to use; DEFAULT is None.</param>
        /// <param name="configurationTypeDescription">Optional configuration to use; DEFAULT is null.</param>
        /// <param name="metadata">Optional metadata to put, especially useful for customer serializer factory; DEFAULT is empty.</param>
        public SerializationDescription(SerializationKind serializationKind, SerializationFormat serializationFormat, TypeDescription configurationTypeDescription = null, CompressionKind compressionKind = CompressionKind.None, IReadOnlyDictionary<string, string> metadata = null)
        {
            new { serializationKind }.Must().NotBeEqualTo(SerializationKind.Invalid);
            new { serializationFormat }.Must().NotBeEqualTo(SerializationFormat.Invalid);
            new { compressionKind }.Must().NotBeEqualTo(CompressionKind.Invalid);

            this.SerializationKind = serializationKind;
            this.SerializationFormat = serializationFormat;
            this.ConfigurationTypeDescription = configurationTypeDescription;
            this.CompressionKind = compressionKind;
            this.Metadata = metadata ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the <see cref="SerializationKind" /> to serialize into.
        /// </summary>
        public SerializationKind SerializationKind { get; private set; }

        /// <summary>
        /// Gets the <see cref="SerializationFormat" /> to serialize into.
        /// </summary>
        public SerializationFormat SerializationFormat { get; private set; }

        /// <summary>
        /// Gets the <see cref="CompressionKind" /> to use.
        /// </summary>
        public CompressionKind CompressionKind { get; private set; }

        /// <summary>
        /// Gets the <see cref="TypeDescription" /> of the configuration.
        /// </summary>
        public TypeDescription ConfigurationTypeDescription { get; private set; }

        /// <summary>
        /// Gets a map of metadata for custom use.
        /// </summary>
        public IReadOnlyDictionary<string, string> Metadata { get; private set; }

        /// <inheritdoc />
        public override string ToString()
        {
            var metadataString = string.Join(",", this.Metadata.Select(_ => Invariant($"[{_.Key}={_.Value}]")));
            var result = Invariant($"{nameof(SerializationDescription)}: {nameof(this.SerializationKind)}={this.SerializationKind}, {nameof(this.SerializationFormat)}={this.SerializationFormat}, {nameof(this.CompressionKind)}={this.CompressionKind}, {nameof(this.ConfigurationTypeDescription)}={this.ConfigurationTypeDescription}, {nameof(this.Metadata)}={metadataString},");
            return result;
        }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(SerializationDescription first, SerializationDescription second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            var metadataEqual = first.Metadata.DictionaryEqual(second.Metadata);

            return first.SerializationKind == second.SerializationKind && first.SerializationFormat == second.SerializationFormat
                   && first.CompressionKind == second.CompressionKind
                   && first.ConfigurationTypeDescription == second.ConfigurationTypeDescription
                   && metadataEqual;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(SerializationDescription first, SerializationDescription second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(SerializationDescription other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as SerializationDescription);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize()
            .Hash(this.SerializationKind)
            .Hash(this.SerializationFormat)
            .Hash(this.CompressionKind)
            .Hash(this.ConfigurationTypeDescription)
            .HashDictionary(this.Metadata).Value;
    }
}