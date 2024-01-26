using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace MinipcBot.commands
{
    public class ProfileModule : BaseCommandModule
    {
        [Command("profile")]
        [Description("Показывает профиль пользователя.")]
        public async Task CheckProfile(CommandContext ctx, DiscordUser user = null)
        {
            // If user is not provided, use the user who triggered the command
            user ??= ctx.User;

            // Get user's Discord member information
            var member = await ctx.Guild.GetMemberAsync(user.Id);

            // Build and send a message with user's profile information
            var profileEmbed = new DiscordEmbedBuilder
            {
                Title = $"{user.Username}'s Profile",
                Color = DiscordColor.Blue, // You can change the color as per your preference
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = user.AvatarUrl }
            };

            profileEmbed.AddField("Имя пользователя", user.Username, true);
            profileEmbed.AddField("Присоединился к Discord", user.CreationTimestamp.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"), true);

            if (member != null)
            {
                // Include additional information if the user is a member of the server
                profileEmbed.AddField("Присоединился к Серверу", member.JoinedAt.ToString("yyyy-MM-dd HH:mm:ss") ?? "Not in server", true);

                // Extract role names without IDs
                var roleNames = member.Roles.Select(r => r.Name);

                profileEmbed.AddField("Roles", string.Join(", ", roleNames), true);
            }

            await ctx.RespondAsync(embed: profileEmbed);
        }
    }
}