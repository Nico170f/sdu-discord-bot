using Discord;
using Discord.WebSocket;
using new_discord_bot.Configuration;
using new_discord_bot.Data;
using new_discord_bot.Services;

namespace new_discord_bot.Commands
{
	public class BalanceCommand : ICommand
	{
		private readonly UserService _userService;

		public string Name => "balance";
		public string Description => "Show your current balance";

		public BalanceCommand(UserService userService)
		{
			_userService = userService;
		}

		public async Task Execute(SocketSlashCommand command)
		{
			EmbedBuilder embedBuilder;

			if (command.Data.Options.Any())
			{
				embedBuilder = await HasOption(command);
			}
			else
			{
				User user = await _userService.GetUserAsync(command.User.Id);
				embedBuilder = BalanceEmbed(command.User, user, false);
			}
			
			await command.RespondAsync(embed: embedBuilder.Build());
		}

		private async Task<EmbedBuilder> HasOption(SocketSlashCommand command)
		{
			if(command.Data.Options.Count > 1)
			{
				EmbedBuilder tooManyOptionsEmbed = new EmbedBuilder()
					.WithDescription("You can only check the balance of one user at a time");

				return tooManyOptionsEmbed;
			}

			SocketSlashCommandDataOption userOption = command.Data.Options.First();
			IUser? discUser = userOption.Value as IUser;

			if (discUser == null)
			{
				EmbedBuilder invalidUserEmbed = new EmbedBuilder()
					.WithDescription("Invalid user specified.");

				return invalidUserEmbed;
			}


			User dbUser = await _userService.GetUserAsync(discUser.Id);
			EmbedBuilder embedBuilder = BalanceEmbed(discUser, dbUser, true);

			return embedBuilder;
		}

		public EmbedBuilder BalanceEmbed(IUser discUser, User dbUser, bool withOption)
		{
			string username = withOption ? $"{discUser.Mention} has " : "You have ";

			EmbedBuilder embedBuilder = new EmbedBuilder()
				.WithAuthor(discUser.ToString(), discUser.GetAvatarUrl() ?? discUser.GetDefaultAvatarUrl())
				//.WithTitle("Roles")
				.WithDescription($"{username} ${dbUser.Balance}")
				.WithColor(Colors.Green)
				.WithCurrentTimestamp();

			return embedBuilder;
		}

		public SlashCommandProperties Create()
		{
			SlashCommandBuilder newCommand = new SlashCommandBuilder()
			.WithName(this.Name)
			.WithDescription(this.Description)
			.AddOption("user", ApplicationCommandOptionType.User, "The user to show the balance of");

			return newCommand.Build();
		}
	}
}


//users = await _userService.GetUsersAsync();
//foreach (var u in users)
//{
//	Console.WriteLine($"{u.Id} - {u.Balance} - {u.Spins}");
//}