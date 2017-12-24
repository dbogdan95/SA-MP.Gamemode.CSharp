using SampSharp.GameMode.Pools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Game.World.Properties
{
    public sealed class BusinessType
    {
        #region Fields
        private static readonly BusinessType[] businessTypes =
        {
            new BusinessType(BusinessTypes.TypeNone, -1, "None", 0),
            new BusinessType(BusinessTypes.TypeDealership, 55, "DealerShip", 100000),
            new BusinessType(BusinessTypes.TypeAmmo, 6, "Ammunition", 50000),
            new BusinessType(BusinessTypes.TypePaynSpray, 63, "Pay'n'Spray", 40000),
            new BusinessType(BusinessTypes.TypeModShop, 27, "ModShop", 30000),
            new BusinessType(BusinessTypes.TypeBurger, 10, "Burger Shot", 60000),
            new BusinessType(BusinessTypes.TypeCluckin, 14, "Cluckin Bell", 60000),
            new BusinessType(BusinessTypes.TypePizza, 29, "Pizza Stack", 60000),
            new BusinessType(BusinessTypes.TypeHospital, 22, "Hospital", 500000),
            new BusinessType(BusinessTypes.TypeClub, 48, "Night Club", 50000),
            new BusinessType(BusinessTypes.TypeSex, 21, "Sex Shop", 30000),
            new BusinessType(BusinessTypes.TypeBarber, 7, "Barber", 20000),
            new BusinessType(BusinessTypes.TypeBank, 52, "Bank", 40000),
            new BusinessType(BusinessTypes.TypeGas, -1, "Gas Station", 10000)
        };
        #endregion

        private BusinessType(BusinessTypes type, int icon, string name, int price)
        {
            Type = type;
            Icon = icon;
            Name = name;
            Price = price;
        }

        public BusinessTypes Type { get; }
        public int Icon { get; }
        public int Price { get; }
        public string Name { get; }

        public override string ToString()
        {
            return Name;
        }

        public static BusinessType FromType(BusinessTypes type)
        {
            return businessTypes[(int)type];
        }

        public static IEnumerable<BusinessType> AllTypes()
        {
            return businessTypes.Cast<BusinessType>();
        }
    }
}
