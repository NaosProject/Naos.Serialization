﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredJsonConverter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;
    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Json converter to use.
    /// </summary>
    public class RegisteredJsonConverter
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RegisteredJsonConverter"/> class.
        /// </summary>
        /// <param name="serializingConverterBuilderFunction">Builder function for the converter to use when serializing.</param>
        /// <param name="deserializingConverterBuilderFunction">Builder function for the converter to use when deserializing.</param>
        /// <param name="outputKind"><see cref="RegisteredJsonConverterOutputKind" /> of this converter.</param>
        /// <param name="handledTypes"><see cref="Type" />'s handled by this converter.</param>
        public RegisteredJsonConverter(Func<JsonConverter> serializingConverterBuilderFunction, Func<JsonConverter> deserializingConverterBuilderFunction, RegisteredJsonConverterOutputKind outputKind, IReadOnlyCollection<Type> handledTypes)
        {
            new { serializingConverterBuilderFunction }.Must().NotBeNull();
            new { deserializingConverterBuilderFunction }.Must().NotBeNull();
            new { handledTypes }.Must().NotBeNull().And().NotBeEmptyEnumerable();

            this.SerializingConverterBuilderFunction = serializingConverterBuilderFunction;
            this.DeserializingConverterBuilderFunction = deserializingConverterBuilderFunction;
            this.OutputKind = outputKind;
            this.HandledTypes = handledTypes;
        }

        /// <summary>
        /// Gets the builder function for the converter to use when serializing.
        /// </summary>
        public Func<JsonConverter> SerializingConverterBuilderFunction { get; private set; }

        /// <summary>
        /// Gets the builder function for the converter to use when deserializing.
        /// </summary>
        public Func<JsonConverter> DeserializingConverterBuilderFunction { get; private set; }

        /// <summary>
        /// Gets the <see cref="RegisteredJsonConverterOutputKind" />.
        /// </summary>
        public RegisteredJsonConverterOutputKind OutputKind { get; private set; }

        /// <summary>
        /// Gets the <see cref="Type" /> that this converter will handle.
        /// </summary>
        public IReadOnlyCollection<Type> HandledTypes { get; private set; }
    }

    /// <summary>
    /// Enumeration of the outputs of the <see cref="RegisteredJsonConverter" />.
    /// </summary>
    public enum RegisteredJsonConverterOutputKind
    {
        /// <summary>
        /// Completely unknown.
        /// </summary>
        Unknown,

        /// <summary>
        /// Outputs what JSON will consider a string.
        /// </summary>
        String,

        /// <summary>
        /// Outputs what JSON will consider an object (i.e. a start object is emitted.)
        /// </summary>
        Object,
    }
}