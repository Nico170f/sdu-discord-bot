using Discord;
using Discord.WebSocket;

namespace new_discord_bot.Commands
{
	public class GambleCommand : ICommand
	{
		public string Name => "gamble";
		public string Description => "Ping command to check bot's status.";

		public async Task Execute(SocketSlashCommand command)
		{
			await command.RespondAsync("Pong! 1111");
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
