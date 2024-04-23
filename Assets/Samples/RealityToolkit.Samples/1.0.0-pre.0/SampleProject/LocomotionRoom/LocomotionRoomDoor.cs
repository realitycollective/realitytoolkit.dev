using RealityCollective.ServiceFramework.Services;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.LocomotionRoom
{
    public class LocomotionRoomDoor : MonoBehaviour
    {
        private ISampleProjectService sampleProjectService;
        private Vector3 closedPosition;

        private async void Awake()
        {
            closedPosition = transform.localPosition;

            await ServiceManager.WaitUntilInitializedAsync();

            sampleProjectService = ServiceManager.Instance.GetService<ISampleProjectService>();
            sampleProjectService.LocomotionRoomCompleted += SampleProjectService_LocomotionRoomCompleted;
        }

        private void OnDestroy()
        {
            if (sampleProjectService != null)
            {
                sampleProjectService.LocomotionRoomCompleted -= SampleProjectService_LocomotionRoomCompleted;
            }
        }

        private void SampleProjectService_LocomotionRoomCompleted()
        {
            transform.localPosition = new Vector3(closedPosition.x, 4f, closedPosition.z);
        }
    }
}