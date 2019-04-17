// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RegisteredJsonConverter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using Naos.Serialization.Domain;
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
        /// <param name="converterBuilderFunction">Builder function.</param>
        /// <param name="outputKind"><see cref="RegisteredJsonConverterOutputKind" /> of this converter.</param>
        /// <param name="handledTypes"><see cref="Type" />'s handled by this converter.</param>
        /// <param name="details">Details about the registration.</param>
        public RegisteredJsonConverter(Func<JsonConverter> converterBuilderFunction, RegisteredJsonConverterOutputKind outputKind, IReadOnlyCollection<Type> handledTypes, RegistrationDetails details)
        {
            new { converterBuilderFunction }.Must().NotBeNull();
            new { handledTypes }.Must().NotBeNull().And().NotBeEmptyEnumerable();
            new { details }.Must().NotBeNull();

            this.ConverterBuilderFunction = converterBuilderFunction;
            this.OutputKind = outputKind;
            this.HandledTypes = handledTypes;
            this.Details = details;
        }

        /// <summary>
        /// Gets the builder function.
        /// </summary>
        public Func<JsonConverter> ConverterBuilderFunction { get; private set; }

        /// <summary>
        /// Gets the <see cref="RegisteredJsonConverterOutputKind" />.
        /// </summary>
        public RegisteredJsonConverterOutputKind OutputKind { get; private set; }

        /// <summary>
        /// Gets the <see cref="Type" /> that this converter will handle.
        /// </summary>
        public IReadOnlyCollection<Type> HandledTypes { get; private set; }

        /// <summary>
        /// Gets the details about the registration.
        /// </summary>
        public RegistrationDetails Details { get; private set; }
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