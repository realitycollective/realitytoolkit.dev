// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR;

namespace Microsoft.MixedReality.OpenXR.Remoting
{
    /// <summary>
    /// Provides information and configuration for Holographic Remoting remote app.
    /// </summary>
    public static class AppRemoting
    {
        /// <summary>
        /// Sets the Holographic Remoting remote app configuration for the connection and initializes XR.
        /// Uses XR Management to initialize the default XR loader and start it. If a loader is enabled, XR will be launched.
        /// This method must be run as a coroutine itself, as initializing XR has to happen in a coroutine.
        /// </summary>
        /// <param name="configuration">The set of parameters to use for remoting.</param>
        public static System.Collections.IEnumerator Connect(RemotingConfiguration configuration) => m_appRemotingPlugin.Connect(configuration);


        /// <summary>
        /// Sets the Holographic Remoting remote app configuration for the runtime to listen on and initializes XR.
        /// Uses XR Management to initialize the default XR loader and start it. If a loader is enabled, XR will be launched.
        /// This method must be run as a coroutine itself, as initializing XR has to happen in a coroutine.
        /// </summary>
        /// <param name="configuration">The set of parameters to use for remoting.</param>
        public static System.Collections.IEnumerator Listen(RemotingListenConfiguration listenConfiguration) => m_appRemotingPlugin.Listen(listenConfiguration);

        /// <summary>
        /// Disconnects from the remote and stops the active XR session.
        /// </summary>
        public static void Disconnect() => m_appRemotingPlugin.Disconnect();

        /// <summary>
        /// Provides information on the current remoting session, if one exists.
        /// </summary>
        /// <param name="connectionState">The current connection state of the remote session.</param>
        /// <param name="disconnectReason">If the connection state is disconnected, this helps explain why.</param>
        /// <returns>Whether the information was successfully retrieved.</returns>
        public static bool TryGetConnectionState(out ConnectionState connectionState, out DisconnectReason disconnectReason)
            => m_appRemotingPlugin.TryGetConnectionState(out connectionState, out disconnectReason);
        
        private static readonly AppRemotingPlugin m_appRemotingPlugin = OpenXRSettings.Instance.GetFeature<AppRemotingPlugin>();
    }

    public enum RemotingVideoCodec
    {
        Auto = 0,
        H265,
        H264,
    }

    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct RemotingConfiguration
    {
        /// <summary>
        /// The host name or IP address of the player running in network server mode to connect to.
        /// </summary>
        public string RemoteHostName;

        /// <summary>
        /// The port number of the server's handshake port.
        /// </summary>
        public ushort RemotePort;

        /// <summary>
        /// The max bitrate in Kbps to use for the connection.
        /// </summary>
        public uint MaxBitrateKbps;

        /// <summary>
        /// The video codec to use for the connection.
        /// </summary>
        public RemotingVideoCodec VideoCodec;

        /// <summary>
        /// Enable/disable audio remoting.
        /// </summary>
        public bool EnableAudio;
    }

    [Serializable, StructLayout(LayoutKind.Sequential, Pack = 8)]
    public struct RemotingListenConfiguration
    {
        /// <summary>
        /// The host name or IP address of the player running in network server mode to connect to.
        /// </summary>
        public string ListenInterface;

        /// <summary>
        /// The port number of the server's handshake port.
        /// </summary>
        public ushort HandshakeListenPort;

        /// <summary>
        /// The port number of the server's transport port.
        /// </summary>
        public ushort TransportListenPort;

        /// <summary>
        /// The max bitrate in Kbps to use for the connection.
        /// </summary>
        public uint MaxBitrateKbps;

        /// <summary>
        /// The video codec to use for the connection.
        /// </summary>
        public RemotingVideoCodec VideoCodec;

        /// <summary>
        /// Enable/disable audio remoting.
        /// </summary>
        public bool EnableAudio;
    }

    /// <summary>
    /// Matches OpenXR's XrRemotingConnectionStateMSFT.
    /// </summary>
    public enum ConnectionState
    {
        Disconnected = 0,
        Connecting = 1,
        Connected = 2,
    }

    /// <summary>
    /// Matches OpenXR's XrRemotingDisconnectReasonMSFT.
    /// </summary>
    public enum DisconnectReason
    {
        None = 0,
        Unknown = 1,
        NoServerCertificate = 2,
        HandshakePortBusy = 3,
        HandshakeUnreachable = 4,
        HandshakeConnectionFailed = 5,
        AuthenticationFailed = 6,
        RemotingVersionMismatch = 7,
        IncompatibleTransportProtocols = 8,
        HandshakeFailed = 9,
        TransportPortBusy = 10,
        TransportUnreachable = 11,
        TransportConnectionFailed = 12,
        ProtocolVersionMismatch = 13,
        ProtocolError = 14,
        VideoCodecNotAvailable = 15,
        Canceled = 16,
        ConnectionLost = 17,
        DeviceLost = 18,
        DisconnectRequest = 19,
        HandshakeNetworkUnreachable = 20,
        HandshakeConnectionRefused = 21,
        VideoFormatNotAvailable = 22,
        PeerDisconnectRequest = 23,
        PeerDisconnectTimeout = 24,
        SessionOpenTimeout = 25,
        RemotingHandshakeTimeout = 26,
        InternalError = 27,
        HandshakePermissionDenied = 28,
    }
}
