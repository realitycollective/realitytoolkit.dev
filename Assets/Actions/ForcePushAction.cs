using RealityToolkit.Input.Events;
using RealityToolkit.Input.InteractionActions;
using RealityToolkit.Input.Interactors;
using UnityEngine;

public class ForcePushAction : BaseInteractionAction
{
    [SerializeField, Tooltip("The power of the force push executed on the object.")]
    private float forcePower = 10f;

    private new Rigidbody rigidbody;

    /// <inheritdoc/>
    protected override void Awake()
    {
        base.Awake();
        rigidbody = GetComponent<Rigidbody>();
    }

    /// <inheritdoc/>
    protected override void OnLastGrabExited(InteractionExitEventArgs eventArgs)
    {
        base.OnLastGrabExited(eventArgs);

        // This action only works with controller based interactors.
        if (eventArgs.Interactor is IControllerInteractor controllerInteractor)
        {
            // We determine the direction of our force push using the interactor's position in the scene
            // and the interactable's position and normalize it.
            var forceDirection = (transform.position - controllerInteractor.GameObject.transform.position).normalized;

            // Scale by our configured force power.
            forceDirection = forcePower * forceDirection;

            // Finally add the force push to the rigidboy.
            rigidbody.AddForce(forceDirection, ForceMode.Impulse);
        }
    }
}
