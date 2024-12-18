using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot.Commands
{
	public interface ICommand
	{
		public string Name { get; }
		public string Description { get; }
		//public List<>
		Task Execute(SocketSlashCommand command);

		SlashCommandProperties Create();
	}
}
