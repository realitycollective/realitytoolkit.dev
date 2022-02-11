// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;

namespace Microsoft.MixedReality.OpenXR
{
    internal class PluginInitializer
    {
        private static bool m_initialized = false;
        internal static void InitializePlugin()
        {
            if (!m_initialized)
            {
                m_initialized = true;
                NativeLib.SetPluginEnvironment(PluginEnvironment.unityVersion, Application.unityVersion);
                NativeLib.SetPluginEnvironment(PluginEnvironment.openXRPluginVersion, OpenXRRuntime.pluginVersion);
                NativeLib.SetPluginEnvironment(PluginEnvironment.mrOpenXRPluginVersion, typeof(OpenXRContext).Assembly.GetName().Version.ToString());
                NativeLib.InitializePlugin();
            }
        }
    }

    internal abstract class OpenXRFeaturePlugin<TPlugin>
        : OpenXRFeature, IOpenXRContext, ISubsystemPlugin where TPlugin : OpenXRFeaturePlugin<TPlugin>
    {
        internal static readonly NativeLibToken nativeLibToken;

        private List<SubsystemController> m_subsystemControllers = new List<SubsystemController>();
        public ulong Instance { get; private set; }
        public ulong SystemId { get; private set; }
        public ulong Session { get; private set; }
        public bool IsSessionRunning { get; private set; }
        public XrSessionState SessionState { get; private set; }
        public ulong SceneOriginSpace { get; private set; }

        public event OpenXRContextEvent InstanceCreated;       // after instance is created
        public event OpenXRContextEvent InstanceDestroying;    // before instance is destroyed
        public event OpenXRContextEvent SessionCreated;        // after session is created
        public event OpenXRContextEvent SessionDestroying;     // before session is destroyed
        public event OpenXRContextEvent SessionBegun;          // after session is begun
        public event OpenXRContextEvent SessionEnding;         // before session is ended

        public bool IsAnchorExtensionSupported { get; private set; }

        public IntPtr GetInstanceProcAddr(string functionName)
        {
            return Instance == 0
                ? IntPtr.Zero
                : NativeLib.GetInstanceProcAddr(Instance, OpenXRFeature.xrGetInstanceProcAddr, functionName);
        }

        static OpenXRFeaturePlugin()
        {
            NativeLibTokenAttribute attribute = typeof(TPlugin).GetCustomAttributes(
                typeof(NativeLibTokenAttribute), inherit: false).FirstOrDefault() as NativeLibTokenAttribute;
            if (attribute == null)
            {
                Debug.LogError($"{typeof(TPlugin).Name} lacks NativeLibToken attribute");
                return;
            }
            nativeLibToken = attribute.NativeLibToken;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            PluginInitializer.InitializePlugin();
        }

        protected void AddSubsystemController(SubsystemController subsystemController)
        {
            m_subsystemControllers.Add(subsystemController);
        }

        private bool IsExtensionEnabled(string extensionName, uint minimumRevision = 1)
        {
            if (!OpenXRRuntime.IsExtensionEnabled(extensionName))
                return false;

            return OpenXRRuntime.GetExtensionVersion(extensionName) >= minimumRevision;
        }

        protected override void OnSubsystemCreate()
        {
            m_subsystemControllers.ForEach(controller => controller.OnSubsystemCreate(this));
        }

        protected override void OnSubsystemStart()
        {
            NativeLib.OnSubsystemsStarting(nativeLibToken);
            m_subsystemControllers.ForEach(controller => controller.OnSubsystemStart(this));
        }

        protected override void OnSubsystemStop()
        {
            m_subsystemControllers.ForEach(controller => controller.OnSubsystemStop(this));
            NativeLib.OnSubsystemsStopped(nativeLibToken);
        }

        protected override void OnSubsystemDestroy()
        {
            m_subsystemControllers.ForEach(controller => controller.OnSubsystemDestroy(this));
        }

        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            return NativeLib.HookGetInstanceProcAddr(nativeLibToken, func);
        }

