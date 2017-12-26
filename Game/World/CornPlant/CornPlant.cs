using SampSharp.GameMode;
using System;
using SampSharp.Streamer.World;
using SampSharp.GameMode.Pools;

namespace Game.World
{
    public partial class CornPlant : IdentifiedOwnedPool<CornPlant, DynamicArea>
    {
        DynamicObject __object;
        DynamicArea __area;
        Vector3 __pos;
        bool __alive;

        public CornPlant(Vector3 pos)
        {
            Random r = new Random();
            int ang = r.Next(0, 360);

            __object = new DynamicObject(Common.CORN_PLANT_MODELID, pos, new Vector3(90.0, 0, (float)ang));
            __area = DynamicArea.CreateCircle(pos.X, pos.Y, 1.0f);
            __pos = pos;
            __alive = true;
            __object.Moved += CornPlant_OnMoved;
            Owner = __area;

            //cornPlantPerArea.Add(__area, this);
            //cornPlantPerObject.Add(__object, this);
        }

        private void CornPlant_OnMoved(object sender, EventArgs e)
        {
            DynamicObject obj = sender as DynamicObject;
            __alive = true;
        }

        ~CornPlant()
        {
            __object.Dispose();
            __area.Dispose();
        }

        public void Kill()
        {
            __alive = false;
            __object.Position = new Vector3(__pos.X, __pos.Y, __pos.Z - 2.0f);
        }

        public void Grow() => __object.Move(__pos, 0.05f);
        public bool Alive { get => __alive; set => __alive = value; }
    }
}
