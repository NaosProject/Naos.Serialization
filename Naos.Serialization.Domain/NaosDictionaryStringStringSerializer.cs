// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosDictionaryStringStringSerializer.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using OBeautifulCode.Reflection.Recipes;

    using Spritely.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Interface to work with compression.
    /// </summary>
    public class NaosDictionaryStringStringSerializer : IStringSerializeAndDeserialize
    {
        /// <summary>
        /// For specifying a key and value in a string.
        /// </summary>
        public const string KeyValueDelimiter = "=";

        /// <summary>
        /// For specifying multiple entries in a single string.
        /// </summary>
        public static readonly string LineDelimiter = Environment.NewLine;

        /// <inheritdoc cref="IHaveSerializationKind" />
        public SerializationKind SerializationKind => SerializationKind.Default;

        /// <inheritdoc cref="IHaveConfigurationType" />
        public Type ConfigurationType => null;

        /// <inheritdoc cref="IStringSerializeAndDeserialize" />
        public string SerializeToString(object objectToSerialize)
        {
            if (objectToSerialize == null)
            {
                return null;
            }

            var dictionary = objectToSerialize as IReadOnlyDictionary<string, string>;
            dictionary.Named(Invariant($"typeMustBeConvertableTo-{nameof(IReadOnlyDictionary<string, string>)}-found-{objectToSerialize.GetType()}")).Must()
                .NotBeNull().OrThrowFirstFailure();

            return SerializeDictionaryToString(dictionary);
        }

        /// <summary>
        /// Serializes a dictionary to a string.
        /// </summary>
        /// <param name="dictionary">Dictionary to serialize.</param>
        /// <returns>String serialized dictionary.</returns>
        public static string SerializeDictionaryToString(IReadOnlyDictionary<string, string> dictionary)
        {
            if (dictionary == null)
            {
                return null;
            }

            if (!dictionary.Any())
            {
                return string.Empty;
            }

            var stringBuilder = new StringBuilder();

            foreach (var keyValuePair in dictionary)
            {
                var key = keyValuePair.Key;
                var value = keyValuePair.Value;

                key.Contains(KeyValueDelimiter).Named(Invariant($"Key-cannot-contain-{nameof(KeyValueDelimiter)}--{KeyValueDelimiter}--found-on-key--{key}")).Must()
                    .BeFalse().OrThrowFirstFailure();
                (value ?? string.Empty).Contains(KeyValueDelimiter).Named(Invariant($"Key-cannot-contain-{nameof(KeyValueDelimiter)}--{KeyValueDelimiter}--found-on-value--{value}"))
                    .Must().BeFalse().OrThrowFirstFailure();

                key.Contains(LineDelimiter)
                    .Named(
                        Invariant(
                            $"Key-cannot-contain-{nameof(LineDelimiter)}--{(Environment.NewLine == LineDelimiter ? "NEWLINE" : LineDelimiter)}--found-on-key--{key}"))
                    .Must().BeFalse().OrThrowFirstFailure();
                (value ?? string.Empty).Contains(LineDelimiter)
                    .Named(
                        Invariant(
                            $"Key-cannot-contain-{nameof(LineDelimiter)}--{(Environment.NewLine == LineDelimiter ? "NEWLINE" : LineDelimiter)}--found-on-value--{value}"))
                    .Must().BeFalse().OrThrowFirstFailure();

                stringBuilder.Append(Invariant($"{key}{KeyValueDelimiter}{value ?? string.Empty}"));
                stringBuilder.Append(LineDelimiter);
            }

            var ret = stringBuilder.ToString();
            return ret;
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public T Deserialize<T>(string serializedString)
        {
            if (serializedString == null)
            {
                return default(T);
            }

            return (T)this.Deserialize(serializedString, typeof(T));
        }

        /// <inheritdoc cref="IStringSerializeAndDeserialize"/>
        public object Deserialize(string serializedString, Type type)
        {
            if (serializedString == null)
            {
                return null;
            }

            new { type }.Must().NotBeNull().OrThrowFirstFailure();

            var dictionary = type.Construct() as IReadOnlyDictionary<string, string>;
            dictionary.Named(Invariant($"typeMustBeConvertableTo-{nameof(IReadOnlyDictionary<string, string>)}-found-{type}")).Must().NotBeNull().OrThrowFirstFailure();

            return DeserializeToDictionary(serializedString);
        }

        /// <summary>
        /// Deserialize the string to a dictionary of string, string.
        /// </summary>
        /// <param name="serializedString">String to deserialize.</param>
        /// <returns>Deserialized dictionary.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Name/spelling is correct.")]
        public static Dictionary<string, string> DeserializeToDictionary(string serializedString)
        {
            if (serializedString == null)
            {
                return null;
            }

            if (string.IsNullOrEmpty(serializedString))
            {
                return new Dictionary<string, string>();
            }

            try
            {
                var ret = new Dictionary<string, string>();
                var lines = serializedString.Split(new[] { LineDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var line in lines)
                {
                    var items = line.Split(new[] { KeyValueDelimiter }, StringSplitOptions.RemoveEmptyEntries);
                    items.Length.Named(Invariant($"Line-must-split-on-{nameof(KeyValueDelimiter)}--{KeyValueDelimiter}-to-1-or-2-items-this-did-not--{line}"))
                        .Must().BeInRange(1, 2).OrThrowFirstFailure();
                    var key = items[0];
                    var value = items.Length == 2 ? items[1] : null;
                    ret.Add(key, value);
                }

                return ret;
            }
            catch (Exception ex)
            {
                throw new NaosSerializationException(Invariant($"Failed to deserialize '{serializedString}'"), ex);
            }
        }
    }
}