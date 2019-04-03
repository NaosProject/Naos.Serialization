// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultipleConstructorScenarioTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Security;
    using FakeItEasy;
    using FluentAssertions;
    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using Newtonsoft.Json;
    using Xunit;

    public static class MultipleConstructorScenarioTest
    {
        [Fact]
        public static void Presence_of_default_constructor__Is_ignored__When_there_is_some_more_specific_one()
        {
            // Arrange
            var expectedParameterless = new MultipleConstructorTestModel();
            var expectedParameter = new MultipleConstructorTestModel(A.Dummy<string>());

            // Act & Assert
            expectedParameterless.RoundtripSerializeWithEquatableAssertion();
            expectedParameter.RoundtripSerializeWithEquatableAssertion();
        }
    }

    public class MultipleConstructorTestModel : IEquatable<MultipleConstructorTestModel>
    {
        private const string ParameterlessConstructorValue = "OhNos";

        private const string ParameterConstructorValue = "OhYes";

        public MultipleConstructorTestModel()
        {
            this.SomeValue = ParameterlessConstructorValue;
        }

        [JsonConstructor]
        public MultipleConstructorTestModel(string someValue)
        {
            this.SomeValue = someValue; // ParameterConstructorValue;
        }

        public string SomeValue { get; private set; }

        public bool Equals(MultipleConstructorTestModel other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            return string.Equals(this.SomeValue, other.SomeValue);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return this.Equals((MultipleConstructorTestModel)obj);
        }

        public override int GetHashCode()
        {
            return this.SomeValue != null ? this.SomeValue.GetHashCode() : 0;
        }
    }
}
