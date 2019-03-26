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
    internal class DictionaryJsonConverter : JsonConverter
    {
        private static readonly Type[] SupportedDictionaryTypes = new[] { typeof(Dictionary<,>), typeof(IDictionary<,>), typeof(ReadOnlyDictionary<,>), typeof(IReadOnlyDictionary<,>), typeof(ConcurrentDictionary<,>) };

        public override bool CanWrite => false;

        /// <inheritdoc />
        public override void WriteJson(
            JsonWriter writer,
            object value,
            JsonSerializer serializer)
        {
            throw new NotSupportedException();
            //writer.WriteStartObject();

            //var valueAsEnumerable = (IEnumerable)value;

            //var elementsToWrite = new List<object>();
            //foreach (var element in valueAsEnumerable)
            //{
            //    var jo = JObject.FromObject(element, serializer);
            //    if (jo.)
            //}

            //var output = new JArray(elementsToWrite.ToArray());

            //output.WriteTo(writer);
            //writer.WriteValue();
        }

        /// <inheritdoc />
        public override object ReadJson(
            JsonReader reader,
            Type objectType,
            object existingValue,
            JsonSerializer serializer)
        {
            const JsonToken propertyNameToken = JsonToken.PropertyName;
            const string jsonReaderUnderlyingTokenFieldName = "_tokenType";

            var genericArguments = objectType.GetGenericArguments();
            new { genericArguments.Length }.Must().BeEqualTo(2, "More ore less than 2 generic arguments means this cannot be a dictionary type that should supported.");

            var keyType = genericArguments.First();
            var valueType = genericArguments.Last();
            var wrappedDictionaryType = typeof(Dictionary<,>).MakeGenericType(genericArguments);

            var wrappedDictionary = wrappedDictionaryType.Construct();
            var wrappedDictionaryAddMethod = wrappedDictionaryType.GetMethod(nameof(Dictionary<object, object>.Add));

            if (reader.TokenType == JsonToken.StartArray)
            {
                reader.Read();
                if (reader.TokenType == JsonToken.EndArray)
                {
                    return new Dictionary<string, string>();
                }
                else
                {
                    throw new JsonSerializationException("Non-empty JSON array does not make a valid Dictionary!");
                }
            }
            else if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                object WrappedDeserialize(JsonReader localReader, Type targetType)
                {
                    if (localReader.TokenType == JsonToken.Null)
                    {
                        return null;
                    }
                    else
                    {
                        var undoPropertyNameTokenFieldHack = false;
                        if (localReader.TokenType == propertyNameToken)
                        {
                            undoPropertyNameTokenFieldHack = true;
                            localReader.SetFieldValue(jsonReaderUnderlyingTokenFieldName, JsonToken.String);
                        }

                        var localResult = serializer.Deserialize(localReader, targetType);
                        if (undoPropertyNameTokenFieldHack)
                        {
                            localReader.SetFieldValue(jsonReaderUnderlyingTokenFieldName, propertyNameToken);
                        }

                        return localResult;
                    }
                }

                reader.Read();
                while (reader.TokenType != JsonToken.EndObject)
                {
                    if (reader.TokenType != propertyNameToken)
                    {
                        throw new JsonSerializationException("Unexpected token!");
                    }

                    var key = WrappedDeserialize(reader, keyType);
                    reader.Read();

                    var value = WrappedDeserialize(reader, valueType);
                    reader.Read();

                    wrappedDictionaryAddMethod.Invoke(wrappedDictionary, new[] { key, value });
                }
            }
            else
            {
                throw new JsonSerializationException("Unexpected token!");
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
