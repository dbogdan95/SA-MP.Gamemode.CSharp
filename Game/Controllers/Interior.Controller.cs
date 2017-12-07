using SampSharp.GameMode.Controllers;
using System;
using System.Xml;
using SampSharp.GameMode;
using Game.World;

namespace Game.Controllers
{
    class InteriorController : IController, IEventListener
    {
        public void RegisterEvents(BaseMode gameMode)
        {
            gameMode.Initialized += Interior_OnInitialized;
        }

        private void Interior_OnInitialized(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(@"interiors.xml");

            foreach (XmlNode node in doc.DocumentElement)
            {
                new Interior
                (
                    Convert.ToInt32(node.Attributes["id"].InnerText), node.InnerText,
                    new Vector3(Convert.ToSingle(node.Attributes["x"].InnerText), Convert.ToSingle(node.Attributes["y"].InnerText), Convert.ToSingle(node.Attributes["z"].InnerText)),
                    Convert.ToSingle(node.Attributes["a"].InnerText)
                );
            }
        }
    }
}
