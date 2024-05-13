using RealityCollective.ServiceFramework.Services;
using RealityToolkit.Player.Bounds;

namespace RealityToolkit.Samples.SampleProject.Quests
{
    /// <summary>
    /// This quest requires the user to purposefully go out of level bounds.
    /// </summary>
    public class PlayerWentOutOfBoundsQuest : SampleQuest
    {
        private IPlayerBoundsModule playerBoundsModule;

        /// <inheritdoc/>
        protected override async void Awake()
        {
            base.Awake();

            await ServiceManager.WaitUntilInitializedAsync();
            playerBoundsModule = ServiceManager.Instance.GetService<IPlayerBoundsModule>();
            playerBoundsModule.PlayerOutOfBounds += PlayerBoundsModule_PlayerOutOfBounds;
        }

        /// <inheritdoc/>
        protected override void OnDestroy()
        {
            if (playerBoundsModule != null)
            {
                playerBoundsModule.PlayerOutOfBounds -= PlayerBoundsModule_PlayerOutOfBounds;
            }

            base.OnDestroy();
        }

        private void PlayerBoundsModule_PlayerOutOfBounds(float severity, UnityEngine.Vector3 returnToBoundsDirection) => IsComplete = true;
    }
}