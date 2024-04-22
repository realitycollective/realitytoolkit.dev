using RealityCollective.Extensions;
using RealityCollective.ServiceFramework.Services;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject.UI
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
            ServiceManager.Instance.GetService<ISampleProjectService>().CompleteLocomotionRoom();
            gameObject.Destroy();
        }
    }
}