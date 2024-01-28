using System;
using System.Collections.Generic;
using System.Linq;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using MiniPC.Discord.Classes;

namespace MiniPC.Discord.Commands
{
    public class GamesCommands : BaseCommandModule
    {
        [Command("flip")]
        [HelpCategory("Games")]
        [Description("Играет в орел или решка с ботом.")]
        public async Task FlipCoin(CommandContext ctx)
        {
            // Генерируем случайное число (0 или 1)
            Random random = new Random();
            int result = random.Next(2);

            // Отправляем сообщение с результатом в чат
            await ctx.RespondAsync($"Выпало: {(result == 0 ? "Орел" : "Решка")}");
        }
        
        private static Dictionary<ulong, List<int>> playerHands = new Dictionary<ulong, List<int>>();
        private static Dictionary<ulong, ulong> gameInvites = new Dictionary<ulong, ulong>();
        
        [Command("invite")]
        [HelpCategory("Games")]
        [Description("Приглашает игрока к участию в игре 21 очко.")]
        public async Task InvitePlayer(CommandContext ctx, DiscordMember player)
        {
            // Check if the game has already started
            if (playerHands.ContainsKey(ctx.User.Id))
            {
                await ctx.RespondAsync("Игра уже начата.");
                return;
            }

            // Check if the player is inviting themselves
            if (ctx.User.Id == player.Id)
            {
                await ctx.RespondAsync("Вы не можете пригласить сами себя.");
                return;
            }

            // Check if the invited player has been invited by someone else
            if (gameInvites.ContainsValue(player.Id))
            {
                await ctx.RespondAsync("Этот игрок уже приглашен к другой игре.");
                return;
            }

            // Send an invitation message
            await ctx.RespondAsync($"{player.Username}, вы приглашены на игру 21 очко. Принять приглашение? (Да/Нет)");

            // Add the invite to the dictionary
            gameInvites[player.Id] = ctx.User.Id;
        }

        [Command("accept")]
        [HelpCategory("Games")]
        [Description("Принимает приглашение на игру 21 очко.")]
        public async Task AcceptInvite(CommandContext ctx)
        {
            // Check if the user has been invited
            if (!gameInvites.ContainsKey(ctx.User.Id))
            {
                await ctx.RespondAsync("У вас нет приглашений на игру.");
                return;
            }

            ulong inviterId = gameInvites[ctx.User.Id];

            // Start the game
            await StartGame(ctx, inviterId, ctx.User.Id);

            // Remove the invite
            gameInvites.Remove(ctx.User.Id);
        }

        [Command("decline")]
        [HelpCategory("Games")]
        [Description("Отклоняет приглашение на игру 21 очко.")]
        public async Task DeclineInvite(CommandContext ctx)
        {
            // Check if the user has been invited
            if (!gameInvites.ContainsKey(ctx.User.Id))
            {
                await ctx.RespondAsync("У вас нет приглашений на игру.");
                return;
            }

            // Remove the invite
            gameInvites.Remove(ctx.User.Id);
            await ctx.RespondAsync("Приглашение на игру отклонено.");
        }

        private async Task StartGame(CommandContext ctx, ulong player1Id, ulong player2Id)
        {
            // Initialize hands for both players
            playerHands[player1Id] = new List<int> { GetRandomCard(), GetRandomCard() };
            playerHands[player2Id] = new List<int> { GetRandomCard(), GetRandomCard() };

            // Send a message indicating the start of the game and the initial hands
            await ctx.RespondAsync($"Игра 21 очко начата между {ctx.Client.GetUserAsync(player1Id).Result.Username} и {ctx.Client.GetUserAsync(player2Id).Result.Username}.");
            await ctx.RespondAsync($"{ctx.Client.GetUserAsync(player1Id).Result.Username}: {GetHandAsString(playerHands[player1Id])}");
            await ctx.RespondAsync($"{ctx.Client.GetUserAsync(player2Id).Result.Username}: {GetHandAsString(playerHands[player2Id])}");

            // Player 1's turn
            await PlayerTurn(ctx, player1Id);

            // Player 2's turn
            await PlayerTurn(ctx, player2Id);

            // Determine the winner
            DetermineWinner(ctx, player1Id, player2Id);
        }

