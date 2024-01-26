using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System;

namespace MinipcBot.commands
{
    public class GameModule : BaseCommandModule
    {
        [Command("рпс")]
        [Description("Играет в камень, ножницы, бумага с ботом.")]
        public async Task RockPaperScissors(CommandContext ctx)
        {
            // Provide available options to the user
            await ctx.RespondAsync("Выберите вариант: `камень`, `бумага` или `ножницы`.");
        }

        [Command("рпс")]
        public async Task RockPaperScissors(CommandContext ctx, string playerChoice)
        {
            // Convert player's choice to lowercase for case-insensitivity
            playerChoice = playerChoice.ToLower();

            // Validate the player's choice
            if (playerChoice != "камень" && playerChoice != "бумага" && playerChoice != "ножницы")
            {
                await ctx.RespondAsync("Пожалуйста, выберите между `камень`, `бумага` или `ножницы`.");
                return;
            }

            // Generate a random choice for the bot
            Random random = new Random();
            int botChoiceIndex = random.Next(3);
            string[] choices = { "камень", "бумага", "ножницы" };
            string botChoice = choices[botChoiceIndex];

            // Determine the winner
            string result = GetGameResult(playerChoice, botChoice);

            // Build and send a message with the result
            await ctx.RespondAsync($"Вы выбрали: {playerChoice}\nБот выбрал: {botChoice}\n{result}");
        }

        // Helper method to determine the game result
        private string GetGameResult(string playerChoice, string botChoice)
        {
            if (playerChoice == botChoice)
            {
                return "Ничья!";
            }
            else if ((playerChoice == "камень" && botChoice == "ножницы") ||
                (playerChoice == "бумага" && botChoice == "камень") ||
                (playerChoice == "ножницы" && botChoice == "бумага"))
            {
                return "Вы выиграли!";
            }
            else
            {
                return "Бот выиграл!";
            }
        }
    }
}
