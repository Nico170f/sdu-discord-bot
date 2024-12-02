using new_discord_bot.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot.Services
{
	internal class CommandLoaderService
	{
		private readonly UserService _userService;

		public CommandLoaderService(UserService userService)
		{
			_userService = userService;
		}

		public Dictionary<string, ICommand> LoadCommands()
		{
			Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();
			ICommand gambleCmd = new GambleCommand();
			ICommand balanceCmd = new BalanceCommand(_userService);
			ICommand spinsCmd = new SpinsCommand(_userService);
			ICommand leaderboardCmd = new LeaderboardCommand(_userService);

			try
			{
				commands.Add(gambleCmd.Name, gambleCmd);
				commands.Add(balanceCmd.Name, balanceCmd);
				commands.Add(spinsCmd.Name, spinsCmd);
				commands.Add(leaderboardCmd.Name, leaderboardCmd);
			}
			catch (Exception ex)
			{
				throw new Exception("Unable to load one or more commands");
			}

			return commands;
		}
	}
}
