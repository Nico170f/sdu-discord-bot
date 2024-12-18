
namespace new_discord_bot.Assets
{
	public class Fruit
	{
		public string Name { get; }
		public int Value { get; }
		public int Rarity { get; }
		public string Emoji { get; set; } //Remove set
		public string Letter { get; }
		public Dictionary<int, double> Payouts { get; }

		public Fruit(string name, int value, int rarity, string emoji, string letter, Dictionary<int, double> payouts )
		{
			Name = name;
			Value = value;
			Rarity = rarity;
			Emoji = emoji;
			Letter = letter;
			Payouts = payouts;
		}

		public double GetPayout(int count, double spinPrice)
		{
			double basePayout = 0;
			if (count >= 12) basePayout = Payouts[12];
			else if (count >= 10) basePayout = Payouts[10];
			else if (count >= 8) basePayout = Payouts[8];

			return basePayout * spinPrice; // Scale payout based on spin price
		}
	}
}
