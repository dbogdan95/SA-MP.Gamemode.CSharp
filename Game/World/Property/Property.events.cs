using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Events;

namespace Game.World.Property
{
    partial class Property
    {
        private void __area_Enter(object sender, PlayerEventArgs e)
        {
            e.Player.SendClientMessage("Touch property");
            e.Player.KeyStateChanged += Player_KeyStateChanged_Exterior;
        }

        private void __area_Leave(object sender, PlayerEventArgs e)
        {
            e.Player.SendClientMessage("Leave property");
            e.Player.KeyStateChanged -= Player_KeyStateChanged_Exterior;
        }

        private void __interior_TouchExit(object sender, PlayerEventArgs e)
        {
            e.Player.SendClientMessage("Touch interior door");
            e.Player.KeyStateChanged += Player_KeyStateChanged_Interior;
        }

        private void __interior_LeaveExit(object sender, PlayerEventArgs e)
        {
            e.Player.SendClientMessage("Leave interior door");
            e.Player.KeyStateChanged -= Player_KeyStateChanged_Interior;
        }

        private void Player_KeyStateChanged_Interior(object sender, KeyStateChangedEventArgs e)
        {
            if (e.NewKeys == Keys.SecondaryAttack)
                __togglePlayerToProp((sender as Player), false);
        }

        private void Player_KeyStateChanged_Exterior(object sender, KeyStateChangedEventArgs e)
        {
            if (e.NewKeys == Keys.SecondaryAttack)
                __togglePlayerToProp((sender as Player), true);
        }
    }
}
