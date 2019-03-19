// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DictionaryJsonConverter.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;
    using System.Collections;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using OBeautifulCode.Reflection.Recipes;

    /// <summary>
    /// Custom dictionary converter to do the right thing.
    /// Supports:
    /// - <see cref="IDictionary{TKey, TValue}"/>
    /// - <see cref="Dictionary{TKey, TValue}"/>
    /// - <see cref="IReadOnlyDictionary{TKey, TValue}" />
    /// - <see cref="ReadOnlyDictionary{TKey, TValue}" />
    /// - <see cref="ConcurrentDictionary{TKey, TValue}" />.
    /// </summary>
    internal class DictionaryJsonConverter : JsonConverter
    {
        private static readonly Type[] SupportedDictionaryTypes = new[] { typeof(Dictionary<,>), typeof(IDictionary<,>), typeof(ReadOnlyDictionary<,>), typeof(IReadOnlyDictionary<,>), typeof(ConcurrentDictionary<,>) };

        /// <inheritdoc />
        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            var valueAsEnumerable = (IEnumerable)value;

            var elementsToWrite = new List<object>();
            foreach (var element in valueAsEnumerable)
            {
                var jo = JObject.FromObject(element, serializer);
                elementsToWrite.Add(jo);
            }

            var output = new JArray(elementsToWrite.ToArray());

            output.WriteTo(writer);
        }

        /// <inheritdoc />
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }

            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            // create a List<KeyValuePair<K,V>> where K and V are the generic type parameters of the dictionary we are deserializing into
            // Get the .Add() method so that we can add elements to that list
            var genericArguments = objectType.GetGenericArguments();
            var keyValuePairType = typeof(KeyValuePair<,>).MakeGenericType(genericArguments);
            var listOfKeyValuePairType = typeof(List<>).MakeGenericType(keyValuePairType);
            var listOfKeyValuePairs = listOfKeyValuePairType.Construct();
            var listOfKeyValuePairAddMethod = listOfKeyValuePairType.GetMethod(nameof(List<object>.Add));

            // the object was serialized as a JArray of KeyValuePair, so deserialize it as such
            var deserializedJArray = JToken.ReadFrom(reader);

            foreach (var element in deserializedJArray)
            {
                var keyValuePair = element.ToObject(keyValuePairType, serializer);

                // ReSharper disable once PossibleNullReferenceException
                listOfKeyValuePairAddMethod.Invoke(listOfKeyValuePairs, new[] { keyValuePair });
            }

            // add each KeyValuePair<T,K> into a new Dictionary<T,K>
            var wrappedDictionaryType = typeof(Dictionary<,>).MakeGenericType(genericArguments);
            var wrappedDictionary = wrappedDictionaryType.Construct();
            var wrappedDictionaryAddMethod = wrappedDictionaryType.GetMethod(nameof(Dictionary<object, object>.Add));

            var listOfKeyValuePairsAsEnumerable = (IEnumerable)listOfKeyValuePairs;
            foreach (var element in listOfKeyValuePairsAsEnumerable)
            {
                var key = element.GetPropertyValue<object>(nameof(KeyValuePair<object, object>.Key));
                var value = element.GetPropertyValue<object>(nameof(KeyValuePair<object, object>.Value));

                wrappedDictionaryAddMethod.Invoke(wrappedDictionary, new[] { key, value });
            }

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
