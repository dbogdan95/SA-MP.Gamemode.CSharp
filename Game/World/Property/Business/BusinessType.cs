using SampSharp.GameMode.Pools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.Property.Business
{
    public class BusinessType : IdentifiedPool<BusinessType>
    {
        public int Icon { get; set; }
        public int Price { get; set; }

        private string __name;

        public BusinessType(int id, int icon, string name, int price)
        {
            Id = id;
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
