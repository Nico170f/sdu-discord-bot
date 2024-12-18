using Discord;
using Discord.WebSocket;
using new_discord_bot.Configuration;
using LibNoise;
using LibNoise.Primitive;
using new_discord_bot.Assets;
using new_discord_bot.Events;
using static System.Runtime.InteropServices.JavaScript.JSType;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using Discord.Rest;
using new_discord_bot.Data;

namespace new_discord_bot.Games.Slots
{
    public class SpinData
    {
        public Fruit?[,] grid { get; }
		// public Fruit?[,] gridWon { get; }
		public Dictionary<Fruit, int> won { get; }
		public bool Won => won.Count > 0;

		public SpinData(Fruit?[,] grid, Dictionary<Fruit, int> won)
		{
			this.grid = grid;
			this.won = won;
		}
	}



    public class BonanzaSlot : ISlot/*, IEventImplementer*/
    {
        public string Label { get; } = "bonanza";
        public string Name { get; } = "Sweet Bonanza";
        public Emoji? Emoji { get; } = new Emoji("🍭");
        public string Description { get; } = "Load up on sugar in Sweet Bonanza™";
        public int Width { get; } = 6;
        public int Height { get; } = 5;
		
		private readonly SimplexPerlin perlinGenerator = new SimplexPerlin(0, NoiseQuality.Standard);

		private static readonly Fruit[] rarityFactors = 
        {
			new Fruit("Bananas", 1, 100, "🍌", "1", new Dictionary<int, double> { { 8, 0.25 }, { 10, 0.75 }, { 12, 2.00 } }),
			new Fruit("Grapes", 2, 90, "🍇", "2", new Dictionary<int, double> { { 8, 0.40 }, { 10, 0.90 }, { 12, 4.00 } }),
			new Fruit("Melons", 3, 90, "🍈", "3", new Dictionary<int, double> { { 8, 0.50 }, { 10, 1.00 }, { 12, 5.00 } }),
			new Fruit("Peaches", 3, 80, "🍑", "4", new Dictionary<int, double> { { 8, 0.80 }, { 10, 1.10 }, { 12, 8.00 } }),
			new Fruit("Apples", 3, 80, "🍎", "5", new Dictionary<int, double> { { 8, 1.00 }, { 10, 1.50 }, { 12, 10.00 } }),
			new Fruit("Blue Candy", 3, 70, "🔷", "6", new Dictionary<int, double> { { 8, 1.50 }, { 10, 2.00 }, { 12, 12.00 } }),
			new Fruit("Green Candy", 3, 70, "🟩", "7", new Dictionary<int, double> { { 8, 2.00 }, { 10, 5.00 }, { 12, 15.00 } }),
			new Fruit("Purple Candy", 3, 65, "🟣", "8", new Dictionary<int, double> { { 8, 2.50 }, { 10, 10.00 }, { 12, 25.00 } }),
			new Fruit("Hearts", 3, 60, "❤️", "9", new Dictionary<int, double> { { 8, 10.00 }, { 10, 25.00 }, { 12, 50.00 } }),
			new Fruit("Lollipop", 3, 60, "🍭", "0", new Dictionary<int, double> { { 4, 3.00 }, { 5, 5.00 }, { 6, 100.00 } })
		};

		private static int[] cumulativeWeights = new int[rarityFactors.Length];
		private static int totalWeight = -1;
		private float scale = 0.15f; // Adjust for cluster size

		private double totalWin = 0;
		private double spinPrice = 1.0;

		//private readonly UserContext userContext;

		public BonanzaSlot(/*UserContext userContext*/)
		{
			//this.userContext = userContext;

			if (totalWeight == -1)
			{
				Console.WriteLine("Setting up weights");
				// Generate cumulative weight distribution
				for (int i = 0; i<rarityFactors.Length; i++)
				{
					totalWeight += rarityFactors[i].Rarity;
					cumulativeWeights[i] = totalWeight;
				}
			}
		}


		//public async Task Execute(SocketSlashCommand command)
  //      {
		//	return;
		//}

		public async Task Execute(SocketMessageComponent command)
		{
			await StartSpin(command, null);
		}


