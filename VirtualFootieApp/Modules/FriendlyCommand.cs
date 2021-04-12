using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VFA.Lib;
using VFA.Lib.Support;
using VirtualFootieApp.Database;

namespace VirtualFootieApp.Modules
{
    public class FriendlyCommand : ModuleBase
    {
        [Command("friendly")]
        public async Task Friendly(string discord_id)
        {
            discord_id = discord_id.Replace("<@", string.Empty);
            discord_id = discord_id.Replace("!", string.Empty);
            discord_id = discord_id.Replace(">", string.Empty);
            
            var targetGuild = await Context.Guild.GetUserAsync(Convert.ToUInt64(discord_id));
            string target_handle = targetGuild.Username;

            var dBLayer = new DBLayer();
            var user = Context.User;

            var home11 = dBLayer.GetTeamForUser(user.Username).ToList();
            var target11 = new List<APIPlayerData>();

            int initiatorStats = Calculate11Strength(home11);

            if (initiatorStats == 0)
            {
                var errorEmbed = PrepareErrorEmbed("You do not have anyone in your team, use ;sign to start", user.Username);
                await ReplyAsync(null, false, errorEmbed);
                return;
            }

            int targetStats = 0;

            if (user.Username.ToLower().Equals(target_handle.ToLower()))
                targetStats = initiatorStats;
            else
            {
                target11 = dBLayer.GetTeamForUser(target_handle).ToList();
                targetStats = Calculate11Strength(target11);
            }

            if (targetStats == 0)
            {
                var errorEmbed = PrepareErrorEmbed("Your opponent does not have a team.", user.Username);
                await ReplyAsync(null, false, errorEmbed);
                return;
            }

            var result = new SimpleMatchEngine().DetermineWinner(initiatorStats, targetStats);
            var matchResult = GenerateGoals(result, home11, target11);

            var friendlyEmbed = PrepareFriendlyEmbed(user.Username, matchResult);
            await ReplyAsync(null, false, friendlyEmbed);

            //await ReplyAsync($"winner : {result}");
        }

        public Embed PrepareErrorEmbed(string message, string user)
        {
            var embed = new EmbedBuilder();
            embed.Color = Color.Red;
            embed.Title = "Result";            

            EmbedFieldBuilder result = new EmbedFieldBuilder
            {
                IsInline = false,
                Name = "Failed",
                Value = message
            };

            embed.WithFields(result);
            embed.WithFooter($"initiated by {user} on {DateTime.UtcNow} UTC");

            return embed.Build();
        }

        public int Calculate11Strength(IEnumerable<APIPlayerData> main11)
        {
            var result = 0;

            foreach ( var player in main11 )
            {
                result += player.defending.Value;
                result += player.dribbling.Value;
                result += player.passing.Value;
                result += player.physicality.Value;
                result += player.shooting.Value;
                result += player.pace.Value;
            }

            return result;
        }

        public Embed PrepareFriendlyEmbed(string user, MatchResult matchResult)
        {
            var embed = new EmbedBuilder();
            embed.Color = Color.Blue;
            embed.Title = "";
            //embed.AddField(claim.rating.ToString(), claim.position);

            EmbedFieldBuilder homeTeam = new EmbedFieldBuilder
            {
                IsInline = true,
                Name = "** **",
                Value = "team 1"
            };

            EmbedFieldBuilder score = new EmbedFieldBuilder
            {
                IsInline = true,
                Name = "** **",
                Value = $"{matchResult.HomeGoals}-{matchResult.AwayGoals}"
            };

            EmbedFieldBuilder awayTeam = new EmbedFieldBuilder
            {
                IsInline = true,
                Name = "** **",
                Value = "team 2"
            };

            EmbedFieldBuilder status = new EmbedFieldBuilder
            {
                IsInline = false,
                Name = "** **",
                Value = "Status - First Half"
            };

            EmbedFieldBuilder homegoals = new EmbedFieldBuilder
            {
                IsInline = true,
                Name = "Home",
                Value = PrepareGoalsListing(matchResult.HomeScorers)
            };

            EmbedFieldBuilder awaygoals = new EmbedFieldBuilder
            {
                IsInline = true,
                Name = "Away",
                Value = PrepareGoalsListing(matchResult.AwayScorers)
            };

            embed.WithFields(homeTeam, score, awayTeam, status, homegoals, awaygoals);
            embed.WithFooter($"initiated by {user} on {DateTime.UtcNow} UTC");

            return embed.Build();
        }

