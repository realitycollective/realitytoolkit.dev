using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Locomotion;

namespace RealityToolkit.Samples.SampleProject.Quests
{
    /// <summary>
    /// For this quest, the user has to move using a <see cref="Locomotion.Movement.IFreeLocomotionProvider"/>.
    /// </summary>
    public class MoveQuest : SampleQuest, ILocomotionServiceHandler
    {
        private ILocomotionService locomotionService;

        /// <inheritdoc/>
        protected override async void Awake()
        {
            base.Awake();

            await ServiceManager.WaitUntilInitializedAsync();

            locomotionService = ServiceManager.Instance.GetService<ILocomotionService>();
            locomotionService.Register(gameObject);
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            if (locomotionService != null)
            {
                locomotionService.Unregister(gameObject);
            }

            base.OnDestroy();
        }

        /// <inheritdoc/>
        public void OnMoving(LocomotionEventData eventData) => IsComplete = true;

        /// <inheritdoc/>
        public void OnTeleportTargetRequested(LocomotionEventData eventData) { }

        /// <inheritdoc/>
        public void OnTeleportStarted(LocomotionEventData eventData) { }

        /// <inheritdoc/>
        public void OnTeleportCompleted(LocomotionEventData eventData) { }

        /// <inheritdoc/>
        public void OnTeleportCanceled(LocomotionEventData eventData) { }
    }
}