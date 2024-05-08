// Copyright (c) Reality Collective. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using UnityEngine;

namespace RealityToolkit.Samples.SampleProject
{
    /// <summary>
    /// Just a simple door controller for <see cref="SampleRoom"/> enter and exit doors.
    /// </summary>
    public class SampleRoomDoor : MonoBehaviour
    {
        private Vector3 closedPosition;
        private Vector3 openPosition;
        private const float openVerticalOffset = .4f;

        private bool isOpening;
        private bool isClosing;
        private float animationStartTime;
        private const float animationDuration = 2f;

        /// <summary>
        /// This tells whether the door is open. Amazing right?!
        /// </summary>
        public bool IsOpen { get; private set; }

        /// <summary>
        /// See <see cref="MonoBehaviour"/>.
        /// </summary>
        private void Awake()
        {
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
                transform.localPosition = Vector3.Slerp(closedPosition, openPosition, t);

                if (t >= 1f)
                {
                    isOpening = false;
                }
            }
            else if (isClosing)
            {
                var t = (Time.time - animationStartTime) / animationDuration;
                transform.localPosition = Vector3.Slerp(openPosition, closedPosition, t);

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
            isOpening = false;
            IsOpen = false;
            animationStartTime = Time.time;
            isClosing = true;
        }

        /// <summary>
        /// Wouldn't believe it, but this opens the door.
        /// </summary>
        public void Open()
        {
            isClosing = false;
            IsOpen = true;
            animationStartTime = Time.time;
            isOpening = true;
        }
    }
}