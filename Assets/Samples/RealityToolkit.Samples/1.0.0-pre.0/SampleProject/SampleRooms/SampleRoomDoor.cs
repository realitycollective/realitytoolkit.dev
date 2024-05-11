// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.SampleRooms
{
    /// <summary>
    /// Just a simple door controller for <see cref="SampleRoom"/> enter and exit doors.
    /// </summary>
    [RequireComponent(typeof(AudioSource))]
    public class SampleRoomDoor : MonoBehaviour
    {
        private Vector3 closedPosition;
        private Vector3 openPosition;
        private const float openVerticalOffset = 3.8f;
        private AudioSource audioSource;

        private bool isOpening;
        private bool isClosing;
        private float animationStartTime;
        private float animationDuration;

        /// <summary>
        /// This tells whether the door is open. Amazing right?!
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            animationDuration = audioSource.clip.length;
            closedPosition = transform.localPosition;
            openPosition = new Vector3(closedPosition.x, closedPosition.y + openVerticalOffset, closedPosition.z);
        }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void Update()
        {
            if (isOpening)
            {
                var t = (Time.time - animationStartTime) / animationDuration;
                var target = Vector3.Slerp(closedPosition, openPosition, t);
                transform.localPosition = new Vector3(closedPosition.x, target.y, closedPosition.z);

                if (t >= 1f)
                {
                    isOpening = false;
                }
            }
            else if (isClosing)
            {
                var t = (Time.time - animationStartTime) / animationDuration;
                var target = Vector3.Slerp(openPosition, closedPosition, t);
                transform.localPosition = new Vector3(closedPosition.x, target.y, closedPosition.z);

                if (t >= 1f)
                {
                    isClosing = false;
                }
            }
        }

        /// <summary>
        /// Wouldn't believe it, but this closes the door.
        /// </summary>
        public void Close()
        {
            if (!IsOpen)
            {
                return;
            }

            isOpening = false;
            IsOpen = false;
            animationStartTime = Time.time;
            isClosing = true;
            audioSource.Play();
        }

        /// <summary>
        /// Wouldn't believe it, but this opens the door.
        /// </summary>
        public void Open()
        {
            if (IsOpen)
            {
                return;
            }

            isClosing = false;
            IsOpen = true;
            animationStartTime = Time.time;
            isOpening = true;
            audioSource.Play();
        }
    }
}