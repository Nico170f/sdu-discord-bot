using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot.Assets
{
	public class Fruit
	{
		public string Name { get; }
		public int Value { get; }
		public int Rarity { get; }
		public string Emoji { get; }

		public Fruit(string name, int value, int rarity, string emoji)
		{
			Name = name;
			Value = value;
			Rarity = rarity;
			Emoji = emoji;
		}
	}
}
