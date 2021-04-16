using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Dapper;
using VFA.Lib.Support;

namespace VirtualFootieApp.Database
{
    public class DBLayer
    {
        public IEnumerable<APIPlayerData> LoadAllPlayers()
        {
            using (IDbConnection conn = new DBConn().Connection)
            {
                string query = "SELECT * from PlayersData";
                var result = conn.Query<APIPlayerData>(query);
                return result;
            }
        }

        public int UserBalance(string user)
        {
            using (IDbConnection conn = new DBConn().Connection)
            {
                string query = "SELECT balance from DiscordUsers where discord_handle = @discord_handle";
                var result = conn.Query<int>(query, new { discord_handle = user });

                if (result == null || !result.Any())
                    return 0;

                return result.First();
            }
        }

        public int AddClaimToUser(int userID, int playerID)
        {
            using (IDbConnection conn = new DBConn().Connection)
            {
                string query = "INSERT INTO UserClub ( user_id, player_id) values ( @user_id, @player_id )";
                var result = conn.ExecuteScalar<int>(query, new { user_id = userID, player_id = playerID });

                return result;
               
            }
        }

        public int GetUserIDByDiscordHandle(string user)
        {
            using (IDbConnection conn = new DBConn().Connection)
            {
                StringBuilder sb = new StringBuilder();

                sb.Append("IF NOT EXISTS(select * from DiscordUsers where discord_handle = @discord_handle) ")
                .Append("begin ")
                .Append("INSERT INTO DiscordUsers(discord_handle) values(@discord_handle) ")
                .Append("select SCOPE_IDENTITY() ")
                .Append("END ELSE BEGIN ")
                .Append("SELECT id from DiscordUsers where discord_handle = @discord_handle ")
                .Append("END");

                int result = conn.ExecuteScalar<int>(sb.ToString(), new { discord_handle = user });

                return result;
            }
        }

        public IEnumerable<APIPlayerData> GetClubPlayersForUser(string user)
        {
            using (IDbConnection conn = new DBConn().Connection)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select * from UserClub u ")
                .Append("inner join PlayersData p on u.player_id = p.PlayerID ")
                .Append("where user_id = ( select id from DiscordUsers where discord_handle = @user )");

                return conn.Query<APIPlayerData>(sb.ToString(), new { user = user });
            }
        }

        public string GetTeamNameForUser(string user)
        {
            using (IDbConnection conn = new DBConn().Connection)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select team_name from DiscordUsers ")
                .Append("where discord_handle = @user");

                var result = conn.Query<string>(sb.ToString(), new { user = user });
                return result.First();
            }
        }

        public int SetName(string user, string team_name)
        {
            using (IDbConnection conn = new DBConn().Connection)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("Update DiscordUsers ")
                .Append("SET team_name = @team_name ")
                .Append("where discord_handle = @user ")
                .Append("select @@ROWCOUNT");

                var result = conn.Query<int>(sb.ToString(), new { user = user, team_name = team_name });
                return result.First();
            }
        }

        public IEnumerable<APIPlayerData> GetTeamForUser(string user)
        {
            using (IDbConnection conn = new DBConn().Connection)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select * from UserClub u ")
                .Append("INNER JOIN PlayersData p on u.player_id = p.PlayerID ")
                .Append("where u.user_id = (select id from DiscordUsers where discord_handle=@user) and u.is_main_11 = 1");

                string query = sb.ToString();

                var result = conn.Query<APIPlayerData>(sb.ToString(), new { user = user });
                return result;
            }
        }

        public int AddPlayerToStarters(string user, int player_id)
        {
            using (IDbConnection conn = new DBConn().Connection)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE UserClub ")
                .Append("SET is_main_11 = 1 ")
                .Append("where user_id = ( select id from DiscordUsers where discord_handle = @user )  and player_id = @player_id ")
                .Append("select @@ROWCOUNT");

                var result = conn.Query<int>(sb.ToString(), new { user = user, player_id = player_id });
                return result.First();
            }
        }

        public int RemovePlayerFromStarters(string user, int player_id)
        {
            using (IDbConnection conn = new DBConn().Connection)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("UPDATE UserClub ")
                .Append("SET is_main_11 = 0 ")
                .Append("where user_id = ( select id from DiscordUsers where discord_handle = @user )  and player_id = @player_id ")
                .Append("select @@ROWCOUNT");

                var result = conn.Query<int>(sb.ToString(), new { user = user, player_id = player_id });
                return result.First();
            }
        }


        public int GetFirstElevenStrength(string user)
        {
            using (IDbConnection conn = new DBConn().Connection)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("select  (p.pace + p.physicality + p.defending + p.dribbling + p.passing + p.shooting) as TeamStats from userclub u ")
                .Append("inner join PlayersData p on u.player_id = p.PlayerID ")
                .Append("inner join DiscordUsers d on d.id = u.user_id ")
                .Append("where d.discord_handle = @user and u.is_main_11 = 1");

                var result = conn.Query<int>(sb.ToString(), new { user = user });

                if (result.Any()) return result.First();
                else return 0;
            }
        }

        public int SellPlayer(string user, int playerID, double sellPrice)
        {
            using (IDbConnection conn = new DBConn().Connection)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append("declare @userID int ")
                .Append("declare @rowsDeleted int ")
                .Append("set @userID = (select id from DiscordUsers where discord_handle = @user) ")
                .Append("DELETE FROM UserClub where user_id = @userid and player_id = @playerID ")
                .Append("set @rowsDeleted = @@ROWCOUNT ")
                .Append("if @rowsDeleted  = 1 ")
                .Append("begin ")
                .Append("update DiscordUsers set balance = balance + @sellPrice where id = @userID ")
                .Append("end ")
                .Append("select balance from DiscordUsers where id = @userID");

                var result = conn.QuerySingle<int>(sb.ToString(), new { user = user, playerID = playerID, sellPrice = sellPrice });                
                return result;
            }                    
        }
    }
}
