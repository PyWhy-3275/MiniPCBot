using MiniPC.Discord;
using Log = Serilog.Log;

Log.Information("INITIALIZING BOT");
MiniBot bot = new();
#if true // Bot Switch
Log.Warning("Bot is turned on!");
bot.MainAsync().GetAwaiter().GetResult();
#endif
Log.Information("EXIT");