using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using new_discord_bot.Config;
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

			var bot = provider.GetRequiredService<Bot>();
			await bot.RunAsync();
		}

		private static IServiceCollection ConfigureServices()
		{
			var services = new ServiceCollection();

			// Add configuration settings
			//var config = new ConfigurationBuilder()
			//	.SetBasePath(Directory.GetCurrentDirectory())
			//	.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			//	.Build();

			//IConfigurationBuilder configBuilder = new ConfigurationBuilder();
			//configBuilder.AddJsonFile("appsettings.json");

			//services.AddSingleton<IConfiguration>(config);

			// Add other services for the bot
			services.AddDbContext<UserContext>();  // Will automatically inject the UserContext
			services.AddScoped<UserService>();  // Scoped service to handle user-related operations


			services.AddSingleton<DiscordSocketClient>();
			services.AddSingleton<SlashCommandService>();
			services.AddSingleton<EventService>();
			services.AddSingleton<Bot>();

			return services;
		}



	}


	public class Bot
	{
		private readonly DiscordSocketClient _client;
		private readonly SlashCommandService _slashCommandService;
		private readonly EventService _eventService;
		private readonly UserService _userService;
		private readonly UserContext _userContext;

		//private readonly IConfiguration _configuration;

		public Bot(DiscordSocketClient client, SlashCommandService slashCommandService, EventService eventService/*, IConfiguration configuration*/, UserService userService, UserContext userContext)
		{
			_client = client;
			_slashCommandService = slashCommandService;
			_eventService = eventService;
			//_configuration = configuration;
			_userService = userService;
			_userContext = userContext;
		}

		public async Task RunAsync()
		{
			_client.Log += LogAsync;
			_client.Ready += ReadyAsync;

			//await _client.LoginAsync(TokenType.Bot, _configuration["BotToken"]);
			await _client.LoginAsync(TokenType.Bot, BotConfig.BotToken);
			await _client.StartAsync();

			await Task.Delay(-1);  // Keep the bot running
		}

		private Task LogAsync(LogMessage log)
		{
			Console.WriteLine(log);
			return Task.CompletedTask;
		}

		private async Task ReadyAsync()
		{
			await _slashCommandService.RegisterCommandsAsync();  // Register commands
			_eventService.SubscribeToEvents();  // Subscribe to events

			Console.WriteLine("Bot is ready!");

			_userContext.Database.EnsureCreated();

			//var user = new User { Username = "Alice", Age = 30, Email = "alice@example.com" };
			//await _userService.AddUserAsync(user);

			//var users = await _userService.GetUsersAsync();
			//foreach (var u in users)
			//{
			//	Console.WriteLine($"{u.Username} - {u.Age} - {u.Email}");
			//}
		}
	}
}