using System;
using System.Collections.Generic;
using System.Text;
using Game.World.Players;

namespace Game.Accounts
{
    partial class Account
    {
        private void Player_Disconnected(object sender, SampSharp.GameMode.Events.DisconnectEventArgs e)
        {
            if(__player.IsLogged)
            {
                Save();
            }
        }
    }
}
