using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity;
using DSharpPlus.VoiceNext;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;
using DSharpPlus.Interactivity.Extensions;

namespace MinipcBot.Commands
{
    public class MusicModule1 : BaseCommandModule
    {
        [Command("Play")]
        public async Task PlayCommand(CommandContext ctx, [RemainingText] string song)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var voiceNext = ctx.Client.GetVoiceNext();

            // Ensure the user is in a voice channel
            if (ctx.Member.VoiceState?.Channel == null)
            {
                await ctx.RespondAsync("Вы должны находиться в голосовом канале, чтобы использовать эту команду!");
                return;
            }

            // Ensure the bot has necessary permissions
            var permissions = ctx.Member.VoiceState.Channel.PermissionsFor(ctx.Guild.CurrentMember);
            if (!permissions.HasPermission(Permissions.ManageChannels))
            {
                await ctx.RespondAsync("У меня нет разрешения подключаться к вашему голосовому каналу!");
                return;
            }

            // Get or join the voice connection
            var voiceConnection = voiceNext.GetConnection(ctx.Guild);
            if (voiceConnection == null)
            {
                voiceConnection = await voiceNext.ConnectAsync(ctx.Member.VoiceState.Channel);
                Console.WriteLine("Проверка вывода: " + ctx.Member.VoiceState.Channel.Name);
            }

            // Play the requested song
            await ctx.RespondAsync($"Playing {song}");
            await voiceConnection.SendSpeakingAsync(true);

            // Wait for the user to send "stop" command
            var result = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.User.Id && x.Content.ToLower() == "stop", TimeSpan.FromMinutes(10));

            if (result.TimedOut)
            {
                await voiceConnection.SendSpeakingAsync(false);
                voiceConnection.Disconnect();
                await ctx.RespondAsync("Проигрывание остановлено по таймауту.");
            }
            else
            {
                await voiceConnection.SendSpeakingAsync(false);
                voiceConnection.Disconnect();
                await ctx.RespondAsync("Проигрывание остановлено пользователем.");
            }
        }
    }
}




