using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFA.Lib.Support;
using VirtualFootieApp.Database;

namespace VirtualFootieApp.Modules
{
    public class ClubCommand : ModuleBase
    {

        private WeightedRandomGenerator<APIPlayerData> _weightedData;
        private List<APIPlayerData> _allPlayersData;

        [Command("club")]
        public async Task Club()
        {
            var dBLayer = new DBLayer();
            var user = Context.User;

            var result = dBLayer.GetClubForUser(user.Username).ToList();
            
            var sb = new StringBuilder();
            foreach ( var player in result )
            {
                sb.AppendLine(PreparePlayerForDisplay(player));
            }            
            
            // send simple string reply
            await ReplyAsync(sb.ToString());
        }

        public string PreparePlayerForDisplay(APIPlayerData player)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(player.ToString())
                .Append("|")
                .Append(player.position)
                .Append("|")
                .Append(player.rating);

            return sb.ToString();
        }
    }
}
