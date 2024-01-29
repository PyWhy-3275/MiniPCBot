using System;
using DSharpPlus;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using MiniPC.Discord.Classes;

using Log = Serilog.Log;

namespace MiniPC.Discord.Commands;

public class AdminCommands : BaseCommandModule
{
    [Command("mute-global")]
    [HelpCategory("Admins")]
    [Description("Мутит пользователя на всех серверах, где присутствует бот.")]
    public async Task GlobalMute(CommandContext ctx, string username, int muteTimeInMinutes)
    {
        // Получаем список серверов, на которых присутствует бот
        var botServers = ctx.Client.Guilds.Values;

        // Поиск пользователя по его никнейму
        var discordGuilds = botServers.ToList();
        var user = discordGuilds.SelectMany(guild => guild.Members.Values)
            .FirstOrDefault(member => member.Username == username);

        if (user != null)
        {
            // Мут пользователя на каждом сервере
            try
            {
                foreach (var guild in discordGuilds)
                {
                    var member = await guild.GetMemberAsync(user.Id);
                    try
                    {
                        await member.SetMuteAsync(true); // Мут пользователя
                    }
                    catch (Exception e)
                    {
                        Log.Error($"{e}");
                    }
                    // await member.RemoveAllRolesAsync(); // Удаление всех ролей у пользователя
                    break;
                }
            }
            catch (Exception msg)
            {
                Log.Error($"{msg}");
            }
            await ctx.RespondAsync($"Пользователь {username} был замучен на всех серверах на время {muteTimeInMinutes} минут.");

            // Ожидание заданного времени мута
            await Task.Delay(muteTimeInMinutes * 60000);

            // Размут пользователя на каждом сервере
            foreach (var guild in discordGuilds)
            {
                var member = await guild.GetMemberAsync(user.Id);
                await member.SetMuteAsync(false); // Размут пользователя
            }

            await ctx.RespondAsync($"Пользователь {username} был размучен на всех серверах.");
        }
        else
        {
            await ctx.RespondAsync("Пользователь не найден.");
        }
    }
    
    [Command("delete")]
    [HelpCategory("Admins")]
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
    
    [Command("роль")]
    [HelpCategory("Admins")]
    [Description("Создает роль на определенном сервере")]
    public async Task CreateRole(CommandContext ctx, ulong guildId, [RemainingText] string roleName)
    {
        try
        {
            // Получаем сервер по его ID
            var guild = await ctx.Client.GetGuildAsync(guildId);
            if (guild != null)
            {
                // Создаем роль с указанным именем
                var role = await guild.CreateRoleAsync(roleName);
                await ctx.RespondAsync($"Роль \"{role.Name}\" успешно создана на сервере \"{guild.Name}\"");
            }
            else
            {
                await ctx.RespondAsync($"Сервер с ID \"{guildId}\" не найден");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            await ctx.RespondAsync("Произошла ошибка при создании роли");
        }
    }
}