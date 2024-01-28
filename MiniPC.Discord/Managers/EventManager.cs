using System;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using MiniPC.Discord.Helpers;


namespace MiniPC.Discord.Managers;

public static class EventManager
{
    public static Task HandleEventsAsync()
    {
        //Bot.Client.Ready += OnReady;
        //Bot.Client.MessageCreated += OnMessageCreated;
        MiniBot.Client.MessageDeleted += OnMessageDeleted;

        return Task.CompletedTask;
    }

    private static Task OnMessageDeleted(DSharpPlus.DiscordClient sender, DSharpPlus.EventArgs.MessageDeleteEventArgs e)
    {
        Console.WriteLine("Message Deleted!");
        return Task.CompletedTask;
    }
    
    private static async Task UpdateActivity(string action)
    {
        var activity = action == ConfigManager.Activity ? new DiscordActivity() : new DiscordActivity() { ActivityType = ActivityType.ListeningTo };

        activity.Name = action;
        await MiniBot.Client.UpdateStatusAsync(activity);
    }
}