using SampSharp.GameMode;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.SAMP;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World
{
    class Marker : IdentifiedPool<Marker>
    {
        private DynamicObject _object;
        private DynamicArea _area;
        private Vector3 _pos;

        public event EventHandler<PlayerEventArgs> LeaveMarker;
        public event EventHandler<PlayerEventArgs> EnterMarker;

        public Marker(Vector3 pos)
        {
            _area = DynamicArea.CreateSphere(pos, 1.0f);
            _object = new DynamicObject(1317, pos, Vector3.Zero);

            _area.Leave += _area_Leave;
            _area.Enter += _area_Enter;

            Color red = Color.Red;
            red.A = 150;

            _object.SetMaterial(0, 18646, "matcolours", "red-4", red);
        }

        private void _area_Leave(object sender, PlayerEventArgs e)
        {
            LeaveMarker?.Invoke(this, new PlayerEventArgs(e.Player));
        }

        private void _area_Enter(object sender, PlayerEventArgs e)
        {
            EnterMarker?.Invoke(this, new PlayerEventArgs(e.Player));
        }
    }
}