		private async Task StartSpin(SocketMessageComponent command, SpinData? spinData)
		{

			SpinData result = GenerateGroupedRandomGrid(spinData);
			SocketTextChannel socketChannel = GetChannel(command);

			await SendSpinMessage(socketChannel, result);

			await Task.Delay(500);

			if (result.Won)
			{
				await SendWinnings(command, result);
			}
			else
			{
				await SpinEnded(command);
			}
		}

		private async Task SpinEnded(SocketMessageComponent command)
		{
			EmbedBuilder embedBuilder = new EmbedBuilder()
				//.WithTitle("Slot Selector")
				.WithDescription($"💰 Total win: **${totalWin}**\n🏦 New balance: {0}")
				.WithFooter(command.User.Id.ToString(), command.User.GetAvatarUrl())
				.WithColor(Color.Green);

			await command.FollowupAsync(embed: embedBuilder.Build(), components: new ComponentBuilder().WithButton("Respin", "retry").Build());
		}

		private async Task SendSpinMessage(SocketTextChannel channel, SpinData result)
		{
			string spinPrint = GetSpinMessage(result.grid);
			RestUserMessage message = await channel.SendMessageAsync(spinPrint);

			if(result.Won)
			{
				Task task = Task.Delay(500);
				Fruit?[,] clearedFruits = ClearWonFruits(result);
				string clearedPrint = GetSpinMessage(clearedFruits);

				await task;
				await UpdateSpinMessage(message, clearedPrint);

			}

		}

		private async Task UpdateSpinMessage(RestUserMessage message, string newMessage)
		{
			//Edit the message content
			
			await message.ModifyAsync(msg => msg.Content = newMessage);
			Console.WriteLine("Updated message");
		}

		private async Task SendWinnings(SocketMessageComponent command, SpinData result)
		{
			bool gotFeature = false;
            string wonStr = "";

			foreach (var item in result.won)
            {
				if (item.Key.Name == "Lollipop")
				{
					gotFeature = true;
				}

				double payout = item.Key.GetPayout(item.Value, spinPrice);
				totalWin += payout;
				wonStr += $"You got **${payout}** from **{item.Value}** {item.Key.Name}! {item.Key.Emoji}\n";
            }

            if (wonStr.Length > 0)
            {
				SocketTextChannel socketTextChannel = GetChannel(command);
				await socketTextChannel.SendMessageAsync(wonStr);
			}

            if(gotFeature)
            {
                //feature
            } else
            {
				await Task.Delay(1000);
				//.ContinueWith(async (task) => await StartSpin(command, result));
				await StartSpin(command, result);
			}
		}

        //private async Task GotFeature()
        //{

        //}

		private SpinData GenerateGroupedRandomGrid(SpinData? spinData)
        {
			Fruit?[,] grid;

			if (spinData != null)
			{
				PrintGrid(spinData.grid);
				Console.WriteLine("");
				grid = TumbleGrid(spinData);
				PrintGrid(spinData.grid);
			}
			else
			{
				grid = new Fruit[Width, Height];
			}

			perlinGenerator.Seed = (int)(DateTime.UtcNow - DateTime.Today).TotalSeconds;

            Dictionary<Fruit, int> tendency = new Dictionary<Fruit, int>();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
					if(grid[x, y] == null)
					{
						// Get Simplex noise value for the current position
						double noiseValue = perlinGenerator.GetValue(x * scale, y * scale);

						// Map the noise value (-1 to 1) to the range [1, rarityFactors.Length]
						int num = MapNoiseToValueWithRarity(noiseValue);
						grid[x, y] = rarityFactors[num];
						
					}

                    UpdateTendency(tendency, grid[x, y]);
				}
            }

