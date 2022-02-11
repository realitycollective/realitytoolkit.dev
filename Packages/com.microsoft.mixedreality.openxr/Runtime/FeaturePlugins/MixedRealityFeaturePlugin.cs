// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Generic;
using UnityEngine.XR.OpenXR.Features;
using System;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace Microsoft.MixedReality.OpenXR
{

#if UNITY_EDITOR
    [OpenXRFeature(UiName = featureName,
        BuildTargetGroups = new[] { BuildTargetGroup.Standalone, BuildTargetGroup.WSA },
        Company = "Microsoft",
        Desc = "Supports features on HoloLens 2 and Mixed Reality headsets.",
        DocumentationLink = "https://aka.ms/openxr-unity",
        CustomRuntimeLoaderBuildTargets = null,
        OpenxrExtensionStrings = requestedExtensions,
        Required = true,
        Category = FeatureCategory.Feature,
        FeatureId = featureId,
        Version = "1.2.1")]
#endif
    [NativeLibToken(NativeLibToken = NativeLibToken.HoloLens)]
    internal class MixedRealityFeaturePlugin : OpenXRFeaturePlugin<MixedRealityFeaturePlugin>
    {
        internal const string featureId = "com.microsoft.openxr.feature.hololens";
        internal const string featureName = "Mixed Reality Features";
        internal const string mixedRealityExtensions = ""
            + " XR_MSFT_unbounded_reference_space"
            + " XR_MSFT_spatial_anchor"
            + " XR_MSFT_secondary_view_configuration"
            + " XR_MSFT_first_person_observer"
            + " XR_MSFT_spatial_graph_bridge"
            + " XR_MSFT_perception_anchor_interop"
            + " XR_MSFT_spatial_anchor_persistence"
            + " XR_MSFT_scene_understanding"
            + " XR_MSFT_scene_understanding_serialization"
            + " XR_MSFT_spatial_anchor_export_preview"
            + " XR_MSFT_composition_layer_reprojection";

        internal const string requestedExtensions = "XR_MSFT_holographic_window_attachment" + mixedRealityExtensions;

        private SessionSubsystemController m_sessionSubsystemController;
        private PlaneSubsystemController m_planeSubsystemController;
        private AnchorSubsystemController m_anchorSubsystemController;
        private RaycastSubsystemController m_raycastSubsystemController;
        private MeshSubsystemController m_meshSubsystemController;
        private OpenXRViewConfigurationSettings m_viewConfigurationSettings;

        internal MixedRealityFeaturePlugin()
        {
            AddSubsystemController(m_sessionSubsystemController = new SessionSubsystemController(nativeLibToken, this));
            AddSubsystemController(m_anchorSubsystemController = new AnchorSubsystemController(nativeLibToken, this));
            AddSubsystemController(m_planeSubsystemController = new PlaneSubsystemController(nativeLibToken, this));
            AddSubsystemController(m_raycastSubsystemController = new RaycastSubsystemController(nativeLibToken, this));
            AddSubsystemController(m_meshSubsystemController = new MeshSubsystemController(nativeLibToken, this));
            AddSubsystemController(m_viewConfigurationSettings = new OpenXRViewConfigurationSettings(nativeLibToken, this));
        }

        internal IntPtr TryAcquireSceneCoordinateSystem(Pose poseInScene)
        {
            return NativeLib.TryAcquireSceneCoordinateSystem(nativeLibToken, poseInScene);
        }

        internal IntPtr TryAcquirePerceptionSpatialAnchor(ulong anchorHandle)
        {
            return NativeLib.TryAcquirePerceptionSpatialAnchor(nativeLibToken, anchorHandle);
        }

        internal IntPtr TryAcquirePerceptionSpatialAnchor(Guid trackableId)
        {
            return NativeLib.TryAcquirePerceptionSpatialAnchor(nativeLibToken, trackableId);
        }

        internal Guid TryAcquireXrSpatialAnchor(object perceptionAnchor)
        {
            return NativeLib.TryAcquireXrSpatialAnchor(nativeLibToken, perceptionAnchor);
        }

        internal Guid TryAcquireAndReplaceXrSpatialAnchor(object perceptionAnchor, Guid existingId)
        {
            return NativeLib.TryAcquireAndReplaceXrSpatialAnchor(nativeLibToken, perceptionAnchor, existingId);
        }

        internal IReadOnlyList<ViewConfiguration> EnabledViewConfigurations => m_viewConfigurationSettings.EnabledViewConfigurations;

        internal IntPtr TryGetPerceptionDeviceFactory()
        {
            return NativeLib.TryGetPerceptionDeviceFactory(nativeLibToken, OpenXRFeature.xrGetInstanceProcAddr);
        }

#if UNITY_EDITOR
        protected override void GetValidationChecks(List<ValidationRule> results, BuildTargetGroup targetGroup)
        {
            HoloLensFeatureValidator.GetValidationChecks(this, results, targetGroup);
        }
#endif
    }
}
