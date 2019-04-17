// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Tracker.cs" company="Naos Project">
//    Copyright (c) Naos Project 2019. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace Naos.Serialization.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Runtime.Serialization;

    using OBeautifulCode.Validation.Recipes;

    using static System.FormattableString;

    /// <summary>
    /// Class that tracks an object and handles collisions.
    /// </summary>
    /// <typeparam name="T">Type of object being tracked.</typeparam>
    public class Tracker<T>
    {
        /// <summary>
        /// Null object version of the tracked operation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1051:DoNotDeclareVisibleInstanceFields", Justification = "Want this externally available.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "It is immutable.")]
        public readonly Action<T> NullTrackedOperation = _ =>
        {
            /* no-op */
        };

        private readonly object syncTracking = new object();
        private readonly IList<TrackedObjectContainer<T>> trackedObjects = new List<TrackedObjectContainer<T>>();
        private readonly TrackedObjectEqualityFunction equalityFunction;

        /// <summary>
        /// Delegate for checking equality of the tracked object.
        /// </summary>
        /// <param name="first">First object.</param>
        /// <param name="second">Second object.</param>
        /// <returns>A value indicating whether or not they are equal.</returns>
        public delegate bool TrackedObjectEqualityFunction(T first, T second);

        /// <summary>
        /// Initializes a new instance of the <see cref="Tracker{T}"/> class.
        /// </summary>
        /// <param name="equalityFunction">Custom equality function to use.</param>
        public Tracker(TrackedObjectEqualityFunction equalityFunction)
        {
            this.equalityFunction = equalityFunction;
        }

        /// <summary>
        /// Run the operation provided using the provided object and track internally.
        /// </summary>
        /// <param name="trackedObject">Object to track and act on.</param>
        /// <param name="trackedOperation">Operation to run using tracked object.</param>
        /// <param name="trackerCollisionStrategy"><see cref="TrackerCollisionStrategy"/> to use.</param>
        /// <param name="callingType">The type to record internally as the operator on the tracked object.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Is validated.")]
        public void RunTrackedOperation(T trackedObject, Action<T> trackedOperation, TrackerCollisionStrategy trackerCollisionStrategy, Type callingType)
        {
            new { trackedObject }.Must().NotBeNull();
            new { trackedOperation }.Must().NotBeNull();
            new { trackerCollisionStrategy }.Must().NotBeEqualTo(TrackerCollisionStrategy.Invalid);

            lock (this.syncTracking)
            {
                var existing = this.trackedObjects.SingleOrDefault(_ => this.equalityFunction(trackedObject, _.TrackedObject));
                if (existing != null)
                {
                    switch (trackerCollisionStrategy)
                    {
                        case TrackerCollisionStrategy.Skip:
                            existing.UpdateSkipped(callingType, DateTime.UtcNow);
                            break;
                        case TrackerCollisionStrategy.Throw:
                            throw new TrackedObjectCollisionException(Invariant($"Object of type {typeof(T)} with {nameof(trackedObject.ToString)} value of '{trackedObject.ToString()}' is already tracked and {nameof(TrackerCollisionStrategy)} is {trackerCollisionStrategy} - it was registered by {existing.CallingType} on {existing.TrackedTimeInUtc}"));
                        default:
                            throw new NotSupportedException();
                    }
                }
                else
                {
                    var stopwatch = Stopwatch.StartNew();
                    trackedOperation(trackedObject);
                    stopwatch.Stop();

                    var container = new TrackedObjectContainer<T>(trackedObject, callingType, DateTime.UtcNow, stopwatch.Elapsed);
                    this.trackedObjects.Add(container);
                }
            }
        }

        /// <summary>
        /// Gets all the currently tracked containers.
        /// </summary>
        /// <returns>Currently tracked containers.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Don't want an active operation like this in a property.")]
        public IReadOnlyCollection<TrackedObjectContainer<T>> GetAllTrackedObjectContainers()
        {
            lock (this.syncTracking)
            {
                // shallow clone the list
                return this.trackedObjects.Select(_ => _).ToList();
            }
        }

        /// <summary>
        /// Gets all the currently tracked objects.
        /// </summary>
        /// <returns>Currently tracked objects.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Don't want an active operation like this in a property.")]
        public IReadOnlyCollection<T> GetAllTrackedObjects()
        {
            lock (this.syncTracking)
            {
                // shallow clone the list
                return this.trackedObjects.Select(_ => _.TrackedObject).ToList();
            }
        }
    }

    /// <summary>
    /// Object to hold information about the tracking and the object.
    /// </summary>
    /// <typeparam name="T">Type of payload that is being tracked.</typeparam>
    public class TrackedObjectContainer<T>
    {
        private readonly List<TrackedSkipContainer> skipped = new List<TrackedSkipContainer>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackedObjectContainer{T}"/> class.
        /// </summary>
        /// <param name="trackedObject">The object that was tracked.</param>
        /// <param name="callingType">Type that operated on the object.</param>
        /// <param name="trackedTimeInUtc">Time of event in UTC.</param>
        /// <param name="timeOfOperation">Elapsed time of the operation.</param>
        public TrackedObjectContainer(T trackedObject, Type callingType, DateTime trackedTimeInUtc, TimeSpan timeOfOperation)
        {
            this.TrackedObject = trackedObject;
            this.CallingType = callingType;
            this.TrackedTimeInUtc = trackedTimeInUtc;
            this.TimeOfOperation = timeOfOperation;
        }

        /// <summary>
        /// Gets the elapsed time of the operation.
        /// </summary>
        public TimeSpan TimeOfOperation { get; private set; }

        /// <summary>
        /// Gets the object that was tracked.
        /// </summary>
        public T TrackedObject { get; private set; }

        /// <summary>
        /// Gets type that operated on the object.
        /// </summary>
        public Type CallingType { get; private set; }

        /// <summary>
        /// Gets time of event in UTC.
        /// </summary>
        public DateTime TrackedTimeInUtc { get; private set; }

        /// <summary>
        /// Gets the skipped type registrations if any.
        /// </summary>
        public IReadOnlyCollection<TrackedSkipContainer> Skipped => this.skipped;

        /// <summary>
        /// Updates the skipped tracking.
        /// </summary>
        /// <param name="callingType">Type that was attempting to operate on the object.</param>
        /// <param name="skippedTimeInUtc">Time of event in UTC.</param>
        public void UpdateSkipped(Type callingType, DateTime skippedTimeInUtc)
        {
            this.skipped.Add(new TrackedSkipContainer(callingType, skippedTimeInUtc));
        }
    }

    /// <summary>
    /// Object to hold information about the tracking skips.
    /// </summary>
    public class TrackedSkipContainer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackedSkipContainer"/> class.
        /// </summary>
        /// <param name="callingType">Type that was attempting to operate on the object.</param>
        /// <param name="skippedTimeInUtc">Time of event in UTC.</param>
        public TrackedSkipContainer(Type callingType, DateTime skippedTimeInUtc)
        {
            this.CallingType = callingType;
            this.SkippedTimeInUtc = skippedTimeInUtc;
        }

        /// <summary>
        /// Gets type that operated on the object.
        /// </summary>
        public Type CallingType { get; private set; }

        /// <summary>
        /// Gets time of event in UTC.
        /// </summary>
        public DateTime SkippedTimeInUtc { get; private set; }
    }

    /// <summary>
    /// Strategies to use when trying to operate on a tracked item.
    /// </summary>
    public enum TrackerCollisionStrategy
    {
        /// <summary>
        /// Invalid default option.
        /// </summary>
        Invalid,

        /// <summary>
        /// Skip the operation.
        /// </summary>
        Skip,

        /// <summary>
        /// Throw an exception as this was unintended.
        /// </summary>
        Throw,
    }

    /// <summary>
    /// Exception for issues in <see cref="Naos.Serialization"/>.
    /// </summary>
    [Serializable]
    public class TrackedObjectCollisionException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackedObjectCollisionException"/> class.
        /// </summary>
        public TrackedObjectCollisionException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackedObjectCollisionException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        public TrackedObjectCollisionException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackedObjectCollisionException"/> class.
        /// </summary>
        /// <param name="message">Message for exception.</param>
        /// <param name="innerException">Inner exception.</param>
        public TrackedObjectCollisionException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrackedObjectCollisionException"/> class.
        /// </summary>
        /// <param name="info">Serialization info.</param>
        /// <param name="context">Reading context.</param>
        protected TrackedObjectCollisionException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}