using Discord;
using Discord.WebSocket;
using new_discord_bot.Games;

namespace new_discord_bot.Commands
{
	public class GambleCommand : ICommand
	{
		public string Name => "gamble";
		public string Description => "xxxxxxxx";
		
		private readonly Dictionary<string, ISlot> _slots;

		public GambleCommand(Dictionary<string, ISlot> slots)
		{
			_slots = slots;
		}

		public async Task Execute(SocketSlashCommand command)
		{
			object data = command.Data.Options.First().Value;
			string? slotName = data.ToString();

			if(slotName == null)
			{
				await command.RespondAsync("Invalid slot name");
				throw new Exception("Invalid slot name?");
			}

			try
			{
				_slots.TryGetValue(slotName, out ISlot? slot);
				if (slot != null)
				{
					await slot.Execute(command);
					return;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				throw new Exception("An error occurred while executing the command");
			}
		}

		public SlashCommandProperties Create()
		{
			SlashCommandBuilder newCommand = new SlashCommandBuilder()
			.WithName(this.Name)
			.WithDescription(this.Description);

			SlashCommandOptionBuilder builder = new SlashCommandOptionBuilder();
			builder.WithName("slot");
			builder.WithDescription("Slot to bet on");
			builder.WithType(ApplicationCommandOptionType.String);
			builder.WithRequired(true);

			foreach (var slot in _slots)
			{
				builder.AddChoice(slot.Value.Name.ToLower(), slot.Value.Name);
			}

			newCommand.AddOption(builder);


			return newCommand.Build();
		}
	}
}
