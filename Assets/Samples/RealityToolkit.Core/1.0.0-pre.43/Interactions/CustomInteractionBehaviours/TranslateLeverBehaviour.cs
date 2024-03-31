using RealityToolkit.Input.Events;
using RealityToolkit.Input.InteractionBehaviours;
using RealityToolkit.Input.Interactors;
using UnityEngine;

namespace RealityToolkit.Core.Samples.Interactions
{
    public class TranslateLeverBehaviour : BaseInteractionBehaviour
    {
        [SerializeField, Tooltip("The axes to translate upon interaction.")]
        private SnapAxis axes = SnapAxis.Z;

        [SerializeField, Tooltip("Translation limits per axis in each direction.")]
        private Vector3 translateThresholds = new Vector3(0f, 0f, .1f);

        [SerializeField, Tooltip("The pivot transform determines the coordinate space to translate in.")]
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
            var delta = Vector3.zero;

            if ((axes & SnapAxis.X) > 0)
            {
                delta.x = currentInteractorPosition.x - previousInteractorPosition.x;
            }

            if ((axes & SnapAxis.Y) > 0)
            {
                delta.y = currentInteractorPosition.y - previousInteractorPosition.y;
            }

            if ((axes & SnapAxis.Z) > 0)
            {
                delta.z = currentInteractorPosition.z - previousInteractorPosition.z;
            }

            var leverPosition = transform.localPosition;
            leverPosition.x = Mathf.Clamp(leverPosition.x + delta.x, -translateThresholds.x, translateThresholds.x);
            leverPosition.y = Mathf.Clamp(leverPosition.y + delta.y, -translateThresholds.y, translateThresholds.y);
            leverPosition.z = Mathf.Clamp(leverPosition.z + delta.z, -translateThresholds.z, translateThresholds.z);
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
