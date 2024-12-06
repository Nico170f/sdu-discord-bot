using Discord;
using Discord.WebSocket;
using new_discord_bot.Configuration;
using LibNoise;
using LibNoise.Primitive;

namespace new_discord_bot.Games
{

	class Fruit
	{
		public string Name { get; }
		public int Value { get; }
		public int Rarity { get; }
		public string Emoji { get; }

		public Fruit(string name, int value, int rarity, string emoji)
		{
			Name = name;
			Value = value;
			Rarity = rarity;
			Emoji = emoji;
		}
	}

	public class BonanzaSlot : ISlot
	{
		public string Name { get; } = "Bonanza";
		public string Description { get; } = "Sweet Bonanza";
		public int Width { get; } = 6;
		public int Height { get; } = 5;


		public async Task Execute(SocketSlashCommand command)
		{
			//int[] rarityFactors = { 100, 90, 80, 70, 50, 30, 10 };
			Fruit banana = new Fruit("Banana", 1, 100, "🍌");
			Fruit grape = new Fruit("Grapes", 2, 90, "🍇");
			Fruit orange = new Fruit("Melon", 3, 90, "🍈");
			Fruit peach = new Fruit("Peach", 3, 80, "🍑");
			Fruit apple = new Fruit("Apple", 3, 80, "🍎");
			Fruit blueCandy = new Fruit("Blue Candy", 3, 70, "🔷");
			Fruit greenCandy = new Fruit("Green Candy", 3, 70, "🟩");
			Fruit purpleCandy = new Fruit("Purple Candy", 3, 65, "🟣");
			Fruit heart = new Fruit("Heart", 3, 60, "❤️");
			Fruit lollipop = new Fruit("Lollipop", 3, 40, "🍭");
			Fruit[] rarityFactors = { banana, grape, orange, peach, apple, blueCandy, greenCandy, purpleCandy, heart, lollipop };

			Fruit[,] grid = GenerateGroupedRandomGrid(rarityFactors);

			EmbedBuilder embedBuilder = BalanceEmbed(grid);
			await command.RespondAsync(embed: embedBuilder.Build());
		}

		private Fruit[,] GenerateGroupedRandomGrid(Fruit[] rarityFactors)
		{
			Fruit[,] grid = new Fruit[this.Width, this.Height];

			// Initialize SimplexPerlin noise generator
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

			// Generate the noise-based grid
			for (int y = 0; y < this.Height; y++)
			{
				for (int x = 0; x < this.Width; x++)
				{
					// Get Simplex noise value for the current position
					double noiseValue = perlinGenerator.GetValue(x * scale, y * scale);

					// Map the noise value (-1 to 1) to the range [1, rarityFactors.Length]
					int num = MapNoiseToValueWithRarity(noiseValue, cumulativeWeights, totalWeight);
					grid[x, y] = rarityFactors[num];
				}
			}

			return grid;
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

		private void PrintGrid(Fruit[,] grid)
		{
			for (int y = 0; y < grid.GetLength(1); y++)
			{
				for (int x = 0; x < grid.GetLength(0); x++)
				{
					Console.Write(grid[x, y].Emoji + " ");
				}
				Console.WriteLine();
			}
		}


		private EmbedBuilder BalanceEmbed(Fruit[,] grid)
		{
			EmbedBuilder embedBuilder = new EmbedBuilder()
				.WithTitle(this.Name)
				.WithColor(Colors.Green)
				.WithCurrentTimestamp();

			for (int y = 0; y < grid.GetLength(1); y++)
			{
				for (int x = 0; x < grid.GetLength(0); x++)
				{
					//Console.Write(grid[x, y].Emoji + " ");
					embedBuilder.Description += grid[x, y].Emoji + " ";
				}
				//Console.WriteLine();
				embedBuilder.Description += "\n";
			}

			return embedBuilder;
		}
	}
}



