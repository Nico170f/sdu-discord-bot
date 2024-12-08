using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot.Games.Slots
{
    public interface ISlot
    {
        public string Label { get; }
		public string Name { get; }
		public Emoji? Emoji { get; }
		public string Description { get; }
        public int Width { get; }
        public int Height { get; }
        Task Execute(SocketSlashCommand command);
    }
}
