using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace MinipcBot.commands
{
   // public class OnlineMembersModule : BaseCommandModule
   // {
        //[Command("online")]
       // [Description("Показывает количество пользователей онлайн и их никнеймы.")]
        //public async Task OnlineMembers(CommandContext ctx)
       // {
            // Get the list of members who are online
            //var onlineMembers = ctx.Guild.Members.Where(member => member.Presence?.Status == UserStatus.Online);

            // Get the count of online members
            //int onlineCount = onlineMembers.Count();

            // Build the message with online members' nicknames
            //string message = $"Количество пользователей онлайн: {onlineCount}\n";

            // Append the nicknames of online members to the message
           // foreach (var member in onlineMembers)
            //{
                // Check if the member has a nickname set, otherwise use their username
              //  string nickname = !string.IsNullOrEmpty(member.Nickname) ? member.Nickname : member.Username;
              //  message += $"{nickname}\n";
           // }

            // Send the message with online members' information
           // await ctx.RespondAsync(message);
        //}
    //}
}