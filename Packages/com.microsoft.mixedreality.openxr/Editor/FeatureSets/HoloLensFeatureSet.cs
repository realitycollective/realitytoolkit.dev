// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using Microsoft.MixedReality.OpenXR.Remoting;
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace Microsoft.MixedReality.OpenXR.Editor
{
    [OpenXRFeatureSet(
        FeatureSetId = featureSetId,
#if UNITY_OPENXR_1_2_OR_NEWER
        FeatureIds = new string[]
        {
            MixedRealityFeaturePlugin.featureId,
            HandTrackingFeaturePlugin.featureId,
            MotionControllerFeaturePlugin.featureId,
        },
        RequiredFeatureIds = new string[]
        {
            MixedRealityFeaturePlugin.featureId,
        },
        DefaultFeatureIds = new string[]
        {
            MixedRealityFeaturePlugin.featureId,
            HandTrackingFeaturePlugin.featureId,
            MotionControllerFeaturePlugin.featureId,
        },
#else
        FeatureIds = new string[]
        {
            MixedRealityFeaturePlugin.featureId,
            HandTrackingFeaturePlugin.featureId,
            EyeGazeInteraction.featureId,
            MicrosoftHandInteraction.featureId,
        },
#endif
        UiName = "Microsoft HoloLens",
        // This will appear as a tooltip for the (?) icon in the loader UI.
        Description = "Enable the full suite of features for Microsoft HoloLens 2.",
        SupportedBuildTargets = new BuildTargetGroup[] { BuildTargetGroup.WSA }
    )]
    sealed class HoloLensFeatureSet
    {
        internal const string featureSetId = "com.microsoft.openxr.featureset.hololens";
    }
}
