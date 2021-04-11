using Discord;
using Discord.Net;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using VirtualFootieApp.Services;
using VFA.Lib.Support;
using System.Collections.Generic;
using VFA.Lib.Storage;

namespace VirtualFootieApp
{
    class Program
    {
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;
        private List<CardTypeClaimChance> _cardTypeClaimChances;
        private WeightedRandomGenerator<Player> _weightedData;

        public static void Main(string[] args)
         => new Program().MainAsync().GetAwaiter().GetResult();


        public Program()
        {            
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json")
                .AddJsonFile("players.json", optional: false, reloadOnChange: true);

            _config = _builder.Build();

            this.PrepareClaimChancesData();
        }

        public void PrepareClaimChancesData()
        {
            _cardTypeClaimChances = new List<CardTypeClaimChance>()
            {
                 new CardTypeClaimChance(Enums.CardCategory.Gold, 5),
                 new CardTypeClaimChance(Enums.CardCategory.Silver, 40),
                 new CardTypeClaimChance(Enums.CardCategory.Gold, 5),
                 new CardTypeClaimChance(Enums.CardCategory.Gold, 0.05)
            };
        }

        public void PrepareWeightedGenerator()
        {
            _weightedData = new WeightedRandomGenerator<Player>();
            var players = new PlayerData().GetAllPlayers();

            foreach ( var player in players )
            {
                //_weightedData.AddEntry(player, )
            }
        }



        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {                
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;
                
             
                client.Log += LogAsync;
                client.Ready += ReadyAsync;
                services.GetRequiredService<CommandService>().Log += LogAsync;

                await client.LoginAsync(TokenType.Bot, _config["Token"]);
                await client.StartAsync();

                await services.GetRequiredService<CommandHandler>().InitializeAsync();

                await Task.Delay(-1);
            }
        }

        private Task LogAsync(LogMessage log)
        {
            Console.WriteLine(log.ToString());
            return Task.CompletedTask;
        }

        private Task ReadyAsync()
        {
            Console.WriteLine($"Connected as -> [{_client.CurrentUser}] :)");
            return Task.CompletedTask;
        }

        private ServiceProvider ConfigureServices()
        {
            // this returns a ServiceProvider that is used later to call for those services
            // we can add types we have access to here, hence adding the new using statement:
            // using csharpi.Services;
            // the config we build is also added, which comes in handy for setting the command prefix!
            return new ServiceCollection()
                .AddSingleton(_config)
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandler>()
                .BuildServiceProvider();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
