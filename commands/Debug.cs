using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace MinipcBot.commands
{
    public class DebugModule : BaseCommandModule
    {
        [Command("ping")]
        public async Task PingCommand(CommandContext ctx)
        {
            DateTime startTime = DateTime.Now;
            var msg = await ctx.RespondAsync($"WS Ping: {ctx.Client.Ping}");
            DateTime endTime = DateTime.Now;
            TimeSpan processingTime = endTime - startTime;
            await msg.ModifyAsync($"WS Ping: {ctx.Client.Ping} \n Обработка сообщение заняло: {processingTime.TotalMilliseconds} MS");
        }
    }

}
