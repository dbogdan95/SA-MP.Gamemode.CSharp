using Game.World.Properties;
using SampSharp.GameMode;
using SampSharp.GameMode.Display;
using SampSharp.GameMode.Events;
using SampSharp.GameMode.Pools;
using SampSharp.GameMode.World;
using SampSharp.Streamer.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.GasStations
{
    partial class GasStation : StaticWorldObject<GasStation>
    {
        public static readonly float MAX_GAS_STATION_FUEL = 7000.0f;

        public event EventHandler<PlayerEventArgs> EnterGasStation;
        public event EventHandler<PlayerEventArgs> LeaveGasStation;

        public Vector3 AreaMins { get; private set; }
        public Vector3 AreaMaxs { get; private set; }
        public float X { get; private set; }
        public float Y { get; private set; }
        public float Z { get; private set; }
        public float Gas
        {
            get => __gas;
            set => __gas = Math.Clamp(value, 0, MAX_GAS_STATION_FUEL);
        }

        #region Implementation of StaticWorldObject

        public override Vector3 Position
        {
            get
            {
                return new Vector3((AreaMins.X + AreaMaxs.X) / 2,
                    (AreaMins.Y + AreaMaxs.Y) / 2,
                    (AreaMins.Z + AreaMaxs.Z) / 2);
            }
        }

        #endregion

        public GasStation(int id, Vector3 areaMins, Vector3 areaMaxs) : base(id)
        {
            AreaMins = areaMins;
            AreaMaxs = areaMaxs;

            X = (AreaMins.X + AreaMaxs.X) / 2.0f;
            Y = (AreaMins.Y + areaMaxs.Y) / 2.0f;
            Z = (AreaMins.Z + AreaMaxs.Z) / 2.0f;

            Gas = MAX_GAS_STATION_FUEL;

            __on = true;
            __area = DynamicArea.CreateCube(areaMins, areaMaxs);

            __area.Enter += __area_Enter;
            __area.Leave += __area_Leave;
        }

        public void On() => __on = true;
        public void Off() => __on = false;

        protected void Pay(int money)
        {
            Business b = Business.FindByDomain(Id, BusinessTypes.TypeGas);
            if (b != null)
            {
                b.Deposit += (int)(money - money * Common.TAX_BUSINESS_PER_PRODUCTS);
                b.UpdateSql();
            }
        }
    }
}
