using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot.Assets
{
    public abstract class Item
    {
        public string Name { get; }
        public int Value { get; }
        public int Rarity { get; }
        public string Emoji { get; }
        
        public Dictionary<int, double> Payouts { get; }
        public Item(string name, int value, int rarity, string emoji, Dictionary<int, double> payouts)
        {
            Name = name;
            Value = value;
            Rarity = rarity;
            Emoji = emoji;
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
