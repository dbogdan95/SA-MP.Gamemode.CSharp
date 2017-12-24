using SampSharp.GameMode.Controllers;
using System;
using SampSharp.GameMode;
using Game.Core;
using MySql.Data.MySqlClient;
using Game.World.Properties;

namespace Game.Controllers
{
    class PropertyController : IController, IEventListener
    {
        public void RegisterEvents(BaseMode gameMode)
        {
            gameMode.Initialized += Property_OnInitialized;
        }

        private void Property_OnInitialized(object sender, EventArgs e)
        {
            
        }
    }
}
