using Discord.Commands;
using System.Text;
using System.Threading.Tasks;
using VirtualFootieApp.Database;

namespace VirtualFootieApp.Modules
{
    public class BalanceCommand : ModuleBase
    {
        [Command("bal")]
        public async Task Balance()
        {            
            var sb = new StringBuilder();         
            var user = Context.User;

            var balance = new DBLayer().UserBalance(user.Username);
            sb.Append($" Your balance is {balance}");

            await ReplyAsync(sb.ToString());
        }
    }
}
