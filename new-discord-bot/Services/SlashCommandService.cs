using Discord.WebSocket;
using Discord;
using Discord.Commands;
using new_discord_bot.Commands;

namespace new_discord_bot.Services
{
	public class SlashCommandService
	{
		private Dictionary<string, ICommand> _commands;
		private readonly DiscordSocketClient _client;
		private readonly CommandLoaderService _commandLoader;
		private readonly UserService _userService;


		public SlashCommandService(DiscordSocketClient client, UserService userService)
		{
			_client = client;
			_userService = userService;
			_client.SlashCommandExecuted += HandleCommandAsync;
			_commandLoader = new CommandLoaderService(_userService);
			_commands = _commandLoader.LoadCommands();
		}

		public async Task RegisterCommandsAsync()
		{
			// _commands = _commandLoader.LoadCommands();
			SlashCommandProperties[] commands = new SlashCommandProperties[_commands.Count];

			int index = 0;
			foreach (var command in _commands)
			{
				Console.WriteLine("Creating command: " + command.Key);
				commands[index++] = command.Value.Create();
			}

			await _client.Rest.BulkOverwriteGuildCommands(commands, 569218374123520031);
			// await _client.Rest.BulkOverwriteGlobalCommands([]);
		}

		public async Task HandleCommandAsync(SocketSlashCommand command)
		{
			try
			{
				_commands.TryGetValue(command.Data.Name, out ICommand? commandHandler);
				if (commandHandler != null)
				{
					await commandHandler.Execute(command);
					return;
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				await command.RespondAsync("An error occurred while executing the command");
			}


		}
	}
}
