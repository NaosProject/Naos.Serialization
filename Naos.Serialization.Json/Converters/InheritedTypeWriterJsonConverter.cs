// --------------------------------------------------------------------------------------------------------------------
// <copyright file="InheritedTypeWriterJsonConverter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Threading;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// An <see cref="InheritedTypeJsonConverterBase"/> that handles writes/serialization.
    /// </summary>
    internal class InheritedTypeWriterJsonConverter : InheritedTypeJsonConverterBase
    {
        private static readonly ThreadLocal<bool> WriteJsonCalled = new ThreadLocal<bool>(() => false);

        /// <inheritdoc />
        public override bool CanRead => false;

        /// <inheritdoc />
        public override bool CanWrite => true;

        /// <inheritdoc />
        public override bool CanConvert(Type objectType)
        {
            // WriteJson needs to use the JsonSerializer passed to the method so that
            // the various settings in the serializer are utilized for writing.
            // Using the serializer as-is, however, will cause infinite recursion because
            // this Converter is utilized by the serializer.  If we modify the serializer
            // to remove this converter, it will affect all other consumers of the serializer.
            // Also, the object to serialize might contain types that require this converter.
            // The only way to manage this to store some state when WriteJson is called,
            // detect that state here, and tell json.net that we cannot convert the type.
            if (WriteJsonCalled.Value)
            {
                WriteJsonCalled.Value = false;
                return false;
            }

            var bindableAttribute = GetBindableAttribute(objectType);
            if (bindableAttribute == null)
            {
                return false;
            }

            // Two-way instructs the converter to add the name of the type to the JSON payload
            // when serializing.  This is sometimes required to disambiguate types when de-serializing.
            // Sometimes, multiple types have the same named properties and share an abstract parent.  De-serializing
            // the JSON into the abstract parent won't work without some extra information to determine which
            // of those multiple types to create.  In this case, we want to call our implementation of WriteJson.
            var result = IsTwoWayBindable(bindableAttribute);

            return result;
        }

        /// <inheritdoc />
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotSupportedException("This is a write-only converter");
        }

        /// <inheritdoc />
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            new { value }.Must().NotBeNull();

            var typeName = value.GetType().FullName + ", " + value.GetType().Assembly.GetName().Name;
            WriteJsonCalled.Value = true;
            var jo = JObject.FromObject(value, serializer);
            jo.Add(TypeTokenName, typeName);
            jo.WriteTo(writer);
        }
    }
}