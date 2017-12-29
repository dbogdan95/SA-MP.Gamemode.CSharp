using SampSharp.GameMode;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.World.FastFoods
{
    public static class FastFoodFactory
    {
        public static FastFood Create(FastFoodTypes type, int id, Vector3 pos)
        {
            FastFood fastFood = null;

            switch(type)
            {
                case FastFoodTypes.TypePizzaStack: fastFood = new PizzaStack(id, pos);
                    break;
                case FastFoodTypes.TypeCluckinBell: fastFood = null;
                    break;
                case FastFoodTypes.TypeBurgetShot: fastFood = null;
                    break;
            }
            return fastFood;
        }
    }
}
