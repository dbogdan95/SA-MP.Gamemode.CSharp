using Game.Factions;
using Game.World.Players;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Events;

namespace Game.World.Properties
{
    partial class Property
    {
        private void __area_Enter(object sender, PlayerEventArgs e)
        {
            Player p = (e.Player as Player);

            p.PropertyInteracting = this;
            p.KeyStateChanged += Player_KeyStateChanged_Exterior;
            
            if (this is Generic)
            {
                Generic g = (this as Generic);

                if (g.Faction != null)
                    p.GameText(g.Faction.Name, 3000, 1);
            }
        }
        private void __area_Leave(object sender, PlayerEventArgs e)
        {
            Player player = (e.Player as Player);

            if (player.Property != player.PropertyInteracting)
                player.PropertyInteracting = null;

            player.KeyStateChanged -= Player_KeyStateChanged_Exterior;
        }
        private void __interior_TouchExit(object sender, PlayerEventArgs e)
        {
            e.Player.KeyStateChanged += Player_KeyStateChanged_Interior;
        }
        private void __interior_LeaveExit(object sender, PlayerEventArgs e)
        {
            e.Player.KeyStateChanged -= Player_KeyStateChanged_Interior;
        }
        private void Player_KeyStateChanged_Interior(object sender, KeyStateChangedEventArgs e)
        {
            if (e.NewKeys == Keys.SecondaryAttack)
                __togglePlayer((sender as Player), false);
        }
        private void Player_KeyStateChanged_Exterior(object sender, KeyStateChangedEventArgs e)
        {
            if (e.NewKeys == Keys.SecondaryAttack)
            {
                Player player = sender as Player;

                if (player.State != PlayerState.OnFoot)
                    return;

                player.ClearAnimations();

                if (Locked || ((this is Generic) && (this as Generic).Faction != null && player.Faction != (this as Generic).Faction))
                {
                    player.GameText("~r~Locked", 1, 1);
                    Util.PlaySoundInRangeOfPoint((Sounds)24600, player.Position, 15.0f);
                    return;
                }
                __togglePlayer(player, true);
            }
        }
    }
}
