// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosDateTimeStringSerializer.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Text.RegularExpressions;

    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// String serializer for <see cref="DateTime" />.
    /// </summary>
    public class NaosDateTimeStringSerializer : IStringSerializeAndDeserialize
    {
        private enum StringEncodingPattern
        {
            /// <summary>
            /// Universal kind.
            /// </summary>
            Utc,

            /// <summary>
            /// Utc with only six (not seven) decimal places after seconds.
            /// </summary>
            Utc_Six_Fs,

            /// <summary>
            /// Unspecified kind.
            /// </summary>
            Unspecified,

            /// <summary>
            /// Local kind.
            /// </summary>
            Local,
        }

        private class PatternParserSettings
        {
            public DateTimeKind DateTimeKind { get; set; }

            public DateTimeStyles DateTimeStyles { get; set; }

            public string FormatString { get; set; }

            public DateTimeParseMethod DateTimeParseMethod { get; set; }
        }

        /// <inheritdoc />
        public Type ConfigurationType => null;

        /// <summary>
        /// Map of <see cref="DateTimeKind"/> to a format string used for serialization.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is immutable.")]
        public static readonly IReadOnlyDictionary<DateTimeKind, string> DateTimeKindToFormatStringMap =
            new ReadOnlyDictionary<DateTimeKind, string>(
                new Dictionary<DateTimeKind, string>
                    {
                        { DateTimeKind.Utc, "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'" }, // Need some regexes here, need to also support Utc with "yyyy-MM-dd'T'HH:mm:ss'Z'"
                        { DateTimeKind.Unspecified, "yyyy-MM-dd'T'HH:mm:ss.fffffff''" }, // maybe also fall back on generic DateTime.TryParse if all else fails...
                        { DateTimeKind.Local, "yyyy-MM-dd'T'HH:mm:ss.fffffffK" },
                    });

        /// <summary>
        /// Map of <see cref="DateTimeKind"/> to <see cref="DateTimeStyles"/> used for serialization.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is immutable.")]
        public static readonly IReadOnlyDictionary<DateTimeKind, DateTimeStyles> DateTimeKindToStylesMap = new ReadOnlyDictionary<DateTimeKind, DateTimeStyles>(
            new Dictionary<DateTimeKind, DateTimeStyles>
                {
                    { DateTimeKind.Utc, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal },
                    { DateTimeKind.Unspecified, DateTimeStyles.None },
                    { DateTimeKind.Local, DateTimeStyles.AssumeLocal },
                });

        private static readonly IReadOnlyDictionary<DateTimeKind, DateTimeParseMethod> DateTimeKindToParseMethodMap =
            new ReadOnlyDictionary<DateTimeKind, DateTimeParseMethod>(
                new Dictionary<DateTimeKind, DateTimeParseMethod>
                    {
                        {
                            DateTimeKind.Utc,
                            (parse, formatString, provider, styles) => DateTime.ParseExact(parse, formatString, provider, styles)
                        },
                        {
                            DateTimeKind.Unspecified,
                            (parse, formatString, provider, styles) => DateTime.ParseExact(parse, formatString, provider, styles)
                        },
                        {
                            DateTimeKind.Local,
                            (parse, formatString, provider, styles) => DateTime.ParseExact(parse, formatString, provider, styles)
                        },
                    });

        /// <summary>
        /// Format provider used for serialization.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Is immutable.")]
        private static readonly IFormatProvider FormatProvider = CultureInfo.InvariantCulture;

        private static readonly StringComparison StringComparisonType = StringComparison.InvariantCultureIgnoreCase;

        private delegate DateTime DateTimeParseMethod(string valueToParse, string formatString, IFormatProvider formatProvider, DateTimeStyles styles);

        private static readonly IReadOnlyDictionary<StringEncodingPattern, PatternParserSettings> PatternToSettingsMap =
            new Dictionary<StringEncodingPattern, PatternParserSettings>
            {
                {
                    StringEncodingPattern.Utc, new PatternParserSettings
                    {
                        DateTimeKind = DateTimeKind.Utc,
                        DateTimeStyles = DateTimeKindToStylesMap[DateTimeKind.Utc],
                        FormatString = DateTimeKindToFormatStringMap[DateTimeKind.Utc],
                        DateTimeParseMethod = DateTimeKindToParseMethodMap[DateTimeKind.Utc],
                    }
                },
                {
                    StringEncodingPattern.Utc_Six_Fs, new PatternParserSettings
                    {
                        DateTimeKind = DateTimeKind.Utc,
                        DateTimeStyles = DateTimeKindToStylesMap[DateTimeKind.Utc],
                        FormatString = "yyyy-MM-dd'T'HH:mm:ss.ffffff'Z'",
                        DateTimeParseMethod = DateTimeKindToParseMethodMap[DateTimeKind.Utc],
                    }
                },
                {
                    StringEncodingPattern.Unspecified, new PatternParserSettings
                    {
                        DateTimeKind = DateTimeKind.Unspecified,
                        DateTimeStyles = DateTimeKindToStylesMap[DateTimeKind.Unspecified],
                        FormatString = DateTimeKindToFormatStringMap[DateTimeKind.Unspecified],
                        DateTimeParseMethod = DateTimeKindToParseMethodMap[DateTimeKind.Unspecified],
                    }
                },
                {
                    StringEncodingPattern.Local, new PatternParserSettings
                    {
                        DateTimeKind = DateTimeKind.Local,
                        DateTimeStyles = DateTimeKindToStylesMap[DateTimeKind.Local],
                        FormatString = DateTimeKindToFormatStringMap[DateTimeKind.Local],
                        DateTimeParseMethod = DateTimeKindToParseMethodMap[DateTimeKind.Local],
                    }
                },
            };

        /// <inheritdoc />
        public string SerializeToString(object objectToSerialize)
        {
            var type = (objectToSerialize ?? default(DateTime)).GetType();
            (type == typeof(DateTime) || type == typeof(DateTime?)).Named(Invariant($"typeMustBeDateTimeOrNullableDateTime-{type}")).Must().BeTrue();

            if (objectToSerialize == null)
            {
                /* support for DateTime? */
                return null;
            }

            return SerializeDateTimeToString((DateTime)objectToSerialize);
        }

        /// <summary>
        /// Serializes a <see cref="DateTime" /> to a string.
        /// </summary>
        /// <param name="dateTimeToSerialize"><see cref="DateTime" /> to serialize.</param>
        /// <returns>Serialized string.</returns>
        public static string SerializeDateTimeToString(DateTime dateTimeToSerialize)
        {
            DateTimeKindToFormatStringMap.TryGetValue(dateTimeToSerialize.Kind, out string formatString).Named(
                Invariant($"DidNotFindValueIn{nameof(DateTimeKindToFormatStringMap)}ForKind{dateTimeToSerialize.Kind}")).Must().BeTrue();

            return dateTimeToSerialize.ToString(formatString, FormatProvider);
        }

        /// <inheritdoc />
        public T Deserialize<T>(string serializedString)
        {
            return (T)this.Deserialize(serializedString, typeof(T));
        }

        /// <inheritdoc />
        public object Deserialize(string serializedString, Type type)
        {
            new { type }.Must().NotBeNull();

            (type == typeof(DateTime) || type == typeof(DateTime?)).Named("typeMustBeDateTimeOrNullableDateTime").Must().BeTrue();

            if (serializedString == null)
            {
                /* support for DateTime? */
                return null;
            }

            return DeserializeToDateTime(serializedString);
        }

        /// <summary>
        /// Deserializes to a <see cref="DateTime" />.
        /// </summary>
        /// <param name="serializedString">Serialized string.</param>
        /// <returns><see cref="DateTime" /> from string.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:IdentifiersShouldNotContainTypeNames", MessageId = "string", Justification = "Name/spelling is correct.")]
        public static DateTime DeserializeToDateTime(string serializedString)
        {
            if (string.IsNullOrEmpty(serializedString))
            {
                return default(DateTime);
            }

            try
            {
                var kind = DiscoverKindInSerializedString(serializedString);
                var settings = PatternToSettingsMap[kind];
                settings.Named(Invariant($"{nameof(settings)}For{kind}")).Must().NotBeNull();

                var ret = settings.DateTimeParseMethod(serializedString, settings.FormatString, FormatProvider, settings.DateTimeStyles);
                ret.Kind.Named(Invariant($"ReturnKind-{ret.Kind}-MustBeSameAsDiscovered-{kind}")).Must().BeEqualTo(settings.DateTimeKind);

                return ret;
            }
            catch (Exception ex)
            {
                throw new NaosSerializationException(Invariant($"Failed to deserialize '{serializedString}'"), ex);
            }
        }

#pragma warning disable SA1201 // Elements should appear in the correct order
        private const string MatchLocalRegexPattern = @"[-+]\d\d:\d\d$";
#pragma warning restore SA1201 // Elements should appear in the correct order

        private static readonly Regex MatchLocalRegex = new Regex(MatchLocalRegexPattern, RegexOptions.Compiled);

        private static StringEncodingPattern DiscoverKindInSerializedString(string serializedString)
        {
            new { serializedString }.Must().NotBeNullNorWhiteSpace();

            if (serializedString.EndsWith("Z", StringComparisonType))
            {
                if (serializedString.Length == PatternToSettingsMap[StringEncodingPattern.Utc].FormatString.Length - 4)
                {
                    return StringEncodingPattern.Utc;
                }
                else if (serializedString.Length == PatternToSettingsMap[StringEncodingPattern.Utc_Six_Fs].FormatString.Length - 4)
                {
                    return StringEncodingPattern.Utc_Six_Fs;
                }
                else
                {
                    throw new NotSupportedException(Invariant($"Provided {nameof(serializedString)}: {serializedString} is not a supported UTC format."));
                }
            }

            // ends in timezone info (like -03:00 or +03:00)
            // no ending is considered unspecified kind
            if (MatchLocalRegex.Match(serializedString).Success)
            {
                return StringEncodingPattern.Local;
            }
            else
            {
                return StringEncodingPattern.Unspecified;
            }
        }
    }
}