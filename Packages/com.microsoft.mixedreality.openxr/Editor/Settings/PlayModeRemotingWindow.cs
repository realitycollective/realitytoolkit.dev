// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR.Remoting;
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.OpenXR;

namespace Microsoft.MixedReality.OpenXR.Editor
{
    /// <summary>
    /// Represents a standalone window for accessing holographic remoting for play mode settings.
    /// </summary>
    internal class PlayModeRemotingWindow : EditorWindow
    {
        private const string ConnectionInfo = "Clicking the \"Play\" button will connect Unity editor to the Holographic Remoting Player running on above IP address.";

        private static readonly GUIContent FeatureEnabledLabel = EditorGUIUtility.TrTextContent("Disable Holographic Remoting for Play Mode");
        private static readonly GUIContent FeatureDisabledLabel = EditorGUIUtility.TrTextContent("Enable Holographic Remoting for Play Mode");
        private static readonly GUIContent FixLabel = EditorGUIUtility.TrTextContent("Fix");

        private UnityEditor.Editor playModeRemotingPluginEditor;
        private PlayModeRemotingPlugin feature;
        private Vector2 scrollPos;

        /// <summary>
        /// Initializes the Remoting Window class
        /// </summary>
        [MenuItem(PlayModeRemotingValidator.PlayModeRemotingMenuPath)]
        [MenuItem(PlayModeRemotingValidator.PlayModeRemotingMenuPath2)]
        private static void Init()
        {
            GetWindow<PlayModeRemotingWindow>("Holographic Remoting for Play Mode");
        }

        private void OnGUI()
        {
            using (var scroll = new EditorGUILayout.ScrollViewScope(scrollPos))
            {
                scrollPos = scroll.scrollPosition;

                if (feature == null)
                {
                    FeatureHelpers.RefreshFeatures(BuildTargetGroup.Standalone);
                    feature = OpenXRSettings.Instance.GetFeature<PlayModeRemotingPlugin>();
                }

                if (playModeRemotingPluginEditor == null && feature != null)
                {
                    playModeRemotingPluginEditor = UnityEditor.Editor.CreateEditor(feature);
                }

                if (playModeRemotingPluginEditor == null)
                {
                    return;
                }

                EditorGUILayout.Space();
                playModeRemotingPluginEditor.OnInspectorGUI();

                if (feature != null && feature.enabled)
                {
                    EditorGUILayout.Space();
                    bool hasValidSettings = feature.HasValidSettings();
                    bool isLoaderAssigned = PlayModeRemotingValidator.IsLoaderAssigned();
                    bool isXRInitializationEnabled = PlayModeRemotingValidator.IsXRInitializationEnabled();
                    bool areDependenciesEnabled = PlayModeRemotingValidator.AreDependenciesEnabled();

                    if (hasValidSettings && isLoaderAssigned && isXRInitializationEnabled && areDependenciesEnabled)
                    {
                        EditorGUILayout.HelpBox(ConnectionInfo, MessageType.Info);
                    }
                    else
                    {
                        if (!hasValidSettings)
                        {
                            EditorGUILayout.HelpBox(PlayModeRemotingValidator.RemotingNotConfigured, MessageType.Error);
                        }

                        if (!isLoaderAssigned)
                        {
                            EditorGUILayout.HelpBox(PlayModeRemotingValidator.OpenXRLoaderNotAssigned, MessageType.Error);
                            if (GUILayout.Button(FixLabel))
                            {
                                PlayModeRemotingValidator.AssignLoader();
                            }
                        }

                        if (!isXRInitializationEnabled)
                        {
                            EditorGUILayout.HelpBox(PlayModeRemotingValidator.InitializeOnStartupUnchecked, MessageType.Error);
                            if (GUILayout.Button(FixLabel))
                            {
                                PlayModeRemotingValidator.EnableXRInitialization();
                            }
                        }

                        if (!areDependenciesEnabled)
                        {
                            EditorGUILayout.HelpBox(PlayModeRemotingValidator.DependenciesNotEnabled, MessageType.Error);
                            if (GUILayout.Button(FixLabel))
                            {
                                PlayModeRemotingValidator.EnableDependencies();
                            }
                        }
                    }
                }

                // Disable the "enable/disable" button when editor is already playing
                using (new EditorGUI.DisabledScope(EditorApplication.isPlaying == true))
                {
                    EditorGUILayout.Space();
                    if (GUILayout.Button(feature.enabled ? FeatureEnabledLabel : FeatureDisabledLabel))
                    {
                        feature.enabled = !feature.enabled;
                        if (feature.enabled)
                        {
                            // If the user turned on the feature, try to enable dependencies as well.
                            PlayModeRemotingValidator.EnableDependencies();
                        }
                    }
                }
            }
        }
    }
}
