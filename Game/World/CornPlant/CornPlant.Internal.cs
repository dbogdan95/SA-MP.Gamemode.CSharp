using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.World;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Text;
using SampSharp.GameMode;

namespace Game.World
{
    public partial class CornPlant
    {
        //static List<CornPlant> plantList = new List<CornPlant>();
        static Dictionary<DynamicArea, CornPlant> cornPlantPerArea = new Dictionary<DynamicArea, CornPlant>();
        static Dictionary<DynamicObject, CornPlant> cornPlantPerObject = new Dictionary<DynamicObject, CornPlant>();

		public static CornPlant GetCornPlantPerDynamicArea(DynamicArea dynamicArea)
        {
			if(cornPlantPerArea.ContainsKey(dynamicArea))
            {
                return cornPlantPerArea[dynamicArea];
            }
            return null;
        }

        public static CornPlant GetCornPlantPerDynamicObject(DynamicObject dynamicObject)
        {
            if (cornPlantPerObject.ContainsKey(dynamicObject))
            {
                return cornPlantPerObject[dynamicObject];
            }
            return null;
        }
    }
}
