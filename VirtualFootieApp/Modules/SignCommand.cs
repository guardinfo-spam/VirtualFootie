using Discord;
using Discord.Commands;
using System;
using System.Text;
using System.Threading.Tasks;
using VFA.Lib.Support;
using VirtualFootieApp.Database;
using VirtualFootieApp.Services;

namespace VirtualFootieApp.Modules
{
    public class SignCommand : ModuleBase
    {
        [Command("sign")]
        public async Task Sign()
        {
            var dBLayer = new DBLayer();
            var user = Context.User;
            int userID = dBLayer.GetUserIDByDiscordHandle(user.Username);

            var userData = dBLayer.GetDiscordUserData(userID);            

            if ( userData.is_banned )
            {
                await ReplyAsync(null, false, this.PrepareBanEmbed(user.Username));
                return;
            }

            if (userData.last_claim_datetime_utc.HasValue)
            {
                TimeSpan? dateDiff = DateTime.UtcNow - userData.last_claim_datetime_utc;
                if (dateDiff.Value.TotalHours < AppSettingsService._claimFrequencyInHours)
                {
                    await ReplyAsync(null, false, this.PrepareDeclineClaimEmbed(user.Username ,AppSettingsService._claimFrequencyInHours * 60 - dateDiff.Value.TotalMinutes));
                    return;
                }
            }
           
            var claim = CacheService.playersWeightedData.GetRandom();
            var result = dBLayer.AddClaimToUser(userID, claim.Item1.PlayerID);

            var claimEmbed = PrepareSignEmbed(claim.Item1, claim.Item2, user);
            await ReplyAsync(null, false, claimEmbed);
        }

        
        public Embed PrepareDeclineClaimEmbed(string user, double minutes)
        {
            var embed = new EmbedBuilder();
            embed.Color = Color.Red;
            embed.Title = $"You can claim again in {minutes.ToString("N0")} minutes";
            embed.WithFooter($"initiated by {user} on {DateTime.UtcNow} UTC");
            return embed.Build();
        }
        
        public Embed PrepareBanEmbed(string user)
        {
            var embed = new EmbedBuilder();
            embed.Color = Color.Red;
            embed.Title = $"You're banned, no claims for you, sorry!";
            embed.WithFooter($"initiated by {user} on {DateTime.UtcNow} UTC");
            return embed.Build();
        }
        
        public Embed PrepareSignEmbed(APIPlayerData claim, double price, IUser user)
        {           
            var embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = $"{claim.ToString()} signed for your club!";
            embed.AddField(claim.rating.ToString(), claim.position);

            EmbedFieldBuilder country = new EmbedFieldBuilder
            {
                IsInline = true,
                Name = "Country",
                Value = claim.nation
            };

            EmbedFieldBuilder club = new EmbedFieldBuilder
            {
                IsInline = true,
                Name = "Club",
                Value = claim.club
            };

            EmbedFieldBuilder cost = new EmbedFieldBuilder
            {
                IsInline = false,
                Name = "Price",
                Value = price.ToString("N0")
            };

            EmbedFieldBuilder statsSeparator = new EmbedFieldBuilder
            {
                IsInline = false,
                Name = "** **",
                Value = "======================"
            };

            EmbedFieldBuilder stats1 = new EmbedFieldBuilder
            {
                IsInline = true,
                Name = "** **",
                Value = new StringBuilder().AppendLine($"{claim.pace}  **PAC**").AppendLine($"{claim.shooting}  **SHO**").AppendLine($"{claim.passing}  **PAS**").ToString()
            };

            EmbedFieldBuilder stats2 = new EmbedFieldBuilder
            {
                IsInline = true,
                Name = "** **",
                Value = new StringBuilder().AppendLine($"{claim.dribbling}  **DRI**").AppendLine($"{claim.defending}  **DEF**").Append($"{claim.physicality}  **PHY**").ToString()
            };

            embed.WithFields(country, club, cost, statsSeparator, stats1, stats2);
            embed.WithFooter($"initiated by {user.Username} on {DateTime.UtcNow} UTC");
            
            return embed.Build();            
        }
    }
}
