using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Locomotion;
using RealityToolkit.Locomotion.Teleportation;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.LocomotionRoom.UI
{
    public class UIBlinkTeleportStation : MonoBehaviour
    {
        public void EnabhleBlinkTeleport()
        {
            ServiceManager.Instance.GetService<ILocomotionService>().EnableLocomotionProvider(typeof(BlinkTeleportLocomotionProvider));
        }
    }
}