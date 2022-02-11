// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace Microsoft.MixedReality.OpenXR.Remoting
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Holographic Remoting remote app",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.WSA },
        Company = "Microsoft",
        Desc = "Feature to enable Holographic Remoting remote app.",
        DocumentationLink = "https://aka.ms/openxr-unity-app-remoting",
        OpenxrExtensionStrings = requestedExtensions,
        Category = FeatureCategory.Feature,
        Required = false,
        Priority = -100,    // hookup before other plugins so it affects json before GetProcAddr.
        FeatureId = featureId,
        Version = "1.2.1")]
#endif
    [NativeLibToken(NativeLibToken = NativeLibToken.Remoting)]
    internal class AppRemotingPlugin : OpenXRFeaturePlugin<AppRemotingPlugin>
    {
        private enum RemotingState
        {
            Idle = 0,
            Connect = 1,
            Listen = 2,
        }

        private RemotingConfiguration m_remotingConfiguration;
        private RemotingListenConfiguration m_remotingListenConfiguration;
        private RemotingState m_remotingState;

        internal const string featureId = "com.microsoft.openxr.feature.appremoting";
        private const string requestedExtensions = "XR_MSFT_holographic_remoting";

        private bool m_runtimeOverrideAttempted = false;
        private OpenXRRuntimeRestartHandler m_restartHandler = null;

        private readonly bool m_appRemotingEnabled =
#if UNITY_EDITOR
            false;
#else
            true;
#endif

        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            if (m_appRemotingEnabled && !m_runtimeOverrideAttempted)
            {
                m_runtimeOverrideAttempted = true;
                if (!NativeLib.TryEnableRemotingOverride(nativeLibToken))
                {
                    Debug.LogError($"Failed to enable remoting runtime.");
                }
            }
            return base.HookGetInstanceProcAddr(func);
        }

        private void Awake()
        {
            if (m_appRemotingEnabled && m_restartHandler == null)
            {
                m_restartHandler = new OpenXRRuntimeRestartHandler(this, skipRestart: true, skipQuitApp: true);
            }
        }

        private void OnDestroy()
        {
            if (m_restartHandler != null)
            {
                m_restartHandler.Dispose();
                m_restartHandler = null;
            }
        }

        protected override void OnInstanceDestroy(ulong instance)
        {
            if (m_appRemotingEnabled && m_runtimeOverrideAttempted)
            {
                m_runtimeOverrideAttempted = false;
                NativeLib.ResetRemotingOverride(nativeLibToken);
            }

            Debug.Log($"[AppRemotingPlugin] OnInstanceDestroy, remotingState was {m_remotingState}.");
            if (m_remotingState != RemotingState.Listen)
            {
                m_remotingState = RemotingState.Idle;
            }
            base.OnInstanceDestroy(instance);
        }

        protected override void OnSystemChange(ulong systemId)
        {
            base.OnSystemChange(systemId);

            if (systemId != 0 && m_appRemotingEnabled)
            {
                Debug.Log($"[AppRemotingPlugin] OnSystemChange, systemId = {systemId}, remotingState = {m_remotingState}.");

                if (m_remotingState == RemotingState.Connect)
                {
                    NativeLib.ConnectRemoting(nativeLibToken, m_remotingConfiguration);
                }
                else if (m_remotingState == RemotingState.Listen)
                {
                    NativeLib.ListenRemoting(nativeLibToken, m_remotingListenConfiguration);
                }
            }
        }

        protected override void OnSessionStateChange(int oldState, int newState)
        {
            if (m_appRemotingEnabled && (XrSessionState)newState == XrSessionState.LossPending)
            {
                if (m_remotingState == RemotingState.Connect)
                {
                    Debug.LogError($"[AppRemotingPlugin] Cannot establish a connection to Holographic Remoting Player " +
                        $"on the target with IP Address {m_remotingConfiguration.RemoteHostName}:{m_remotingConfiguration.RemotePort}.");
                }
                else if (m_remotingState == RemotingState.Listen)
                {
                    Debug.Log("[AppRemotingPlugin] Listening to incoming Holographic Remoting connection is interrupted.");
                }
            }
        }

        public System.Collections.IEnumerator Connect(RemotingConfiguration configuration)
        {
            if (m_remotingState == RemotingState.Idle)
            {
                m_remotingConfiguration = configuration;
                m_remotingListenConfiguration = default;
                m_remotingState = RemotingState.Connect;

                if (XRGeneralSettings.Instance.Manager.activeLoader == null)
                {
                    Debug.Log("[AppRemotingPlugin] Connect InitializeLoader");
                    yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
                }

                if (XRGeneralSettings.Instance.Manager.activeLoader != null)
                {
                    Debug.Log("[AppRemotingPlugin] Connect StartSubsystems");
                    XRGeneralSettings.Instance.Manager.StartSubsystems();
                }
            }
            else
            {
                Debug.LogError("Cannot connect when previous connection is still in progress");
            }
        }

        public System.Collections.IEnumerator Listen(RemotingListenConfiguration configuration)
        {
            var defaultWait = new WaitForSeconds(0.5f);

            if (m_remotingState == RemotingState.Idle)
            {
                m_remotingListenConfiguration = configuration;
                m_remotingConfiguration = default;
                m_remotingState = RemotingState.Listen;

                while (m_remotingState == RemotingState.Listen)
                {
                    if (XRGeneralSettings.Instance.Manager.activeLoader == null)
                    {
                        Debug.Log("[AppRemotingPlugin] Listen, InitializeLoader");
                        yield return XRGeneralSettings.Instance.Manager.InitializeLoader();
                    }

                    if (XRGeneralSettings.Instance.Manager.activeLoader != null)
                    {
                        Debug.Log("[AppRemotingPlugin] Listen, StartSubsystems");
                        XRGeneralSettings.Instance.Manager.StartSubsystems();
                        yield return defaultWait;
                    }

                    while (true)
                    {
                        if (!TryGetConnectionState(out ConnectionState connectionState, out _) ||
                            connectionState == ConnectionState.Disconnected)
                        {
                            Debug.Log("[AppRemotingPlugin] Listen, After disconnection, Stop XR Loader.");
                            StopXrLoader();
                            break;  // If disconnected, stop XR session and try to restart.
                        }
                        if (XRGeneralSettings.Instance.Manager.activeLoader == null)
                        {
                            break;  // if XR loader is already stopped, try to restart.
                        }
                        yield return defaultWait;
                    }

                    Debug.Log("[AppRemotingPlugin] Listen, Try restart XR session");
                    yield return defaultWait;
                }
            }
            else
            {
                Debug.LogError("[AppRemotingPlugin] Cannot listen when previous connection is still in progress");
            }
        }

        public void Disconnect()
        {
            if (OpenXR.OpenXRContext.Current.Instance != 0)
            {
                NativeLib.DisconnectRemoting(NativeLibToken.Remoting);
            }

            StopXrLoader();

            m_remotingState = RemotingState.Idle;
        }

        private void StopXrLoader()
        {
            if (XRGeneralSettings.Instance.Manager.activeLoader != null)
            {
                XRGeneralSettings.Instance.Manager.StopSubsystems();
                Debug.Log("[AppRemotingPlugin] Disconnect StopSubsystems");

                if (XRGeneralSettings.Instance.Manager.isInitializationComplete)
                {
                    XRGeneralSettings.Instance.Manager.DeinitializeLoader();
                    Debug.Log("[AppRemotingPlugin] Disconnect DeinitializeLoader");
                }
            }

        }

        public bool TryGetConnectionState(out ConnectionState connectionState, out DisconnectReason disconnectReason)
        {
            return NativeLib.TryGetRemotingConnectionState(NativeLibToken.Remoting, out connectionState, out disconnectReason);
        }

#if UNITY_EDITOR
        protected override void GetValidationChecks(System.Collections.Generic.List<ValidationRule> results, BuildTargetGroup targetGroup)
        {
            AppRemotingValidator.GetValidationChecks(this, results, targetGroup);
        }
#endif
    }
}
