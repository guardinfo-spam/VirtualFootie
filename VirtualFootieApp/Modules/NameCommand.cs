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
    public class NameCommand : ModuleBase
    {
        [Command("name")]
        public async Task Name(string team_name)
        {
            var dBLayer = new DBLayer();
            var user = Context.User;

            var result = dBLayer.SetName(user.Username, team_name);
            
            var sb = new StringBuilder();
            if (result == 1) sb.Append($"Your new team name has been set to {team_name}");
            else sb.Append("unable to set team name. Please try again later");

            await ReplyAsync(sb.ToString());
        }

    
    }
}
