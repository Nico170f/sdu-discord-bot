namespace new_discord_bot.Assets
{
	public class Fruit : Item
	{
		public string Letter { get; }
		public Fruit(string name, int value, int rarity,string letter, string emoji, Dictionary<int, double> payouts) : base(name, value, rarity, emoji, payouts)
		{
			Letter = letter;
		}
	}
}
