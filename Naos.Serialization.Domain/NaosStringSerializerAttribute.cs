// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosStringSerializerAttribute.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain.Extensions
{
    using System;

    using OBeautifulCode.Reflection.Recipes;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Attribute to specify the type of <see cref="IStringSerializeAndDeserialize" /> to use for this type during serializations that support this override.
    /// </summary>
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Property)]
    public sealed class NaosStringSerializerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NaosStringSerializerAttribute"/> class.
        /// </summary>
        /// <param name="serializerType">Type of <see cref="IStringSerializeAndDeserialize" /> to use when string serializing where supported.</param>
        public NaosStringSerializerAttribute(Type serializerType)
        {
            new { serializerType }.Must().NotBeNull().OrThrowFirstFailure();
            var canContruct = serializerType.Construct();
            (canContruct is IStringSerializeAndDeserialize)
                .Named(Invariant($"Type specified {serializerType} was not an implementer of {typeof(IStringSerializeAndDeserialize)}")).Must().NotBeNull()
                .OrThrowFirstFailure();

            this.SerializerType = serializerType;
        }

        /// <summary>
        /// Gets the type of <see cref="IStringSerializeAndDeserialize" />.
        /// </summary>
        public Type SerializerType { get; private set; }
    }

    /// <summary>
    /// Attribute to specify the type of <see cref="IStringSerializeAndDeserialize" /> to use for elements in a collection or array.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class NaosElementStringSerializerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NaosElementStringSerializerAttribute"/> class.
        /// </summary>
        /// <param name="elementSerializerType">Type of <see cref="IStringSerializeAndDeserialize" /> to use when string serializing where supported.</param>
        public NaosElementStringSerializerAttribute(Type elementSerializerType)
        {
            new { serializerType = elementSerializerType }.Must().NotBeNull().OrThrowFirstFailure();
            var canContruct = elementSerializerType.Construct();
            (canContruct is IStringSerializeAndDeserialize)
                .Named(Invariant($"Type specified {elementSerializerType} was not an implementer of {typeof(IStringSerializeAndDeserialize)}")).Must().NotBeNull()
                .OrThrowFirstFailure();

            this.ElementSerializerType = elementSerializerType;
        }

        /// <summary>
        /// Gets the type of <see cref="IStringSerializeAndDeserialize" /> for elements.
        /// </summary>
        public Type ElementSerializerType { get; private set; }
    }
}