using SampSharp.GameMode;
using SampSharp.GameMode.Controllers;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public class Vehicle : BaseVehicle
    {
        public float Fuel { get; set; }

        public Vector3 PostionFromOffset(Vector3 offset)
        {
            Vector3 v = Position;
            float r = Angle * Convert.ToSingle(Math.PI / 180);

            return new Vector3((Math.Sin(r) * offset.Y + Math.Cos(r) * offset.X + v.X), (Math.Cos(r) * offset.Y - Math.Sin(r) * offset.X + v.Y), (offset.Z + v.Z));
        }
    }

    public class VehicleController : ITypeProvider
    {
        public void RegisterTypes()
        {
            Vehicle.Register<Vehicle>();
        }
    }
}
