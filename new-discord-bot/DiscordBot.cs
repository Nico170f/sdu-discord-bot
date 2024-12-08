using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using new_discord_bot.Data;
using new_discord_bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot
{
	public class DiscordBot
	{
		private readonly DiscordSocketClient _client;
		private readonly EventService _eventService;
		private readonly IConfiguration _configuration;

		public DiscordBot(DiscordSocketClient client, EventService eventService, IConfiguration configuration)
		{
			_client = client;
			_eventService = eventService;
			_configuration = configuration;
		}

		public async Task RunAsync()
		{
			_eventService.SubscribeToEvents();
			await _client.LoginAsync(TokenType.Bot, _configuration["BotToken"]);
			await _client.StartAsync();
			await Task.Delay(-1);
		}

	}
}
