using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot.Data
{
	public class User : IComparable<User>
	{
		public ulong Id { get; set; }
		public int Balance { get; set; }
		public int Spins { get; set; }

		public int CompareTo(User? other)
		{
			if (other == null) return 1;

			return other.Balance.CompareTo(this.Balance);
		}
	}
}
