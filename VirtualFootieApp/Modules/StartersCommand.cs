using Discord.Commands;
using System.Text;
using System.Threading.Tasks;
using VFA.Lib.Support;
using VirtualFootieApp.Database;

namespace VirtualFootieApp.Modules
{
    public class StartersCommand : ModuleBase
    {
        [Command("starters")]
        public async Task Starters(string command_type, int player_id)
        {
            var sb = new StringBuilder();

            if (!command_type.ToLower().Equals("add") && !command_type.ToLower().Equals("rm"))
                sb.Append("please only use add or rm");
            else
            {
                var dBLayer = new DBLayer();
                var user = Context.User;

                if ( command_type.ToLower().Equals("add") )
                {
                    int result = dBLayer.AddPlayerToStarters(user.Username, player_id);

                    if (result == 1) sb.Append("player added to your starting 11 succesfully ");
                    else sb.Append("unable to add  player to your starting 11");
                }
                else
                {
                    int result = dBLayer.RemovePlayerFromStarters(user.Username, player_id);

                    if (result == 1) sb.Append("player removed from your starting 11 succesfully ");
                    else sb.Append("unable to remove  player from your starting 11");
                }
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
