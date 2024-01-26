using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinipcBot.commands
{
    public class BlackjackModule : BaseCommandModule
    {
        private static Dictionary<ulong, List<int>> playerHands = new Dictionary<ulong, List<int>>();

        [Command("invite")]
        [Description("Приглашает игрока к участию в игре 21 очко.")]
        public async Task InvitePlayer(CommandContext ctx, DiscordMember player)
        {
            // Check if the game has already started
            if (playerHands.ContainsKey(ctx.Guild.Id))
            {
                await ctx.RespondAsync("Игра уже начата. Нельзя пригласить нового игрока.");
                return;
            }

            // Initialize the player's hand with two random cards
            List<int> hand = new List<int> { GetRandomCard(), GetRandomCard() };
            playerHands[player.Id] = hand;

            // Send an invitation message
            await ctx.RespondAsync($"Игрок {player.Username} присоединился к игре 21 очко. Его карты: {GetHandAsString(hand)}");
        }

        [Command("startgame")]
        [Description("Начинает игру 21 очко.")]
        public async Task StartGame(CommandContext ctx)
        {
            // Check if the game has already started
            if (playerHands.ContainsKey(ctx.Guild.Id))
            {
                await ctx.RespondAsync("Игра уже начата.");
                return;
            }

            // Initialize hands for all players with two random cards
            List<ulong> players = ctx.Guild.Members.Select(member => member.Id).ToList();
            foreach (var playerId in players)
            {
                playerHands[playerId] = new List<int> { GetRandomCard(), GetRandomCard() };
            }

            // Send a message indicating the start of the game
            await ctx.RespondAsync("Игра 21 очко начата! Ваши карты: ");
            foreach (var playerId in players)
            {
                var player = await ctx.Guild.GetMemberAsync(playerId);
                await ctx.RespondAsync($"{player.Username}: {GetHandAsString(playerHands[playerId])}");
            }

            // Determine the winner and handle busts
            var winners = DetermineWinners();
            await ctx.RespondAsync($"Победители: {string.Join(", ", winners.Select(w => ctx.Guild.GetMemberAsync(w).Result.Username))}");
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

        private List<ulong> DetermineWinners()
        {
            // Determine the winners based on the total value of their hands
            int maxScore = 0;
            List<ulong> winners = new List<ulong>();

            foreach (var player in playerHands)
            {
                int totalScore = player.Value.Sum();

                // Check for bust
                if (totalScore <= 21)
                {
                    if (totalScore > maxScore)
                    {
                        maxScore = totalScore;
                        winners.Clear();
                        winners.Add(player.Key);
                    }
                    else if (totalScore == maxScore)
                    {
                        winners.Add(player.Key);
                    }
                }
            }

            return winners;
        }
    }
}