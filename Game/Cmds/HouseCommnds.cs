using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using Game.World.Properties;
using Game.World.Players;

namespace Game.Cmds
{
    class HouseCommnds
    {
        [Command("rent")]
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

            if (house.Rent > player.Money)
            {
                player.SendClientMessage("[ERROR] You don't have enough funds to rent this house.");
                return;
            }

            player.GameText("welcome home", 3000, 1);

            player.RentedRoom = house;
            player.Money -= house.Rent;

            house.Deposit += (int)(house.Rent - house.Rent * Common.TAX_HOUSE_PER_RENT);
            house.UpdateSql();
            house.PutPlayerIn(player);
        }

        [Command("buy")]
        private static void CMD_Buy(BasePlayer sender)
        {
            Player player = (sender as Player);
            Property property = player.PropertyInteracting;

            if (!(property is Property))
            {
                player.SendClientMessage("[ERROR] There is no property around you.");
                return;
            }

            if (property.Price == 0)
                return;

            // TODO: add money to to owner bank account

            if (property is Business)
            {
                if(player.Business != null)
                {
                    player.SendClientMessage("[ERROR] You already have a business.");
                    return;
                }

                player.GameText("time for business", 3000, 1);
            }
            else if (property is House)
            {
                if (player.House != null)
                {
                    player.SendClientMessage("[ERROR] You already have a house.");
                    return;
                }

                (property as House).Rent = 0;
                player.GameText("home sweet home", 3000, 1);
            }

            player.Money -= property.Price;

            property.Price = 0;
            property.SetOwnerUpdate(player.MyAccount.Id);
            property.PutPlayerIn(player);
            property.UpdateLabel();
            property.UpdateSql();
        }
    }
}
