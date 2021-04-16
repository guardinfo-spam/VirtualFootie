using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
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
                .AddJsonFile(path: "config.json");                

            _config = _builder.Build();

            int.TryParse(_config["PlayerSalePricePercentage"], out AppSettingsService._playerSalePercentage);

            AppSettingsService._ratingsConfig = _config.GetSection("Ratings").Get<List<RatingsConfig>>();
            int.TryParse(_config["ClaimFrequencyInHours"], out AppSettingsService._claimFrequencyInHours);
            CachePlayersWeightedData();
        }

        public void CachePlayersWeightedData()
        {
            var dBLayer = new DBLayer();
            var playersData = dBLayer.LoadAllPlayers().ToList();
            CacheService.playersWeightedData = new WeightedRandomGenerator();
            playersData.ForEach(p => CacheService.playersWeightedData.AddEntry(p, DetermineWeight(p.rating), DeterminePrice(p)));
        }        

        public double DetermineWeight(int rating)
        {
            var weight = AppSettingsService._ratingsConfig.Where(p => p.CatMin <= rating && p.CatMax >= rating).FirstOrDefault().ClaimChancePercentage;
            return weight;
        }

        public int DeterminePrice(APIPlayerData playerData)
        {
            var price = AppSettingsService._ratingsConfig.Where(p => p.CatMin <= playerData.rating && p.CatMax >= playerData.rating).FirstOrDefault().PricePerBS * playerData.BaseStats;
            return price;
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
