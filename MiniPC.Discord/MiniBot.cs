using System;
using Serilog;
using DSharpPlus;
using DSharpPlus.Net;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.Logging;

using MiniPC.Discord.Helpers;
using MiniPC.Discord.Managers;
using MiniPC.Discord.Commands;

using Log = Serilog.Log;

namespace MiniPC.Discord
{
    internal class MiniBot
    {
        public static DiscordClient Client { get; private set; }

        private CommandsNextExtension Commands { get; set; }
        private SlashCommandsExtension _slashCommands;

        public DiscordActivity Activity { get; private set; }
        
        private string[] _words = { "jeb", "debil", "idiot", "kurw" }; //ToDo: from json

        private string[] _responses = { "No nieźle", "Fajnie", "nie bluźnij", "Chyba ty", "Nie obrażaj, kolego" };
        
        public MiniBot()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            var logFactory = new LoggerFactory().AddSerilog();
            
            Client = new DiscordClient(new DiscordConfiguration()
            {
                AutoReconnect = true,
                TokenType = TokenType.Bot,
                LoggerFactory = logFactory,
                Token = ConfigManager.Token,
                Intents = DiscordIntents.All,
                MinimumLogLevel = LogLevel.Debug,
            });

            this.Commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { ConfigManager.Prefix },
                EnableDms = ConfigManager.GetDms(),
            });
            
            Commands.SetHelpFormatter<HelpFormatter>();
            //Commands.RegisterCommands<MusicCommands>();
            Commands.RegisterCommands<AnimeCommands>();
            Commands.RegisterCommands<AdminCommands>();
            
            _slashCommands = Client.UseSlashCommands();
        }
        
        public async Task MainAsync()
        {
            Client.MessageCreated += async (s, e) =>
            {
                var rnd = new Random().Next(_responses.Length);

                foreach (var item in _words)
                {
                    if (e.Message.Content.ToLower().Contains(item)) //e.Message.Content.ToLower().StartsWith(item)
                    {
                        await e.Message.RespondAsync(_responses[rnd]).ConfigureAwait(false);
                    }
                }
            };
            #region LavaLink
            var endpoint = new ConnectionEndpoint
            {
                Hostname = "85.88.163.80",
                Port = 3128 //443,
            };

            var lavalinkConfig = new LavalinkConfiguration
            {
                Password = "saher.inzeworld.com", // youshallnotpass
                RestEndpoint = endpoint,
                SocketEndpoint = endpoint,
            };

            var lavalink = Client.UseLavalink();
            #endregion
            
            
            Client.Ready += async (s, e) =>
            {
                Log.Information("BOT IS READY");
                await s.UpdateStatusAsync(Activity);
                //await lavalink.ConnectAsync(lavalinkConfig);
                await EventManager.HandleEventsAsync();
            };

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}