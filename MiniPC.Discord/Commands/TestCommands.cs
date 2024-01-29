using System;
using System.Text;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using System.Collections.Generic;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using System.Linq;
using System.IO;

namespace MiniPC.Discord.Commands;

public class RussianRouletteGame
{
    private readonly DiscordChannel _channel;
    private readonly DiscordUser _player1;
    private readonly DiscordUser _player2;

    public RussianRouletteGame(DiscordChannel channel, DiscordUser player1, DiscordUser player2)
    {
        _channel = channel;
        _player1 = player1;
        _player2 = player2;
    }

    public async Task StartGameAsync()
    {
        Random random = new Random();

        // Выбираем случайно одного из игроков, который будет стрелять
        DiscordUser shooter = random.Next(2) == 0 ? _player1 : _player2;

        // Отправляем сообщение об этом игроке
        await _channel.SendMessageAsync($"{shooter.Username} стреляет!");

        // Имитируем задержку перед окончанием игры
        await Task.Delay(2000);

        // Если выстреленный игрок равен стрельцу, то он умирает
        if (_player1 == shooter || _player2 == shooter)
        {
            await _channel.SendMessageAsync($"{shooter.Username} умер!");
        }
        else
        {
            await _channel.SendMessageAsync($"{shooter.Username} выжил!");
        }
    }
}

public class TestCommands : BaseCommandModule
{
    public TestCommands() 
    {
        
    }

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

    [Command("roulette")]
    public async Task Roulette(CommandContext ctx, DiscordUser opponent)
    {
        if (opponent == ctx.User)
        {
            await ctx.RespondAsync("Вы не можете играть с самим собой!");
            return;
        }

        var interactivity = ctx.Client.GetInteractivity();
        var message = await ctx.RespondAsync($"Вы хотите выстрелить в {opponent.Username}? (Да/Нет)");

        var response = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel && x.Author == ctx.User);
        if (response.Result.Content.Equals("Да", StringComparison.OrdinalIgnoreCase))
        {
            var game = new RussianRouletteGame(ctx.Channel, ctx.User, opponent);
            await game.StartGameAsync();
        }
        else
        {
            await ctx.RespondAsync("Вы отказались от стрельбы.");
        }

        await message.DeleteAsync();
        await response.Result.DeleteAsync();
    }
    private List<string> players = new List<string>(); // Список участников игры

    [Command("присоединиться")]
    public async Task JoinMafia(CommandContext ctx)
    {
        // Проверяем, что игра еще не начата
        if (players.Count != 0)
        {
            await ctx.Channel.SendMessageAsync("Игра уже началась!");
            return;
        }

        // Проверяем, что игрок еще не в списке участников
        if (players.Contains(ctx.User.Username))
        {
            await ctx.Channel.SendMessageAsync("Ты уже в списке участников!");
            return;
        }

        // Добавляем игрока в список участников
        players.Add(ctx.User.Username);
        await ctx.Channel.SendMessageAsync($"{ctx.User.Mention} присоединился(ась) к игре!");

        // Если достаточно игроков, запускаем игру
        if (players.Count >= 3)
        {
            await StartMafia(ctx);
        }
    }

    [Command("стартмафия")]
    public async Task StartMafia(CommandContext ctx)
    {
        // Проверяем, что игра не начата еще
        if (players.Count == 0)
        {
            await ctx.Channel.SendMessageAsync("Подождите, еще нет достаточного количества игроков!");
            return;
        }

        // Проверяем, что игра не начата уже
        if (players.Count >= 3)
        {
            await ctx.Channel.SendMessageAsync("Игра уже началась!");
            return;
        }

        // Получаем список упоминаний игроков
        List<string> playerMentions = players.Select(x => ctx.Member.Mention).ToList();

        // Создаем новый список упоминаний с одним мафиози и одним доктором
        List<string> roles = new List<string> { "Мафиози", "Доктор" };
        roles.AddRange(Enumerable.Repeat("Мирный житель", players.Count - 2));

        // Отправляем всем сообщение с ролями
        for (int i = 0; i < players.Count; i++)
        {
            int index = new System.Random().Next(0, roles.Count);
            string role = roles[index];
            roles.RemoveAt(index);
            await ctx.Member.SendMessageAsync($"Вы - {role}!");
        }

        players.Clear(); // Очищаем список участников

        await ctx.Channel.SendMessageAsync(string.Join(", ", playerMentions) + ", игра началась!");
    }
}