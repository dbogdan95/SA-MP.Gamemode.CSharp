using Game.World.Property;
using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Cmds
{
    class HouseCommnds
    {
        [Command("rentroom")]
        private static void CMD_RentRoom(BasePlayer sender)
        {
            Player player = (sender as Player);
            
            if (!(player.PropertyInteracting is House))
            {
                player.SendClientMessage("[ERROR] There is no house around you.");
                return;
            }

            House house = player.PropertyInteracting as House;

            if (house.Interior == null)
            {
                player.SendClientMessage("[ERROR] This house is not capable to host people.");
                return;
            }

            if (house.Rent == 0)
            {
                player.SendClientMessage("[ERROR] This house is not for rent.");
                return;
            }

            if (player.House != null)
            {
                player.SendClientMessage("[ERROR] You must be homeless to rent a house.");
                return;
            }
            
            if(house.Rent > player.Money)
            {
                player.SendClientMessage("[ERROR] You don't have enough funds to rent this house.");
                return;
            }

            player.GameText("welcome home", 3000, 1);
            player.PlaySound(1068);

            player.Money -= house.Rent;
            house.Deposit += house.Rent;
            house.UpdateSql();
            house.PutPlayerIn(player);
        }
    }
}
