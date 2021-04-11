using Discord.Commands;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFA.Lib.Support;
using VirtualFootieApp.Database;

namespace VirtualFootieApp.Modules
{
    public class TeamCommand : ModuleBase
    {
        [Command("team")]
        public async Task Team()
        {
            var dBLayer = new DBLayer();
            var user = Context.User;

            var players = dBLayer.GetTeamForUser(user.Username).ToList();
            var teamName = dBLayer.GetTeamNameForUser(user.Username);            
            
            var sb = new StringBuilder();

            sb.AppendLine($"Team: {teamName}");

            if ( players != null && players.Count > 0)
            {
                for (int index = 0; index < players.Count; index++)
                {
                    sb.Append(index + 1)
                      .Append(".")
                      .AppendLine(PreparePlayerForDisplay(players[index]));
                }
            }
            else
            {
                sb.Append("You haven't set any players. Use the ';starters add player' command");
            }
            

            await ReplyAsync(sb.ToString());
        }

        public string PreparePlayerForDisplay(APIPlayerData player)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(player.ToString())
                .Append(" | ")
                .Append(player.position)
                .Append(" | ")
                .Append(player.rating);

            return sb.ToString();
        }
    }
}
