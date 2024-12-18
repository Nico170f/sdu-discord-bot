﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot.Data
{
	public class User : IComparable<User>
	{
		public ulong Id { get; set; }
		public double Balance { get; set; } = 100;
		public int TotalSpins { get; set; } = 0;
		public int TotalFeatures { get; set; } = 0;
		public int HighestBalance { get; set; } = 0;

		public int CompareTo(User? other)
		{
			if (other == null) return 1;

			return other.Balance.CompareTo(this.Balance);
		}
	}
}
