// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredBsonSerializer.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.Collections.Generic;

    using MongoDB.Bson.Serialization;

    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Bson converter to use.
    /// </summary>
    public class RegisteredBsonSerializer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredBsonSerializer"/> class.
        /// </summary>
        /// <param name="serializerBuilderFunction">Builder function.</param>
        /// <param name="handledTypes"><see cref="Type" />'s handled by this converter.</param>
        public RegisteredBsonSerializer(Func<IBsonSerializer> serializerBuilderFunction, IReadOnlyCollection<Type> handledTypes)
        {
            new { converterBuilderFunction = serializerBuilderFunction }.Must().NotBeNull();
            new { handledTypes }.Must().NotBeNull().And().NotBeEmptyEnumerable();

            this.SerializerBuilderFunction = serializerBuilderFunction;
            this.HandledTypes = handledTypes;
        }

        /// <summary>
        /// Gets the builder function.
        /// </summary>
        public Func<IBsonSerializer> SerializerBuilderFunction { get; private set; }

        /// <summary>
        /// Gets the <see cref="Type" /> that this converter will handle.
        /// </summary>
        public IReadOnlyCollection<Type> HandledTypes { get; private set; }
    }
}