using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VFA.Lib.Storage;
using VFA.Lib.Support;
using VirtualFootieApp.Database;
using VirtualFootieApp.Services;

namespace VirtualFootieApp
{
    class Program
    {
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;
        private List<CardTypeClaimChance> _cardTypeClaimChances;
        private WeightedRandomGenerator<APIPlayerData> _weightedData;
        private List<APIPlayerData> _allPlayersData;

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
            this.CacheData();
        }

        public void CacheData()
        {
            _allPlayersData = new DBLayer().LoadAllPlayers().ToList();
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
            _weightedData = new WeightedRandomGenerator<APIPlayerData>();
            
            foreach ( var player in _allPlayersData )
            {
                _weightedData.AddEntry(player, DetermineWeight(player.rating.HasValue?player.rating.Value : 50));
            }

            _allPlayersData = null;
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
