// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Locomotion;
using RealityToolkit.Samples.SampleProject.SampleRooms;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
            : base(name, priority)
        {
            scenesToLoad = profile.ScenesToLoad;
        }

        private readonly List<string> scenesToLoad;
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

            LoadScenes();
        }

        /// <inheritdoc/>
        public void EnterRoom(SampleRoom room)
        {
            CurrentRoom = room;
            IsCleared = false;
            RoomEntered?.Invoke(room);
        }

        /// <inheritdoc/>
        public void ClearRoom(SampleRoom room)
        {
            if (room != CurrentRoom || IsCleared)
            {
                return;
            }

            IsCleared = true;
            RoomCleared?.Invoke(room);

            var nextRoomIndex = ((int)room) + 1;
            if (Enum.IsDefined(typeof(SampleRoom), nextRoomIndex))
            {
                RoomUnlocked?.Invoke((SampleRoom)nextRoomIndex);
            }
        }

        #region Scene Management

        public void LoadScenes()
        {
            foreach (var scene in scenesToLoad)
            {
                if (!IsSceneLoaded(scene))
                {
                    SceneManager.LoadScene(scene, LoadSceneMode.Additive);
                }
            }
        }

        private bool IsSceneLoaded(string sceneName)
        {
            for (var i = 0; i < SceneManager.loadedSceneCount; i++)
            {
                var loadedScene = SceneManager.GetSceneAt(i);
                if (string.Equals(loadedScene.name, sceneName))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion Scene Management
    }
}
