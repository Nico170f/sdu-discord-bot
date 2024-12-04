using Discord;
using Discord.WebSocket;
using new_discord_bot.Services;

namespace new_discord_bot.Commands
{
	public class ResetCommand : ICommand
	{
		public string Name => "reset";
		public string Description => "reset db";
		private readonly UserService _userService;


		public ResetCommand(UserService userService) {
			_userService = userService;
		}

		public async Task Execute(SocketSlashCommand command)
		{
			await _userService.RemoveAllUsersAsync();
			await command.RespondAsync("reset");
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
