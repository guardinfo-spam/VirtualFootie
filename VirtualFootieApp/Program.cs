using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;
using VFA.Lib;
using VFA.Lib.Support;
using VirtualFootieApp.Database;
using VirtualFootieApp.Services;

namespace VirtualFootieApp
{
    class Program
    {
        private readonly IConfiguration _config;
        private DiscordSocketClient _client;
        
        public IServiceProvider BuildProvider() => new ServiceCollection().AddSingleton<CacheService>().BuildServiceProvider();

        public static void Main(string[] args)
         => new Program().MainAsync().GetAwaiter().GetResult();


        public Program()
        {            
            var _builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile(path: "config.json")
                .AddJsonFile("players.json", optional: false, reloadOnChange: true);

            _config = _builder.Build();

            CachedPlayersWeightedData();
        }

        public void CachedPlayersWeightedData()
        {
            var dBLayer = new DBLayer();
            var playersData = dBLayer.LoadAllPlayers().ToList();
            CacheService.playersWeightedData = new WeightedRandomGenerator<APIPlayerData>();
            playersData.ForEach(p => CacheService.playersWeightedData.AddEntry(p, DetermineWeight(p.rating.HasValue ? p.rating.Value : 50), DeterminePrice(p)));
        }        

        public double DetermineWeight(int rating)
        {
            if (rating >= 95 && rating < 100) return 0.05;
            if (rating >= 90 && rating < 95) return 0.2;
            if (rating >= 85 && rating < 90) return 0.75;
            if (rating >= 80 && rating < 85) return 5;
            if (rating >= 75 && rating < 80) return 28;
            
            if (rating >= 70 && rating < 75) return 26;
            if (rating >= 65 && rating < 70) return 20;
            if (rating >= 60 && rating < 65) return 15;
            if (rating >= 45 && rating < 60) return 5;

            return 20;
        }

        public int DeterminePrice(APIPlayerData playerData)
        {
            return 1000;
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
                .AddScoped<IMatchEngine, SimpleMatchEngine>()
                .BuildServiceProvider();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
