using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OneeChan.Database;
using OneeChan.Services;
using Serilog;

namespace OneeChan
{
    internal class Program
    {
        private static string _logLevel;
        private DiscordSocketClient _client;
        private readonly IConfigurationRoot _config;

        public Program()
        {
            _config = BuildConfig();
        }

        private static void Main(string[] args = null)
        {
            if (args.Count() != 0) _logLevel = args[0];
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File("logs/OneeChan.log", rollingInterval: RollingInterval.Day)
                .WriteTo.Console()
                .CreateLogger();
            new Program().MainAsync().GetAwaiter().GetResult();
        }


        public async Task MainAsync()
        {
            using (var services = ConfigureServices())
            {
                var client = services.GetRequiredService<DiscordSocketClient>();
                _client = client;

                // Setup logging
                services.GetRequiredService<LoggingService>();

                // Initializing command Handler
                await services.GetRequiredService<CommandHandlingService>().InitializeAsync();

                services.GetRequiredService<HouseKeepingService>();

                await _client.LoginAsync(TokenType.Bot, _config["Discord:Token"]);
                await _client.StartAsync();

                // Block program until task is done.
                await Task.Delay(-1);
            }
        }

        private ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection()
                // Base
                .AddSingleton(_config)
                .AddDbContext<OneeChanEntities>()
                .AddSingleton<DiscordSocketClient>()
                .AddSingleton<CommandService>()
                .AddSingleton<CommandHandlingService>()
                // Logging
                .AddSingleton<LoggingService>()
                .AddLogging(configure => configure.AddSerilog())
                //Add additional services below
                .AddSingleton<HouseKeepingService>();

            if (!string.IsNullOrEmpty(_logLevel))
                switch (_logLevel.ToLower())
                {
                    case "info":
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);
                        break;
                    }
                    case "error":
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                        break;
                    }
                    case "debug":
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Debug);
                        break;
                    }
                    default:
                    {
                        services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Error);
                        break;
                    }
                }
            else
                services.Configure<LoggerFilterOptions>(options => options.MinLevel = LogLevel.Information);

            return services.BuildServiceProvider();
        }


        private IConfigurationRoot BuildConfig()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("config.json")
                .AddEnvironmentVariables("ONEECHAN_"); //TODO Properly handle secrets before even commiting
            return builder.Build();
        }
    }
}