        private async Task PlayerTurn(CommandContext ctx, ulong playerId)
        {
          // Send a message indicating the player's turn
          await ctx.RespondAsync($"{ctx.Client.GetUserAsync(playerId).Result.Username}, ваш ход. Взять еще карту? (Да/Нет)");

          var interactivity = ctx.Client.GetInteractivity();
          var response = await interactivity.WaitForMessageAsync(
            x => x.Author.Id == playerId && (x.Content.ToLower() == "да" || x.Content.ToLower() == "нет"),
            TimeSpan.FromMinutes(1)); // Set a timeout for the response
          //if (response == null)
         // {
        //await ctx.RespondAsync($"Время ожидания ответа истекло. Игра прервана.");
       // return;
        //  }

            string answer = response.Result.Content.ToLower();

            if (answer == "да")
            {
                // Player chose to hit
                playerHands[playerId].Add(GetRandomCard());
                await ctx.RespondAsync($"{ctx.Client.GetUserAsync(playerId).Result.Username}, вы взяли карту. Текущие карты: {GetHandAsString(playerHands[playerId])}");

                // Check if the player busted (total points over 21)
                if (GetHandTotal(playerHands[playerId]) > 21)
                {
                    await ctx.RespondAsync($"{ctx.Client.GetUserAsync(playerId).Result.Username}, перебор! Вы проиграли.");
                    // You may want to end the game here or handle the situation accordingly
                }
                else
                {
                    // Continue the game with the next player's turn
                    await PlayerTurn(ctx, playerId);
                }
            }
            else if (answer == "нет")
            {
                // Player chose to stand
                await ctx.RespondAsync($"{ctx.Client.GetUserAsync(playerId).Result.Username}, вы решили не брать больше карт.");
            }
        }

        private void DetermineWinner(CommandContext ctx, ulong player1Id, ulong player2Id)
        {
            int player1Total = GetHandTotal(playerHands[player1Id]);
            int player2Total = GetHandTotal(playerHands[player2Id]);

            // Determine the winner
            if (player1Total > 21 && player2Total > 21)
            {
                // Both players busted, it's a draw
                ctx.RespondAsync("Оба игрока проиграли. Ничья!");
                playerHands.Remove(player1Id);
                playerHands.Remove(player2Id);
            }
            else if (player1Total > 21)
            {
                // Player 1 busted, player 2 wins
              ctx.RespondAsync($"{ctx.Client.GetUserAsync(player2Id).Result.Username} побеждает! {ctx.Client.GetUserAsync(player1Id).Result.Username} перебрал.");
              playerHands.Remove(player1Id);
              playerHands.Remove(player2Id);
            }
            else if (player2Total > 21)
            {
              // Player 2 busted, player 1 wins
              ctx.RespondAsync($"{ctx.Client.GetUserAsync(player1Id).Result.Username} побеждает! {ctx.Client.GetUserAsync(player2Id).Result.Username} перебрал.");
              playerHands.Remove(player1Id);
              playerHands.Remove(player2Id);
            }
            else if (player1Total > player2Total)
            {
              // Player 1 has more points, player 1 wins
              ctx.RespondAsync($"{ctx.Client.GetUserAsync(player1Id).Result.Username} побеждает! {ctx.Client.GetUserAsync(player2Id).Result.Username} имеет меньше очков.");
              playerHands.Remove(player1Id);
              playerHands.Remove(player2Id);
            }
            else if (player2Total > player1Total)
            {
              // Player 2 has more points, player 2 wins
              ctx.RespondAsync($"{ctx.Client.GetUserAsync(player2Id).Result.Username} побеждает! {ctx.Client.GetUserAsync(player1Id).Result.Username} имеет меньше очков.");
              playerHands.Remove(player1Id);
              playerHands.Remove(player2Id);
            }
            else
            {
              // Both players have the same points, it's a draw
              ctx.RespondAsync("Ничья! У обоих игроков одинаковое количество очков.");
              playerHands.Remove(player1Id);
              playerHands.Remove(player2Id);
            }
        }

