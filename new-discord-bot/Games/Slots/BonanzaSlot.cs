using Discord;
using Discord.WebSocket;
using new_discord_bot.Configuration;
using LibNoise;
using LibNoise.Primitive;
using new_discord_bot.Assets;
using new_discord_bot.Events;

namespace new_discord_bot.Games.Slots
{
    public class SpinData
    {
        public Fruit[,] grid { get; }
        public Dictionary<Fruit, int> won { get; }

		public SpinData(Fruit[,] grid, Dictionary<Fruit, int> won)
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

        private int currentWin = 0;

        //public List<Event> events = new List<Event>{ 
        //    new Event(InteractionType.)
        //};


		private static readonly Fruit[] rarityFactors = 
        {
			new Fruit("Bananas", 1, 100, "🍌"),
			new Fruit("Grapes", 2, 90, "🍇"),
            new Fruit("Melons", 3, 90, "🍈"),
			new Fruit("Peaches", 3, 80, "🍑"),
			new Fruit("Apples", 3, 80, "🍎"),
		    new Fruit("Blue Candy", 3, 70, "🔷"),
			new Fruit("Green Candy", 3, 70, "🟩"),
			new Fruit("Purple Candy", 3, 65, "🟣"),
			new Fruit("Hearts", 3, 60, "❤️"),
			new Fruit("Lollipop", 3, 40, "🍭")
	    };

		public async Task Execute(SocketSlashCommand command)
        {
			//SpinData result = GenerateGroupedRandomGrid(rarityFactors);
            //EmbedBuilder embedBuilder = BalanceEmbed(result.grid);
            //await command.RespondAsync(embed: embedBuilder.Build());
        }

		public async Task Execute(SocketMessageComponent command)
		{
			SpinData result = GenerateGroupedRandomGrid(rarityFactors);
			await command.RespondAsync(text: GetSpinMessage(result.grid) + "\n");

			if (result.won.Count == 0)
			{
				await SendSpinEnd(command);
				return;
			} else
            {
				await PrintResult(command, result);
                return;
			}

		}

		private async Task PrintResult(SocketMessageComponent command, SpinData result)
		{
			bool gotFeature = false;
            foreach (var item in result.won)
            {
                if(item.Key.Name == "Lollipop")
                {
					gotFeature = true;
					continue;
                }

                string wonStr = $"You got $x from {item.Value} {item.Key.Name}'s! ";
				await command.RespondAsync(text: item.Key.Name + " " + item.Value);
            }

            if(gotFeature)
            {
                //feature
            } else
            {
                //tumble();
			}

		}

        private async Task GotFeature()
        {

        }


        private async Task SendSpinEnd(SocketMessageComponent command)
        {
			//Create a Retry button

			//await command.RespondAsync("You have no wins! Would you like to try again?", components: new ComponentBuilder().WithButton("Retry", "retry").Build());

			if (command.Channel is SocketTextChannel channel)
			{
				await channel.SendMessageAsync("Retry?", components: new ComponentBuilder().WithButton("Retry", "retry").Build());
			}
		}


		private SpinData GenerateGroupedRandomGrid(Fruit[] rarityFactors)
        {
            Fruit[,] grid = new Fruit[Width, Height];

            int seed = (int)(DateTime.UtcNow - DateTime.Today).TotalSeconds;
            SimplexPerlin perlinGenerator = new SimplexPerlin(seed, NoiseQuality.Standard);
            float scale = 0.5f; // Adjust for cluster size

            // Generate cumulative weight distribution
            int totalWeight = 0;
            int[] cumulativeWeights = new int[rarityFactors.Length];
            for (int i = 0; i < rarityFactors.Length; i++)
            {
                totalWeight += rarityFactors[i].Rarity;
                cumulativeWeights[i] = totalWeight;
            }

            Dictionary<Fruit, int> tendency = new Dictionary<Fruit, int>();
            // Generate the noise-based grid
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    // Get Simplex noise value for the current position
                    double noiseValue = perlinGenerator.GetValue(x * scale, y * scale);

                    // Map the noise value (-1 to 1) to the range [1, rarityFactors.Length]
                    int num = MapNoiseToValueWithRarity(noiseValue, cumulativeWeights, totalWeight);
                    Fruit fruit = rarityFactors[num];
                    grid[x, y] = fruit;
                    UpdateTendency(tendency, fruit);
				}
            }

			Dictionary<Fruit, int> won = GetWinnings(tendency);
            SpinData result = new SpinData(grid, won);
			return result;
        }

        private int MapNoiseToValueWithRarity(double noise, int[] cumulativeWeights, int totalWeight)
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
			Console.WriteLine("Tendency:" + tendency.Count);

			foreach (var item in tendency)
			{
				if (item.Key.Name == "Lollipop")
				{
					if (item.Value >= 4)
					{
						won.Add(item.Key, item.Value);
					}

					continue;
				}

				if (item.Value >= 8)
				{
					won.Add(item.Key, item.Value);
				}
			}

			return won;
		}

		private void UpdateTendency(Dictionary<Fruit, int> tendency, Fruit fruit)
		{
			if (tendency.ContainsKey(fruit))
			{
				tendency[fruit]++;
			}
			else
			{
				tendency.Add(fruit, 1);
			}
		}


		private string GetSpinMessage(Fruit[,] grid)
        {
            string message = "";
			for (int y = 0; y < grid.GetLength(1); y++)
			{
				for (int x = 0; x < grid.GetLength(0); x++)
				{
					message += grid[x, y].Emoji + " ";
				}
				message += "\n";
			}
			return message;
        }


        //private EmbedBuilder BalanceEmbed(Fruit[,] grid)
        //{
        //    EmbedBuilder embedBuilder = new EmbedBuilder()
        //        .WithTitle(Name)
        //        .WithColor(Colors.Green)
        //        .WithCurrentTimestamp();

        //    for (int y = 0; y < grid.GetLength(1); y++)
        //    {
        //        for (int x = 0; x < grid.GetLength(0); x++)
        //        {
        //            embedBuilder.Description += grid[x, y].Emoji + " ";
        //        }
        //        embedBuilder.Description += "\n";
        //    }

        //    return embedBuilder;
        //}
    }
}



//private void PrintGrid(Fruit[,] grid)
//{
//    for (int y = 0; y < grid.GetLength(1); y++)
//    {
//        for (int x = 0; x < grid.GetLength(0); x++)
//        {
//            Console.Write(grid[x, y].Emoji + " ");
//        }
//        Console.WriteLine();
//    }
//}