using Discord;
using Discord.Commands;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;

namespace gambling_discord_bot
{
	internal class old_Program
	{
		private static DiscordSocketClient _client;
		private static CommandService _commands;
		private static LoggingService _loggingService;
		public static string FieldA { get; set; } = "test";
		public static int FieldB { get; set; } = 10;
		public static bool FieldC { get; set; } = true;

		static async Task Main(string[] args)
		{
			_client = new DiscordSocketClient();
			_commands = new CommandService();
			_loggingService = new LoggingService(_client, _commands);

			_client.Log += Log;

			var token = "";

			await _client.LoginAsync(TokenType.Bot, token);
			await _client.StartAsync();

			_client.Ready += Ready;
			_client.SlashCommandExecuted += SlashCommandHandler;

			
			// Block this task until the program is closed.
			await Task.Delay(-1);
		}

		private static Task Log(LogMessage msg)
		{
			Console.WriteLine(msg.ToString());
			return Task.CompletedTask;
		}

		private static async Task Ready()
		{
			Console.WriteLine("Bot is ready");

			await CommandBuilder.BuildCommand(_client);
			await CommandSettings();

			ulong guildId = 1224272485512511558;
			var guild = _client.GetGuild(guildId);

			var guildCommand = new SlashCommandBuilder();
			guildCommand.WithName("first-command");
			guildCommand.WithDescription("This is my first guild slash command!");

			var globalCommand = new SlashCommandBuilder();
			globalCommand.WithName("first-global-command");
			globalCommand.WithDescription("This is my first global slash command");

			try
			{
				await guild.CreateApplicationCommandAsync(guildCommand.Build());
				await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
				// Using the ready event is a simple implementation for the sake of the example. Suitable for testing and development.
				// For a production bot, it is recommended to only run the CreateGlobalApplicationCommandAsync() once for each command.
			}
			catch (ApplicationCommandException exception)
			{
				var json = JsonConvert.SerializeObject(exception.Errors, Formatting.Indented);
				Console.WriteLine(json);
			}
		}


		private static async Task SlashCommandHandler(SocketSlashCommand command)
		{

			switch (command.Data.Name)
			{
				case "list-roles":
					await HandleListRoleCommand(command);
					return;
				case "settings":
					await HandleSettingsCommand(command);
					return;
			}

			await command.RespondAsync($"You executed {command.Data.Name}");

		}


		//private static async Task ButtonSpawner()
		//{

		//}

		//[Command("spawner")]
		//public async Task Spawn()
		//{
		//	var builder = new ComponentBuilder()
		//		.WithButton("label", "custom-id");

		//	await ReplyAsync("Here is a button!", components: builder.Build());
		//}


		private static async Task HandleListRoleCommand(SocketSlashCommand command)
		{
			// We need to extract the user parameter from the command. since we only have one option and it's required, we can just use the first option.
			var guildUser = (SocketGuildUser)command.Data.Options.First().Value;

			// We remove the everyone role and select the mention of each role.
			var roleList = string.Join(",\n", guildUser.Roles.Where(x => !x.IsEveryone).Select(x => x.Mention));

			var embedBuilder = new EmbedBuilder()
				.WithAuthor(guildUser.ToString(), guildUser.GetAvatarUrl() ?? guildUser.GetDefaultAvatarUrl())
				.WithTitle("Roles")
				.WithDescription(roleList)
				.WithColor(Color.Green)
				.WithCurrentTimestamp();

			// Now, Let's respond with the embed.
			await command.RespondAsync(embed: embedBuilder.Build(), ephemeral: true);
		}




