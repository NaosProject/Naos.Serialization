// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AllTypesOfDictionaryJsonConverter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Newtonsoft.Json;

    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Custom dictionary converter to do the right thing.
    /// Supports:
    /// - <see cref="IDictionary{TKey, TValue}"/>
    /// - <see cref="Dictionary{TKey, TValue}"/>
    /// - <see cref="IReadOnlyDictionary{TKey, TValue}" />
    /// - <see cref="ReadOnlyDictionary{TKey, TValue}" />
    /// - <see cref="ConcurrentDictionary{TKey, TValue}" />.
    /// </summary>
    internal class AllTypesOfDictionaryJsonConverter : JsonConverter
    {
        private static readonly Type[] SupportedDictionaryTypes = new[] { typeof(IDictionary<,>), typeof(ReadOnlyDictionary<,>), typeof(IReadOnlyDictionary<,>), typeof(ConcurrentDictionary<,>) };

        /// <inheritdoc />
        public override bool CanWrite => false;

        /// <inheritdoc />
        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            throw new NotSupportedException(nameof(DictionaryJsonConverter) + " is a read-only converter.");
        }

        /// <inheritdoc />
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            var genericArguments = objectType.GetGenericArguments();
            new { genericArguments.Length }.Must().BeEqualTo(2, "More ore less than 2 generic arguments means this cannot be a dictionary type that should supported.");

            var wrappedDictionaryType = typeof(Dictionary<,>).MakeGenericType(genericArguments);

            var wrappedDictionary = serializer.Deserialize(reader, wrappedDictionaryType);

            object result;
            var unboundedGenericReturnType = objectType.GetGenericTypeDefinition();
            if ((unboundedGenericReturnType == typeof(IDictionary<,>)) || (unboundedGenericReturnType == typeof(Dictionary<,>)))
            {
                // nothing to do, the dictionary is already of the expected return type
                result = wrappedDictionary;
            }
            else if ((unboundedGenericReturnType == typeof(ReadOnlyDictionary<,>)) || (unboundedGenericReturnType == typeof(IReadOnlyDictionary<,>)))
            {
                result = typeof(ReadOnlyDictionary<,>).MakeGenericType(genericArguments).Construct(wrappedDictionary);
            }
            else if (unboundedGenericReturnType == typeof(ConcurrentDictionary<,>))
            {
                result = typeof(ConcurrentDictionary<,>).MakeGenericType(genericArguments).Construct(wrappedDictionary);
            }
            else
            {
                throw new InvalidOperationException("The following type was not expected: " + objectType);
            }

            return result;
        }

        /// <inheritdoc />
        public override bool CanConvert(
            Type objectType)
        {
            bool result;

            if (objectType == null)
            {
                result = false;
            }
            else
            {
                if (objectType.IsGenericType)
                {
                    var unboundGenericObjectType = objectType.GetGenericTypeDefinition();

                    result = SupportedDictionaryTypes.Contains(unboundGenericObjectType);
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }
    }
}
