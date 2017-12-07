using SampSharp.GameMode;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.Streamer.World;
using System;

namespace Game.World
{
    public class Interior : Pool<Interior>
    {
        public event EventHandler<PlayerEventArgs> LeaveDoor;
        public event EventHandler<PlayerEventArgs> TouchDoor;

        private int             __gameInterior;
        private string          __name;
        private Vector3         __pos;
        private float           __a;
        private DynamicPickup   __pickup;
        private DynamicArea     __area;

        public Interior(int gameInterior, string name, Vector3 pos, float a)
        {
            __gameInterior = gameInterior;
            __name = name;
            __pos = pos;
            __a = a;
            __pickup = new DynamicPickup(19523, 23, pos);
            __area = DynamicArea.CreateSphere(pos, 2.0f);

            __area.Enter += __area_Enter;
            __area.Leave += __area_Leave;
        }

        private void __area_Leave(object sender, SampSharp.GameMode.Events.PlayerEventArgs e)
        {
            LeaveDoor?.Invoke(this, e);
        }

        private void __area_Enter(object sender, SampSharp.GameMode.Events.PlayerEventArgs e)
        {
            TouchDoor?.Invoke(this, e);
        }

        // Summary:
        //     Gets the position of door.
        public Vector3 Position => __pos;
        //
        // Summary:
        //     Gets the rotation.
        public float Angle => __a;
        //
        // Summary:
        //     Gets the game interior.
        public int GameInterior => __gameInterior;

        public override string ToString() => __name;
    }
}
