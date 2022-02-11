// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using System.IO;
using UnityEngine.XR.OpenXR.Features;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace Microsoft.MixedReality.OpenXR.Remoting
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Holographic Remoting for Play Mode",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone },
        Company = "Microsoft",
        Desc = "Holographic Remoting for Play Mode in Unity editor",
        DocumentationLink = "https://aka.ms/openxr-unity-editor-remoting",
        OpenxrExtensionStrings = requestedExtensions,
        Category = FeatureCategory.Feature,
        Required = false,
        Priority = -100,    // hookup before other plugins so it affects json before GetProcAddr.
        FeatureId = featureId,
        Version = "1.2.1")]
#endif
    [NativeLibToken(NativeLibToken = NativeLibToken.Remoting)]
    internal class PlayModeRemotingPlugin : OpenXRFeaturePlugin<PlayModeRemotingPlugin>, ISerializationCallbackReceiver
    {
        internal const string featureId = "com.microsoft.openxr.feature.playmoderemoting";
        private const string requestedExtensions = "XR_MSFT_holographic_remoting";
        private const string SettingsFileName = "MixedRealityOpenXRRemotingSettings.asset";
        private static string UserSettingsFolder => Path.Combine(Application.dataPath, "..", "UserSettings");
        private static string SettingsAssetPath => Path.Combine(UserSettingsFolder, SettingsFileName);

        [SerializeField, Tooltip("The host name or IP address of the player running in network server mode to connect to."), Obsolete("Use the remotingSettings values instead")]
        private string m_remoteHostName = string.Empty;

        [SerializeField, Tooltip("The port number of the server's handshake port."), Obsolete("Use the remotingSettings values instead")]
        private ushort m_remoteHostPort = 8265;

        [SerializeField, Tooltip("The max bitrate in Kbps to use for the connection."), Obsolete("Use the remotingSettings values instead")]
        private uint m_maxBitrate = 20000;

        [SerializeField, Tooltip("The video codec to use for the connection."), Obsolete("Use the remotingSettings values instead")]
        private RemotingVideoCodec m_videoCodec = RemotingVideoCodec.Auto;

        [SerializeField, Tooltip("Enable/disable audio remoting."), Obsolete("Use the remotingSettings values instead")]
        private bool m_enableAudio = false;

        private bool m_runtimeOverrideAttempted = false;

        private readonly bool m_playModeRemotingEnabled =
#if UNITY_EDITOR
            true;
#else
            false;
#endif

        protected override IntPtr HookGetInstanceProcAddr(IntPtr func)
        {
            if (m_playModeRemotingEnabled && !m_runtimeOverrideAttempted)
            {
                m_runtimeOverrideAttempted = true;
                if (!NativeLib.TryEnableRemotingOverride(nativeLibToken))
                {
                    Debug.LogError($"Failed to enable remoting runtime.");
                }
            }
            return base.HookGetInstanceProcAddr(func);
        }

        protected override void OnInstanceDestroy(ulong instance)
        {
            if (m_playModeRemotingEnabled && m_runtimeOverrideAttempted)
            {
                m_runtimeOverrideAttempted = false;
                NativeLib.ResetRemotingOverride(nativeLibToken);
            }
            base.OnInstanceDestroy(instance);
        }

        protected override void OnSystemChange(ulong systemId)
        {
            base.OnSystemChange(systemId);
            EnsureSettingsLoaded();

            if (systemId != 0 && m_playModeRemotingEnabled)
            {
                NativeLib.ConnectRemoting(nativeLibToken, new RemotingConfiguration
                {
                    RemoteHostName = RemotingSettings.RemoteHostName,
                    RemotePort = RemotingSettings.RemoteHostPort,
                    MaxBitrateKbps = RemotingSettings.MaxBitrate,
                    VideoCodec = RemotingSettings.VideoCodec,
                    EnableAudio = RemotingSettings.EnableAudio
                });
            }
        }

        protected override void OnSessionStateChange(int oldState, int newState)
        {
            if (m_playModeRemotingEnabled && (XrSessionState)newState == XrSessionState.LossPending)
            {
                _ = NativeLib.TryGetRemotingConnectionState(NativeLibToken.Remoting, out ConnectionState connectionState, out DisconnectReason disconnectReason);

                Debug.LogError($"Play to Holographic Remoting is disconnected unexpectedly. " +
                    $"Host address = {RemotingSettings.RemoteHostName}:{RemotingSettings.RemoteHostPort}. " +
                    $"ConnectionState = {connectionState}, DisconnectReason = {disconnectReason}. ");

#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#endif
            }
        }

        internal bool HasValidSettings()
        {
            if (RemotingSettings == null)
            {
                EnsureSettingsLoaded();
            }
            return RemotingSettings != null && !string.IsNullOrEmpty(RemotingSettings.RemoteHostName);
        }

        internal RemotingSettings RemotingSettings { get; private set; } = null;

        internal void EnsureSettingsLoaded()
        {
            if (RemotingSettings == null)
            {
                // If this file doesn't yet exist, create it and port from the old values.
                RemotingSettings = CreateInstance<RemotingSettings>();

#pragma warning disable CS0618 // to use the obsolete fields to port to the new asset file
                RemotingSettings.RemoteHostName = m_remoteHostName;
                RemotingSettings.RemoteHostPort = m_remoteHostPort;
                RemotingSettings.MaxBitrate = m_maxBitrate;
                RemotingSettings.VideoCodec = m_videoCodec;
                RemotingSettings.EnableAudio = m_enableAudio;
#pragma warning restore CS0618

                if (File.Exists(SettingsAssetPath))
                {
                    using (StreamReader settingsReader = new StreamReader(SettingsAssetPath))
                    {
                        JsonUtility.FromJsonOverwrite(settingsReader.ReadToEnd(), RemotingSettings);
                    }
                }
            }
        }

        private void SaveSettings()
        {
            if (RemotingSettings == null)
            {
                return;
            }

            if (!Directory.Exists(UserSettingsFolder))
            {
                Directory.CreateDirectory(UserSettingsFolder);
            }

            using (StreamWriter settingsWriter = new StreamWriter(SettingsAssetPath))
            {
                settingsWriter.Write(JsonUtility.ToJson(RemotingSettings, true));
            }
        }

#if UNITY_EDITOR
        protected override void GetValidationChecks(System.Collections.Generic.List<ValidationRule> results, BuildTargetGroup targetGroup)
        {
            PlayModeRemotingValidator.GetValidationChecks(this, results);
        }
#endif

        void ISerializationCallbackReceiver.OnBeforeSerialize() => SaveSettings();

        void ISerializationCallbackReceiver.OnAfterDeserialize() { } // Can't call EnsureSettingsLoaded() here, since Application.dataPath can't be accessed during deserialization
    }

    internal class RemotingSettings : ScriptableObject
    {
        [field: SerializeField, Tooltip("The host name or IP address of the player running in network server mode to connect to.")]
        public string RemoteHostName { get; set; } = string.Empty;

        [field: SerializeField, Tooltip("The port number of the server's handshake port.")]
        public ushort RemoteHostPort { get; set; } = 8265;

        [field: SerializeField, Tooltip("The max bitrate in Kbps to use for the connection.")]
        public uint MaxBitrate { get; set; } = 20000;

        [field: SerializeField, Tooltip("The video codec to use for the connection.")]
        public RemotingVideoCodec VideoCodec { get; set; } = RemotingVideoCodec.Auto;

        [field: SerializeField, Tooltip("Enable/disable audio remoting.")]
        public bool EnableAudio { get; set; } = false;
    }
}
