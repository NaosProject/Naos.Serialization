// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NewtonsoftJsonSerializerSettingsFactory.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Json
{
    using System;

    using Naos.Serialization.Domain;

    using Newtonsoft.Json;

    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Factory to build <see cref="JsonSerializerSettings" />.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Newtonsoft", Justification = "Spelling/name is correct.")]
    public static class NewtonsoftJsonSerializerSettingsFactory
    {
        /// <summary>
        ///  Gets the settings to use from the <see cref="SerializationKind" /> and optional configuration <see cref="Type" /> provided.
        /// </summary>
        /// <param name="serializationKind">Kind to determine the settings.</param>
        /// <param name="serializationDirection">Direction to determine the settings.</param>
        /// <param name="configurationType">Optional configuration Type.</param>
        /// <returns><see cref="JsonSerializerSettings" /> to use with <see cref="Newtonsoft" /> when serializing.</returns>
        public static JsonSerializerSettings BuildSettings(SerializationKind serializationKind, SerializationDirection serializationDirection, Type configurationType = null)
        {
            new { serializationKind }.Must().NotBeEqualTo(SerializationKind.Invalid);

            if (serializationKind == SerializationKind.Custom)
            {
                if (configurationType == null)
                {
                    throw new ArgumentException(Invariant($"Must specify {nameof(configurationType)} if using {nameof(serializationKind)} of {nameof(SerializationKind)}.{SerializationKind.Custom}"));
                }

                var configuration = JsonConfigurationManager.Configure(configurationType);

                switch (serializationDirection)
                {
                    case SerializationDirection.Serialize:
                        return configuration.WriteSerializationSettings;
                    case SerializationDirection.Deserialize:
                        return configuration.ReadSerializationSettings;
                    default:
                        throw new NotSupportedException(Invariant($"Value of {nameof(serializationDirection)} - {serializationDirection} is not currently supported."));
                }
            }
            else
            {
                return JsonConfigurationBase.SerializationKindToSettingsSelectorByDirection[serializationKind](serializationDirection);
            }
        }
    }
}