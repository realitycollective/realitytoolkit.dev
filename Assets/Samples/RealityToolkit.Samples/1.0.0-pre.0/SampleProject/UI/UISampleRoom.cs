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

        private ILocomotionService locomotionService;

        private async void Awake()
        {
            page1.SetActive(true);
            page2.SetActive(false);

            await ServiceManager.WaitUntilInitializedAsync();

            locomotionService = ServiceManager.Instance.GetService<ILocomotionService>();
        }

        public void GetStarted()
        {
            page1.SetActive(false);
            page2.SetActive(true);
        }

        public void EnableFreeMovement()
        {
            locomotionService.MovementEnabled = true;
        }
    }
}