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

    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.TypeRepresentation;
    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Model object to desribe a serializer so you can persist and share the definition and rehydrate the serializer later.
    /// </summary>
    public class SerializationDescription : IEquatable<SerializationDescription>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SerializationDescription"/> class.
        /// </summary>
        /// <param name="serializationFormat">The <see cref="SerializationFormat" /> to serialize into.</param>
        /// <param name="serializationRepresentation">The <see cref="SerializationRepresentation" /> to serialize into.</param>
        /// <param name="serializationKind">Optional <see cref="SerializationKind" /> to use; DEFAULT is Default.</param>
        /// <param name="compressionKind">Optional <see cref="CompressionKind" /> to use; DEFAULT is None.</param>
        /// <param name="configurationTypeDescription">Optional configuration to use; DEFAULT is null.</param>
        /// <param name="metadata">Optional metadata to put, especially useful for customer serializer factory; DEFAULT is empty.</param>
        public SerializationDescription(SerializationFormat serializationFormat, SerializationRepresentation serializationRepresentation, SerializationKind serializationKind = SerializationKind.Default, TypeDescription configurationTypeDescription = null, CompressionKind compressionKind = CompressionKind.None, IReadOnlyDictionary<string, string> metadata = null)
        {
            new { serializationFormat }.Must().NotBeEqualTo(SerializationFormat.Invalid);
            new { serializationRepresentation }.Must().NotBeEqualTo(SerializationRepresentation.Invalid);
            new { serializationKind }.Must().NotBeEqualTo(SerializationKind.Invalid);
            new { compressionKind }.Must().NotBeEqualTo(CompressionKind.Invalid);

            this.SerializationFormat = serializationFormat;
            this.SerializationRepresentation = serializationRepresentation;
            this.SerializationKind = serializationKind;
            this.ConfigurationTypeDescription = configurationTypeDescription;
            this.CompressionKind = compressionKind;
            this.Metadata = metadata ?? new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the <see cref="SerializationFormat" /> to serialize into.
        /// </summary>
        public SerializationFormat SerializationFormat { get; private set; }

        /// <summary>
        /// Gets the <see cref="SerializationRepresentation" /> to serialize into.
        /// </summary>
        public SerializationRepresentation SerializationRepresentation { get; private set; }

        /// <summary>
        /// Gets the <see cref="SerializationKind" /> to use.
        /// </summary>
        public SerializationKind SerializationKind { get; private set; }

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

            var metadataEqual = first.Metadata.Count == second.Metadata.Count;
            foreach (var firstKey in first.Metadata.Keys)
            {
                if (!metadataEqual)
                {
                    break;
                }

                metadataEqual = second.Metadata.ContainsKey(firstKey) && second.Metadata[firstKey] == first.Metadata[firstKey];
            }

            return first.SerializationFormat == second.SerializationFormat && first.SerializationRepresentation == second.SerializationRepresentation
                   && first.SerializationKind == second.SerializationKind && first.CompressionKind == second.CompressionKind
                   && first.ConfigurationTypeDescription == second.ConfigurationTypeDescription && metadataEqual;
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
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.SerializationFormat).Hash(this.SerializationRepresentation)
            .Hash(this.SerializationKind).Hash(this.CompressionKind).Hash(this.ConfigurationTypeDescription)
            .HashElements(this.Metadata.OrderBy(_ => _.Key).Select(_ => new Tuple<string, string>(_.Key, _.Value))).Value;
    }
}