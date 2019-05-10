// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SerializerBase.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using OBeautifulCode.Reflection.Recipes;
    using OBeautifulCode.Validation.Recipes;

    /// <summary>
    /// Represents a serialized object along with a description of the type of the object.
    /// </summary>
    public abstract class SerializerBase : ISerializeAndDeserialize
    {
        /// <summary>
        /// Strategy on how to deal with unregistered types.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Prefer field here.")]
#pragma warning disable SA1401 // Fields should be private
        protected readonly UnregisteredTypeEncounteredStrategy unregisteredTypeEncounteredStrategy;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// The initialized configuration provided or appropriate null implementation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "It is not mutated.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Prefer field here.")]
#pragma warning disable SA1401 // Fields should be private
        protected readonly SerializationConfigurationBase configuration;
#pragma warning restore SA1401 // Fields should be private

        /// <summary>
        /// Initializes a new instance of the <see cref="SerializerBase"/> class.
        /// </summary>
        /// <param name="configurationType">Configuration type to use.</param>
        /// <param name="unregisteredTypeEncounteredStrategy">Optional strategy of what to do when encountering a type that has never been registered; if the type is a <see cref="IImplementNullObjectPattern" /> and value is default then <see cref="UnregisteredTypeEncounteredStrategy.Throw" /> is used.</param>
        protected SerializerBase(
            Type configurationType,
            UnregisteredTypeEncounteredStrategy unregisteredTypeEncounteredStrategy)
        {
            new { configurationType }.Must().NotBeNull();

            this.unregisteredTypeEncounteredStrategy =
                (unregisteredTypeEncounteredStrategy == UnregisteredTypeEncounteredStrategy.Default &&
                 !configurationType.IsAssignableTo(typeof(IImplementNullObjectPattern))) ||
                unregisteredTypeEncounteredStrategy == UnregisteredTypeEncounteredStrategy.Throw
                    ? UnregisteredTypeEncounteredStrategy.Throw
                    : UnregisteredTypeEncounteredStrategy.Attempt;

            this.ConfigurationType = configurationType;
            this.configuration = SerializationConfigurationManager.ConfigureWithReturn<SerializationConfigurationBase>(this.ConfigurationType);
        }

        /// <inheritdoc />
        public Type ConfigurationType { get; private set; }

        /// <inheritdoc />
        public abstract byte[] SerializeToBytes(object objectToSerialize);

        /// <inheritdoc />
        public abstract string SerializeToString(object objectToSerialize);

        /// <inheritdoc />
        public abstract SerializationKind Kind { get; }

        /// <inheritdoc />
        public abstract T Deserialize<T>(string serializedString);

        /// <inheritdoc />
        public abstract object Deserialize(string serializedString, Type type);

        /// <inheritdoc />
        public abstract T Deserialize<T>(byte[] serializedBytes);

        /// <inheritdoc />
        public abstract object Deserialize(byte[] serializedBytes, Type type);
    }
}
