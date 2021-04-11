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
    public class ClaimCommand : ModuleBase
    {

        private WeightedRandomGenerator<APIPlayerData> _weightedData;
        private List<APIPlayerData> _allPlayersData;

        [Command("claim")]
        public async Task Claim()
        {
            var dBLayer = new DBLayer();
            var user = Context.User;

            if ( _weightedData == null )
            {
                 _allPlayersData = dBLayer.LoadAllPlayers().ToList();
                _weightedData = new WeightedRandomGenerator<APIPlayerData>();

                _allPlayersData.ForEach(p => _weightedData.AddEntry(p, DetermineWeight(p.rating.HasValue ? p.rating.Value : 50)));
                _allPlayersData = null;
            }

            var claim = _weightedData.GetRandom();

            int userID = dBLayer.GetUserIDByDiscordHandle(user.Username);

            var result = dBLayer.AddClaimToUser(userID, claim.PlayerID);            

            // initialize empty string builder for reply
            var sb = new StringBuilder();            

            // build out the reply
            sb.Append($"you claimed : {claim.ToString()}, rating: {claim.rating}");
            
            // send simple string reply
            await ReplyAsync(sb.ToString());
        }

        public double DetermineWeight(int rating)
        {
            if (rating >= 95 && rating < 100) return 0.05;
            if (rating >= 90 && rating <= 94) return 0.25;
            if (rating >= 85 && rating < 90) return 0.7;
            if (rating >= 80 && rating < 85) return 10;
            if (rating >= 65 && rating < 79) return 20;

            return 20;
        }
    }
}
