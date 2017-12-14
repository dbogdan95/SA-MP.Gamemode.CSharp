using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Account
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
