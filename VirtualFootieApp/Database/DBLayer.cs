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
                string query = "SELECT * from DiscordUsers where discord_handle = @discord_handle";
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


    }
}
