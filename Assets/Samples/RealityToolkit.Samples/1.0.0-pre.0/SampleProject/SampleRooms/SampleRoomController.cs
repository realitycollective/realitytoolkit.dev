// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityCollective.Utilities.Extensions;
using RealityToolkit.Samples.SampleProject.Quests;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.SampleRooms
{
    /// <summary>
    /// This controller worksw with the <see cref="ISampleProjectService"/> and manages
    /// the assigned <see cref="room"/>'s state.
    /// </summary>
    public class SampleRoomController : MonoBehaviour
    {
        [SerializeField]
        private SampleRoom room = null;

        [SerializeField]
        private List<SampleQuest> quests = null;

        [SerializeField]
        private AudioSource successAudioSource = null;

        [SerializeField, Tooltip("The door used to exit the room.")]
        private SampleRoomDoor exitDoor = null;

        private ISampleProjectService sampleProjectService;

        /// <summary>
        /// THe room intro title.
        /// </summary>
        public string Title => room.Title;

        /// <summary>
        /// The room intro description.
        /// </summary>
        public string Description => room.Description;

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private async void Awake()
        {
            await ServiceManager.WaitUntilInitializedAsync();

            sampleProjectService = ServiceManager.Instance.GetService<ISampleProjectService>();
            sampleProjectService.RoomCleared += SampleProjectService_RoomCleared;

            if (quests != null)
            {
                foreach (var quest in quests)
                {
                    quest.Completed += Quest_Completed;
                }
            }
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void OnDestroy()
        {
            if (quests != null)
            {
                foreach (var quest in quests)
                {
                    if (quest.IsNotNull())
                    {
                        quest.Completed -= Quest_Completed;
                    }
                }
            }

            if (sampleProjectService != null)
            {
                sampleProjectService.RoomCleared -= SampleProjectService_RoomCleared;
            }
        }

        /// <summary>
        /// The player has entered the room.
        /// </summary>
        public void OnRoomEntered() => sampleProjectService.EnterRoom(room);

        private void Quest_Completed()
        {
            foreach (var quest in quests)
            {
                if (!quest.IsComplete)
                {
                    return;
                }
            }

            sampleProjectService.ClearRoom(room);
        }

        private void SampleProjectService_RoomCleared(SampleRoom room)
        {
            if (this.room != room || exitDoor.IsNull())
            {
                return;
            }

            StartCoroutine(OpenExitDelayed());
        }

        private IEnumerator OpenExitDelayed()
        {
            successAudioSource.Play();
            yield return new WaitForSeconds(2f);
            exitDoor.Open();
        }
    }
}