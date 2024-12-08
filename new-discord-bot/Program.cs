using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using new_discord_bot.Configuration;
using new_discord_bot.Data;
using new_discord_bot.Services;



namespace new_discord_bot
{
	class Program
	{
		public static Task Main(string[] args) => new Program().RunBotAsync();

		public async Task RunBotAsync()
		{
			var services = ConfigureServices();
			var provider = services.BuildServiceProvider();

			var bot = provider.GetRequiredService<DiscordBot>();
			await bot.RunAsync();
		}

		private static IServiceCollection ConfigureServices()
		{
			var services = new ServiceCollection();
			services.AddSingleton<IConfiguration>(new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.Build());

			services.AddDbContext<UserContext>();  // Will automatically inject the UserContext
			services.AddScoped<UserService>();  // Scoped service to handle user-related operations
			services.AddSingleton<DiscordSocketClient>();
			services.AddSingleton<SlashCommandService>();
			services.AddSingleton<EventService>();
			services.AddSingleton<DiscordBot>();
			return services;
		}
	}

}