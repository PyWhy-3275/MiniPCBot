﻿using System;
using DSharpPlus;
using System.Linq;
using System.Net.Http;
using Newtonsoft.Json;
using DSharpPlus.Entities;
using System.Threading.Tasks;
using MiniPC.Discord.Classes;
using DSharpPlus.CommandsNext;
using System.Collections.Generic;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using MiniPC.Discord.Managers;
using Newtonsoft.Json.Linq;

namespace MiniPC.Discord.Commands;



public class AnimeCommands : BaseCommandModule
{
    private readonly HttpClient _httpClient;
    private readonly string _tenorApiKey = ConfigManager.TokenTenor;

    public AnimeCommands()
    {
        _httpClient = new HttpClient();
    }

    [Command("animecalendar")]
    [HelpCategory("Anime")]
    public async Task AnimeCalendar(CommandContext ctx)
    {
        string url = "https://shikimori.one/api/calendar";

        using (var client = new HttpClient())
        {
            client.DefaultRequestHeaders.Add("User-Agent", "My Discord Bot");

            var response = await client.GetStringAsync(url);
            var animeList = JsonConvert.DeserializeObject<Anime[]>(response);

            var groupedByDayOfWeek = animeList.GroupBy(a => a.next_episode_at.DayOfWeek);
            
            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.Title = "Аниме на неделе";
            embedBuilder.Description = "Количество аниме по дням недели:";

            foreach (var group in groupedByDayOfWeek)
            {
                embedBuilder.AddField(group.Key.ToString(), group.Count() + " аниме", true);
            }

            var embed = embedBuilder.Build();
            
            var button = new DiscordButtonComponent(ButtonStyle.Primary, "anime_list", "Показать список аниме");
            var builder = new DiscordMessageBuilder()
                .WithContent("Нажмите кнопку, чтобы показать список аниме")
                .AddComponents(button);
            
            var msg = await ctx.RespondAsync(embed);
            var message = await msg.RespondAsync(builder);
            // Обработчик кнопки
            var interactivity = ctx.Client.GetInteractivity();
            var buttonResult = await interactivity.WaitForButtonAsync(message, ctx.User);
        }
    }
    
    [Command("calendar")]
    [HelpCategory("Anime")]
    [Description("Выберите день недели: Понедельник, Вторник, Среда, Четверг, Пятница, Суббота, Воскресенье")]
    public async Task Calendar(CommandContext ctx, string day)
    {
        if (Enum.TryParse(day, true, out DayOfWeek selectedDay))
        {
            var embed = GetAnimeForSelectedDay(selectedDay);
            await ctx.RespondAsync(embed: embed);
        }
        else
        {
            await ctx.RespondAsync("Неверно выбран день недели");
        }
    }

    [Command("randomanime")]
    [HelpCategory("Anime")]
    public async Task RandomAnime(CommandContext ctx)
    {
        var embed = GetRandomAnime();
        await ctx.RespondAsync(embed: embed);
    }

    [Command("gif")]
    [Description("Отправляет случайный gif по запросу с использованием Tenor API.")]
    public async Task Gif(CommandContext ctx, [RemainingText]string query)
    {
        var url = $"https://tenor.googleapis.com/v2/search?q={Uri.EscapeDataString(query)}&key={_tenorApiKey}&client_key=my_test_client_key&limit=8";
        var response = await _httpClient.GetStringAsync(url);
        var json = JObject.Parse(response);
        var results = json["results"] as JArray;
        if (results != null && results.Count > 0)
        {
            // Получаем рандомный результат
            var gifResult = results[new Random().Next(results.Count)];

            // Парсим информацию о GIF и теги
            var gifUrl = gifResult["media_formats"]["gif"]["url"].ToString();
            var tags = string.Join(", ", gifResult["tags"].Select(t => t.ToString()));

            // Создаем embed сообщение
            var embed = new DiscordEmbedBuilder
            {
                Title = query,
                ImageUrl = gifUrl,
                Description = $"{tags}"
            };

            // Отправляем сообщение
            await ctx.RespondAsync(embed: embed);
        }
        else
        {
            // Если результаты поиска отсутствуют
            await ctx.RespondAsync("GIF по запросу не найден!");
        }
    }
    
    private DiscordEmbed GetRandomAnime()
    {
        string url = "https://shikimori.one/api/calendar";

        using (var client = new HttpClient())
        {
            var response = client.GetStringAsync(url).Result;
            var animeList = JsonConvert.DeserializeObject<List<Anime>>(response);

            var randomAnime = animeList.OrderBy(x => Guid.NewGuid()).First();

            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.Title = "Случайное аниме";
            embedBuilder.AddField(randomAnime.anime.russian, 
                $"Дата выхода: {randomAnime.next_episode_at.ToString("dd.MM.yyyy HH:mm")}\nСледующий эпизод: {randomAnime.next_episode} [Клик](https://shikimori.one{randomAnime.anime.url})");
            embedBuilder.WithThumbnail($"https://shikimori.one/system/animes/original/{randomAnime.anime.id}.jpg");
            return embedBuilder.Build();
        }
    }
    
    private DiscordEmbed GetAnimeForSelectedDay(DayOfWeek selectedDay)
    {
        string url = "https://shikimori.one/api/calendar";

        using (var client = new HttpClient())
        {
            var response = client.GetStringAsync(url).Result;
            var animeList = JsonConvert.DeserializeObject<List<Anime>>(response);

            var selectedAnime = animeList.Where(a => a.next_episode_at.DayOfWeek == selectedDay);

            var embedBuilder = new DiscordEmbedBuilder();
            embedBuilder.Title = $"Аниме на {selectedDay}";

            foreach (var anime in selectedAnime)
            {
                var episodeInfo = anime.next_episode > 0 ? $"Эпизод {anime.next_episode}" : "Сезон закончен";
            
                embedBuilder.AddField(anime.anime.russian, 
                    $"Дата выхода: {anime.next_episode_at.ToString("dd.MM.yyyy HH:mm")}\n{episodeInfo} [Клик](https://shikimori.one{anime.anime.url})");
            }

            return embedBuilder.Build();

        }
    }
}

public class Anime
{
    public int next_episode { get; set; }
    public DateTime next_episode_at { get; set; }
    public AnimeItem anime { get; set; }
}

public class AnimeItem
{
    public string name { get; set; }
    public int id { get; set; }
    public string russian { get; set; }
    public string url { get; set; }
    public int episodes { get; set; }
    public int episodes_aired { get; set; }
    public DateTime aired_on { get; set; }
    public string status { get; set; }
}