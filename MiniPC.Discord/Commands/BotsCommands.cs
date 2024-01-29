using System.Linq;
using System.Text;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using MiniPC.Discord.Classes;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;

namespace MiniPC.Discord.Commands
{
   public class BotCommands : BaseCommandModule
   {
        [Command("online")]
        [Description("Показывает список пользователей, которые в данный момент находятся в сети на сервере.")]
        public async Task OnlineMembers(CommandContext ctx)
        {
            //Get the list of human members who are online
            var onlineMembers = ctx.Guild.Members.Values.Where(member => !member.IsBot && member.Presence?.Status == UserStatus.Online);
            // Build the message with online human members' usernames
            string message = "Пользователи, находящиеся в данный момент в сети на сервере:\n";
            // Append the usernames of online human members to the message
            foreach (var member in onlineMembers)
            {
                
                message += $"{member.Guild.Name}/{member.Nickname ?? member.Username}\n"; // Use the nickname if available, or the username if not
            }

            // Send the message with online human members' usernames
            await ctx.RespondAsync(message);
        }

       [Command("profile")]
       [HelpCategory("Bot")]
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
           profileEmbed.AddField("Присоединился к Discord",
               user.CreationTimestamp.UtcDateTime.ToString("yyyy-MM-dd HH:mm:ss"), true);

           if (member != null)
           {
               // Include additional information if the user is a member of the server
               profileEmbed.AddField("Присоединился к Серверу",
                   member.JoinedAt.ToString("yyyy-MM-dd HH:mm:ss") ?? "Not in server", true);

               // Extract role names without IDs
               var roleNames = member.Roles.Select(r => r.Name);

               profileEmbed.AddField("Roles", string.Join(", ", roleNames), true);
           }

           await ctx.RespondAsync(embed: profileEmbed);
       }
       [Command("servers")]
       [HelpCategory("Bot")]
       [Description("Показывает на каких серверах есть бот и их ID.")]
       public async Task ListServers(CommandContext ctx)
       {
           // Получаем список серверов, на которых присутствует бот
           var botServers = ctx.Client.Guilds.Values;
           // Создаем строку для отображения информации о серверах
           StringBuilder serverInfo = new StringBuilder();
           serverInfo.Append("Серверы, на которых есть бот:\n");
           // Добавляем информацию о каждом сервере в строку
           foreach (var guild in botServers)
           {
               serverInfo.AppendLine($"- {guild.Name} (ID: {guild.Id})");
           }
           // Отправляем сообщение с информацией о серверах
           await ctx.RespondAsync(serverInfo.ToString());
       }
   }
}