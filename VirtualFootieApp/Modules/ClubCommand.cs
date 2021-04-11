using Discord.Commands;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFA.Lib.Support;
using VirtualFootieApp.Database;

namespace VirtualFootieApp.Modules
{
    public class ClubCommand : ModuleBase
    {
        [Command("club")]
        public async Task Club()
        {
            var dBLayer = new DBLayer();
            var user = Context.User;

            var result = dBLayer.GetClubPlayersForUser(user.Username).ToList();
            var teamName = dBLayer.GetTeamNameForUser(user.Username);
            
            var sb = new StringBuilder();

            sb.AppendLine($"Team: {teamName}");
            
            for ( int index = 0; index < result.Count; index++ )
            {
                sb.Append(index + 1)
                  .Append(".")
                  .AppendLine(PreparePlayerForDisplay(result[index]));
            }            

            await ReplyAsync(sb.ToString());
        }

        public string PreparePlayerForDisplay(APIPlayerData player)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(player.PlayerID)
                .Append("|")
                .Append(player.ToString())
                .Append(" | ")
                .Append(player.position)
                .Append(" | ")
                .Append(player.rating);

            return sb.ToString();
        }
    }
}
