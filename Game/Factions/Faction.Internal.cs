using Game.Core;
using Game.World.Players;
using Game.World.Properties;
using Game.World.Vehicles;
using MySql.Data.MySqlClient;
using SampSharp.GameMode.SAMP;
using System;
using System.Collections.Generic;
using System.Text;

namespace Game.Factions
{
    partial class Faction
    {
        private void __Faction(int id, FactionCategory type, Color color, string name)
        {
            Id = id;
            __name = name;
            __color = color;
            __type = type;
        }

        private void __addRanks(int rankid, Rank rank)
        {
            if (__ranks.ContainsKey(rankid))
                throw new Exception("Cannot be two or more ranks with same number: " + rankid);

            __ranks.Add(rankid, rank);
        }

        private void __insertOrUpdateMember(Vehicle vehicle)
        {
            vehicle.Insert();

            using (var conn = Database.Connect())
            {
                var cmd = new MySqlCommand("INSERT INTO vehicles_factions (baseVehicle, baseFaction, rank) " +
                    "VALUES (@bp, @bf, r) ON DUPLICATE KEY UPDATE baseFaction=@f, rank=@r", conn);

                cmd.Parameters.AddWithValue("@bp", vehicle.SQLID);
                cmd.Parameters.AddWithValue("@bf", Id);
                cmd.Parameters.AddWithValue("@r", vehicle.Rank);
                cmd.ExecuteNonQuery();
            }
        }

        private void __insertOrUpdateMember(Player player)
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd;
                cmd = new MySqlCommand("INSERT INTO players_factions (basePlayer, baseFaction, rank) " +
                    "VALUES (@bp, @f, @r) ON DUPLICATE KEY UPDATE baseFaction=@f, rank=@r", conn);

                cmd.Parameters.AddWithValue("@bp", player.MyAccount.Id);
                cmd.Parameters.AddWithValue("@f", Id);
                cmd.Parameters.AddWithValue("@r", player.Rank);
                cmd.ExecuteNonQuery();
            }
        }

        private void __deleteMember(Player player)
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd;
                cmd = new MySqlCommand("DELETE FROM players_faction WHERE basePlayer=@bp AND baseFaction=@bf", conn);
                cmd.Parameters.AddWithValue("@bp", player.MyAccount.Id);
                cmd.Parameters.AddWithValue("@bf", Id);
                cmd.ExecuteNonQuery();
            }
        }

        private void __deleteMember(Vehicle vehicle)
        {
            using (var conn = Database.Connect())
            {
                MySqlCommand cmd;
                cmd = new MySqlCommand("DELETE FROM vehicles WHERE id=@bv", conn);
                cmd.Parameters.AddWithValue("@bv", vehicle.SQLID);
                cmd.ExecuteNonQuery();
            }
        }
    }
}
