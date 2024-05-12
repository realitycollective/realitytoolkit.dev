using System;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.Quests
{
    /// <summary>
    /// Abstract base for any kind of quest the user has to perform within the sample experience.
    /// </summary>
    public abstract class SampleQuest : MonoBehaviour
    {
        [SerializeField, Tooltip("The instruction text to complete the quest.")]
        private string instruction = null;

        /// <summary>
        /// The instruction text to complete the quest.
        /// </summary>
        public string Instruction => instruction;

        /// <summary>
        /// Is this quest currently active and being tracked?
        /// </summary>
        public bool IsActive { get; set; }

        private bool isComplete;
        /// <summary>
        /// Is this quest complete / finished?
        /// </summary>
        public bool IsComplete
        {
            get => isComplete;
            protected set
            {
                if (isComplete == value || !IsActive)
                {
                    return;
                }

                isComplete = value;

                if (isComplete)
                {
                    Completed?.Invoke();
                }
            }
        }

        /// <summary>
        /// The quest has been completed.
        /// </summary>
        public event Action Completed;

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void Awake() { }

        /// <summary>
        /// <see cref="MonoBehaviour"/>.
        /// </summary>
        protected virtual void OnDestroy() { }
    }
}