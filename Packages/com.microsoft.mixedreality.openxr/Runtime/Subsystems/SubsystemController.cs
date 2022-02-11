// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Microsoft.MixedReality.OpenXR
{
    // https://www.khronos.org/registry/OpenXR/specs/1.0/html/xrspec.html#XrSessionState
    internal enum XrSessionState : int
    {
        Unknown = 0,
        Idle = 1,
        Ready = 2,
        Synchronized = 3,
        Visible = 4,
        Focused = 5,
        Stopping = 6,
        LossPending = 7,
        Exiting = 8,
    };

    internal delegate void OpenXRContextEvent(IOpenXRContext sender, EventArgs args);
    internal delegate void OpenXRContextEvent<TEventArgs>(IOpenXRContext sender, TEventArgs args) where TEventArgs : EventArgs;

    internal interface IOpenXRContext
    {
        ulong Instance { get; }
        ulong SystemId { get; }
        ulong Session { get; }
        bool IsSessionRunning { get; }
        XrSessionState SessionState { get; }
        ulong SceneOriginSpace { get; }

        event OpenXRContextEvent InstanceCreated;       // after instance is created
        event OpenXRContextEvent InstanceDestroying;    // before instance is destroyed
        event OpenXRContextEvent SessionCreated;        // after session is created
        event OpenXRContextEvent SessionDestroying;     // before session is destroyed
        event OpenXRContextEvent SessionBegun;          // after session is begun
        event OpenXRContextEvent SessionEnding;         // before session is ended

        bool IsAnchorExtensionSupported { get; }
    }

    internal interface ISubsystemPlugin
    {
        void CreateSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id)
            where TDescriptor : ISubsystemDescriptor
            where TSubsystem : ISubsystem;
        void StartSubsystem<T>() where T : class, ISubsystem;
        void StopSubsystem<T>() where T : class, ISubsystem;
        void DestroySubsystem<T>() where T : class, ISubsystem;
    }

    internal abstract class SubsystemController
    {
        protected readonly IOpenXRContext Context;

        public SubsystemController(IOpenXRContext context)
        {
            Context = context;
        }

        public virtual void OnSubsystemCreate(ISubsystemPlugin plugin)
        {
        }
        public virtual void OnSubsystemStart(ISubsystemPlugin plugin)
        {
        }
        public virtual void OnSubsystemStop(ISubsystemPlugin plugin)
        {
        }
        public virtual void OnSubsystemDestroy(ISubsystemPlugin plugin)
        {
        }
    }
}
