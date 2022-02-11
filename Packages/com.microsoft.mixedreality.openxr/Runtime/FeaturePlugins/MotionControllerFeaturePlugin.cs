// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace Microsoft.MixedReality.OpenXR
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Motion Controller Model",
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.WSA },
        Company = "Microsoft",
        Desc = "Supports loading a glTF model for controllers.",
        DocumentationLink = "https://aka.ms/openxr-unity",
        CustomRuntimeLoaderBuildTargets = null,
        OpenxrExtensionStrings = requestedExtensions,
        Required = false,
        Category = FeatureCategory.Feature,
        FeatureId = featureId,
        Version = "1.2.1")]
#endif
    [NativeLibToken(NativeLibToken = NativeLibToken.Controller)]
    internal class MotionControllerFeaturePlugin : OpenXRFeaturePlugin<MotionControllerFeaturePlugin>
    {
        internal const string featureId = "com.microsoft.openxr.feature.controller";
        private const string requestedExtensions = "XR_MSFT_controller_model";
    }
}
