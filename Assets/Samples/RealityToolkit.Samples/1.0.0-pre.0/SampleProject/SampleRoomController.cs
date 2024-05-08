// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityCollective.Utilities.Extensions;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject
{
    /// <summary>
    /// This controller worksw with the <see cref="ISampleProjectService"/> and manages
    /// the assigned <see cref="room"/>'s state.
    /// </summary>
    public class SampleRoomController : MonoBehaviour
    {
        [SerializeField]
        private SampleRoom room = SampleRoom.Undefined;

        [SerializeField, Tooltip("The door used to exit the room.")]
        private SampleRoomDoor exitDoor = null;

        private ISampleProjectService sampleProjectService;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private async void Awake()
        {
            await ServiceManager.WaitUntilInitializedAsync();

            sampleProjectService = ServiceManager.Instance.GetService<ISampleProjectService>();
            sampleProjectService.RoomCleared += SampleProjectService_RoomCleared;
            sampleProjectService.RoomUnlocked += SampleProjectService_RoomUnlocked;
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnDestroy()
        {
            if (sampleProjectService != null)
            {
                sampleProjectService.RoomCleared -= SampleProjectService_RoomCleared;
                sampleProjectService.RoomUnlocked -= SampleProjectService_RoomUnlocked;
            }
        }

        private void SampleProjectService_RoomUnlocked(SampleRoom room) { }

        private void SampleProjectService_RoomCleared(SampleRoom room)
        {
            if (this.room != room || exitDoor.IsNull())
            {
                return;
            }

            exitDoor.Open();
        }
    }
}