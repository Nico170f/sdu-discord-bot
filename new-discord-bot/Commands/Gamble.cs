using Discord;
using Discord.WebSocket;
using new_discord_bot.Games.Slots;

namespace new_discord_bot.Commands
{
    public class GambleCommand : ICommand
	{
		public string Name => "gamble";
		public string Description => "The house NEVER wins 🤑";
		private readonly Slots _slots;

		public GambleCommand(Slots slots)
		{
			_slots = slots;
		}

		public async Task Execute(SocketSlashCommand command)
		{
			await _slots.Execute(command);
		}


		public SlashCommandProperties Create()
		{
			SlashCommandBuilder newCommand = new SlashCommandBuilder()
			.WithName(this.Name)
			.WithDescription(this.Description);

			SlashCommandOptionBuilder builder = new SlashCommandOptionBuilder();
			builder.WithName("type");
			builder.WithDescription("What type would you like to play?");
			builder.WithType(ApplicationCommandOptionType.String);
			builder.AddChoice("🎰 Slot machines", "slots");
			builder.WithRequired(true);

			newCommand.AddOption(builder);

			return newCommand.Build();
		}
	}
}


//object data = command.Data.Options.First().Value;
//string? slotName = data.ToString();

//if(slotName == null)
//{
//	await command.RespondAsync("Invalid slot name");
//	throw new Exception("Invalid slot name?");
//}

//try
//{
//	_slots.TryGetValue(slotName, out ISlot? slot);
//	if (slot != null)
//	{
//		await slot.Execute(command);
//		return;
//	}
//}
//catch (Exception ex)
//{
//	Console.WriteLine(ex.Message);
//	throw new Exception("An error occurred while executing the command");
//}