using System;
using System.Collections.Generic;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Text;
using System.Threading.Tasks;
using MiniPC.Discord.Classes;

namespace MiniPC.Discord.Commands
{
    public class MisticCommands : BaseCommandModule
    {
        [Command("chance")]
        [HelpCategory("Etc")]
        [Description("Предсказывает шанс наступления события.")]
        public async Task PredictChance(CommandContext ctx, params string[] question)
        {
            // Генерируем случайное число от 0 до 100
            Random random = new Random();
            int chance = random.Next(101);

            // Формируем вопрос из массива строк
            string userQuestion = string.Join(" ", question);

            // Отправляем сообщение с предсказанием в чат
            await ctx.RespondAsync($"Вопрос: {userQuestion}\nШанс события: {chance}%");
        }
        
        private static Dictionary<ulong, List<(string Title, string Content)>> notes = new Dictionary<ulong, List<(string, string)>>();

        [Command("note")]
        [HelpCategory("Etc")]
        [Description("Создает или дополняет заметку.")]
        public async Task NoteCommand(CommandContext ctx, string title, params string[] content)
        {
            // Check if the user has notes
            if (!notes.ContainsKey(ctx.User.Id))
            {
                notes[ctx.User.Id] = new List<(string, string)>();
            }

            // Form the note content
            string noteContent = string.Join(" ", content);

            // Add the note to the user's notes
            notes[ctx.User.Id].Add((title, noteContent));

            // Send confirmation to the chat
            await ctx.RespondAsync($"Заметка сохранена: \"{title}\"  \"{noteContent}\"");
        }

        [Command("mynotes")]
        [HelpCategory("Etc")]
        [Description("Отображает все заметки пользователя.")]
        public async Task MyNotes(CommandContext ctx)
        {
            // Check if the user has notes
            if (!notes.ContainsKey(ctx.User.Id) || notes[ctx.User.Id].Count == 0)
            {
                await ctx.RespondAsync("У вас нет заметок.");
                return;
            }

            // Form the list of user's notes
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Ваши заметки:");

            for (int i = 0; i < notes[ctx.User.Id].Count; i++)
            {
                sb.AppendLine($"{i + 1}. {notes[ctx.User.Id][i].Title}");
            }

            // Send the list of notes to the chat
            await ctx.RespondAsync(sb.ToString());
        }

        [Command("notes")]
        [HelpCategory("Etc")]
        [Description("Отображает содержимое выбранной заметки или дополняет ее.")]
        public async Task ViewOrEditNote(CommandContext ctx, int noteNumber, params string[] content)
        {
            // Check if the user has notes
            if (!notes.ContainsKey(ctx.User.Id) || notes[ctx.User.Id].Count == 0)
            {
                await ctx.RespondAsync("У вас нет заметок.");
                return;
            }

            // Check if the specified note number exists
            if (noteNumber < 1 || noteNumber > notes[ctx.User.Id].Count)
            {
                await ctx.RespondAsync("Неверный номер заметки.");
                return;
            }

            // If no additional content provided, display the note content
            if (content.Length == 0)
            {
                await ctx.RespondAsync($"Заметка \"{notes[ctx.User.Id][noteNumber - 1].Title}\":\n{notes[ctx.User.Id][noteNumber - 1].Content}");
                return;
            }

            // Append or edit the note content
            string noteContent = string.Join(" ", content);
            notes[ctx.User.Id][noteNumber - 1] = (notes[ctx.User.Id][noteNumber - 1].Title, noteContent);

            // Send confirmation to the chat
            await ctx.RespondAsync($"Заметка \"{notes[ctx.User.Id][noteNumber - 1].Title}\" обновлена.");
        }

        [Command("deletenote")]
        [HelpCategory("Etc")]
        [Description("Удаляет заметку по номеру.")]
        public async Task DeleteNote(CommandContext ctx, int noteNumber)
        {
            // Check if the user has notes
            if (!notes.ContainsKey(ctx.User.Id) || notes[ctx.User.Id].Count == 0)
            {
                await ctx.RespondAsync("У вас нет заметок.");
                return;
            }

            // Check if the specified note number exists
            if (noteNumber < 1 || noteNumber > notes[ctx.User.Id].Count)
            {
                await ctx.RespondAsync("Неверный номер заметки.");
                return;
            }

            // Remove the note
            notes[ctx.User.Id].RemoveAt(noteNumber - 1);

            // Send confirmation to the chat
            await ctx.RespondAsync($"Заметка с номером {noteNumber} удалена.");
        }
    }
}
