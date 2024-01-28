using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace MiniPC.Discord.Commands;

public class TestCommands : BaseCommandModule
{
    private static readonly string OriginalAnimeName = "test"; // Название аниме (заглушка)

    [Command("guessanime")]
    public async Task GuessAnime(CommandContext ctx)
    {
        var embedBuilder = new DiscordEmbedBuilder()
            .WithTitle("Угадай название аниме")
            .WithDescription("Отгадай название аниме по кадру");

        var embed = await ctx.RespondAsync(embed: embedBuilder.Build());
       

        var hiddenChars = new char[OriginalAnimeName.Length];

        for (int i = 0; i < OriginalAnimeName.Length; i++)
        {
            hiddenChars[i] = OriginalAnimeName[i] == '_' ? '_' : '*';
        }

        var revealedChars = 0;

        var msg = await ctx.RespondAsync(embed: new DiscordEmbedBuilder().WithDescription(new string(hiddenChars)).Build());
        var msgId = msg.Id;
       

        while (true)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var resp = await interactivity.WaitForMessageAsync(x => x.Channel.Id == ctx.Channel.Id && x.Author.Id == ctx.User.Id, TimeSpan.FromMinutes(1));

            if (resp.TimedOut)
            {
                await ctx.RespondAsync("Время истекло! Проигрыш!");
            
                break;
            }

            var guessedAnswer = resp.Result.Content.ToLower();

            bool revealed = false;

            if (guessedAnswer == OriginalAnimeName.ToLower())
            {
                await ctx.RespondAsync($"Поздравляем! Вы угадали! Название аниме: {OriginalAnimeName}");
             
                break; // Exit the loop after the correct answer
            }
            else if (guessedAnswer.Length == 1 && char.IsLetter(guessedAnswer[0]))
            {
                // Пользователь ввел одну букву, проверяем соответствие
                for (int i = 0; i < OriginalAnimeName.Length; i++)
                {
                    if (OriginalAnimeName[i] == guessedAnswer[0] && hiddenChars[i] == '*')
                    {
                        hiddenChars[i] = guessedAnswer[0];
                        revealedChars++;
                        revealed = true;
                    }
                }
            }
            else
            {
                // Вывести сообщение о проигрыше
                await ctx.RespondAsync("Вы не угадали! Проигрыш!");
            
                break;
            }

            if (revealed)
            {
                var upd = new DiscordEmbedBuilder().WithDescription(new string(hiddenChars)).Build();
                var message = await ctx.Channel.GetMessageAsync(msgId);

                if (message is DiscordMessage discordMessage)
                {
                    await discordMessage.ModifyAsync(embed: upd);
                   
                }
            }

            if (revealedChars == OriginalAnimeName.Length)
            {
                await ctx.RespondAsync($"Поздравляем! Вы угадали! Название аниме: {OriginalAnimeName}");
            
                break;
            }
        }
    }
    
    [Command("answer")]
    public async Task Answer(CommandContext ctx, [RemainingText] string guessedAnswer)
    {
        if (string.IsNullOrEmpty(guessedAnswer))
        {
            await ctx.RespondAsync("Пожалуйста, введите ответ на загадку.");
            return;
        }

        await ProcessAnswer(ctx, guessedAnswer);
    }
    
    private async Task ProcessAnswer(CommandContext ctx, string guessedAnswer)
    {
        var hiddenChars = new char[OriginalAnimeName.Length];

        for (int i = 0; i < OriginalAnimeName.Length; i++)
        {
            hiddenChars[i] = OriginalAnimeName[i] == '_' ? '_' : '*';
        }

        var revealedChars = 0;

        bool revealed = false;

        if (guessedAnswer == OriginalAnimeName.ToLower())
        {
            await ctx.RespondAsync($"Поздравляем! Вы угадали! Название аниме: {OriginalAnimeName}");

            return;
        }
        else if (guessedAnswer.Length == 1 && char.IsLetter(guessedAnswer[0]))
        {
            // Пользователь ввел одну букву, проверяем соответствие
            for (int i = 0; i < OriginalAnimeName.Length; i++)
            {
                if (OriginalAnimeName[i] == guessedAnswer[0] && hiddenChars[i] == '*')
                {
                    hiddenChars[i] = guessedAnswer[0];
                    revealedChars++;
                    revealed = true;
                }
            }
        }
        else
        {
            // Вывести сообщение о проигрыше
            await ctx.RespondAsync("Вы не угадали! Проигрыш!");

            return;
        }

        if (revealed)
        {
            var upd = new DiscordEmbedBuilder().WithDescription(new string(hiddenChars)).Build();
            await ctx.RespondAsync(embed: upd);

        }

        if (revealedChars == OriginalAnimeName.Length)
        {
            await ctx.RespondAsync($"Поздравляем! Вы угадали! Название аниме: {OriginalAnimeName}");

            return;
        }
        
    }
    private static Dictionary<int, string> roleplayProcesses = new Dictionary<int, string>();

    [Command("addrp")]
    [Description("Добавляет новый ролевой процесс.")]
    public async Task AddRoleplayProcess(CommandContext ctx, [RemainingText] string process)
    {
        int processId = roleplayProcesses.Count + 1; // Генерируем уникальный идентификатор для процесса
        roleplayProcesses.Add(processId, process);
        await ctx.RespondAsync($"Ролевой процесс добавлен (ID: {processId}): {process}");
    }

    [Command("listrp")]
    [Description("Выводит список текущих ролевых процессов.")]
    public async Task ListRoleplayProcesses(CommandContext ctx)
    {
        if (roleplayProcesses.Count > 0)
        {
            var processesList = new StringBuilder();
            foreach (var process in roleplayProcesses)
            {
                processesList.AppendLine($"ID: {process.Key}, Процесс: {process.Value}");
            }
            await ctx.RespondAsync($"Текущие ролевые процессы:\n{processesList.ToString()}");
        }
        else
        {
            await ctx.RespondAsync("На данный момент нет активных ролевых процессов.");
        }
    }

    [Command("delrp")]
    [Description("Удаляет ролевой процесс по его ID.")]
    public async Task DeleteRoleplayProcess(CommandContext ctx, int processId)
    {
        if (roleplayProcesses.ContainsKey(processId))
        {
            roleplayProcesses.Remove(processId);
            await ctx.RespondAsync($"Ролевой процесс с ID {processId} удален.");
        }
        else
        {
            await ctx.RespondAsync($"Ролевой процесс с ID {processId} не найден.");
        }
    }
}