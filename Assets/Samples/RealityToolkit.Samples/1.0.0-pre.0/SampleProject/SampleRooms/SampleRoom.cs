// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

namespace RealityToolkit.Samples.SampleProject.SampleRooms
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
        /// The free movment sample showcases smooth free movement.
        /// </summary>
        LocomotionFree,
        /// <summary>
        /// The teleport sample showcases teleport locomotion.
        /// </summary>
        LocomotionTeleport,
        /// <summary>
        /// The player rig samples teaches about collision between the player and the environment.
        /// </summary>
        PlayerRigPhysics,
        /// <summary>
        /// The player rig bounds samples teaches about the player / camera bounds feature.
        /// </summary>
        PlayerRigBounds,
        /// <summary>
        /// The interaction sample room showcases input and interactions with virtual objects.
        /// </summary>
        Interaction
    }
}