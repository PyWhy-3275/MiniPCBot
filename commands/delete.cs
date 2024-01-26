using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;

namespace MinipcBot.commands
{
    public class DeleteMessagesCommandModule : BaseCommandModule
    {
        [Command("delete")]
        [Description("Удаляет сообщения в чате.")]
        [RequirePermissions(Permissions.ManageMessages)]
        public async Task DeleteMessages(CommandContext ctx, [Description("Количество удаляемых сообщений (максимум 100).")] int count)
        {
            // Validate the count to be between 1 and 100
            if (count < 1 || count > 100)
            {
                await ctx.RespondAsync("Пожалуйста, укажите количество сообщений для удаления от 1 до 100.");
                return;
            }

            // Удаляем сообщения
            var messages = await ctx.Channel.GetMessagesAsync(count + 1); // +1 для удаления команды
            await ctx.Channel.DeleteMessagesAsync(messages);

            // Отправляем подтверждение удаления в чат
            var confirmationMessage = await ctx.RespondAsync($"Удалено {count} сообщений.");

            // Удаляем подтверждение через 3 секунды
            await Task.Delay(3000);
            await confirmationMessage.DeleteAsync();
        }
    }
}