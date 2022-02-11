// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.OpenXR;

namespace Microsoft.MixedReality.OpenXR
{
    /// <summary>
    /// Represents the set of gestures that may be recognized by a GestureRecognizer.
    /// </summary>
    [Flags]
    public enum GestureSettings
    {
        None = 0,
        Tap = 1,
        DoubleTap = 1 << 1,
        Hold = 1 << 2,
        ManipulationTranslate = 1 << 3,
        NavigationX = 1 << 4,
        NavigationY = 1 << 5,
        NavigationZ = 1 << 6,
        NavigationRailsX = 1 << 7,
        NavigationRailsY = 1 << 8,
        NavigationRailsZ = 1 << 9,
    }

    /// <summary>
    /// Represents the type of gesture recognizer event
    /// </summary>
    public enum GestureEventType
    {
        RecognitionStarted,
        RecognitionEnded,

        Tapped,

        HoldStarted,
        HoldCompleted,
        HoldCanceled,

        ManipulationStarted,
        ManipulationUpdated,
        ManipulationCompleted,
        ManipulationCanceled,

        NavigationStarted,
        NavigationUpdated,
        NavigationCompleted,
        NavigationCanceled,
    }

    /// <summary>
    /// Represents the hand that initiated the gesture
    /// </summary>
    public enum GestureHandedness
    {
        /// <summary>
        /// The gesture is not associated with any specific hand, for example when a gesture is triggered by voice command.
        /// </summary>
        Unspecified,

        /// <summary>
        /// The gesture is initiated by left hand.
        /// </summary>
        Left,

        /// <summary>
        /// The gesture is initiated by right hand.
        /// </summary>
        Right,
    }

    /// <summary>
    /// The data of a gesture event, include the event type, handedness, poses etc.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct GestureEventData
    {
        /// <summary>
        /// Get the type of gesture event.
        /// </summary>
        public GestureEventType EventType => nativeData.eventType;

        /// <summary>
        /// Get which hand triggers this gesture event, or it's not related to specific hand.
        /// </summary>
        public GestureHandedness Handedness => nativeData.handedness;

        /// <summary>
        /// Get the data for tap or double tap event.  
        /// It only has value if and only if the eventType == GestureEventType.Tapped
        /// </summary>
        public TappedEventData? TappedData => nativeData.Get<TappedEventData>(nativeData.tappedData, nativeData.IsTappedEvent());

        /// <summary>
        /// Get the data for manipulation gesture event.
        /// It only has value if and only if the eventType == GestureEventType.ManipulationStarted/Updated/Completed
        /// </summary>
        public ManipulationEventData? ManipulationData => nativeData.Get<ManipulationEventData>(nativeData.manipulationData, nativeData.IsManipulationEvent());

        /// <summary>
        /// Get the data for navigation gesture event.
        /// It only has value if and only if the eventType == GestureEventType.NavigationStarted/Updated/Completed
        /// </summary>
        public NavigationEventData? NavigationData => nativeData.Get<NavigationEventData>(nativeData.navigationData, nativeData.IsNavigationEvent());

        private readonly NativeGestureEventData nativeData;
    }

    /// <summary>
    /// The data of a tap gesture event
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct TappedEventData
    {
        /// <summary>
        /// The tap number represented by this gesture, either 1 or 2.
        /// </summary>
        public uint TapCount;
    }

    /// <summary>
    /// The data of a manipulation gesture event
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct ManipulationEventData
    {
        /// <summary>
        /// Get the relative translation of the hand since the start of a Manipulation gesture.
        /// </summary>
        public Vector3 CumulativeTranslation;
    }

    /// <summary>
    /// The data of a navigation gesture event
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct NavigationEventData
    {
        /// <summary>
        /// Gets whether the navigation gesture the user is performing involves motion on the horizontal axis.
        /// </summary>
        public bool IsNavigatingX => m_directionFlags.HasFlag(NativeDirectionFlags.X);
        /// <summary>
        /// Gets whether the navigation gesture the user is performing involves motion on the vertical axis.
        /// </summary>
        public bool IsNavigatingY => m_directionFlags.HasFlag(NativeDirectionFlags.Y);
        /// <summary>
        /// Gets whether the navigation gesture the user is performing involves motion on the depth axis.
        /// </summary>
        public bool IsNavigatingZ => m_directionFlags.HasFlag(NativeDirectionFlags.Z);

        /// <summary>
        /// Gets the normalized offset of the hand or motion controller within the unit cube for all axes for this Navigation gesture.
        /// </summary>
        /// <remarks> X direction is from left to right.  Y direction is from bottom to top. Z direction is from back to forward.</remarks>
        public Vector3 NormalizedOffset;
        private NativeDirectionFlags m_directionFlags;
    }

    /// <summary>
    /// A gesture recognizer interprets user interactions from hands, motion controllers, and system voice commands 
    /// to surface spatial gesture events, which users target using their gaze or hand's pointing ray.
    /// </summary>
    public class GestureRecognizer : Disposable
    {
        /// <summary>
        /// Create a new GestureRecognizer using the given settings.
        /// </summary>
        /// <remarks>If the given setting is not compatible, the new GestureRecognizer will still be created,
        /// though it won't produce any gesture event.  The setting can be corrected later through the GestureSettings property.</remarks>
        public GestureRecognizer(GestureSettings settings)
        {
            GestureSettings = settings;
        }

        /// <summary>
        /// Set the gesture settings to configure which gestures to recognize.
        /// </summary>
        public GestureSettings GestureSettings
        {
            get { return m_requestedSettings; }
            set
            {
                if (m_requestedSettings != value)
                {
                    m_requestedSettings = value;
                    if (m_gestureSubsystem != null)
                    {
                        m_gestureSubsystem.GestureSettings = value;
                    }
                    else
                    {
                        m_gestureSubsystem = GestureSubsystem.TryCreateGestureSubsystem(value);
                    }
                }
            }
        }

        /// <summary>
        /// Start monitor the user interactions and recognize the configured gestures.
        /// </summary>
        public void Start()
        {
            if (m_gestureSubsystem != null)
            {
                m_gestureSubsystem.Start();
            }
        }

        /// <summary>
        /// Stop monitor the user interactions
        /// </summary>
        public void Stop()
        {
            if (m_gestureSubsystem != null)
            {
                m_gestureSubsystem.Stop();
            }
        }

        /// <summary>
        /// Get the next gesture recognition event data, or return false when the event queue is empty.
        /// </summary>
        /// <param name="eventData">App allocated data struct to receive the data.</param>
        /// <remarks>If function returns false, the content in eventData is undefined and should be avoided.</remarks>
        public bool TryGetNextEvent(ref GestureEventData eventData)
        {
            return m_gestureSubsystem != null && m_gestureSubsystem.TryGetNextEvent(ref eventData);
        }

        /// <summary>
        /// Cancel all pending gestures and reset to initial state.  All events in the queue will be disgarded.
        /// </summary>
        public void CancelPendingGestures()
        {
            if (m_gestureSubsystem != null)
            {
                m_gestureSubsystem.CancelPendingGestures();
            }
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            if (m_gestureSubsystem != null)
            {
                m_gestureSubsystem.Dispose();
            }
        }

        private GestureSubsystem m_gestureSubsystem;
        private GestureSettings m_requestedSettings;
    }
}