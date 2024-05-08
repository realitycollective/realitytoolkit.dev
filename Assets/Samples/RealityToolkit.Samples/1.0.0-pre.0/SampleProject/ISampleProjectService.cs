// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Interfaces;

namespace RealityToolkit.Samples.SampleProject
{
    public delegate void OnRoomClearedDelegate(SampleRoom room);
    public delegate void OnRoomUnlockedDelegate(SampleRoom room);

    public interface ISampleProjectService : IService
    {
        /// <summary>
        /// A <see cref="SampleRoom"/> has been cleared.
        /// </summary>
        event OnRoomClearedDelegate RoomCleared;

        /// <summary>
        /// A <see cref="SampleRoom"/> has been unlocked.
        /// </summary>
        event OnRoomUnlockedDelegate RoomUnlocked;

        /// <summary>
        /// Clears the <paramref name="room"/> and unlocks the next <see cref="SampleRoom"/>.
        /// </summary>
        /// <param name="room">The <see cref="SampleRoom"/> cleared.</param>
        void ClearRoom(SampleRoom room);
    }
}