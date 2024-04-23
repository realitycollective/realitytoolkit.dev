using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Locomotion;
using RealityToolkit.Locomotion.Teleportation;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.LocomotionRoom.UI
{
    public class UIDashTeleportStation : MonoBehaviour
    {
        public void EnabhleDashTeleport()
        {
            ServiceManager.Instance.GetService<ILocomotionService>().EnableLocomotionProvider(typeof(DashTeleportLocomotionProvider));
        }
    }
}