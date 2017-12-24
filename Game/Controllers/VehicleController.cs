using SampSharp.GameMode.Controllers;
using SampSharp.GameMode;
using Game.World.Players;

namespace Game.Controllers
{
    class VehicleController : BaseVehicleController
    {
        public override void RegisterEvents(BaseMode gameMode)
        {
            gameMode.PlayerStateChanged += (sender, args) => (sender as Player).OnStateChanged(args);

            base.RegisterEvents(gameMode);
        }
    }
}