        public string PrepareGoalsListing(List<GoalScorers> goalScorers)
        {
            StringBuilder result = new StringBuilder();
            if ( goalScorers != null && goalScorers.Count > 0 )
            {
                foreach ( var goalData in goalScorers )
                {
                    result.AppendLine($"{goalData.Name} ({goalData.Minute})");
                }
            }
            else
            {
                result.Append("** **");
            }

            return result.ToString();
        }

        public MatchResult GenerateGoals(int result, List<APIPlayerData> homeTeam, List<APIPlayerData> awayTeam)
        {
            //0 means home team wins
            //2 means away team wins
            int endMinute = 90;

            //first each teams goals count
            var matchResult = new MatchResult();

            var randomGen = new Random();
            int homeGoalsCount;
            int awayGoalsCount;

            if ( result == 0 )
            {
                homeGoalsCount = randomGen.Next(1, 9);
                awayGoalsCount = 10;

                if ( homeGoalsCount == 1 )
                {
                    awayGoalsCount = 0;
                }
                else
                {
                    while (awayGoalsCount >= homeGoalsCount)
                    {
                        awayGoalsCount = randomGen.Next(0, homeGoalsCount - 1);
                    }
                }

                matchResult.HomeGoals = homeGoalsCount;
                matchResult.AwayGoals = awayGoalsCount;                
            }
            else
            {
                homeGoalsCount = 10;
                awayGoalsCount = randomGen.Next(1, 9);

                if (awayGoalsCount == 1)
                {
                    homeGoalsCount = 0;
                }
                else
                {
                    while (homeGoalsCount >= awayGoalsCount)
                    {
                        homeGoalsCount = randomGen.Next(0, awayGoalsCount - 1);
                    }
                }

                matchResult.HomeGoals = homeGoalsCount;
                matchResult.AwayGoals = awayGoalsCount;
            }

            //now the scorers

            if ( homeGoalsCount > 0 )
            {
                for ( int i = 0; i < homeGoalsCount; i++)
                {
                    int homeScorerID = randomGen.Next(0, homeTeam.Count() - 1);
                    while (homeTeam[homeScorerID].position.ToLower().Equals("gk"))
                    {
                        homeScorerID = randomGen.Next(0, homeTeam.Count() - 1);
                    }

                    int goalMinute = 0;
                    if (i == 0)
                    {
                        goalMinute = randomGen.Next(1, 60);
                    }
                    else
                    {
                        if (matchResult.HomeScorers[i - 1].Minute == endMinute) endMinute++;
                        goalMinute = randomGen.Next(matchResult.HomeScorers[i - 1].Minute + 1, endMinute);
                    }

                    matchResult.HomeScorers.Add(new GoalScorers { Name = homeTeam[homeScorerID].ToString(), Minute = goalMinute });
                }
            }

            endMinute = 90;

            if (awayGoalsCount > 0)
            {
                for (int i = 0; i < awayGoalsCount; i++)
                {
                    int awayScorerID = randomGen.Next(0, awayTeam.Count() - 1);
                    while (awayTeam[awayScorerID].position.ToLower().Equals("gk"))
                    {
                        awayScorerID = randomGen.Next(0, awayTeam.Count() - 1);
                    }

                    int goalMinute = 0;
                    if (i == 0)
                    {
                        goalMinute = randomGen.Next(1, 60);
                    }
                    else
                    {
                        if (matchResult.AwayScorers[i - 1].Minute == endMinute) endMinute++;
                        goalMinute = randomGen.Next(matchResult.AwayScorers[i - 1].Minute + 1, endMinute);
                    }

                    matchResult.AwayScorers.Add(new GoalScorers { Name = awayTeam[awayScorerID].ToString(), Minute = goalMinute });
                }
            }

            return matchResult;
        }        
    }
}
