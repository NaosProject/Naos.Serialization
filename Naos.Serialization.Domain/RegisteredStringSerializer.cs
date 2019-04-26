// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredStringSerializer.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Collections.Generic;
    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// String converter to use.
    /// </summary>
    public class RegisteredStringSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredStringSerializer"/> class.
        /// </summary>
        /// <param name="serializerBuilderFunction">Builder function.</param>
        /// <param name="handledTypes"><see cref="Type" />'s handled by this converter.</param>
        public RegisteredStringSerializer(Func<IStringSerializeAndDeserialize> serializerBuilderFunction, IReadOnlyCollection<Type> handledTypes)
        {
            new { serializerBuilderFunction }.Must().NotBeNull();
            new { handledTypes }.Must().NotBeNull().And().NotBeEmptyEnumerable();

            this.SerializerBuilderFunction = serializerBuilderFunction;
            this.HandledTypes = handledTypes;
        }

        /// <summary>
        /// Gets the builder function.
        /// </summary>
        public Func<IStringSerializeAndDeserialize> SerializerBuilderFunction { get; private set; }

        /// <summary>
        /// Gets the <see cref="Type" /> that this serializer will handle.
        /// </summary>
        public IReadOnlyCollection<Type> HandledTypes { get; private set; }
    }
}