using Game.Display;
using Game.World.Players;
using SampSharp.GameMode;
using SampSharp.GameMode.SAMP;
using SampSharp.Streamer.World;

namespace Game.World.Properties
{
    partial class Property
    {
        private void __Spawn(PropertyType type, Interior interior, Vector3 pos, float angle)
        {
            // Nu poate fii None, nu?
            if (type == PropertyType.TypeNone)
                type = PropertyType.TypeGeneric;

            // Buildup
            __type = type;
            __area = DynamicArea.CreateSphere(pos, 1.5f);
            __pickup = new DynamicPickup((int)type, 23, pos);
            __label = new DynamicTextLabel(ToString(), Color.White, pos, 30.0f);
            __label.TestLOS = true;
            __pos = pos;
            __angle = angle;
            __price = 0;
            Interior = interior;

            // Inregistram evenimentele
            __area.Enter += __area_Enter;
            __area.Leave += __area_Leave;
        }

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
