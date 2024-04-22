using RealityCollective.ServiceFramework.Definitions;
using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Locomotion;
using UnityEngine;

namespace RealityToolkit.Samples.SampleProject
{
    [System.Runtime.InteropServices.Guid("a7e7f7a9-fd59-4589-a501-080dd2d97afd")]
    public class SampleProjectService : BaseServiceWithConstructor, ISampleProjectService
    {
        public SampleProjectService(string name, uint priority, BaseProfile profile)
            : base(name, priority)
        {

        }

        private ILocomotionService locomotionService;

        /// <inheritdoc/>
        public override void Initialize()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            locomotionService = ServiceManager.Instance.GetService<ILocomotionService>();
        }

        /// <inheritdoc/>
        public override void Start()
        {
            // The sample project starts out with introducing
            // the user to locmotion. Thus we want to make sure it is
            // initially disabled. Another way of doing this would to configure
            // the auto start behaviour on the locomotion service profile itself.
            locomotionService.LocomotionEnabled = false;
            locomotionService.MovementEnabled = false;
            locomotionService.TeleportationEnabled = false;
        }
    }
}
