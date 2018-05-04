// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NaosBsonSerializerHelper.cs" company="Naos">
//    Copyright (c) Naos 2017. All Rights Reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Bson
{
    using System;
    using System.IO;

    using MongoDB.Bson;
    using MongoDB.Bson.IO;
    using MongoDB.Bson.Serialization;

    using Naos.Serialization.Domain;

    using Spritely.Recipes;

    /// <summary>
    /// Helper class for using <see cref="BsonSerializer"/>.
    /// </summary>
    public static class NaosBsonSerializerHelper
    {
        /// <summary>
        /// Serializes an object into a byte array.
        /// </summary>
        /// <param name="objectToSerialize">Object to serialize.</param>
        /// <returns>Serialized object into a byte array.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "I like this name...")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It should not actually be an issue.")]
        public static byte[] SerializeToBytes(object objectToSerialize)
        {
            new { objectToSerialize }.Must().NotBeNull().OrThrow();

            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new BsonBinaryWriter(memoryStream))
                {
                    BsonSerializer.Serialize(writer, objectToSerialize.GetType(), objectToSerialize);
                    writer.Close();
                    memoryStream.Close();
                    return memoryStream.ToArray();
                }
            }
        }

        /// <summary>
        /// Serializes an object into a <see cref="BsonDocument"/>.
        /// </summary>
        /// <param name="objectToSerialize">Object to serialize.</param>
        /// <returns>Serialized object into a byte array.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "I like this name...")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It should not actually be an issue.")]
        public static BsonDocument SerializeToDocument(object objectToSerialize)
        {
            new { objectToSerialize }.Must().NotBeNull().OrThrow();

            var document = new BsonDocument();

            using (var writer = new BsonDocumentWriter(document))
            {
                BsonSerializer.Serialize(writer, objectToSerialize.GetType(), objectToSerialize);
                writer.Close();
            }

            return document;
        }

        /// <summary>
        /// Deserializes a <see cref="BsonDocument"/> to an object.
        /// </summary>
        /// <param name="bsonDocumentToDeserialize"><see cref="BsonDocument"/> to deserialize.</param>
        /// <param name="type">Type to deserialize into.</param>
        /// <returns>Serialized object into a byte array.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "I like this name...")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It should not actually be an issue.")]
        public static object DeserializeFromDocument(BsonDocument bsonDocumentToDeserialize, Type type)
        {
            new { bsonDocumentToDeserialize }.Must().NotBeNull().OrThrowFirstFailure();

            object ret;
            if (type == typeof(DynamicTypePlaceholder))
            {
                ret = BsonSerializer.Deserialize<dynamic>(bsonDocumentToDeserialize);
            }
            else
            {
                ret = BsonSerializer.Deserialize(bsonDocumentToDeserialize, type);
            }

            return ret;
        }

        /// <summary>
        /// Deserializes a <see cref="BsonDocument"/> to an object.
        /// </summary>
        /// <param name="bsonDocumentToDeserialize"><see cref="BsonDocument"/> to deserialize.</param>
        /// <typeparam name="T">Type to deserialize into.</typeparam>
        /// <returns>Serialized object into a byte array.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "I like this name...")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It should not actually be an issue.")]
        public static T DeserializeFromDocument<T>(BsonDocument bsonDocumentToDeserialize)
        {
            new { bsonDocumentToDeserialize }.Must().NotBeNull().OrThrowFirstFailure();

            var ret = BsonSerializer.Deserialize<T>(bsonDocumentToDeserialize);

            return ret;
        }

        /// <summary>
        /// Deserializes the byte array into an object.
        /// </summary>
        /// <param name="serializedBytes">Byte array to deserialize.</param>
        /// <typeparam name="T">Type of object to deserialize.</typeparam>
        /// <returns>Deserialized bytes into object of specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "I like this name...")]
        public static T Deserialize<T>(byte[] serializedBytes)
        {
            var ret = Deserialize(serializedBytes, typeof(T));
            return (T)ret;
        }

        /// <summary>
        /// Deserializes the byte array into an object.
        /// </summary>
        /// <param name="serializedBytes">Byte array to deserialize.</param>
        /// <param name="type">Type to deserialize into.</param>
        /// <returns>Deserialized bytes into object of specified type.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:Identifiers should not contain type names", Justification = "I like this name...")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "It should not actually be an issue.")]
        public static object Deserialize(byte[] serializedBytes, Type type)
        {
            new { serializedBytes }.Must().NotBeNull().OrThrow();

            using (var memoryStream = new MemoryStream(serializedBytes))
            {
                using (var reader = new BsonBinaryReader(memoryStream))
                {
                    var ret = BsonSerializer.Deserialize(reader, type);
                    reader.Close();
                    memoryStream.Close();
                    return ret;
                }
            }
        }

        /// <summary>
        /// Converts a string of JSON into a <see cref="BsonDocument"/>.
        /// </summary>
        /// <param name="json">JSON to convert.</param>
        /// <returns>Converted JSON into <see cref="BsonDocument"/>.</returns>
        public static BsonDocument ToBsonDocument(this string json)
        {
            var document = BsonDocument.Parse(json);
            return document;
        }
    }
}