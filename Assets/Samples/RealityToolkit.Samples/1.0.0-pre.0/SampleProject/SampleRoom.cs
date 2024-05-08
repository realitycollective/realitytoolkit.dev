// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Samples.SampleProject
{
    /// <summary>
    /// The sample experience consists of multiple rooms. Each room is dedicated
    /// to a specific feature or feature group of the toolkit.
    /// </summary>
    public enum SampleRoom
    {
        /// <summary>
        /// An undefined sample room, likely still in progress and thus should not be available in builds.
        /// </summary>
        Undefined = 0,
        /// <summary>
        /// The locomotion sample room introduces movement and teleportation.
        /// </summary>
        Locomotion,
        /// <summary>
        /// The interaction sample room showcases input and interactions with virtual objects.
        /// </summary>
        Interaction
    }
}