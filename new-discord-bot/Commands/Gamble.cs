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
			SocketTextChannel channel;

			if (command.Channel is SocketTextChannel textChannel)
			{
				channel = await this.CreateGambleChannel(command);
			}
			else
			{
				await command.RespondAsync("This command can only be used in a text channel");
				return;
			}

			await channel.SendMessageAsync(embed: this.GetEmbed(command).Build(), components: this.GetComponent(command).Build());
			await command.RespondAsync("Created thread!");
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
		}


		private ComponentBuilder GetComponent(SocketSlashCommand command)
		{
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
				.WithPlaceholder("Select a slot machine")
				.WithCustomId("slot-selector")
				.WithMinValues(1)
				.WithMaxValues(1)
				.AddOption("🍭 Sweet Bonanza", "bonanza", "Load up on sugar in Sweet Bonanza™")
				.AddOption("Option B", "opt-b", "Option B bla bla");

			ComponentBuilder builder = new ComponentBuilder()
				.WithSelectMenu(menuBuilder);

			return builder;
		}


		private EmbedBuilder GetEmbed(SocketSlashCommand command)
		{
			EmbedBuilder embedBuilder = new EmbedBuilder()
				.WithTitle("Slot Selector")
				.WithDescription("Select the slot you wish to gamble on 😈")
				.WithFooter(command.User.Username, command.User.GetAvatarUrl())
				.WithColor(Color.Blue);

			return embedBuilder;
		}

		private async Task<SocketTextChannel> CreateGambleChannel(SocketSlashCommand command)
		{
			SocketTextChannel thread;

			try
			{
				SocketTextChannel channel = (SocketTextChannel)command.InteractionChannel;
				thread = await channel.CreateThreadAsync(command.User.Username + "'s gamble thread", autoArchiveDuration: ThreadArchiveDuration.ThreeDays);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				await command.RespondAsync("An error occurred while creating the gamble channel");
				throw new Exception("An error occurred while creating the gamble channel");
			}

			return thread;
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
