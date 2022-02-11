// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR.Remoting;
using UnityEditor;

namespace Microsoft.MixedReality.OpenXR.Editor
{
    [CustomEditor(typeof(PlayModeRemotingPlugin))]
    internal class PlayModeHolographicRemotingInspector : UnityEditor.Editor
    {
        private PlayModeRemotingPlugin playModeRemotingPlugin;
        private UnityEditor.Editor remotingSettingsEditor;
        private RemotingSettings remotingSettings;

        private void OnEnable()
        {
            playModeRemotingPlugin = target as PlayModeRemotingPlugin;
        }

        public override void OnInspectorGUI()
        {
            if (playModeRemotingPlugin.RemotingSettings == null || remotingSettings != playModeRemotingPlugin.RemotingSettings)
            {
                playModeRemotingPlugin.EnsureSettingsLoaded();
                remotingSettings = playModeRemotingPlugin.RemotingSettings;
                DestroyImmediate(remotingSettingsEditor);
            }

            if (remotingSettingsEditor == null && remotingSettings != null)
            {
                remotingSettingsEditor = CreateEditor(remotingSettings);
            }

            if (remotingSettingsEditor == null)
            {
                return;
            }

            EditorGUI.BeginDisabledGroup(EditorApplication.isPlaying);
            remotingSettingsEditor.OnInspectorGUI();
        }
    }
}
