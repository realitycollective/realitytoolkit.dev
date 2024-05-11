using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Locomotion;
using RealityToolkit.Locomotion.Teleportation;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.Quests
{
    public class InstantTeleportActivator : MonoBehaviour
    {
        public void Activate()
        {
            ServiceManager.Instance.GetService<ILocomotionService>().EnableLocomotionProvider(typeof(InstantTeleportLocomotionProvider));
        }
    }
}