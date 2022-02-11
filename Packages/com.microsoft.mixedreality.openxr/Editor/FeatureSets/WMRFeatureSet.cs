// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using UnityEditor;
using UnityEditor.XR.OpenXR.Features;

namespace Microsoft.MixedReality.OpenXR.Editor
{
    [OpenXRFeatureSet(
        FeatureSetId = featureSetId,
#if UNITY_OPENXR_1_2_OR_NEWER
        FeatureIds = new string[]
        {
            MixedRealityFeaturePlugin.featureId,
            MotionControllerFeaturePlugin.featureId,
            HandTrackingFeaturePlugin.featureId,
        },
        RequiredFeatureIds = new string[]
        {
            MixedRealityFeaturePlugin.featureId
        },
        DefaultFeatureIds = new string[]
        {
            MixedRealityFeaturePlugin.featureId,
            MotionControllerFeaturePlugin.featureId,
            HandTrackingFeaturePlugin.featureId,
        },
#else
        FeatureIds = new string[]
        {
            MotionControllerFeaturePlugin.featureId,
        },
#endif
        UiName = "Windows Mixed Reality",
        // This will appear as a tooltip for the (?) icon in the loader UI.
        Description = "Enable the full suite of features for Windows Mixed Reality headsets.",
        SupportedBuildTargets = new BuildTargetGroup[] { BuildTargetGroup.Standalone }
    )]
    sealed class WMRFeatureSet
    {
        internal const string featureSetId = "com.microsoft.openxr.featureset.wmr";
    }
}
