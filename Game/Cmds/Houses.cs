using SampSharp.GameMode.SAMP.Commands;
using SampSharp.GameMode.World;
using Game.World.Properties;
using Game.World.Players;
using static Game.Cmds.Properties;

namespace Game.Cmds
{
    class Houses
    {
        [Command("rent", PermissionChecker = typeof(HouseAroundPermissionChecker))]
        private static void CMD_RentRoom(BasePlayer sender)
        {
            Player player = (sender as Player);
            House house = player.PropertyInteracting as World.Properties.House;

            if (house.Interior == null)
            {
                player.SendClientMessage("*** This house is not capable to host people.");
                return;
            }

            if (house.Rent == 0)
            {
                player.SendClientMessage("*** This house is not for rent.");
                return;
            }

            if (player.House != null)
            {
                player.SendClientMessage("*** You must be homeless to rent a house.");
                return;
            }

            if (house.Rent > player.Money)
            {
                player.SendClientMessage("*** You don't have enough funds to rent this house.");
                return;
            }

            player.GameText("welcome home", 3000, 1);

            player.RentedRoom = house;
            player.Money -= house.Rent;

            house.Deposit += (int)(house.Rent - house.Rent * Common.TAX_HOUSE_PER_RENT);
            house.UpdateSql();
            house.PutPlayerIn(player);
        }

        [Command("buy", PermissionChecker = typeof(PropAroundPermissionChecker))]
        private static void CMD_Buy(BasePlayer sender)
        {
            Player player = (sender as Player);
            Property property = player.PropertyInteracting;

            if (property.Price == 0)
                return;

            // TODO: add money to to owner bank account

            if (property is Business)
            {
                if(player.Business != null)
                {
                    player.SendClientMessage("*** You already have a business.");
                    return;
                }

                player.GameText("time for business", 3000, 1);
            }
            else if (property is House)
            {
                if (player.House != null)
                {
                    player.SendClientMessage("*** You already have a house.");
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
