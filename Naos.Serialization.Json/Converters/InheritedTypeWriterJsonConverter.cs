// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InheritedTypeWriterJsonConverter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// An <see cref="InheritedTypeJsonConverterBase"/> that handles writes/serialization.
    /// </summary>
    internal class InheritedTypeWriterJsonConverter : InheritedTypeJsonConverterBase
    {
        private bool writeJsonCalled;

        /// <inheritdoc />
        public override bool CanRead => false;

        /// <inheritdoc />
        public override bool CanWrite => true;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            // WriteJson needs to use the JsonSerializer passed to the method so that the various
            // settings in the serializer are utilized for writing.  Using the serializer as-is,
            // however, will cause infinite recursion because this Converter is utilized by the serializer.
            // We cannot modify the serializer to remove this converter because the object to serialize
            // might contain types that require this converter.  The only way to manage this to store
            // some state when WriteJson is called, detect that state here, and tell json.net that we
            // cannot convert the type.
            if (this.writeJsonCalled)
            {
                this.writeJsonCalled = false;
                return false;
            }

            var result = this.ShouldBeHandledByThisConverter(objectType);

            return result;
        }

        /// <inheritdoc />
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            throw new NotSupportedException("This is a write-only converter");
        }

        /// <inheritdoc />
        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            new { value }.Must().NotBeNull();

            var typeName = value.GetType().FullName + ", " + value.GetType().Assembly.GetName().Name;

            this.writeJsonCalled = true;
            var jsonObject = JObject.FromObject(value, serializer);
            jsonObject.Add(ConcreteTypeTokenName, typeName);
            jsonObject.WriteTo(writer);
        }
    }
}