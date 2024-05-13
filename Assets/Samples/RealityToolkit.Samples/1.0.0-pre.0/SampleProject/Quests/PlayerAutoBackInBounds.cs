using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Player.Bounds;

namespace RealityToolkit.Samples.SampleProject.Quests
{
    /// <summary>
    /// This quest requires the user to be reset back into bounds by auto reset.
    /// </summary>
    public class PlayerAutoBackInBounds : SampleQuest
    {
        private IPlayerBoundsModule playerBoundsModule;

        /// <inheritdoc/>
        protected override async void Awake()
        {
            base.Awake();

            await ServiceManager.WaitUntilInitializedAsync();
            playerBoundsModule = ServiceManager.Instance.GetService<IPlayerBoundsModule>();
            playerBoundsModule.PlayerBackInBounds += PlayerBoundsModule_PlayerBackInBounds;
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            if (playerBoundsModule != null)
            {
                playerBoundsModule.PlayerBackInBounds -= PlayerBoundsModule_PlayerBackInBounds;
            }

            base.OnDestroy();
        }

        private void PlayerBoundsModule_PlayerBackInBounds(bool didAutoReset)
        {
            if (!didAutoReset)
            {
                return;
            }

            IsComplete = true;
        }
    }
}