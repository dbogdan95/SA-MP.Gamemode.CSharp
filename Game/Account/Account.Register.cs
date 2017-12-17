using System;
using Game.Core;
using MySql.Data.MySqlClient;
using SampSharp.GameMode.Definitions;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using System.Globalization;

namespace Game.Account
{
    partial class Account
    {
        void Register()
        {
            InputDialog input = new InputDialog("Register", "Welcome " + __player.Name + 
                "!\nPlease choose a password of your account.", true, "next", "leave");

            input.Response += Register_Response;
            input.Show(__player);
        }

        private void Register_Response(object sender, DialogResponseEventArgs e)
        {
            if (e.DialogButton == DialogButton.Right)
            {
                __player.Kick();
                return;
            }

            InputDialog input;
            if (!ValidatePassword(e.InputText, out string err))
            {
                input = new InputDialog("Register", err, true, "next", "leave");
                input.Response += Register_Response;
            }
            else
            {
                PasswordHash = Util.Sha256_hash(e.InputText);
                
                input = new InputDialog("Register", "Please enter your email address.\n" +
                    "We will use it to help you to recover your password at need.",  false, "next", "leave");

                input.Response += Email_Response;
            }
            input.Show(__player);
        }

        private void Email_Response(object sender, DialogResponseEventArgs e)
        {
            if (e.DialogButton == DialogButton.Right)
            {
                __player.Kick();
                return;
            }

            InputDialog input;
            if (!IsValidEmailAddress(e.InputText))
            {
                input = new InputDialog("Register", "Invalid email adress, please try again.", false, "next", "leave");
                input.Response += Email_Response;
            }
            else
            {
                Email = e.InputText;

                input = new InputDialog("Register", "Please type your birthday.\n" +
                    "We will use it to celebrate your birthday together.\n" +
                    "{D10859}FORMAT: dd/mm/yyyy (zz/LL/aaaa)", 
                    false, "finish", "leave");

                input.Response += Birthday_Response;
            }
            input.Show(__player);
        }

        private void Birthday_Response(object sender, DialogResponseEventArgs e)
        {
            if (e.DialogButton == DialogButton.Right)
            {
                __player.Kick();
                return;
            }

            if (DateTime.TryParseExact(e.InputText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                out DateTime date))
            {
                if (date.Year > DateTime.Now.Year)
                {
                    InputDialog input = new InputDialog("Register", "Hola from the future!" +
                        "\n{D10859}FORMAT: dd/mm/yyyy (zz/LL/aaaa)", false, "finish", "leave");

                    input.Response += Birthday_Response;
                    input.Show(__player);
                    return;
                }

                Birthday = date;

                MessageDialog msg = new MessageDialog("Register", "You are a?", "Male", "Female");

                msg.Response += Gender_Response;
                msg.Show(__player);
            }
            else
            {
                InputDialog input = new InputDialog("Register", "Invalid birthday date.\n" +
                    "It must be in format:{D10859} dd/mm/yyyy (zz/LL/aaaa)", false, "finish", "leave");

                input.Response += Birthday_Response;
                input.Show(__player);
            }
        }

        private void Gender_Response(object sender, DialogResponseEventArgs e)
        {
            Gender = (byte)(e.DialogButton == DialogButton.Left ? 1 : 2);
            Insert();

            MessageDialog message = new MessageDialog("Done", "Great!\nNow, let's get started!", "Yay!");
            message.Response += (sender2, e2) =>
            {
                Spawn();
                __player.GameText("welcome~n~~w~" + e2.Player.Name, 3000, 1);
            };
            message.Show(__player);
        }
    }
}
