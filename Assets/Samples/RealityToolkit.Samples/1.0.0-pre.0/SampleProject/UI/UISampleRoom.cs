using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Locomotion;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.LocomotionRoom.UI
{
    public class UISampleRoom : MonoBehaviour, ILocomotionServiceHandler
    {
        [SerializeField]
        private GameObject page1 = null;

        [SerializeField]
        private GameObject page2 = null;

        private ILocomotionService locomotionService;
        private ISampleProjectService sampleProjectService;

        private async void Awake()
        {
            page1.SetActive(true);
            page2.SetActive(false);

            await ServiceManager.WaitUntilInitializedAsync();

            sampleProjectService = ServiceManager.Instance.GetService<ISampleProjectService>();

            locomotionService = ServiceManager.Instance.GetService<ILocomotionService>();
            locomotionService.Register(gameObject);
        }

        private void OnDestroy()
        {
            if (locomotionService != null)
            {
                locomotionService.Unregister(gameObject);
            }
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

        public void OnMoving(LocomotionEventData eventData)
        {
            if (sampleProjectService.CurrentRoom != SampleRoom.LocomotionFree ||
                sampleProjectService.IsCleared)
            {
                return;
            }

            sampleProjectService.ClearRoom(SampleRoom.LocomotionFree);
        }

        public void OnTeleportTargetRequested(LocomotionEventData eventData) { }

        public void OnTeleportStarted(LocomotionEventData eventData) { }

        public void OnTeleportCompleted(LocomotionEventData eventData) { }

        public void OnTeleportCanceled(LocomotionEventData eventData) { }
    }
}