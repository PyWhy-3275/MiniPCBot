using MiniPC.Discord;
using MiniPC.Discord.Helpers;

Log.Info("INITIALIZING BOT");
MiniBot bot = new();
#if true // Bot Switch
Log.Warning("Bot is turned on!");
bot.MainAsync().GetAwaiter().GetResult();
#endif
Log.Info("EXIT");