		public static async Task CommandSettings()
		{
			ulong guildId = 1224272485512511558;

			var guildCommand = new SlashCommandBuilder()
				.WithName("settings")
				.WithDescription("Changes some settings within the bot.")
				.AddOption(new SlashCommandOptionBuilder()
					.WithName("field-a")
					.WithDescription("Gets or sets the field A")
					.WithType(ApplicationCommandOptionType.SubCommandGroup)
					.AddOption(new SlashCommandOptionBuilder()
						.WithName("set")
						.WithDescription("Sets the field A")
						.WithType(ApplicationCommandOptionType.SubCommand)
						.AddOption("value", ApplicationCommandOptionType.String, "the value to set the field", isRequired: true)
					).AddOption(new SlashCommandOptionBuilder()
						.WithName("get")
						.WithDescription("Gets the value of field A.")
						.WithType(ApplicationCommandOptionType.SubCommand)
					)
				).AddOption(new SlashCommandOptionBuilder()
					.WithName("field-b")
					.WithDescription("Gets or sets the field B")
					.WithType(ApplicationCommandOptionType.SubCommandGroup)
					.AddOption(new SlashCommandOptionBuilder()
						.WithName("set")
						.WithDescription("Sets the field B")
						.WithType(ApplicationCommandOptionType.SubCommand)
						.AddOption("value", ApplicationCommandOptionType.Integer, "the value to set the fie to.", isRequired: true)
					).AddOption(new SlashCommandOptionBuilder()
						.WithName("get")
						.WithDescription("Gets the value of field B.")
						.WithType(ApplicationCommandOptionType.SubCommand)
					)
				).AddOption(new SlashCommandOptionBuilder()
					.WithName("field-c")
					.WithDescription("Gets or sets the field C")
					.WithType(ApplicationCommandOptionType.SubCommandGroup)
					.AddOption(new SlashCommandOptionBuilder()
						.WithName("set")
						.WithDescription("Sets the field C")
						.WithType(ApplicationCommandOptionType.SubCommand)
						.AddOption("value", ApplicationCommandOptionType.Boolean, "the value to set the fie to.", isRequired: true)
					).AddOption(new SlashCommandOptionBuilder()
						.WithName("get")
						.WithDescription("Gets the value of field C.")
						.WithType(ApplicationCommandOptionType.SubCommand)
					)
				);

			try
			{
				await _client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
			}
			catch (ApplicationCommandException exception)
			{
				var json = JsonConvert.SerializeObject(exception.Errors.FirstOrDefault(), Formatting.Indented);
				Console.WriteLine(json);
			}




		}


		private static async Task HandleSettingsCommand(SocketSlashCommand command)
		{
			// First lets extract our variables
			var fieldName = command.Data.Options.FirstOrDefault()?.Name;
			var getOrSet = command.Data.Options.FirstOrDefault()?.Options.FirstOrDefault()?.Name;
			var value = command.Data.Options.FirstOrDefault()?.Options.FirstOrDefault()?.Options?.FirstOrDefault()?.Value;

			if (fieldName == null || getOrSet == null)
			{
				await command.RespondAsync("Invalid command structure.", ephemeral: true);
				return;
			}

			switch (fieldName)
			{
				case "field-a":
					{
						if (getOrSet == "get")
						{
							await command.RespondAsync($"The value of `field-a` is `{FieldA}`");
						}
						else if (getOrSet == "set")
						{
							if (value == null)
							{
								await command.RespondAsync("Value for `field-a` is missing.", ephemeral: true);
								return;
							}
							FieldA = (string)value;
							await command.RespondAsync($"`field-a` has been set to `{FieldA}`");
						}
					}
					break;
				case "field-b":
					{
						if (getOrSet == "get")
						{
							await command.RespondAsync($"The value of `field-b` is `{FieldB}`");
						}
						else if (getOrSet == "set")
						{
							if (value == null)
							{
								await command.RespondAsync("Value for `field-b` is missing.", ephemeral: true);
								return;
							}
							FieldB = (int)(long)value; // Cast to long first because Discord.NET uses long for integers
							await command.RespondAsync($"`field-b` has been set to `{FieldB}`");
						}
					}
					break;
				case "field-c":
					{
						if (getOrSet == "get")
						{
							await command.RespondAsync($"The value of `field-c` is `{FieldC}`");
						}
						else if (getOrSet == "set")
						{
							if (value == null)
							{
								await command.RespondAsync("Value for `field-c` is missing.", ephemeral: true);
								return;
							}
							FieldC = (bool)value;
							await command.RespondAsync($"`field-c` has been set to `{FieldC}`");
						}
					}
					break;
				default:
					await command.RespondAsync("Unknown field.", ephemeral: true);
					break;
			}
		}
	}


		public class CommandBuilder
	{

		public static async Task BuildCommand(DiscordSocketClient _client)
		{
			ulong guildId = 1224272485512511558;

			var guildCommand = new Discord.SlashCommandBuilder()
				.WithName("list-roles")
				.WithDescription("Lists all roles of a user.")
				.AddOption("user", ApplicationCommandOptionType.User, "The users whos roles you want to be listed", isRequired: true);

			try
			{
				await _client.Rest.CreateGuildCommand(guildCommand.Build(), guildId);
			}
			catch (ApplicationCommandException exception)
			{
				var json = JsonConvert.SerializeObject(exception.Errors.FirstOrDefault(), Formatting.Indented);
				Console.WriteLine(json);
			}

		}

	}




	public class LoggingService
	{
		public LoggingService(DiscordSocketClient client, CommandService command)
		{
			client.Log += LogAsync;
			command.Log += LogAsync;
		}
		private Task LogAsync(LogMessage message)
		{
			if (message.Exception is CommandException cmdException)
			{
				Console.WriteLine($"[Command/{message.Severity}] {cmdException.Command.Aliases.First()}"
					+ $" failed to execute in {cmdException.Context.Channel}.");
				Console.WriteLine(cmdException);
			}
			else
				Console.WriteLine($"[General/{message.Severity}] {message}");

			return Task.CompletedTask;
		}
	}
}
