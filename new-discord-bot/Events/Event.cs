using Discord;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot.Events
{
	public class Event
	{
		InteractionType Type { get; }
		string Key { get; }
	}
}
