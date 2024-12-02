using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot.Commands
{
	internal interface ICommand
	{
		public string Name { get; }
		public string Description { get; }
		Task Execute(SocketSlashCommand command);

		SlashCommandProperties Create();
	}
}
