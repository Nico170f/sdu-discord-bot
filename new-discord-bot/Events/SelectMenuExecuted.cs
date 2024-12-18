using Discord.WebSocket;
using new_discord_bot.Data;
using new_discord_bot.Games.Slots;
using new_discord_bot.Services;


namespace new_discord_bot.Events
{
	public class SelectMenuExecuted
	{

		private readonly UserContext _userContext;

		public SelectMenuExecuted(UserContext userContext)
		{
			_userContext = userContext;
		}

		public async Task Handle(SocketMessageComponent component)
		{
			await component.DeferAsync(true);
			string? data = component.Data.Values.First();
			if(data == null)
			{
				await component.RespondAsync("Invalid event type");
				throw new Exception("Invalid event type?");
			}

			string[] eventType = data.Split(":");

			if (eventType[0] == "menu_spin")
			{

				switch (eventType[1])
				{
					case "bonanza":
						await new BonanzaSlot(/*_userContext*/).Execute(component);
						break;
				}
			}


		}

	}
}
