using System;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.Lavalink;
using DSharpPlus.Net;
using DSharpPlus.SlashCommands;
using MiniPC.Discord.Commands;
using MiniPC.Discord.Helpers;
using MiniPC.Discord.Managers;

namespace MiniPC.Discord
{
    internal class MiniBot
    {
        public static DiscordClient Client { get; private set; }
        
        public CommandsNextExtension Commands { get; private set; }
        private SlashCommandsExtension _slashCommands;
        
        public static CommandContext commandContext;
        
        public DiscordActivity Activity { get; private set; }
        
        private string[] words = { "jeb", "debil", "idiot", "kurw" }; //ToDo: from json

        private string[] responses = { "No nieźle", "Fajnie", "nie bluźnij", "Chyba ty", "Nie obrażaj, kolego" };
        
        public MiniBot()
        {
            Client = new DiscordClient(new DiscordConfiguration()
            {
                Token = ConfigManager.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents
            });

            this.Commands = Client.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes = new[] { ConfigManager.Prefix },
                EnableDms = ConfigManager.GetDms(),
            });
            
            Commands.SetHelpFormatter<HelpFormatter>();
            //Commands.RegisterCommands<MusicCommands>();
            Commands.RegisterCommands<AnimeCommands>();
            
            _slashCommands = Client.UseSlashCommands();
        }
        
        public async Task MainAsync()
        {
            Client.MessageCreated += async (s, e) =>
            {
                var rnd = new Random().Next(responses.Length);

                foreach (var item in words)
                {
                    if (e.Message.Content.ToLower().Contains(item)) //e.Message.Content.ToLower().StartsWith(item)
                    {
                        await e.Message.RespondAsync(responses[rnd]).ConfigureAwait(false);
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
                Log.Info("BOT IS READY");
                await s.UpdateStatusAsync(Activity);
                //await lavalink.ConnectAsync(lavalinkConfig);
                await EventManager.HandleEventsAsync();
            };

            await Client.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}