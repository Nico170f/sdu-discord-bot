using Discord;
using Discord.WebSocket;
using new_discord_bot.Configuration;
using new_discord_bot.Data;
using new_discord_bot.Services;

namespace new_discord_bot.Commands
{
	public class LeaderboardCommand : ICommand
	{
		public string Name => "leaderboard";
		public string Description => "xxxxx";
		private readonly UserService _userService;

		public LeaderboardCommand(UserService userService)
		{
			_userService = userService;
		}

		public async Task Execute(SocketSlashCommand command)
		{
			List<User> users = await _userService.GetUsersAsync();
			Embed embed = LeaderboardEmbed(users).Build();
			await command.RespondAsync(embed: embed);
		}

		public EmbedBuilder LeaderboardEmbed(List<User> users)
		{
			if(users.Count == 0)
			{
				return new EmbedBuilder()
				.WithTitle("Leaderboard")
				.WithDescription("No users found")
				.WithColor(Colors.Red);
			}

			users.Sort();


			EmbedBuilder embedBuilder = new EmbedBuilder()
			.WithTitle("Leaderboard")
			.WithColor(Colors.Green);

			User first = users[0];
			embedBuilder.WithDescription($"🥇 <@{first.Id}> - ${first.Balance}");

			if (users.Count > 1)
			{
				User second = users[1];
				embedBuilder.Description += $"\n🥈 <@{second.Id}> - ${second.Balance}";
			}

			if (users.Count > 2)
			{
				User third = users[2];
				embedBuilder.Description += $"\n:third_place: <@{third.Id}> - ${third.Balance}";
			}

			if (users.Count > 3)
			{
				users.RemoveRange(0, 3);
				string restUsersList = string.Join("\n", users.Select((user, index) => $"{index + 4}. <@{user.Id}>"));
				string restBalancesList = string.Join("\n", users.Select(user => $"${user.Balance}"));
				embedBuilder.AddField("Users:", restUsersList, true);
				embedBuilder.AddField("Balance", restBalancesList, true);
				embedBuilder.Description += "\n\nThe rest of the users:";
			}

			return embedBuilder;
		}

		public SlashCommandProperties Create()
		{
			SlashCommandBuilder newCommand = new SlashCommandBuilder()
			.WithName(this.Name)
			.WithDescription(this.Description);

			return newCommand.Build();
		}
	}
}
