using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Locomotion;
using RealityToolkit.Locomotion.Teleportation;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.LocomotionRoom.UI
{
    public class UIInstantTeleportStation : MonoBehaviour
    {
        public void EnabhleInstantTeleport()
        {
            ServiceManager.Instance.GetService<ILocomotionService>().EnableLocomotionProvider(typeof(InstantTeleportLocomotionProvider));
        }
    }
}
