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

			var bot = provider.GetRequiredService<Bot>();
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
		private readonly IConfiguration _configuration;



		public Bot(DiscordSocketClient client, SlashCommandService slashCommandService, EventService eventService, UserService userService, UserContext userContext, IConfiguration configuration)
		{
			_client = client;
			_slashCommandService = slashCommandService;
			_eventService = eventService;
			_userService = userService;
			_userContext = userContext;
			_configuration = configuration;
		}

		public async Task RunAsync()
		{
			_client.Log += LogAsync;
			_client.Ready += ReadyAsync;

			await _client.LoginAsync(TokenType.Bot, _configuration["BotToken"]);
			await _client.StartAsync();

			await Task.Delay(-1);
		}

		private Task LogAsync(LogMessage log)
		{
			Console.WriteLine(log);
			return Task.CompletedTask;
		}

		private async Task ReadyAsync()
		{
			await _slashCommandService.RegisterCommandsAsync();
			_eventService.SubscribeToEvents();
			// _userContext.Database.EnsureDeleted();
			_userContext.Database.EnsureCreated();
			//Console.WriteLine("Bot ics ready!");
		}
	}
}