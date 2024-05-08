// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using RealityToolkit.Player.Rigs;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject
{
    /// <summary>
    /// Simple utility that will open a <see cref="SampleRoomDoor"/> when the player triggers it.
    /// </summary>
    public class SampleRoomDoorTrigger : MonoBehaviour
    {
        [SerializeField]
        private SampleRoomDoor door = null;

        [SerializeField]
        private bool opensDoor = true;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<IPlayerRig>(out _))
            {
                if (opensDoor)
                {
                    door.Open();
                    return;
                }

                door.Close();
            }
        }
    }
}