using Discord;
using Discord.WebSocket;
using new_discord_bot.Configuration;
using new_discord_bot.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot.Games
{
	public interface ISlot
	{
		public string Name { get; }
		public string Description { get; }
		Task Execute(SocketSlashCommand command);

	}

	public class BonanzaSlot : ISlot
	{
		public string Name { get; } = "Bonanza";
		public string Description { get; } = "Sweet Bonanza";

		public async Task Execute(SocketSlashCommand command)
		{
			EmbedBuilder embedBuilder = BalanceEmbed();
			await command.RespondAsync(embed: embedBuilder.Build());
		}


		public EmbedBuilder BalanceEmbed()
		{
			EmbedBuilder embedBuilder = new EmbedBuilder()
				.WithTitle(this.Name)
				.WithDescription($"Sweet bonanza")
				.WithColor(Colors.Green)
				.WithCurrentTimestamp();

			return embedBuilder;
		}
	}


	


}



