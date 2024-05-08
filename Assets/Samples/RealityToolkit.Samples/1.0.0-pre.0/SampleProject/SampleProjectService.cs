// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Locomotion;
using System;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject
{
    /// <summary>
    /// This sample service is reponsible for managing the sample experience. It is not directly
    /// related to any toolkit features.
    /// </summary>
    [System.Runtime.InteropServices.Guid("a7e7f7a9-fd59-4589-a501-080dd2d97afd")]
    public class SampleProjectService : BaseServiceWithConstructor, ISampleProjectService
    {
        /// <inheritdoc/>
        public SampleProjectService(string name, uint priority, SampleProjectServiceProfile profile)
            : base(name, priority) { }

        private ILocomotionService locomotionService;

        /// <inheritdoc/>
        public SampleRoom CurrentRoom { get; private set; }

        /// <inheritdoc/>
        public bool IsCleared { get; private set; }

        /// <inheritdoc/>
        public event OnRoomDelegate RoomEntered;

        /// <inheritdoc/>
        public event OnRoomDelegate RoomCleared;

        /// <inheritdoc/>
        public event OnRoomDelegate RoomUnlocked;

        /// <inheritdoc/>
        public override void Initialize()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            locomotionService = ServiceManager.Instance.GetService<ILocomotionService>();
        }

        /// <inheritdoc/>
        public override void Start()
        {
            // The sample project starts out with introducing
            // the user to locmotion. Thus we want to make sure it is
            // initially disabled. Another way of doing this would to configure
            // the auto start behaviour on the locomotion service profile itself.
            locomotionService.LocomotionEnabled = true;
            locomotionService.MovementEnabled = false;
            locomotionService.TeleportationEnabled = false;
        }

        /// <inheritdoc/>
        public void EnterRoom(SampleRoom room)
        {
            CurrentRoom = room;
            RoomEntered?.Invoke(room);
        }

        /// <inheritdoc/>
        public void ClearRoom(SampleRoom room)
        {
            RoomCleared?.Invoke(room);

            var nextRoomIndex = ((int)room) + 1;
            if (Enum.IsDefined(typeof(SampleRoom), nextRoomIndex))
            {
                RoomUnlocked?.Invoke((SampleRoom)nextRoomIndex);
            }
        }
    }
}
