using RealityToolkit.Input.InteractionBehaviours;
using UnityEngine;

namespace RealityToolkit.Core.Samples.Interactions
{
    [HelpURL("https://www.realitytoolkit.io/docs/interactions/interaction-behaviours/default-behaviours/rotate-lever-behaviour")]
    public class RotateLeverBehaviour : BaseInteractionBehaviour
    {
        [SerializeField]
        [Tooltip("The pivot transform determines the coordinate space to rotate in.")]
        private Transform pivot = null;
    }
}