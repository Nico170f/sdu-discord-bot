using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using new_discord_bot.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot
{
	public class Startup
	{
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddSingleton<DiscordSocketClient>();
			services.AddSingleton<SlashCommandService>();
			services.AddSingleton<EventService>();
			services.AddSingleton<Bot>();
		}
	}
}
