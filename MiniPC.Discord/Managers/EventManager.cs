﻿using DSharpPlus;
using System.Threading.Tasks;

using Log = Serilog.Log;

namespace MiniPC.Discord.Managers;

public static class EventManager
{
    public static Task HandleEventsAsync()
    {
        MiniBot.Client.Ready += OnReady;
        MiniBot.Client.MessageCreated += OnMessageCreated;
        MiniBot.Client.MessageDeleted += OnMessageDeleted;
        //MiniBot.Client.UnknownEvent += OnUnknownEvent;
        return Task.CompletedTask;
    }

    private static Task OnReady(DiscordClient sender, DSharpPlus.EventArgs.ReadyEventArgs e)
    {
        Log.Information($"Bot {sender.CurrentUser.Username} is connected and ready!");
        return Task.CompletedTask;
    }
    
    private static Task OnMessageCreated(DiscordClient sender, DSharpPlus.EventArgs.MessageCreateEventArgs e)
    {
        if (e.Author == null || e.Author.IsBot)
            return Task.CompletedTask;
    
        Log.Information($"Message: [{e.Author.Username}#{e.Author.Discriminator}] {e.Message.Content}");
    
        if (e.Guild != null)
        {
            Log.Debug($"(\nID: {e.Guild.Id}  Guild: {e.Guild.Name}");
        }
        else
        {
            Log.Debug("Guild information not available (DM message).");
        }
    
        return Task.CompletedTask;
    }
    
    private static Task OnMessageDeleted(DSharpPlus.DiscordClient sender, DSharpPlus.EventArgs.MessageDeleteEventArgs e)
    {
        if (e.Message == null || e.Message.Author == null)
            return Task.CompletedTask;

        if (e.Message.Author.IsBot)
            return Task.CompletedTask;

        Log.Information($"Message Deleting! [{e.Message.Author.Username}#{e.Message.Author.Discriminator}] {e.Message.Content}");
        Log.Debug($"(\nID: {e.Guild.Id} Guild: {e.Guild.Name}");
        return Task.CompletedTask;
    }

    private static Task OnUnknownEvent(DSharpPlus.DiscordClient sender, DSharpPlus.EventArgs.UnknownEventArgs e)
    {
        Log.Warning($"Unknown event: {e.EventName}");
        Log.Debug($"Payload: {e}");
        return Task.CompletedTask;
    }
}