        protected override bool OnInstanceCreate(ulong instance)
        {
            Instance = instance;
            NativeLib.SetXrInstance(nativeLibToken, instance);

            IsAnchorExtensionSupported = IsExtensionEnabled("XR_MSFT_spatial_anchor");

            if (InstanceCreated != null)
            {
                InstanceCreated(this, EventArgs.Empty);
            }
            return true;
        }

        protected override void OnInstanceDestroy(ulong instance)
        {
            if (InstanceDestroying != null)
            {
                InstanceDestroying(this, EventArgs.Empty);
            }

            SystemId = 0;
            NativeLib.SetXrSystemId(nativeLibToken, 0);

            Instance = 0;
            NativeLib.SetXrInstance(nativeLibToken, 0);
        }

        protected override void OnSystemChange(ulong systemId)
        {
            SystemId = systemId;
            NativeLib.SetXrSystemId(nativeLibToken, systemId);
        }

        protected override void OnSessionCreate(ulong session)
        {
            Session = session;
            NativeLib.SetXrSession(nativeLibToken, session);

            if (SessionCreated != null)
            {
                SessionCreated(this, EventArgs.Empty);
            }

            string appMode = "undefined";

#if UNITY_EDITOR
            appMode = "PlayMode";
#else
            appMode = "AppMode";
#endif
            NativeLib.SetPluginEnvironment(PluginEnvironment.appName, Application.productName);
            NativeLib.SetPluginEnvironment(PluginEnvironment.appVersion, Application.version);
            NativeLib.SetPluginEnvironment(PluginEnvironment.appMode, appMode);
            NativeLib.SetPluginEnvironment(PluginEnvironment.openXRRuntimeName, OpenXRRuntime.name);
            NativeLib.SetPluginEnvironment(PluginEnvironment.openXRRuntimeVersion, OpenXRRuntime.version);
            NativeLib.SetPluginEnvironment(PluginEnvironment.apiVersion, OpenXRRuntime.apiVersion);
        }

        protected override void OnSessionBegin(ulong session)
        {
            NativeLib.SetXrSessionRunning(nativeLibToken, true);

            if (SessionBegun != null)
            {
                SessionBegun(this, EventArgs.Empty);
            }
            IsSessionRunning = true;
        }

        protected override void OnSessionStateChange(int oldState, int newState)
        {
            SessionState = (XrSessionState)newState;
            NativeLib.SetSessionState(nativeLibToken, (uint)newState);
        }

        protected override void OnSessionEnd(ulong session)
        {
            IsSessionRunning = false;
            if (SessionEnding != null)
            {
                SessionEnding(this, EventArgs.Empty);
            }
            NativeLib.SetXrSessionRunning(nativeLibToken, false);
        }

        protected override void OnSessionDestroy(ulong session)
        {
            if (SessionDestroying != null)
            {
                SessionDestroying(this, EventArgs.Empty);
            }
            Session = 0;
            NativeLib.SetXrSession(nativeLibToken, 0);
        }

        protected override void OnAppSpaceChange(ulong sceneOriginSpace)
        {
            SceneOriginSpace = sceneOriginSpace;
            NativeLib.SetSceneOriginSpace(nativeLibToken, sceneOriginSpace);
        }

        void ISubsystemPlugin.CreateSubsystem<TDescriptor, TSubsystem>(List<TDescriptor> descriptors, string id) =>
            base.CreateSubsystem<TDescriptor, TSubsystem>(descriptors, id);

        void ISubsystemPlugin.StartSubsystem<T>() => base.StartSubsystem<T>();

        void ISubsystemPlugin.StopSubsystem<T>() => base.StopSubsystem<T>();

        void ISubsystemPlugin.DestroySubsystem<T>() => base.DestroySubsystem<T>();
    }

    //Must match PluginEnvironment in PluginEnvironment.h
    enum PluginEnvironment
    {
        unityVersion = 1 << 0,
        openXRPluginVersion = 1 << 1,
        mrOpenXRPluginVersion = 1 << 2,
        graphicsAPI = 1 << 3,
        sessionCreationResult = 1 << 4,
        appName = 1 << 5,
        appVersion = 1 << 6,
        appMode = 1 << 7,
        openXRRuntimeName = 1 << 8,
        openXRRuntimeVersion = 1 << 9,
        apiVersion = 1 << 10
    };
}