using UnityEngine;

namespace RealityToolkit.Core.Samples.Interactions
{
    public class RotateLeverBehaviour : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The pivot transform determines the coordinate space to rotate in.")]
        private Transform pivot = null;
    }
}