// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR;

namespace Microsoft.MixedReality.OpenXR
{
    /// <summary>
    /// The type of mesh to request from the XRMeshSubsystem.
    /// </summary>
    public enum MeshType
    {
        Visual = 1,
        Collider = 2,
    }

    /// <summary>
    /// The level of detail of visual mesh to request from the XRMeshSubsystem.
    /// </summary>
    /// <remarks>Has no effect on the collider mesh.</remarks>
    public enum VisualMeshLevelOfDetail
    {
        Coarse = 1,
        Medium = 2,
        Fine = 3,
        Unlimited = 4,
    }

    /// <summary>
    /// The compute consistency to request from the XRMeshSubsystem.
    /// </summary>
    public enum MeshComputeConsistency {
        /// <summary>
        /// A watertight, globally consistent snapshot, not limited to observable objects in
        /// the scanned regions.
        /// </summary>
        ConsistentSnapshotComplete = 1,
        /// <summary>
        /// A non-watertight snapshot, limited to observable objects in the scanned regions. 
        /// The returned mesh may not be globally optimized for completeness, and therefore
        /// may be returned faster in some scenarios.
        /// </summary>
        ConsistentSnapshotIncompleteFast = 2,
        /// <summary>
        /// A mesh optimized for lower-latency occlusion uses. The returned mesh may not be
        /// globally consistent and might be adjusted piecewise independently.
        /// </summary>
        OcclusionOptimized = 3,
    };

    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct MeshComputeSettings
    {
        private MeshType meshType;
        private VisualMeshLevelOfDetail visualMeshLevelOfDetail;
        private MeshComputeConsistency meshComputeConsistency;

        /// <summary>
        /// Get or set the type of mesh to request from the XRMeshSubsystem.
        /// </summary>
        /// <remarks>Defaults to <see cref="MeshType.Collider"/>.</remarks>
        public MeshType MeshType
        {
            get => meshType != 0 ? meshType : MeshType.Collider;
            set => meshType = value;
        }

        /// <summary>
        /// Get or set the level of detail of visual mesh to request from the XRMeshSubsystem.
        /// </summary>
        /// <remarks>Defaults to <see cref="VisualMeshLevelOfDetail.Coarse"/>.</remarks>
        public VisualMeshLevelOfDetail VisualMeshLevelOfDetail
        {
            get => visualMeshLevelOfDetail != 0 ? visualMeshLevelOfDetail : VisualMeshLevelOfDetail.Coarse;
            set => visualMeshLevelOfDetail = value;
        }

        /// <summary>
        /// Get or set the compute consistency to request from the XRMeshSubsystem.
        /// </summary>
        /// <remarks>Defaults to <see cref="MeshComputeConsistency.OcclusionOptimized"/>.</remarks>
        public MeshComputeConsistency MeshComputeConsistency
        {
            get => meshComputeConsistency != 0 ? meshComputeConsistency : MeshComputeConsistency.OcclusionOptimized;
            set => meshComputeConsistency = value;
        }
    }

    namespace ARSubsystems
    {
        public static class MeshSubsystemExtensions
        {
            /// <summary>
            /// Change the settings for future meshes given by the XRMeshSubsystem.
            /// </summary>
            public static bool TrySetMeshComputeSettings(this XRMeshSubsystem subsystem, MeshComputeSettings settings)
            {
                return InternalMeshSettings.TrySetMeshComputeSettings(settings);
            }
        }
    }

    public static class MeshSettings
    {
        /// <summary>
        /// Change the settings for future meshes given by the XRMeshSubsystem.
        /// </summary>
        public static bool TrySetMeshComputeSettings(MeshComputeSettings settings)
        {
            return InternalMeshSettings.TrySetMeshComputeSettings(settings);
        }
    }
}
