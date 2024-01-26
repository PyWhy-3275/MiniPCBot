using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using System.Text;

namespace MinipcBot.commands
{
    public class FlipModule : BaseCommandModule
    {
        [Command("flip")]
        [Description("Играет в орел или решка с ботом.")]
        public async Task FlipCoin(CommandContext ctx)
        {
            // Генерируем случайное число (0 или 1)
            Random random = new Random();
            int result = random.Next(2);

            // Отправляем сообщение с результатом в чат
            await ctx.RespondAsync($"Выпало: {(result == 0 ? "Орел" : "Решка")}");
        }
    }
}