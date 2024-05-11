// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;
using RealityToolkit.Samples.SampleProject.SampleRooms;

namespace RealityToolkit.Samples.SampleProject
{
    public delegate void OnRoomDelegate(SampleRoom room);

    public interface ISampleProjectService : IService
    {
        /// <summary>
        /// The most recent <see cref="SampleRoom"/> the player has progressed to.
        /// </summary>
        SampleRoom CurrentRoom { get; }

        /// <summary>
        /// Has <see cref="CurrentRoom"/> been cleared?
        /// </summary>
        bool IsCleared { get; }

        /// <summary>
        /// A <see cref="SampleRoom"/> has been entered.
        /// </summary>
        event OnRoomDelegate RoomEntered;

        /// <summary>
        /// A <see cref="SampleRoom"/> has been cleared.
        /// </summary>
        event OnRoomDelegate RoomCleared;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="room"></param>
        void EnterRoom(SampleRoom room);

        /// <summary>
        /// Clears the <paramref name="room"/> and unlocks the next <see cref="SampleRoom"/>.
        /// </summary>
        /// <param name="room">The <see cref="SampleRoom"/> cleared.</param>
        void ClearRoom(SampleRoom room);
    }
}