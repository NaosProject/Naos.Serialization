// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosStringSerializerAttribute.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain.Extensions
{
    using System;

    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Attribute to specify the type of <see cref="IStringSerializeAndDeserialize" /> to use for this type during serializations that support this override.
    /// </summary>
    [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NaosStringSerializerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NaosStringSerializerAttribute"/> class.
        /// </summary>
        /// <param name="serializerType">Type of <see cref="IStringSerializeAndDeserialize" /> to use when string serializing where supported.</param>
        public NaosStringSerializerAttribute(Type serializerType)
        {
            new { serializerType }.Must().NotBeNull();

            serializerType.HasParameterlessConstructor().Named(Invariant($"Type specified {serializerType} must have a paramerterless constructor.")).Must()
                .BeTrue();
            serializerType.ImplementsInterface<IStringSerializeAndDeserialize>().Named(
                Invariant($"Type specified {serializerType} was not an implementer of {typeof(IStringSerializeAndDeserialize)}")).Must().BeTrue();

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
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public sealed class NaosElementStringSerializerAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NaosElementStringSerializerAttribute"/> class.
        /// </summary>
        /// <param name="elementSerializerType">Type of <see cref="IStringSerializeAndDeserialize" /> to use when string serializing where supported.</param>
        public NaosElementStringSerializerAttribute(Type elementSerializerType)
        {
            elementSerializerType.HasParameterlessConstructor()
                .Named(Invariant($"Type specified {elementSerializerType} must have a paramerterless constructor.")).Must().BeTrue();
            elementSerializerType.ImplementsInterface<IStringSerializeAndDeserialize>().Named(
                Invariant($"Type specified {elementSerializerType} was not an implementer of {typeof(IStringSerializeAndDeserialize)}")).Must().BeTrue();

            this.ElementSerializerType = elementSerializerType;
        }

        /// <summary>
        /// Gets the type of <see cref="IStringSerializeAndDeserialize" /> for elements.
        /// </summary>
        public Type ElementSerializerType { get; private set; }
    }
}