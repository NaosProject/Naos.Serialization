// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializationDescription.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;

    using Naos.Compression.Domain;

    using OBeautifulCode.Math;
    using OBeautifulCode.TypeRepresentation;

    using Spritely.Recipes;

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
        /// <param name="configurationTypeDescription">Optional configuration to use, default is null</param>
        public SerializationDescription(SerializationFormat serializationFormat, SerializationRepresentation serializationRepresentation, SerializationKind serializationKind = SerializationKind.Default, TypeDescription configurationTypeDescription = null, CompressionKind compressionKind = CompressionKind.None)
        {
            new { serializationFormat }.Must().NotBeEqualTo(SerializationFormat.Invalid).OrThrowFirstFailure();
            new { serializationRepresentation }.Must().NotBeEqualTo(SerializationRepresentation.Invalid).OrThrowFirstFailure();
            new { serializationKind }.Must().NotBeEqualTo(SerializationKind.Invalid).OrThrowFirstFailure();
            new { compressionKind }.Must().NotBeEqualTo(CompressionKind.Invalid).OrThrowFirstFailure();

            this.SerializationFormat = serializationFormat;
            this.SerializationRepresentation = serializationRepresentation;
            this.SerializationKind = serializationKind;
            this.ConfigurationTypeDescription = configurationTypeDescription;
            this.CompressionKind = compressionKind;
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

            return first.SerializationFormat == second.SerializationFormat && first.SerializationRepresentation == second.SerializationRepresentation
                   && first.SerializationKind == second.SerializationKind && first.CompressionKind == second.CompressionKind
                   && first.ConfigurationTypeDescription == second.ConfigurationTypeDescription;
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
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.SerializationFormat).Hash(this.SerializationRepresentation).Hash(this.SerializationKind).Hash(this.CompressionKind).Hash(this.ConfigurationTypeDescription).Value;
    }
}