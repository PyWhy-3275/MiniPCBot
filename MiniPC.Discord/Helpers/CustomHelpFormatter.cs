using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;
using DSharpPlus.Entities;
using MiniPC.Discord.Classes;

namespace MiniPC.Discord.Helpers;

public class HelpFormatter : BaseHelpFormatter
{
    private StringBuilder StringBuilder { get; }
    private DiscordEmbedBuilder EmbedBuilder { get; }

    public HelpFormatter(CommandContext ctx)
        : base(ctx)
    {
        StringBuilder = new StringBuilder();
        EmbedBuilder = new DiscordEmbedBuilder();

        EmbedBuilder.WithTitle("Help");
        EmbedBuilder.WithColor(DiscordColor.Blue);
        EmbedBuilder.WithAuthor(ctx.Client.CurrentUser.Username, "https://bot.minipc.pw",
            ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png, 64));
        EmbedBuilder.WithFooter("Bot.MiniPC.pw", ctx.Client.CurrentUser.GetAvatarUrl(ImageFormat.Png, 64));
        EmbedBuilder.WithTimestamp(DateTime.Now);
    }

    public override BaseHelpFormatter WithCommand(Command command)
    {
        EmbedBuilder.WithDescription($"`{command.Name}`");

        float? Cost = ((CostAttribute)command.CustomAttributes.FirstOrDefault(a => a is CostAttribute))?.GetCost();

        if (Cost.GetValueOrDefault() != 0.0f)
        {
            EmbedBuilder.AddField("Cost", $"{Cost.GetValueOrDefault()} MiniPC");
        }

        EmbedBuilder.AddField("Description", command.Description);

        if (command.Aliases.Count > 0)
        {
            EmbedBuilder.AddField("Aliases", string.Join("\n", command.Aliases.Select(s => $"`{s}`")));
        }

        if (command.Overloads[0].Arguments.Count > 0)
        {
            EmbedBuilder.AddField("Syntax",
                $"`{command.Name} {string.Join(" ", command.Overloads[0].Arguments.Select(arg => $"<{arg.Name}>"))}`");
            EmbedBuilder.AddField("Arguments",
                string.Join("\n", command.Overloads[0].Arguments.Select(arg => $"`<{arg.Name}>` - {arg.Description}")));
        }

        return this;
    }

    public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
    {
        EmbedBuilder.WithDescription("You can get more specific help by also supplying the name of a command.");

        Dictionary<string, List<Command>> CategorizedCommands = new Dictionary<string, List<Command>>
        {
            { "Tipping", new List<Command>() },
            { "Exchange", new List<Command>() },
            { "Dallar", new List<Command>() },
            { "Jokes", new List<Command>() },
            { "Misc", new List<Command>() }
        };

        foreach (Command command in subcommands)
        {
            if (command.Name == "help")
            {
                continue;
            }

            string Category =
                ((HelpCategoryAttribute)command.CustomAttributes.FirstOrDefault(a => a is HelpCategoryAttribute))
                ?.GetCategory();
            if (Category == null)
            {
                Category = "Uncategorized";
            }

            if (CategorizedCommands.ContainsKey(Category))
            {
                CategorizedCommands[Category].Add(command);
            }
            else
            {
                CategorizedCommands[Category] = new List<Command>(new Command[] { command });
            }
        }

        foreach (var Category in CategorizedCommands)
        {
            if (Category.Value.Count == 0)
            {
                continue;
            }

            EmbedBuilder.AddField($"{Category.Key} Commands",
                string.Join("\n", Category.Value.Select(xc => $"`{xc.Name}` - {xc.Description}")));
        }

        return this;
    }

    // this is called as the last method, this should produce the final 
    // message, and return it

    public override CommandHelpMessage Build()
    {
        return new CommandHelpMessage(embed: EmbedBuilder.Build());
    }
}