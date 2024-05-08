using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Locomotion;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.LocomotionRoom.UI
{
    public class UISampleRoom : MonoBehaviour
    {
        [SerializeField]
        private GameObject page1 = null;

        [SerializeField]
        private GameObject page2 = null;

        private void Awake()
        {
            page1.SetActive(true);
            page2.SetActive(false);
        }

        public void GetStarted()
        {
            page1.SetActive(false);
            page2.SetActive(true);
        }

        public void EnableFreeMovement()
        {
            ServiceManager.Instance.GetService<ILocomotionService>().MovementEnabled = true;
            ServiceManager.Instance.GetService<ISampleProjectService>().ClearRoom(SampleRoom.LocomotionFree);
        }
    }
}