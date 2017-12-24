using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Factions
{
    public class Rank
    {
        private string __name;
        private int __skin;

        public Rank(string name, int skin)
        {
            Name = name;
            __skin = skin;
        }

        public int Skin { get => __skin; }
        public string Name { get => __name; set => __name = value; }

        public override string ToString()
        {
            return "Rank(Name: " + __name + ")";
        }
    }
}
