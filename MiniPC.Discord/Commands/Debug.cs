using System;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace MiniPC.Discord.Commands
{
    public class Debug : BaseCommandModule
    {
        [Command("ping")]
        public async Task PingCommand(CommandContext ctx)
        {
            var startTime = DateTime.Now;
            var msg = await ctx.RespondAsync($"WS Ping: {ctx.Client.Ping}");
            var endTime = DateTime.Now;
            var processingTime = endTime - startTime;
            await msg.ModifyAsync($"WS Ping: {ctx.Client.Ping} \n Обработка сообщение заняло: {processingTime.TotalMilliseconds} MS");
        }
    }

}