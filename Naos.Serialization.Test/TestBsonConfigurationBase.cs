// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TestBsonConfigurationBase.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.ExceptionServices;

    using FluentAssertions;

    using MongoDB.Bson.Serialization;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;

    using Xunit;

    public static class TestBsonConfigurationBase
    {
        [Fact]
        public static void RegisterClassMapsTypeFullyAutomatic___Type_with_id_member___Works()
        {
            // Arrange
            var type = typeof(TestWithId);
            var configuration = new BsonConfigurationTestAutoConstrainedType(type);
            var expectedMemberNames = BsonConfigurationBase.GetMembersToAutomap(type).Select(_ => _.Name).OrderBy(_ => _).ToList();

            // Act
            var classMap = RunTestCode(configuration);

            // Assert
            classMap.Should().NotBeNull();

            classMap.IdMemberMap.MemberType.Should().Be(typeof(string));
            classMap.IdMemberMap.MemberName.Should().Be(nameof(TestWithId.Id));

            var actualMemberNames = classMap.DeclaredMemberMaps.Select(_ => _.MemberName).OrderBy(_ => _).ToList();
            actualMemberNames.Should().Equal(expectedMemberNames);
        }

        [Fact]
        public static void RegisterClassMapsTypeFullyAutomatic___Type_with_no_constraints___Works()
        {
            // Arrange
            var type = typeof(TestMapping);
            var configuration = new BsonConfigurationTestAutoConstrainedType(type);
            var expectedMemberNames = BsonConfigurationBase.GetMembersToAutomap(type).Select(_ => _.Name).OrderBy(_ => _).ToList();

            // Act
            var classMap = RunTestCode(configuration);

            // Assert
            classMap.Should().NotBeNull();

            classMap.IdMemberMap.Should().BeNull();

            var actualMemberNames = classMap.DeclaredMemberMaps.Select(_ => _.MemberName).OrderBy(_ => _).ToList();
            actualMemberNames.Should().Equal(expectedMemberNames);
        }

        [Fact]
        public static void RegisterClassMapsTypeFullyAutomatic___Type_with_valid_constraints___Works()
        {
            // Arrange
            var constraints = new[] { nameof(TestMapping.GuidProperty), nameof(TestMapping.StringIntMap) };
            var expectedMemberNames = constraints.ToList();
            var configuration = new BsonConfigurationTestAutoConstrainedType(typeof(TestMapping), constraints);

            // Act
            var classMap = RunTestCode(configuration);

            // Assert
            classMap.Should().NotBeNull();

            classMap.IdMemberMap.Should().BeNull();

            var actualMemberNames = classMap.DeclaredMemberMaps.Select(_ => _.MemberName).OrderBy(_ => _).ToList();
            actualMemberNames.Should().Equal(expectedMemberNames);
        }

        [Fact]
        public static void RegisterClassMapsTypeFullyAutomatic___Type_with_invalid_constraints___Throws()
        {
            // Arrange
            var configuration = new BsonConfigurationTestAutoConstrainedType(typeof(TestMapping), new[] { "monkey" });
            Action action = () => RunTestCode(configuration);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be("Value must be false.\r\nParameter name: constrainedPropertyDoesNotExistOnType");
        }

        [Fact]
        public static void RegisterClassMapsTypeFullyAutomatic___All_null_type___Throws()
        {
            // Arrange
            var configuration = new BsonConfigurationTestAutoConstrainedType(null);
            Action action = () => RunTestCode(configuration);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: type");
        }

        [Fact]
        public static void RegisterClassMapsTypeFullyAutomatic___Constrained_null_type___Throws()
        {
            // Arrange
            var configuration = new BsonConfigurationTestAutoConstrainedType(null, new[] { "monkeys" });
            Action action = () => RunTestCode(configuration);

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: type");
        }

        [Fact]
        public static void Configure___With_null_ClassTypesToRegisterAlongWithInheritors___Throws()
        {
            // Arrange
            var config = new TestConfigWithSettableFields { SettableClassTypesToRegisterAlongWithInheritors = null };
            Action action = () => config.Configure();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: ClassTypesToRegisterAlongWithInheritors");
        }

        [Fact]
        public static void Configure___With_null_ClassTypesToRegister___Throws()
        {
            // Arrange
            var config = new TestConfigWithSettableFields { SettableClassTypesToRegister = null };
            Action action = () => config.Configure();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: ClassTypesToRegister");
        }

        [Fact]
        public static void Configure___With_null_DependentConfigurationTypes___Throws()
        {
            // Arrange
            var config = new TestConfigWithSettableFields { SettableDependentConfigurationTypes = null };
            Action action = () => config.Configure();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: DependentConfigurationTypes");
        }

        [Fact]
        public static void Configure___With_null_InterfaceTypesToRegisterImplementationOf___Throws()
        {
            // Arrange
            var config = new TestConfigWithSettableFields { SettableInterfaceTypesToRegisterImplementationOf = null };
            Action action = () => config.Configure();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: InterfaceTypesToRegisterImplementationOf");
        }

        [Fact]
        public static void Configure___With_null_SettableTypesToAutoRegister___Throws()
        {
            // Arrange
            var config = new TestConfigWithSettableFields { SettableTypesToAutoRegister = null };
            Action action = () => config.Configure();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentNullException>();
            exception.Message.Should().Be("\r\nParameter name: TypesToAutoRegister");
        }

        [Fact]
        public static void Configure___With_Invalid_TypeTrackerCollisionStrategy___Throws()
        {
            // Arrange
            var config = new TestConfigWithSettableFields { SettableTypeTrackerCollisionStrategy = TrackerCollisionStrategy.Invalid, SettableClassTypesToRegister = new[] { typeof(string) } };
            Action action = () => config.Configure();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<BsonConfigurationException>();
            exception.Message.Should().Be("Failed to run RegisterClassMap on System.String");
            exception.InnerException.Should().NotBeNull();
            exception.InnerException.Should().BeOfType<ArgumentException>();
            exception.InnerException.Message.Should().Be("Value must not be equal to Invalid.\r\nParameter name: trackerCollisionStrategy");
        }

        [Fact]
        public static void Configure___Same_type_twice_with_Throw_TypeTrackerCollisionStrategy___Throws()
        {
            // Arrange
            var config = new TestConfigWithSettableFields { SettableTypeTrackerCollisionStrategy = TrackerCollisionStrategy.Throw, SettableClassTypesToRegister = new[] { typeof(string), typeof(string) } };
            Action action = () => config.Configure();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<BsonConfigurationException>();
            exception.Message.Should().Be("Failed to run RegisterClassMap on System.String");
            exception.InnerException.Should().NotBeNull();
            exception.InnerException.Should().BeOfType<TrackedObjectCollisionException>();
            exception.InnerException.Message.Should().StartWith("Object of type System.Type with ToString value of 'System.String' is already tracked and TrackerCollisionStrategy is Throw - it was registered by Naos.Serialization.Test.TestConfigWithSettableFields on ");
        }

        [Fact]
        public static void Configure___Same_type_twice_with_Skip_TypeTrackerCollisionStrategy___Only_registers_once()
        {
            // Arrange
            var config = new TestConfigWithSettableFields { SettableTypeTrackerCollisionStrategy = TrackerCollisionStrategy.Skip, SettableClassTypesToRegister = new[] { typeof(string), typeof(string) } };

            // Act
            config.Configure();

            // Assert
            config.AllRegisteredTypes.Should().Contain(typeof(string));
        }

        [Fact]
        public static void Configure___Override_collections___All_types_get_registered_as_expected()
        {
            var expectedTypes = new[]
                                    {
                                        typeof(TestConfigureActionBaseFromSub), typeof(TestConfigureActionInheritedSub),
                                        typeof(TestConfigureActionSingle),
                                        typeof(TestConfigureActionFromInterface),
                                        typeof(TestConfigureActionBaseFromAuto), typeof(TestConfigureActionInheritedAuto), typeof(TestConfigureActionFromAuto),
                                    };
            var config = new TestConfigWithSettableFields
                             {
                                 SettableClassTypesToRegisterAlongWithInheritors = new[] { typeof(TestConfigureActionBaseFromSub) },
                                 SettableClassTypesToRegister = new[] { typeof(TestConfigureActionSingle) },
                                 SettableInterfaceTypesToRegisterImplementationOf = new[] { typeof(ITestConfigureActionFromInterface) },
                                 SettableTypesToAutoRegister = new[] { typeof(ITestConfigureActionFromAuto), typeof(TestConfigureActionBaseFromAuto) },
                                 SettableTypeTrackerCollisionStrategy = TrackerCollisionStrategy.Skip,
                             };

            // Act
            config.Configure();

            // Assert
            config.AllRegisteredTypes.Intersect(expectedTypes).Should().BeEquivalentTo(expectedTypes);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "configs", Justification = "Name/spelling is correct.")]
        [Fact]
        public static void Configure___Provided_with_dependent_configs___Configures_dependents()
        {
            var config = new TestConfigWithSettableFields
                             {
                                 SettableDependentConfigurationTypes = new[] { typeof(CustomThrowsConfig) },
                             };

            Action action = () => config.Configure();

            // Act
            var exception = Record.Exception(action);

            // Assert
            exception.Should().NotBeNull();
            exception.Should().BeOfType<ArgumentException>();
            exception.Message.Should().Be(CustomThrowsConfig.ExceptionMessage);
        }

        [Fact]
        public static void IsSubclassOf___Does_not_return_for_interfaces___Confirmation_test_for_internal_use()
        {
            typeof(TestConfigureActionFromInterface).IsSubclassOf(typeof(ITestConfigureActionFromInterface)).Should().BeFalse();
        }

        [Fact]
        public static void AllTrackedTypeContainers___Post_registration___Returns_fully_loaded_set()
        {
            var testType = typeof(TestTracking);
            var configOne = new TestConfigWithSettableFields
                             {
                                 SettableTypesToAutoRegister = new[] { testType },
                             };

            var configTwo = new TestConfigWithSettableFields
                             {
                                 SettableTypesToAutoRegister = new[] { testType },
                             };

            configOne.Configure();
            configTwo.Configure();

            // Act
            var trackedContainersOne = configOne.AllTrackedTypeContainers;
            var trackedContainersTwo = configTwo.AllTrackedTypeContainers;

            // Assert
            trackedContainersTwo.Should().Equal(trackedContainersOne);
            var trackedContainer = trackedContainersOne.Single(_ => _.TrackedObject == testType);
            trackedContainer.Should().NotBeNull();
            var skipped = trackedContainer.Skipped.Single();
            skipped.Should().NotBeNull();
            skipped.CallingType.Should().Be(configTwo.GetType());
        }

        private static BsonClassMap RunTestCode(object configuration)
        {
            try
            {
                return (BsonClassMap)configuration.GetType().GetMethod("TestCode", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(configuration, null);
            }
            catch (TargetInvocationException ex)
            {
                ExceptionDispatchInfo.Capture(ex.InnerException).Throw();
            }

            throw new NotSupportedException();
        }
    }
}
