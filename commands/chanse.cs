using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using System.Text;

namespace MinipcBot.commands
{
 public class ChangeModule : BaseCommandModule
 {
  [Command("chance")]
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
 }
}