			Dictionary<Fruit, int> won = GetWinnings(tendency);
			return new SpinData(grid, won);
        }



		private Fruit?[,] TumbleGrid(SpinData data)
		{
			Fruit?[,] grid = data.grid;

			for (int x = 0; x < Width; x++) // Process each column
			{
				// Start from the bottom row and work upwards
				int targetY = Height - 1;

				for (int y = Height - 1; y >= 0; y--)
				{
					if (grid[x, y] != null && !data.won.ContainsKey(grid[x, y]!))
					{
						// Move non-null fruit to the target position if it's different
						if (targetY != y)
						{
							grid[x, targetY] = grid[x, y];
							grid[x, y] = null;
						}
						targetY--; // Move target position upwards
					}
				}

				// Fill any remaining slots above the target position with nulls
				for (int y = targetY; y >= 0; y--)
				{
					grid[x, y] = null;
				}
			}

			return grid;
		}





		//private Fruit?[,] TumbleGrid2(SpinData data)
		//{
		//	Fruit?[,] grid = data.grid;


		//	for (int y = Height - 1; y >= 0; y--)
		//	{
		//		if(y == 0)
		//		{
		//			break;
		//		}

		//		for (int x = Width - 1; x >= 0; x--)
		//		{
		//			if (grid[x, y] == null ||
		//				data.won.ContainsKey(grid[x, y]!))
		//			{
		//				grid[x, y] = GetAboveFruit(grid, x, y);
		//				grid[x, y - 1] = null;
		//			}
		//		}
		//	}

		//	return grid;
		//}


		//private Fruit? GetAboveFruit(Fruit?[,] grid, int x, int y)
		//{
		//	Fruit? fruit = grid[x, y - 1];
		//	Console.WriteLine("Returning Fruit: " + fruit?.Name);
		//	return fruit;
		//}

		private Fruit?[,] ClearWonFruits(SpinData data)
		{
			Fruit?[,] grid = (Fruit?[,]) data.grid.Clone();

			for (int y = 0; y < Height; y++)
			{
				for (int x = 0; x < Width; x++)
				{
					if (grid[x, y] == null) continue;

					if (data.won.ContainsKey(grid[x, y]!))
					{
						grid[x, y] = null;
					}
				}
			}

			return grid;
		}


		private int MapNoiseToValueWithRarity(double noise)
        {
            // Normalize noise value (-1 to 1) to (0 to 1)
            double normalizedNoise = (noise + 1) / 2;

            // Scale normalized noise to the total weight range
            int scaledValue = (int)(normalizedNoise * totalWeight);

            // Find the number corresponding to the scaled value based on rarity weights
            for (int i = 0; i < cumulativeWeights.Length; i++)
            {
                if (scaledValue < cumulativeWeights[i])
                {
                    return i;
                }
            }

            // Fallback (should never reach here if weights are correct)
            return 0;
        }


		private Dictionary<Fruit, int> GetWinnings(Dictionary<Fruit, int> tendency)
		{
			Dictionary<Fruit, int> won = new Dictionary<Fruit, int>();
			//double totalWin = 0;

			foreach (var item in tendency)
			{
				if (item.Key.Name == "Lollipop" && item.Value >= 4)
				{
					won.Add(item.Key, item.Value);
					continue;
				}

				if (item.Value >= 8)
				{
					won.Add(item.Key, item.Value);
				}
			}

			return won;
		}

		private void UpdateTendency(Dictionary<Fruit, int> tendency, Fruit? fruit)
		{
			if (fruit == null) return;

			if (tendency.ContainsKey(fruit))
			{
				tendency[fruit]++;
			}
			else
			{
				tendency.Add(fruit, 1);
			}
		}


		private string GetSpinMessage(Fruit?[,] grid)
        {
            string message = "";
			for (int y = 0; y < grid.GetLength(1); y++)
			{
				for (int x = 0; x < grid.GetLength(0); x++)
				{
					if(grid[x, y] != null)
					{
						message += grid[x, y]!.Emoji + " ";
					} else
					{
						message += "💰 ";
					}
				}
				message += "\n";
			}
			return message;
        }


        private SocketTextChannel GetChannel(SocketMessageComponent command)
		{
			if (command.Channel is SocketTextChannel channel)
			{
				return channel;
			}
			
			throw new Exception("Could not get channel");
		}


		private void PrintGrid(Fruit?[,] grid)
		{
			for (int y = 0; y < grid.GetLength(1); y++)
			{
				for (int x = 0; x < grid.GetLength(0); x++)
				{

					if (grid[x, y] == null)
					{
						Console.Write("X ");
					}
					else
					{
						Console.Write(grid[x, y]!.Letter + " ");
					}
				}
				Console.WriteLine();
			}
		}

	}
}
