// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyBagSerializerTest.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using FakeItEasy;

    using FluentAssertions;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Domain.Extensions;
    using Naos.Serialization.PropertyBag;

    using Spritely.Recipes;

    using Xunit;

    using static System.FormattableString;

    public static class PropertyBagSerializerTest
    {
        [Fact]
        public static void Constructor___Kind_not_default___Throws()
        {
            // Arrange
            Action action = () => new NaosPropertyBagSerializer(SerializationKind.Custom);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be equal to Default.\r\nParameter name: serializationKind");
        }

        [Fact]
        public static void Constructor___Configuration_type_not_null___Throws()
        {
            // Arrange
            Action action = () => new NaosPropertyBagSerializer(SerializationKind.Default, typeof(string));

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be true.\r\nParameter name: Configuration type - System.String - must derive from PropertyBagConfigurationBase.");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "RoundTrip", Justification = "Name/spelling is correct.")]
        [Fact]
        public static void RoundTrip___Supported_scenarios___Works()
        {
            // Arrange
            var serializer = new NaosPropertyBagSerializer();
            var input = new NoConstrutorWithPropertyOfStringIntTimeSpanDateTimeAndIEnumerable
                            {
                                BaseVersion = new InheritTypeDerive(),
                                DeriveVersion = new InheritTypeDerive(),
                                DeriveVersionArray = new[] { new InheritTypeDerive(), },
                                DeriveVersionCollection = new[] { new InheritTypeDerive(), }.ToList(),
                                String = A.Dummy<string>(),
                                Int = A.Dummy<int>(),
                                TimeSpan = A.Dummy<TimeSpan>(),
                                DateTime = A.Dummy<DateTime>(),
                                DateTimeNullable = A.Dummy<DateTime>(),
                                StringAttributed = A.Dummy<string>(),
                                CustomWithAttribute = new CustomWithAttribute(),
                                CustomWithoutInterface = new CustomWithoutInterface(),
                                CustomWithInterface = new CustomWithInterface(),
                                StringArray = new[] { A.Dummy<string>(), },
                                StringCollection = new[] { A.Dummy<string>(), }.ToList(),
                                IntCollection = new[] { A.Dummy<int>(), }.ToList(),
                                TimeSpanCollection = new[] { A.Dummy<TimeSpan>(), }.ToList(),
                                DateTimeCollection = new[] { A.Dummy<DateTime>(), }.ToList(),
                                CustomWithoutInterfaceCollection = new[] { new CustomWithoutInterface(), new CustomWithoutInterface(), }.ToList(),
                                CustomWithInterfaceCollection = new[] { new CustomWithInterface(), new CustomWithInterface(), }.ToList(),
                                CustomElementArray = new[] { new CustomElement(), new CustomElement(), },
                                CustomElementCollection = new[] { new CustomElement(), new CustomElement(), }.ToList(),
                                EnumParseCollection = new[] { EnumParse.Default, EnumParse.Value, },
                                EnumAttributeCollection = new[] { EnumAttribute.Default, EnumAttribute.Value, },
                                EnumAttributePropertyCollection = new[] { EnumAttributeProperty.Default, EnumAttributeProperty.Value, },
                                EnumParse = EnumParse.Value,
                                EnumAttribute = EnumAttribute.Value,
                                EnumAttributeProperty = EnumAttributeProperty.Value,
                            };

            // Act
            var serializedPropertyBag = serializer.SerializeToPropertyBag(input);
            var serializedString = serializer.SerializeToString(input);
            var serializedBytes = serializer.SerializeToBytes(input);

            var actualPropertyBag = serializer.Deserialize<NoConstrutorWithPropertyOfStringIntTimeSpanDateTimeAndIEnumerable>(serializedPropertyBag);
            var actualString = serializer.Deserialize<NoConstrutorWithPropertyOfStringIntTimeSpanDateTimeAndIEnumerable>(serializedString);
            var actualBytes = serializer.Deserialize<NoConstrutorWithPropertyOfStringIntTimeSpanDateTimeAndIEnumerable>(serializedBytes);

            // Assert
            void AssertCorrect(NoConstrutorWithPropertyOfStringIntTimeSpanDateTimeAndIEnumerable actual)
            {
                actual.BaseVersion.Should().NotBeNull();
                actual.DeriveVersion.Should().NotBeNull();
                actual.DeriveVersionArray.Count().Should().Be(input.DeriveVersionArray.Count());
                actual.DeriveVersionCollection.Count().Should().Be(input.DeriveVersionCollection.Count());
                actual.String.Should().Be(input.String);
                actual.Int.Should().Be(input.Int);
                actual.TimeSpan.Should().Be(input.TimeSpan);
                actual.DateTime.Should().Be(input.DateTime);
                actual.DateTimeNullable.Should().Be(input.DateTimeNullable);
                actual.StringAttributed.Should().Be(CustomStringSerializer.CustomReplacementString);
                actual.CustomWithAttribute.Should().NotBeNull();
                actual.CustomWithoutInterface.Should().NotBeNull();
                actual.CustomWithInterface.Should().NotBeNull();
                actual.StringDefault.Should().BeNull();
                actual.IntDefault.Should().Be(default(int));
                actual.TimeSpanDefault.Should().Be(default(TimeSpan));
                actual.DateTimeDefault.Should().Be(default(DateTime));
                actual.DateTimeNullableDefault.Should().BeNull();
                actual.CustomWithoutInterfaceDefault.Should().BeNull();
                actual.CustomWithInterfaceDefault.Should().BeNull();
                actual.StringArray.Should().Equal(input.StringArray);
                actual.StringCollection.Should().Equal(input.StringCollection);
                actual.IntCollection.Should().Equal(input.IntCollection);
                actual.TimeSpanCollection.Should().Equal(input.TimeSpanCollection);
                actual.DateTimeCollection.Should().Equal(input.DateTimeCollection);
                actual.CustomWithoutInterfaceCollection.Should().HaveCount(input.CustomWithoutInterfaceCollection.Count());
                actual.CustomWithInterfaceCollection.Should().HaveCount(input.CustomWithInterfaceCollection.Count());
                actual.CustomElementArray.Length.Should().Be(input.CustomElementArray.Length);
                actual.CustomElementArray.Any(_ => _ == null).Should().BeFalse();
                actual.CustomElementCollection.Count().Should().Be(input.CustomElementCollection.Count());
                actual.CustomElementCollection.Any(_ => _ == null).Should().BeFalse();
                actual.EnumParseCollection.Should().Equal(input.EnumParseCollection);
                actual.EnumAttributeCollection.Should().BeEquivalentTo(Enumerable.Range(0, input.EnumAttributeCollection.Count).Select(_ => EnumAttributeProperty.Replaced));
                actual.EnumAttributePropertyCollection.Should().BeEquivalentTo(Enumerable.Range(0, input.EnumAttributePropertyCollection.Count).Select(_ => EnumAttributeProperty.Replaced));
                actual.EnumDefault.Should().Be(EnumParse.Default);
                actual.EnumParse.Should().Be(input.EnumParse);
                actual.EnumAttribute.Should().Be(EnumAttribute.Replaced);
                actual.EnumAttributeProperty.Should().Be(EnumAttributeProperty.Replaced);
            }

            AssertCorrect(actualPropertyBag);
            AssertCorrect(actualString);
            AssertCorrect(actualBytes);
        }

        [Fact]
        public static void Deserializing_constructors___When_properties_exist___Works()
        {
            // Arrange
            var serializer = new NaosPropertyBagSerializer();
            var input = new ConstructorWithProperties(A.Dummy<string>(), A.Dummy<string>(), A.Dummy<string>());

            // Act
            var serializedString = serializer.SerializeToString(input);
            var actual = serializer.Deserialize<ConstructorWithProperties>(serializedString);

            // Act
            actual.PropertyGetOnly.Should().Be(input.PropertyGetOnly);
            actual.PropertyPrivateSet.Should().Be(input.PropertyPrivateSet);
            actual.PropertyPublicSet.Should().Be(input.PropertyPublicSet);
        }

        [Fact]
        public static void Deserializing_constructors___When_properties_do_not_exist___Throws()
        {
            // Arrange
            var serializer = new NaosPropertyBagSerializer();
            var input = new ConstructorWithoutProperties(A.Dummy<string>(), A.Dummy<string>());

            // Act
            var serializedString = serializer.SerializeToString(input);
            var exception = Record.Exception(() => serializer.Deserialize<ConstructorWithProperties>(serializedString));

            // Act
            exception.Should().NotBeNull();
            exception.Message.Should().Be("Could not find a parameterless constructor or a constructor whose parameter names matched the properties provided; type: Naos.Serialization.Test.PropertyBagSerializerTest+ConstructorWithoutProperties, properties: Property,ToString,GetType.");
        }

        [Fact]
        public static void Configuration___Specifying_type___Works()
        {
            // Arrange
            var configurationType = typeof(PropertyBagConfig);
            var serializer = new NaosPropertyBagSerializer(SerializationKind.Default, configurationType);
            var input = new StringProperty { StringItem = A.Dummy<string>() };

            // Act
            var serializedString = serializer.SerializeToString(input);
            var actual = serializer.Deserialize<StringProperty>(serializedString);

            // Act
            actual.StringItem.Should().Be(CustomStringSerializer.CustomReplacementString);
        }

        private class ConstructorWithProperties
        {
            public string PropertyGetOnly { get; }

            public string PropertyPrivateSet { get; private set; }

            public string PropertyPublicSet { get; set; }

            public ConstructorWithProperties(string propertyGetOnly, string propertyPrivateSet, string propertyPublicSet)
            {
                this.PropertyGetOnly = propertyGetOnly;
                this.PropertyPrivateSet = propertyPrivateSet;
                this.PropertyPublicSet = propertyPublicSet;
            }
        }

        private class ConstructorWithoutProperties
        {
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Here to test reflection.")]
            public string Property { get; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "propertyDoesNotExist", Justification = "Here to test reflection.")]
            public ConstructorWithoutProperties(string property, string propertyDoesNotExist)
            {
                this.Property = property;
            }
        }

        private class PropertyBagConfig : PropertyBagConfigurationBase
        {
            protected override IReadOnlyCollection<Type> DependentConfigurationTypes => new[] { typeof(PropertyBagConfigDepend) };
        }

        private class PropertyBagConfigDepend : PropertyBagConfigurationBase
        {
            protected override IReadOnlyDictionary<Type, IStringSerializeAndDeserialize> CustomTypeToSerializerMappings()
            {
                return new Dictionary<Type, IStringSerializeAndDeserialize> { { typeof(string), new CustomStringSerializer() } };
            }
        }

        private class StringProperty
        {
            public string StringItem { get; set; }
        }

        private class NoConstrutorWithPropertyOfStringIntTimeSpanDateTimeAndIEnumerable
        {
            public InheritTypeBase BaseVersion { get; set; }

            public InheritTypeDerive DeriveVersion { get; set; }

            public InheritTypeDerive[] DeriveVersionArray { get; set; }

            public IReadOnlyCollection<InheritTypeDerive> DeriveVersionCollection { get; set; }

            public string String { get; set; }

            public int Int { get; set; }

            public TimeSpan TimeSpan { get; set; }

            public DateTime DateTime { get; set; }

            public DateTime? DateTimeNullable { get; set; }

            public CustomWithAttribute CustomWithAttribute { get; set; }

            public CustomWithoutInterface CustomWithoutInterface { get; set; }

            public CustomWithInterface CustomWithInterface { get; set; }

            [NaosElementStringSerializer(typeof(CustomElementSerializer))]
            public CustomElement[] CustomElementArray { get; set; }

            [NaosElementStringSerializer(typeof(CustomElementSerializer))]
            public IEnumerable<CustomElement> CustomElementCollection { get; set; }

            [NaosStringSerializer(typeof(CustomStringSerializer))]
            public string StringAttributed { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping.")]
            public string StringDefault { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping.")]
            public int IntDefault { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping.")]
            public TimeSpan TimeSpanDefault { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping.")]
            public DateTime DateTimeDefault { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping.")]
            public DateTime? DateTimeNullableDefault { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping.")]
            public CustomWithoutInterface CustomWithoutInterfaceDefault { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping.")]
            public CustomWithInterface CustomWithInterfaceDefault { get; set; }

            public string[] StringArray { get; set; }

            public IEnumerable<string> StringCollection { get; set; }

            public IReadOnlyCollection<int> IntCollection { get; set; }

            public ICollection<TimeSpan> TimeSpanCollection { get; set; }

            public IList<DateTime> DateTimeCollection { get; set; }

            public IReadOnlyList<CustomWithoutInterface> CustomWithoutInterfaceCollection { get; set; }

            public IEnumerable<CustomWithInterface> CustomWithInterfaceCollection { get; set; }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping.")]
            public EnumParse EnumDefault { get; set; }

            public EnumParse EnumParse { get; set; }

            public EnumAttribute EnumAttribute { get; set; }

            [NaosStringSerializer(typeof(EnumAttributePropertySerializer))]
            public EnumAttributeProperty EnumAttributeProperty { get; set; }

            public IReadOnlyCollection<EnumParse> EnumParseCollection { get; set; }

            public IReadOnlyCollection<EnumAttribute> EnumAttributeCollection { get; set; }

            [NaosElementStringSerializer(typeof(EnumAttributePropertySerializer))]
            public IReadOnlyCollection<EnumAttributeProperty> EnumAttributePropertyCollection { get; set; }
        }

#pragma warning disable SA1602 // Enumeration items should be documented
#pragma warning disable SA1136 // Enum values should be on separate lines
#pragma warning disable SA1502 // Element should not be on a single line
        public enum EnumParse { Default, Value, }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Name/spelling is correct.")]
        [NaosStringSerializer(typeof(EnumAttributeSerializer))]
        public enum EnumAttribute { Default, Value, Replaced, }

        public enum EnumAttributeProperty { Default, Value, Replaced, }
#pragma warning restore SA1502 // Element should not be on a single line
#pragma warning restore SA1136 // Enum values should be on separate lines
#pragma warning restore SA1602 // Enumeration items should be documented

        [NaosStringSerializer(typeof(CustomWithAttributeSerializer))]
        private class CustomWithAttribute
        {
            public override string ToString()
            {
                return this.GetType().Name;
            }
        }

        private class CustomWithAttributeSerializer : IStringSerializeAndDeserialize
        {
            public const string CustomSerializedString = "We have a serializer attribute.";

            public SerializationKind SerializationKind => SerializationKind.Default;

            public Type ConfigurationType => null;

            public string SerializeToString(object objectToSerialize)
            {
                return CustomSerializedString;
            }

            public T Deserialize<T>(string serializedString)
            {
                return (T)this.Deserialize(serializedString, typeof(T));
            }

            public object Deserialize(string serializedString, Type type)
            {
                new { serializedString }.Must().BeEqualTo(CustomSerializedString).OrThrowFirstFailure();
                new { type }.Must().BeEqualTo(typeof(CustomWithAttribute)).OrThrowFirstFailure();

                return new CustomWithAttribute();
            }
        }

        private class CustomStringSerializer : IStringSerializeAndDeserialize
        {
            public const string CustomReplacementString = "We have a string overwriting.";

            public const string CustomSerializedString = "We have a string overwriting serializer attribute.";

            public SerializationKind SerializationKind => SerializationKind.Default;

            public Type ConfigurationType => null;

            public string SerializeToString(object objectToSerialize)
            {
                return CustomSerializedString;
            }

            public T Deserialize<T>(string serializedString)
            {
                return (T)this.Deserialize(serializedString, typeof(T));
            }

            public object Deserialize(string serializedString, Type type)
            {
                new { serializedString }.Must().BeEqualTo(CustomSerializedString).OrThrowFirstFailure();
                new { type }.Must().BeEqualTo(typeof(string)).OrThrowFirstFailure();

                return CustomReplacementString;
            }
        }

        private class CustomElement
        {
        }

        private class CustomElementSerializer : IStringSerializeAndDeserialize
        {
            public const string CustomSerializedString = "Array of these.";

            public SerializationKind SerializationKind => SerializationKind.Default;

            public Type ConfigurationType => null;

            public string SerializeToString(object objectToSerialize)
            {
                return CustomSerializedString;
            }

            public T Deserialize<T>(string serializedString)
            {
                return (T)this.Deserialize(serializedString, typeof(T));
            }

            public object Deserialize(string serializedString, Type type)
            {
                new { serializedString }.Must().BeEqualTo(CustomSerializedString).OrThrowFirstFailure();
                new { type }.Must().BeEqualTo(typeof(CustomElement)).OrThrowFirstFailure();

                return new CustomElement();
            }
        }

        private class EnumAttributeSerializer : IStringSerializeAndDeserialize
        {
            public const string CustomSerializedString = "Enum attribute.";

            public SerializationKind SerializationKind => SerializationKind.Default;

            public Type ConfigurationType => null;

            public string SerializeToString(object objectToSerialize)
            {
                return CustomSerializedString;
            }

            public T Deserialize<T>(string serializedString)
            {
                return (T)this.Deserialize(serializedString, typeof(T));
            }

            public object Deserialize(string serializedString, Type type)
            {
                new { serializedString }.Must().BeEqualTo(CustomSerializedString).OrThrowFirstFailure();
                new { type }.Must().BeEqualTo(typeof(EnumAttribute)).OrThrowFirstFailure();

                return EnumAttribute.Replaced;
            }
        }

        private class EnumAttributePropertySerializer : IStringSerializeAndDeserialize
        {
            public const string CustomSerializedString = "Enum attribute on property.";

            public SerializationKind SerializationKind => SerializationKind.Default;

            public Type ConfigurationType => null;

            public string SerializeToString(object objectToSerialize)
            {
                return CustomSerializedString;
            }

            public T Deserialize<T>(string serializedString)
            {
                return (T)this.Deserialize(serializedString, typeof(T));
            }

            public object Deserialize(string serializedString, Type type)
            {
                new { serializedString }.Must().BeEqualTo(CustomSerializedString).OrThrowFirstFailure();
                new { type }.Must().BeEqualTo(typeof(EnumAttributeProperty)).OrThrowFirstFailure();

                return EnumAttributeProperty.Replaced;
            }
        }

        private class CustomWithoutInterface
        {
            public const string CustomToString = "This is my default tostring.";

            public override string ToString()
            {
                return CustomToString;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping.")]
            public static CustomWithoutInterface Parse(string input)
            {
                new { input }.Must().BeEqualTo(CustomToString).OrThrowFirstFailure();
                return new CustomWithoutInterface();
            }
        }

        private class CustomWithInterface : ISerializeToString
        {
            public const string CustomToString = "This is my default tostring.";

            public const string CustomSerializedString = "This is my serialized string.";

            public override string ToString()
            {
                return CustomToString;
            }

            public string SerializeToString()
            {
                return CustomSerializedString;
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Keeping.")]
            public static CustomWithInterface Parse(string input)
            {
                new { input }.Must().BeEqualTo(CustomSerializedString).OrThrowFirstFailure();
                return new CustomWithInterface();
            }
        }

        [NaosStringSerializer(typeof(InheritTestSerializer))]
        private abstract class InheritTypeBase
        {
            public override string ToString()
            {
                return this.GetType().Name;
            }
        }

        private class InheritTypeDerive : InheritTypeBase
        {
            public override string ToString()
            {
                return this.GetType().Name;
            }
        }

        private class InheritTestSerializer : IStringSerializeAndDeserialize
        {
            public const string CustomSerializedString = "We have a serializer inherited.";

            public SerializationKind SerializationKind => SerializationKind.Default;

            public Type ConfigurationType => null;

            public string SerializeToString(object objectToSerialize)
            {
                return CustomSerializedString;
            }

            public T Deserialize<T>(string serializedString)
            {
                return (T)this.Deserialize(serializedString, typeof(T));
            }

            public object Deserialize(string serializedString, Type type)
            {
                new { serializedString }.Must().BeEqualTo(CustomSerializedString).OrThrowFirstFailure();
                (type == typeof(InheritTypeBase) || type == typeof(InheritTypeDerive)).Must().BeTrue().OrThrowFirstFailure();

                return new InheritTypeDerive();
            }
        }
    }
}
