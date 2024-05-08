using RealityCollective.ServiceFramework.Services;
using RealityCollective.Utilities.Extensions;
using RealityToolkit.Locomotion;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.LocomotionRoom.UI
{
    public class UILocomotionRoom : MonoBehaviour
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

        public void EnableLocomotion()
        {
            ServiceManager.Instance.GetService<ILocomotionService>().LocomotionEnabled = true;
            ServiceManager.Instance.GetService<ISampleProjectService>().ClearRoom(SampleRoom.LocomotionFree);
            gameObject.Destroy();
        }
    }
}