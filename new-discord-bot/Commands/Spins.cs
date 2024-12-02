using Discord;
using Discord.WebSocket;

namespace new_discord_bot.Commands
{
	public class SpinsCommand : ICommand
	{
		public string Name => "spins";
		public string Description => "Show your remaining daily spins";

		public async Task Execute(SocketSlashCommand command)
		{
			await command.RespondAsync("spins! 1111");
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
