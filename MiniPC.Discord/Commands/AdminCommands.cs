using System;
using System.Linq;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

using Log = Serilog.Log;

namespace MiniPC.Discord.Commands;

public class AdminCommands : BaseCommandModule
{
    [Command("globalmute")]
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
                        Console.WriteLine(e);
                        throw;
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
}