﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace new_discord_bot.Events
{
	internal interface IEventImplementer
	{
		List<Event> events { get; }
	}
}