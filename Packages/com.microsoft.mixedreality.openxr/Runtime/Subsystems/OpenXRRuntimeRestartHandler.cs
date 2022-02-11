// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using UnityEngine;
using UnityEngine.XR.OpenXR;

namespace Microsoft.MixedReality.OpenXR
{
    internal class OpenXRRuntimeRestartHandler : IDisposable
    {
        private readonly UnityEngine.XR.OpenXR.Features.OpenXRFeature m_feature;
        private readonly bool? m_skipRestart = null;
        private readonly bool? m_skipQuitApp = null;

        public OpenXRRuntimeRestartHandler(UnityEngine.XR.OpenXR.Features.OpenXRFeature feature, bool? skipRestart = null, bool? skipQuitApp = null)
        {
            m_feature = feature;
            m_skipRestart = skipRestart;
            m_skipQuitApp = skipQuitApp;

            Debug.Log($"[OpenXRRuntimeRestartHandler] is created for {m_feature.GetType().Name}, enabled = {m_feature.enabled}.");

            OpenXRRuntime.wantsToRestart += OpenXRRuntime_wantsToRestart;
            OpenXRRuntime.wantsToQuit += OpenXRRuntime_wantsToQuit;
        }

        public void Dispose()
        {
            Debug.Log($"[OpenXRRuntimeRestartHandler] is disposed for {m_feature.GetType().Name}");
            OpenXRRuntime.wantsToQuit -= OpenXRRuntime_wantsToQuit;
            OpenXRRuntime.wantsToRestart -= OpenXRRuntime_wantsToRestart;
        }

        private bool OpenXRRuntime_wantsToQuit()
        {
            if (m_feature.enabled && m_skipQuitApp == true)
            {
                Debug.Log($"[OpenXRRuntimeRestartHandler] {m_feature.GetType().Name} attempts to skip quitting the app after XR session is finished.");
                return false;   // skip quitting application after XR session is finished.
            }
            else
            {
                return true;    // yield the decision to other wantsToQuit event handlers.
            }
        }

        private bool OpenXRRuntime_wantsToRestart()
        {
            if (m_feature.enabled && m_skipRestart == true)
            {
                Debug.Log($"[OpenXRRuntimeRestartHandler] {m_feature.GetType().Name} attempts to skip restarting XR session.");
                return false;  // skip restarting XR session.
            }
            else
            {
                return true;    // yield the decision to other wantsToRestart event handlers.
            }
        }
    }
}