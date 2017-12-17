using Game.Display;
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
        private void __togglePlayer(Player player, bool In)
        {
            if (player.Lift || player.State != SampSharp.GameMode.Definitions.PlayerState.OnFoot)
                return;

            if (player.PropertyTranslation)
                return;

            player.PropertyTranslation = true;
            player.PropertyDirection = In;

            FadeScreen fade = new FadeScreen(player, 2000, FadeScreenMode.ModeComplete);
            fade.ScreenFadeEnd += (sender, e) =>
            {
                Player pl = e.Player as Player;

                if (e.Mode == FadeScreenMode.ModeFadeIn)
                {
                    if (pl.PropertyDirection)
                        PutPlayerIn(pl);
                    else
                        pl.RemoveFromProperty();
                }

                if (e.Mode == FadeScreenMode.ModeComplete)
                    pl.PropertyTranslation = false;
            };
            fade.Start();
        }
    }
}
