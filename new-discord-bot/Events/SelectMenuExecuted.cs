using Discord.WebSocket;
using new_discord_bot.Games.Slots;
using new_discord_bot.Services;


namespace new_discord_bot.Events
{
	public class SelectMenuExecuted
	{

		public SelectMenuExecuted(/*SlashCommandService slashCommandService*/)
		{
		}

		public async Task HandleSelectMenu(SocketMessageComponent component)
		{
			await component.DeferAsync(true);
			string? eventType = component.Data.Values.First();
			if(eventType == null)
			{
				await component.RespondAsync("Invalid event type");
				throw new Exception("Invalid event type?");
			}

			switch (eventType)
			{
				case "menu:bonanza":
					await new BonanzaSlot().Execute(component);
					break;
			}

		}

	}
}
