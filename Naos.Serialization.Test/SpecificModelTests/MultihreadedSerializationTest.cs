// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MultiThreadedSerializationTest.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Test
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using FakeItEasy;

    using FluentAssertions;

    using Naos.Serialization.Bson;
    using Naos.Serialization.Domain;
    using Naos.Serialization.Json;
    using OBeautifulCode.Type;
    using Xunit;

    using static System.FormattableString;

    public static class MultiThreadedSerializationTest
    {
        [Fact(Skip = "Long running")]
        public static void TestDictionaryMixedKeyValues()
        {
            var tasks = Enumerable.Range(1, 10000).Select(_ => A.Dummy<TestDictionaryMixedKeyValues>())
                .Select(_ => new Task(() =>
                {
                    var serializer = new NaosJsonSerializer(typeof(GenericJsonConfiguration<TestDictionaryMixedKeyValues>));
                    serializer.SerializeToString(_);
                })).ToArray();
            Parallel.ForEach(tasks, _ => _.Start());
            Task.WaitAll(tasks);
        }

        [Fact(Skip = "Long running")]
        public static void TestBaseOther()
        {
            var tasks = Enumerable.Range(1, 10000).Select(_ => A.Dummy<TestBase>())
                .Select(_ => new Task(() =>
                {
                    var serializer = new NaosJsonSerializer(typeof(GenericJsonConfiguration<TestBase>));
                    serializer.SerializeToString(_);
                })).ToArray();
            Parallel.ForEach(tasks, _ => _.Start());
            Task.WaitAll(tasks);
        }

        [Fact(Skip = "Long running")]
        public static void TestBase()
        {
            var serializer = new NaosJsonSerializer(typeof(GenericJsonConfiguration<TestBase>));
            var tasks = Enumerable.Range(1, 100).Select(_ => A.Dummy<TestBase>())
                .Select(_ => new Task(() => serializer.SerializeToString(_))).ToArray();
            Parallel.ForEach(tasks, _ => _.Start());
            Task.WaitAll(tasks);
        }
    }
}
