using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using System.Text;

namespace MinipcBot.commands
{
    public class TestModule : BaseCommandModule
    {
        [Command("Test")]
        public async Task TestCommand(CommandContext ctx)
        {
            var message = await ctx.RespondAsync("React here!");
            var reations = await message.CollectReactionsAsync();

            var strBuilder = new StringBuilder();
            foreach (var reaction in reations)
            {
                strBuilder.AppendLine($"{reaction.Emoji}: {reaction.Total}");
            }

            await message.ModifyAsync(strBuilder.ToString());
        }
    }
}
