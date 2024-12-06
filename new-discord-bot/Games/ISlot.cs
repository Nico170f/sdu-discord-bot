using Discord.WebSocket;
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
		public int Width { get; }
		public int Height { get; }
		Task Execute(SocketSlashCommand command);
	}
}
