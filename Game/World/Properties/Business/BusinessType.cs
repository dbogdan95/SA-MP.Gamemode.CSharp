using SampSharp.GameMode.Pools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.Properties
{
    public class BusinessType : IdentifiedPool<BusinessType>
    {
        public int Icon { get; set; }
        public int Price { get; set; }

        private string __name;

        public BusinessType(BusinessTypes type, int icon, string name, int price)
        {
            Id = (int)type;
            Icon = icon;
            __name = name;
            Price = price;
        }

        public override string ToString()
        {
            return __name;
        }
    }
}
