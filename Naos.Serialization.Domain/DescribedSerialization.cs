// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DescribedSerialization.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;

    using OBeautifulCode.Math.Recipes;
    using OBeautifulCode.Type;
    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Represents a serialized object along with a description of the type of the object.
    /// </summary>
    public class DescribedSerialization : IEquatable<DescribedSerialization>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DescribedSerialization"/> class.
        /// </summary>
        /// <param name="payloadTypeDescription">A description of the type of object serialized.</param>
        /// <param name="serializedPayload">The object serialized to a string.</param>
        /// <param name="serializationDescription">The serializer used to generate the payload.</param>
        /// <exception cref="ArgumentNullException"><paramref name="payloadTypeDescription"/> is null.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serializedPayload"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="serializedPayload"/> is whitespace.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="serializationDescription"/> is null.</exception>
        /// <exception cref="ArgumentException"><paramref name="serializationDescription"/> is whitespace.</exception>
        public DescribedSerialization(TypeDescription payloadTypeDescription, string serializedPayload, SerializationDescription serializationDescription)
        {
            new { payloadTypeDescription }.Must().NotBeNull();
            new { serializationDescription }.Must().NotBeNull();

            this.PayloadTypeDescription = payloadTypeDescription;
            this.SerializedPayload = serializedPayload;
            this.SerializationDescription = serializationDescription;
        }

        /// <summary>
        /// Gets a description of the type of object serialized.
        /// </summary>
        public TypeDescription PayloadTypeDescription { get; private set; }

        /// <summary>
        /// Gets the object serialized to a string (bytes will be Base64 encoded here).
        /// </summary>
        public string SerializedPayload { get; private set; }

        /// <summary>
        /// Gets the description of the serializer used to generate the payload.
        /// </summary>
        public SerializationDescription SerializationDescription { get; private set; }

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are equal.</returns>
        public static bool operator ==(DescribedSerialization first, DescribedSerialization second)
        {
            if (ReferenceEquals(first, second))
            {
                return true;
            }

            if (ReferenceEquals(first, null) || ReferenceEquals(second, null))
            {
                return false;
            }

            return first.PayloadTypeDescription == second.PayloadTypeDescription
                   && first.SerializedPayload == second.SerializedPayload
                   && first.SerializationDescription == second.SerializationDescription;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="first">First parameter.</param>
        /// <param name="second">Second parameter.</param>
        /// <returns>A value indicating whether or not the two items are inequal.</returns>
        public static bool operator !=(DescribedSerialization first, DescribedSerialization second) => !(first == second);

        /// <inheritdoc />
        public bool Equals(DescribedSerialization other) => this == other;

        /// <inheritdoc />
        public override bool Equals(object obj) => this == (obj as DescribedSerialization);

        /// <inheritdoc />
        public override int GetHashCode() => HashCodeHelper.Initialize().Hash(this.PayloadTypeDescription).Hash(this.SerializedPayload).Hash(this.SerializationDescription).Value;
    }
}
