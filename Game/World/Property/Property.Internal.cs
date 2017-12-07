using SampSharp.GameMode;
using SampSharp.GameMode.World;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.Property
{
    partial class Property
    {
        private PropertyType __type;
        private DynamicArea __area = null;
        private DynamicPickup __pickup = null;
        private DynamicMapIcon __icon = null;
        private DynamicTextLabel __label = null;
        private Interior __interior = null;
        private Vector3 __pos;
        private float __angle;
        private bool __locked;
        private List<Player> __PlayersIn = new List<Player>();

        private void __togglePlayerToProp(Player player, bool In)
        {
            if (player.Lift || player.State != SampSharp.GameMode.Definitions.PlayerState.OnFoot)
                return;

            if (In)
                PutPlayerIn(player);
            else
                player.RemoveFromProperty();
        }
    }
}
