// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosDateTimeStringSerializer.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
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
        /// <inheritdoc />
        public SerializationKind SerializationKind => SerializationKind.Default;

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
                        { DateTimeKind.Utc, "yyyy-MM-dd'T'HH:mm:ss.fffffff'Z'" },
                        { DateTimeKind.Unspecified, "yyyy-MM-dd'T'HH:mm:ss.fffffff''" },
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

                DateTimeKindToFormatStringMap.TryGetValue(kind, out string formatString)
                    .Named(Invariant($"DidNotFindValueIn{nameof(DateTimeKindToFormatStringMap)}ForKind{kind}")).Must().BeTrue();

                DateTimeKindToStylesMap.TryGetValue(kind, out DateTimeStyles styles)
                    .Named(Invariant($"DidNotFindValueIn{nameof(DateTimeKindToStylesMap)}ForKind{kind}")).Must().BeTrue();

                DateTimeKindToParseMethodMap.TryGetValue(kind, out DateTimeParseMethod parseMethod)
                    .Named(Invariant($"DidNotFindValueIn{nameof(DateTimeKindToParseMethodMap)}ForKind{kind}")).Must().BeTrue();

                var ret = parseMethod(serializedString, formatString, FormatProvider, styles);
                ret.Kind.Named(Invariant($"ReturnKind-{ret.Kind}-MustBeSameAsDiscovered-{kind}")).Must().BeEqualTo(kind);

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

        private static DateTimeKind DiscoverKindInSerializedString(string serializedString)
        {
            new { serializedString }.Must().NotBeNullNorWhiteSpace();

            if (serializedString.EndsWith("Z", StringComparisonType))
            {
                return DateTimeKind.Utc;
            }

            // ends in timezone info (like -03:00 or +03:00)
            // no ending is considered unspecified kind
            if (MatchLocalRegex.Match(serializedString).Success)
            {
                return DateTimeKind.Local;
            }
            else
            {
                return DateTimeKind.Unspecified;
            }
        }
    }
}