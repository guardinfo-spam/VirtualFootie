﻿using Discord;
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

            var claim = CacheService.playersWeightedData.GetRandom();
            int userID = dBLayer.GetUserIDByDiscordHandle(user.Username);
            var result = dBLayer.AddClaimToUser(userID, claim.PlayerID);

            var claimEmbed = PrepareSignEmbed(claim, user);
            await ReplyAsync(null, false, claimEmbed);
        }

        public Embed PrepareSignEmbed(APIPlayerData claim, IUser user)
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

            embed.WithFields(country, club,statsSeparator, stats1, stats2);
            embed.WithFooter($"claimed by {user.Username} on {DateTime.UtcNow} UTC");
            
            return embed.Build();            
        }
    }
}