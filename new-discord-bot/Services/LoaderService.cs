﻿using new_discord_bot.Commands;
using new_discord_bot.Games;
using new_discord_bot.Games.Slots;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot.Services
{
    public class LoaderService
	{
		private readonly UserService _userService;

		public LoaderService(UserService userService)
		{
			_userService = userService;
		}

		public Dictionary<string, ICommand> LoadCommands()
		{
			Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();
			Dictionary<string, ISlot> slotsMachines = GetSlots();
			Slots slots = new Slots(slotsMachines);

			ICommand gambleCmd = new GambleCommand(slots);
			ICommand balanceCmd = new BalanceCommand(_userService);
			ICommand leaderboardCmd = new LeaderboardCommand(_userService);

			try
			{
				commands.Add(gambleCmd.Name, gambleCmd);
				commands.Add(balanceCmd.Name, balanceCmd);
				commands.Add(leaderboardCmd.Name, leaderboardCmd);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw new Exception("Unable to load one or more commands");
			}

			return commands;
		}

		private Dictionary<string, ISlot> GetSlots()
		{
			Dictionary<string, ISlot> slots = new Dictionary<string, ISlot>();
			ISlot bonanzaSlot = new BonanzaSlot();

			try
			{
				slots.Add(bonanzaSlot.Name, bonanzaSlot);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				throw new Exception("Unable to load one or more slots");
			}

			return slots;
		}
	}
}
