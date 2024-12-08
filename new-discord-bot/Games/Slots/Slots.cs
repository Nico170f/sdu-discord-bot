using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace new_discord_bot.Games.Slots
{
	public class Slots
	{

		private readonly Dictionary<string, ISlot> _slots;

		public Slots(Dictionary<string, ISlot> slots)
		{
			_slots = slots;
		}

		public async Task Execute(SocketSlashCommand command)
		{
			SocketTextChannel channel = await this.CreateGambleChannel(command);
			await channel.SendMessageAsync(embed: this.GetEmbed(command).Build(), components: this.GetComponent(command).Build());
			await command.RespondAsync("Created thread!");
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

		private EmbedBuilder GetEmbed(SocketSlashCommand command)
		{
			EmbedBuilder embedBuilder = new EmbedBuilder()
				//.WithTitle("Slot Selector")
				.WithDescription("Select the slot you wish to gamble on 🤑")
				.WithFooter(command.User.Id.ToString(), command.User.GetAvatarUrl())
				.WithColor(Color.Green);

			return embedBuilder;
		}

		private ComponentBuilder GetComponent(SocketSlashCommand command)
		{
			SelectMenuBuilder menuBuilder = new SelectMenuBuilder()
			.WithPlaceholder("Select a slot machine")
			.WithCustomId("slot-selector")
			.WithMinValues(1)
			.WithMaxValues(1);

			foreach (var slot in _slots)
			{
				menuBuilder.AddOption(slot.Value.Name, $"menu:{slot.Value.Label}", slot.Value.Description, slot.Value.Emoji);
			}


			ComponentBuilder builder = new ComponentBuilder()
			.WithSelectMenu(menuBuilder);

			return builder;
		}
	}
}



//SocketTextChannel channel;

//if (command.Channel is SocketTextChannel textChannel)
//{
//	channel = await this.CreateGambleChannel(command);
//}
//else
//{
//	await command.RespondAsync("This command can only be used in a text channel");
//	return;
//}

//await channel.SendMessageAsync(embed: this.GetEmbed(command).Build(), components: this.GetComponent(command).Build());
//await command.RespondAsync("Created thread!");