using RealityToolkit.Input.Events;
using RealityToolkit.Input.InteractionBehaviours;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Core.Samples.Interactions
{
    public class TranslateLeverBehaviour : BaseInteractionBehaviour
    {
        [SerializeField]
        private float minMaxThrottleOffset = .05f;

        [SerializeField]
        [Tooltip("The pivot transform for the lever to rotate at.")]
        private Transform pivot = null;

        private IControllerInteractor currentInteractor;
        private Vector3 previousInteractorPosition;

        /// <inheritdoc/>
        protected override void Update()
        {
            if (currentInteractor == null)
            {
                return;
            }

            var currentInteractorPosition = pivot.InverseTransformPoint(currentInteractor.GameObject.transform.position);
            var deltaZ = currentInteractorPosition.z - previousInteractorPosition.z;

            var leverPosition = transform.localPosition;
            var leverOffset = Mathf.Clamp(leverPosition.z + deltaZ, -minMaxThrottleOffset, minMaxThrottleOffset);

            leverPosition.z = leverOffset;
            transform.localPosition = leverPosition;

            previousInteractorPosition = currentInteractorPosition;
        }

        /// <inheritdoc/>
        protected override void OnFirstGrabEntered(InteractionEventArgs eventArgs)
        {
            if (eventArgs.Interactor is not IControllerInteractor controllerInteractor)
            {
                return;
            }

            currentInteractor = controllerInteractor;
            previousInteractorPosition = pivot.InverseTransformPoint(currentInteractor.GameObject.transform.position);
        }

        /// <inheritdoc/>
        protected override void OnLastGrabExited(InteractionExitEventArgs eventArgs)
        {
            currentInteractor = null;
        }
    }
}
