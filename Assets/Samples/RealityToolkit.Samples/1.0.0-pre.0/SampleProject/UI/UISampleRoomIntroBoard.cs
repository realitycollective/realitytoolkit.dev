using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Samples.SampleProject.SampleRooms;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.LocomotionRoom.UI
{
    public class UISampleRoomIntroBoard : MonoBehaviour
    {
        [SerializeField]
        private GameObject root = null;

        [SerializeField]
        private TMPro.TextMeshProUGUI titleText = null;

        [SerializeField]
        private TMPro.TextMeshProUGUI descriptionText = null;

        private ISampleProjectService sampleProjectService;

        private async void Awake()
        {
            root.SetActive(false);

            await ServiceManager.WaitUntilInitializedAsync();
            sampleProjectService = ServiceManager.Instance.GetService<ISampleProjectService>();
            sampleProjectService.RoomEntered += SampleProjectService_RoomEntered;
            sampleProjectService.RoomCleared += SampleProjectService_RoomCleared;
        }

        private void OnDestroy()
        {
            if (sampleProjectService != null)
            {
                sampleProjectService.RoomEntered -= SampleProjectService_RoomEntered;
                sampleProjectService.RoomCleared -= SampleProjectService_RoomCleared;
            }
        }

        private void SampleProjectService_RoomEntered(SampleRoomController room)
        {
            titleText.text = room.Title;
            descriptionText.text = room.Description;
            transform.SetPositionAndRotation(room.IntroBoardAnchor.position, room.IntroBoardAnchor.rotation);
            root.SetActive(true);
        }

        private void SampleProjectService_RoomCleared(SampleRoomController room) => Close();

        private void Close() => root.SetActive(false);
    }
}