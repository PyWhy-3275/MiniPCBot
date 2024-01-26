using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;
using MinipcBot.commands;
using System;
using System.Threading.Tasks;
using MinipcBot.Commands;

namespace MinipcBot
{
    class Program
    {
        static async Task Main(string[] args) 
        {
            var discord = new DiscordClient(new DiscordConfiguration()
            {
                Token = "MTIwMDA1MjgxOTA4MDc5NDE5Mg.GXy4FK.QuKb_jvOS9VyeikocCHeTHeKXni5w8m_cIcf5M",
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged | DiscordIntents.MessageContents,
                MinimumLogLevel = LogLevel.Debug
            });
            discord.UseInteractivity(new InteractivityConfiguration()
            {
                PollBehaviour = PollBehaviour.KeepEmojis,
                Timeout = TimeSpan.FromSeconds(30)
            });
            var commands = discord.UseCommandsNext(new CommandsNextConfiguration()
            {
                StringPrefixes= new[] {"!"}
            });


            commands.RegisterCommands<DebugModule>();
            commands.RegisterCommands<TestModule>();
            commands.RegisterCommands<DeleteMessagesCommandModule>();
            commands.RegisterCommands<MusicModule1>();
            commands.RegisterCommands<FlipModule>();
            commands.RegisterCommands<ChangeModule>();
            commands.RegisterCommands<NoteModule>();
            commands.RegisterCommands<ProfileModule>();
            commands.RegisterCommands<GameModule>();
           // commands.RegisterCommands<OnlineMembersModuled>();
            commands.RegisterCommands<BlackjackModule>();

            await discord.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}