// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InheritedTypeJsonConverterBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Newtonsoft.Json;

    /// <summary>
    /// A converter that handles inherited types.
    /// </summary>
    internal abstract class InheritedTypeJsonConverterBase : JsonConverter
    {
        private readonly IReadOnlyCollection<Type> typesToHandle;

        /// <summary>
        /// Initializes a new instance of the <see cref="InheritedTypeJsonConverterBase"/> class.
        /// </summary>
        /// <param name="typesToHandle">Types that when encountered should trigger usage of the converter.</param>
        protected InheritedTypeJsonConverterBase(IReadOnlyCollection<Type> typesToHandle)
        {
            this.typesToHandle = typesToHandle ?? new List<Type>();
        }

        /// <summary>
        /// The concrete type token name constant.
        /// </summary>
        protected const string ConcreteTypeTokenName = "$concreteType";

        /// <summary>
        /// Determines if the specified type should be handled by this converter.
        /// </summary>
        /// <param name="objectType">The type.</param>
        /// <returns>
        /// true if the specified type should be handled by this converter; false otherwise.
        /// </returns>
        protected bool ShouldBeHandledByThisConverter(
            Type objectType)
        {
            return this.typesToHandle.Contains(objectType);
        }
    }
}
