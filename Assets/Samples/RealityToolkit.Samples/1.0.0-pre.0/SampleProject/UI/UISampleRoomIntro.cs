using RealityCollective.ServiceFramework.Services;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.LocomotionRoom.UI
{
    public class UISampleRoomIntro : MonoBehaviour
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
        }

        public void Close()
        {

        }
    }
}