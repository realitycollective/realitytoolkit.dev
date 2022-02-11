// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.OpenXR;

namespace Microsoft.MixedReality.OpenXR
{
    // Used to report reprojection settings for a view configuration to the native layer
    [StructLayout(LayoutKind.Sequential, Pack = 8)]
    internal struct NativeReprojectionSettings
    {
        public ReprojectionMode reprojectionMode;

        public Vector3 reprojectionPlaneOverridePosition;
        public byte reprojectionPlaneOverridePositionHasValue;

        public Vector3 reprojectionPlaneOverrideNormal;
        public byte reprojectionPlaneOverrideNormalHasValue;

        public Vector3 reprojectionPlaneOverrideVelocity;
        public byte reprojectionPlaneOverrideVelocityHasValue;

        internal NativeReprojectionSettings(ReprojectionSettings settings) : this()
        {
            reprojectionMode = settings.ReprojectionMode;

            if (settings.ReprojectionPlaneOverridePosition.HasValue)
            {
                reprojectionPlaneOverridePosition = settings.ReprojectionPlaneOverridePosition.Value;
                reprojectionPlaneOverridePositionHasValue = 1;
            }

            if (settings.ReprojectionPlaneOverrideNormal.HasValue)
            {
                reprojectionPlaneOverrideNormal = settings.ReprojectionPlaneOverrideNormal.Value;
                reprojectionPlaneOverrideNormalHasValue = 1;
            }

            if (settings.ReprojectionPlaneOverrideVelocity.HasValue)
            {
                reprojectionPlaneOverrideVelocity = settings.ReprojectionPlaneOverrideVelocity.Value;
                reprojectionPlaneOverrideVelocityHasValue = 1;
            }
        }
    }

    // Used to provide view configuration information from the native layer
    internal struct OpenXRViewConfiguration
    {
        private NativeLibToken m_nativeLibToken;

        public ViewConfigurationType ViewConfigurationType { get => m_viewConfigurationType; }

        internal bool HasTrackingFlags(NativeSpaceLocationFlags nativeSpaceLocationFlags)
        {
            NativeSpaceLocationFlags flags = NativeLib.GetViewTrackingFlags(m_nativeLibToken, m_viewConfigurationType);
            return (flags & nativeSpaceLocationFlags) == nativeSpaceLocationFlags;
        }

        private ViewConfigurationType m_viewConfigurationType;

        public bool IsActive { get => NativeLib.GetViewConfigurationIsActive(m_nativeLibToken, m_viewConfigurationType); }

        public IReadOnlyList<ReprojectionMode> SupportedReprojectionModes { get => m_supportedReprojectionModes; }
        private ReprojectionMode[] m_supportedReprojectionModes;

        public OpenXRViewConfiguration(NativeLibToken nativeLibToken, ViewConfigurationType viewConfigurationType)
        {
            m_nativeLibToken = nativeLibToken;
            m_viewConfigurationType = viewConfigurationType;

            uint numSupportedModes = NativeLib.GetSupportedReprojectionModesCount(m_nativeLibToken, m_viewConfigurationType);
            m_supportedReprojectionModes = new ReprojectionMode[numSupportedModes];
            NativeLib.GetSupportedReprojectionModes(m_nativeLibToken, m_viewConfigurationType, m_supportedReprojectionModes, numSupportedModes);
        }

        public void SetReprojectionSettings(ReprojectionSettings reprojectionSettings)
        {
            NativeLib.SetReprojectionSettings(m_nativeLibToken, m_viewConfigurationType, new NativeReprojectionSettings(reprojectionSettings));
        }
    }

    internal class OpenXRViewConfigurationSettings : SubsystemController
    {
        private readonly NativeLibToken m_nativeLibToken;
        private List<ViewConfiguration> m_enabledViewConfigurations = new List<ViewConfiguration>();

        public IReadOnlyList<ViewConfiguration> EnabledViewConfigurations => m_enabledViewConfigurations;

        public OpenXRViewConfigurationSettings(NativeLibToken token, IOpenXRContext context) : base(context)
        {
            m_nativeLibToken = token;
            context.SessionBegun += Context_SessionBegun;
            context.SessionEnding += Context_SessionEnding;
        }

        private void Context_SessionBegun(IOpenXRContext sender, EventArgs args)
        {
            // Enabled view configurations are changed when session begin/end
            uint viewConfigurationTypesCount = NativeLib.GetEnabledViewConfigurationTypesCount(m_nativeLibToken);
            ViewConfigurationType[] viewConfigurationTypes = new ViewConfigurationType[viewConfigurationTypesCount];
            NativeLib.GetEnabledViewConfigurationTypes(m_nativeLibToken, viewConfigurationTypes, viewConfigurationTypesCount);

            foreach (ViewConfigurationType viewConfigurationType in viewConfigurationTypes)
            {
                OpenXRViewConfiguration openxrViewConfiguration = new OpenXRViewConfiguration(m_nativeLibToken, viewConfigurationType);
                m_enabledViewConfigurations.Add(new ViewConfiguration(openxrViewConfiguration));
            }
        }

        private void Context_SessionEnding(IOpenXRContext sender, EventArgs args)
        {
            m_enabledViewConfigurations.Clear();
        }
    }
}
