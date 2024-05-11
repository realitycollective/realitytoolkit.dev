using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Locomotion;
using RealityToolkit.Locomotion.Teleportation;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RealityToolkit.Samples.SampleProject.Quests
{
    /// <summary>
    /// For this quest, the user has to teleport at least once using all available
    /// <see cref="ITeleportLocomotionProvider"/>s.
    /// </summary>
    public class TeleportQuest : SampleQuest, ILocomotionServiceHandler
    {
        private ILocomotionService locomotionService;
        private Dictionary<Type, bool> teleportProviders;

        /// <inheritdoc/>
        protected override async void Awake()
        {
            base.Awake();

            await ServiceManager.WaitUntilInitializedAsync();

            locomotionService = ServiceManager.Instance.GetService<ILocomotionService>();
            locomotionService.Register(gameObject);

            var teleportLocomotionProviders = ServiceManager.Instance.GetServices<ITeleportLocomotionProvider>();

            teleportProviders = new Dictionary<Type, bool>();
            foreach (var item in teleportLocomotionProviders)
            {
                teleportProviders.Add(item.GetType(), false);
            }
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
        public void OnMoving(LocomotionEventData eventData) { }

        /// <inheritdoc/>
        public void OnTeleportTargetRequested(LocomotionEventData eventData) { }

        /// <inheritdoc/>
        public void OnTeleportStarted(LocomotionEventData eventData) { }

        /// <inheritdoc/>
        public void OnTeleportCompleted(LocomotionEventData eventData)
        {
            var type = eventData.LocomotionProvider.GetType();
            teleportProviders[type] = true;

            if (teleportProviders.All(p => p.Value))
            {
                IsComplete = true;
            }
        }

        /// <inheritdoc/>
        public void OnTeleportCanceled(LocomotionEventData eventData) { }
    }
}