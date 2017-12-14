using Game.Core;
using MySql.Data.MySqlClient;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using System;

namespace Game.Account
{
    partial class Account
    {
        void Login()
        {
            InputDialog input = new InputDialog("Login", "Welcome back " + __player.Name + "!\nEnter your password below to login.\n{D10859}If this is not your account press 'leave' and please come back with another name.", true, "login", "leave");
            input.Response += Login_Response;
            input.Show(__player);
        }

        private void Login_Response(object sender, DialogResponseEventArgs e)
        {
            if (e.DialogButton == DialogButton.Right)
            {
                __player.Kick();
                return;
            }

            if(!string.IsNullOrEmpty(e.InputText) && Load(e.InputText))
            {
                Spawn();
            }
            else
            {
                InputDialog input = new InputDialog("Login", "This password is not valid\n{D10859}If this is not your account press 'leave' and please come back with another name.", true, "login", "leave");
                input.Response += Login_Response;
                input.Show(__player);
            }
        }
    }
}