        private int GetHandTotal(List<int> hand)
        {
          // Calculate the total points of a hand, treating Aces as 1 or 11
          int total = hand.Sum();

          // Check for Aces and adjust the total if needed
          int aceCount = hand.Count(card => card == 1);
          for (int i = 0; i < aceCount; i++)
          {
            if (total + 10 <= 21)
            {
              total += 10;
            }
          }

          return total;
        }

        private int GetRandomCard()
        {
          // Generates a random card value between 1 and 11
          Random random = new Random();
          return random.Next(1, 12);
        }

        private string GetHandAsString(List<int> hand)
        {
          // Converts a list of card values to a string
          return string.Join(", ", hand);
        }
        private readonly List<string> roles = new List<string>
        {
            "Мафия", "Мирный житель", "Доктор", "Детектив"
        };

        private readonly List<DiscordUser> joinedPlayers = new List<DiscordUser>();
        private readonly Dictionary<DiscordUser, string> playerRoles = new Dictionary<DiscordUser, string>();
        private readonly List<DiscordUser> alivePlayers = new List<DiscordUser>();
        private readonly Dictionary<DiscordUser, DateTime> lastActionTime = new Dictionary<DiscordUser, DateTime>();
        private bool gameInProgress = false;

        [Command("joinmafia")]
        [HelpCategory("Games")]
        [Description("Присоединяется к игре в мафию.")]
        public async Task JoinMafiaGame(CommandContext ctx)
        {
            if (gameInProgress)
            {
                await ctx.RespondAsync("Игра уже идет. Пожалуйста, подождите до следующей.");
                return;
            }

            if (joinedPlayers.Contains(ctx.User))
            {
                await ctx.RespondAsync("Вы уже присоединились к игре.");
                return;
            }

            joinedPlayers.Add(ctx.User);
            await ctx.RespondAsync($"{ctx.User.Username} присоединился к игре в мафию!");
        }

