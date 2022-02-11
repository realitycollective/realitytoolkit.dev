// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if UNITY_EDITOR

using Microsoft.MixedReality.OpenXR.Remoting;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using static UnityEngine.XR.OpenXR.Features.OpenXRFeature;


namespace Microsoft.MixedReality.OpenXR
{
    internal class AppRemotingValidator
    {
        internal static void GetValidationChecks(OpenXRFeature feature, List<ValidationRule> results, BuildTargetGroup targetGroup)
        {
            results.Add(new ValidationRule(feature)
            {
                message = "\"Holographic Remoting remote app\" and \"Initialize XR on Startup\" are both enabled. XR initialization should be delayed until a specific IP address is entered.",
                error = true,
                checkPredicate = () =>
                {
                    XRGeneralSettings settings = XRSettingsHelpers.GetOrCreateXRGeneralSettings(targetGroup);
                    return settings != null && !settings.InitManagerOnStart;
                },
                fixIt = () =>
                {
                    XRGeneralSettings settings = XRSettingsHelpers.GetOrCreateXRGeneralSettings(targetGroup);
                    if (settings != null)
                    {
                        settings.InitManagerOnStart = false;
                    }
                }
            });

            results.Add(new ValidationRule(feature)
            {
                message = "\"Holographic Remoting remote app\" and \"Holographic PlayMode Remoting\" are both enabled. PlayMode Remoting must be disabled for App Remoting to work.",
                error = true,
                checkPredicate = () =>
                {
                    FeatureHelpers.RefreshFeatures(BuildTargetGroup.Standalone);
                    PlayModeRemotingPlugin playModeRemotingFeature = OpenXRSettings.Instance.GetFeature<PlayModeRemotingPlugin>();
                    return !playModeRemotingFeature.enabled;
                },
                fixIt = () =>
                {
                    PlayModeRemotingPlugin playModeRemotingFeature = OpenXRSettings.Instance.GetFeature<PlayModeRemotingPlugin>();
                    playModeRemotingFeature.enabled = false;
                }
            });
        }

        // Workaround: This function is internal to unity editor so far, and it's required to properly change feature set
        // Use reflection to invoke UnityEditor.XR.OpenXR.OpenXREditorSettings.Instance.SetFeatureSetSelected function.
        internal static void OpenXREditorSettingsInstanceSetFeatureSetSelected(BuildTargetGroup buildTarget, string featureSetId, bool shouldEnable)
        {
            var assembly = typeof(OpenXRFeatureSetManager).Assembly;
            var openXREditorSettings = assembly.GetType("UnityEditor.XR.OpenXR.OpenXREditorSettings");
            if (openXREditorSettings != null)
            {
                var getInstance = openXREditorSettings.GetMethod("GetInstance", BindingFlags.NonPublic | BindingFlags.Static);
                if (getInstance != null)
                {
                    var instance = getInstance.Invoke(null, null);
                    var setFeatureSetSelected = openXREditorSettings.GetMethod("SetFeatureSetSelected", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (setFeatureSetSelected != null)
                    {
                        object[] parameters = { buildTarget, featureSetId, shouldEnable };
                        setFeatureSetSelected.Invoke(instance, parameters);
                        return;
                    }
                }
            }
            Debug.LogWarning("Cannot find UnityEditor.XR.OpenXR.OpenXREditorSettings.Instance.SetFeatureSetSelected method");
        }
    }
}
#endif