using System;
using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Definitions;
using SampSharp.Streamer.World;
using System.Collections.Generic;
using System.Xml;
using Game.World;
using Game.World.Players;
using Game.World.Vehicles;

namespace Game
{
    internal class CornPlantController : IController, IEventListener
    {
        public void RegisterEvents(BaseMode gameMode)
        {
            gameMode.Initialized += CornPlant_OnInit;
            gameMode.PlayerUpdate += CornPlant_OnPlayerUpdate;
        }

        private void CornPlant_OnInit(object sender, System.EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"corns.xml");

            foreach (XmlNode node in doc.DocumentElement)
                new CornPlant(new Vector3(Convert.ToSingle(node.Attributes["x"].InnerText), Convert.ToSingle(node.Attributes["y"].InnerText), Convert.ToSingle(node.Attributes["z"].InnerText)));
        }

        private void CornPlant_OnPlayerUpdate(object sender, PlayerUpdateEventArgs e)
        {
            Player player = sender as Player;

            if (player.Vehicle is Vehicle vehicle)
            {
                if (vehicle.Model == VehicleModelType.CombineHarvester)
                {
                    Vector3 p1 = vehicle.PostionFromOffset(new Vector3(4.0254f, 4.8493f, -1.6276f));
                    Vector3 p2 = vehicle.PostionFromOffset(new Vector3(-3.9617f, 5.0254f, -1.5630f));

                    IEnumerable<DynamicArea> list = DynamicArea.GetAreasForLine(p1, p2);

                    foreach (DynamicArea dynamicArea in list)
                    {
                        CornPlant cornPlant = CornPlant.GetCornPlantPerDynamicArea(dynamicArea);

                        if (cornPlant == null)
                            continue;

                        if (!cornPlant.Alive)
                            continue;

                        cornPlant.Kill();
                        cornPlant.Grow();
                    }
                }
            }
        }
    }
}