        [Command("рпс")]
        [HelpCategory("Games")]
        public async Task RockPaperScissors(CommandContext ctx, string playerChoice)
        {
            if (playerChoice == null)
            {
                await ctx.RespondAsync("Выберите вариант: `камень`, `бумага` или `ножницы`.");
                return;
            }
            
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
        
        [Command("startmafia")]
        [HelpCategory("Games")]
        [Description("Начинает игру в мафию.")]
        public async Task StartMafiaGame(CommandContext ctx)
        {
            if (gameInProgress || joinedPlayers.Count < 4)
            {
                await ctx.RespondAsync("Для начала игры в мафию нужно минимум 4 игрока, или игра уже идет.");
                return;
            }

            gameInProgress = true;

            var shuffledRoles = ShuffleRoles();

            for (int i = 0; i < joinedPlayers.Count; i++)
            {
                playerRoles.Add(joinedPlayers[i], shuffledRoles[i]);
                alivePlayers.Add(joinedPlayers[i]);
            }
            foreach (var player in playerRoles)
            {
                var user = player.Key;
                var role = player.Value;

                try
                {
                    var member = await ctx.Guild.GetMemberAsync(user.Id);
                    await member.SendMessageAsync($"Ваша роль в игре мафия: {role}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send a DM to {user.Username}: {ex.Message}");
                }
            }

            await ctx.RespondAsync("Роли были отправлены в личные сообщения. Игра началась!");

            await MainGameLoop(ctx);  // Add 'await' here
        }

        private async Task MainGameLoop(CommandContext ctx)
        {
            while (alivePlayers.Count > 1)
            {
                await NightPhase(ctx);
                await DayPhase(ctx);
            }

            var winner = alivePlayers.FirstOrDefault();
            await ctx.RespondAsync($"Игра окончена! Победил {winner.Mention} с ролью {playerRoles[winner]}!");
            gameInProgress = false;
        }

        private async Task NightPhase(CommandContext ctx)
        {
            try
            {
                await ctx.RespondAsync("Ночь наступила. Игроки, отправляйте свои ночные действия в личные сообщения.");
                foreach (var mafiaPlayer in playerRoles.Where(x => x.Value == "Мафия"))
                {
                    try
                    {
                        var targetNumbers = alivePlayers
                            .Where(player => player != mafiaPlayer.Key)
                            .Select((player, index) => $"{index + 1}. {player.Username}");

                        var targetsList = string.Join("\n", targetNumbers);
                        var targetMessage = $"Выберите номер игрока, которого вы хотите убить:\n{targetsList}";

                        var member = await ctx.Guild.GetMemberAsync(mafiaPlayer.Key.Id);
                        await member.SendMessageAsync(targetMessage);

                        // Add the player to the lastActionTime dictionary
                        lastActionTime[mafiaPlayer.Key] = DateTime.Now;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send a DM to {mafiaPlayer.Key.Username}: {ex.Message}");
                    }
                }

                await ctx.RespondAsync("Ночные действия в процессе. Подождите, пока мафия сделает свой выбор.");

                // 15 seconds delay for Mafia to make a decisi  on
                await Task.Delay(15000);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred during NightPhase: {ex.Message}");
            }
        }

        private async Task DayPhase(CommandContext ctx)
        {
            await ctx.RespondAsync("День наступил. Игроки, обсуждайте и голосуйте за подозрительных!");

            var voteOptions = alivePlayers
                .Select((player, index) => $"{index + 1}. {player.Username}");

            var voteMessage = $"Выберите номер игрока, за которого голосуете:\n{string.Join("\n", voteOptions)}";

            foreach (var player in alivePlayers)
            {
                try
                {
                    var member = await ctx.Guild.GetMemberAsync(player.Id);
                    await member.SendMessageAsync(voteMessage);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send a DM to {player.Username}: {ex.Message}");
                }
            }

            await ctx.RespondAsync("Голосование в процессе. Подождите, пока все игроки проголосуют.");
            await Task.Delay(20000); // 20 seconds delay for players to vote

            // Process votes and eliminate the player with the most votes
            var votes = new Dictionary<DiscordUser, int>();

            foreach (var player in alivePlayers)
            {
                var voteResult = await GetVoteResult(ctx, player);
                if (voteResult != null)
                {
                    if (votes.ContainsKey(voteResult))
                    {
                        votes[voteResult]++;
                    }
                    else
                    {
                        votes[voteResult] = 1;
                    }
                }
            }

            var playerWithMostVotes = votes.OrderByDescending(kv => kv.Value).FirstOrDefault().Key;

            if (playerWithMostVotes != null)
            {
                await ctx.RespondAsync($"{playerWithMostVotes.Mention} был исключен из игры по итогам голосования!");
                alivePlayers.Remove(playerWithMostVotes);
            }

            await Task.Delay(5000); // 5 seconds delay before transitioning to the next phase
        }

        private async Task<DiscordUser> GetVoteResult(CommandContext ctx, DiscordUser player)
        {
            try
            {
                var member = await ctx.Guild.GetMemberAsync(player.Id);
                var dmChannel = await member.CreateDmChannelAsync();

                var messages = await dmChannel.GetMessagesAsync(limit: 1);
                var voteMessage = messages.FirstOrDefault();

                if (voteMessage != null && int.TryParse(voteMessage.Content, out int voteNumber)
                    && voteNumber >= 1 && voteNumber <= alivePlayers.Count)
                {
                    return alivePlayers[voteNumber - 1];
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to get vote result for {player.Username}: {ex.Message}");
            }

            return null;
        }

        private List<string> ShuffleRoles()
        {
            Random random = new Random();
            return roles.OrderBy(x => random.Next()).ToList();
        }
    }

    public static class Extensions
    {
        public static T RandomElement<T>(this IList<T> list, Random random = null)
        {
            random ??= new Random();
            return list[random.Next(list.Count)];
        }
    }
    
}
    

        

