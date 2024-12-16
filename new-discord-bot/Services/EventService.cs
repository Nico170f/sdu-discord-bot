using Discord;
using Discord.WebSocket;
using new_discord_bot.Data;
using new_discord_bot.Events;
using new_discord_bot.Games.Slots;


namespace new_discord_bot.Services
{
	public class EventService
	{
		private readonly DiscordSocketClient _client;
		private readonly SlashCommandService _slashCommandService;
		private readonly UserContext _userContext;

		private readonly SelectMenuExecuted selectMenuExecuted = new SelectMenuExecuted();


		public EventService(DiscordSocketClient client, SlashCommandService slashCommandService, UserContext userContext/*, SelectMenuExecuted selectMenuExecuted*/)
		{
			_client = client;
			_slashCommandService = slashCommandService;
			_userContext = userContext;
		}

		public void SubscribeToEvents()
		{
			_client.Log += LogAsync;
			_client.Ready += ReadyAsync;
			_client.SelectMenuExecuted += selectMenuExecuted.HandleSelectMenu;
			// _client.ButtonExecuted += selectMenuExecuted.HandleButton;
			_client.ButtonExecuted += HandleButton1;
		}

		private Task LogAsync(LogMessage log)
		{
			Console.WriteLine(log);
			return Task.CompletedTask;
		}

		private async Task ReadyAsync()
		{
			await _slashCommandService.RegisterCommandsAsync();
			_userContext.Database.EnsureCreated();
		}

		private Task DeleteDB()
		{
			_userContext.Database.EnsureDeleted();
			return Task.CompletedTask;
		}

		private async Task HandleButton1(SocketMessageComponent component)
		{
			await component.DeferAsync(true);


			string? eventType = component.Data.CustomId;
			if (eventType == null)
			{
				await component.RespondAsync("Invalid event type");
				throw new Exception("Invalid event type?");
			}

			if (eventType == "retry")
			{
				await new BonanzaSlot().Execute(component);
			}

		}

		//public async Task MyMenuHandler(SocketMessageComponent arg)
		//{
		//	//arg.Type = ComponentType.SelectMenu;
		//	var text = string.Join(", ", arg.Data.Values);
		//	await arg.RespondAsync($"You have selected {text}");
		//}
	}
}
