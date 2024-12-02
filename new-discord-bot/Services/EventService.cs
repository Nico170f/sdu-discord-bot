using Discord.WebSocket;


namespace new_discord_bot.Services
{
	public class EventService
	{
		private readonly DiscordSocketClient _client;

		public EventService(DiscordSocketClient client)
		{
			_client = client;
		}

		public void SubscribeToEvents()
		{
			_client.Ready += ReadyAsync;
		}

		private Task ReadyAsync()
		{
			Console.WriteLine("Bot is connected and ready!");
			return Task.CompletedTask;
		}
	}
}
