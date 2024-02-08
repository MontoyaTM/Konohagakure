using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.ButtonCommands.EventArgs;
using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.ModalCommands;
using DSharpPlus.ModalCommands.EventArgs;
using DSharpPlus.ModalCommands.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.EventArgs;
using Konohagakure.VillagerApplication;
using KonohagakureLibrary.Data;
using KonohagakureLibrary.DBUtil;
using Microsoft.Extensions.Logging;

namespace Konohagakure
{
	public class DiscordBot : BackgroundService
	{
		private readonly ILogger<DiscordBot> _logger;
		private readonly IConfiguration _config;

		private DiscordClient Client;
		private CommandsNextExtension Commands;
		private ButtonCommandsExtension ButtonCommands;
		private ModalCommandsExtension ModalCommands;

		public DiscordBot(ILogger<DiscordBot> logger, IConfiguration config)
		{
			_logger = logger;
			_config = config;
		}

		public override async Task StartAsync(CancellationToken cancellationToken)
		{
			_logger.LogInformation("Starting Konohagakure Bot...");

			// 1. Retrieve config details
			string botToken = _config.GetConnectionString("Token");
			string botPrefix = _config.GetConnectionString("Prefix");

			// 2. Creating discord bot configuration for client
			Client = new DiscordClient(new DiscordConfiguration()
			{
				Intents = DiscordIntents.All,
				Token = botToken,
				TokenType = TokenType.Bot,
				AutoReconnect = true,
				MinimumLogLevel = LogLevel.Debug
			});

			// 3. Register Services for DSharpPlus's DependencyInjection
			var botServices = new ServiceCollection()
				.AddSingleton<IConfiguration>(_config)
				.AddTransient<IDatabaseProfileData, PostgreSQLProfileData>()
				.AddTransient<IPostgreSQLDataAccess, PostgreSQLDataAccess>()
					.BuildServiceProvider();

			// 4. Default timeout for Commands using Interactivity
			Client.UseInteractivity(new InteractivityConfiguration()
			{
				Timeout = TimeSpan.FromMinutes(5)
			});

			// 5. Setting up command configurations for Commands, Buttons, and Modals
			var prefixCommandsConfig = new CommandsNextConfiguration()
			{
				StringPrefixes = new string[] { botPrefix },
				EnableMentionPrefix = true,
				EnableDms = true,
				EnableDefaultHelp = true,
				Services = botServices
			};
			Commands = Client.UseCommandsNext(prefixCommandsConfig);

			var buttonCommandsConfig = new ButtonCommandsConfiguration()
			{
				ArgumentSeparator = "-",
				Prefix = botPrefix
			};
			ButtonCommands = Client.UseButtonCommands(buttonCommandsConfig);

			var modalCommandsConfig = new ModalCommandsConfiguration()
			{
				Seperator = "-",
				Prefix = botPrefix,
				Services = botServices
			};
			ModalCommands = Client.UseModalCommands(modalCommandsConfig);

			var slashCommands = Client.UseSlashCommands();

			// 5. Creating Task Handlers
			Client.Ready += OnClient_Ready;
			Client.ComponentInteractionCreated += OnClient_ComponentInteractionCreated;
			Client.ModalSubmitted += OnClient_ModalSubmitted;

			Commands.CommandExecuted += OnCommands_CommandExecuted;
			Commands.CommandErrored += OnCommands_CommandErrored; ;

			ButtonCommands.ButtonCommandExecuted += OnButtonCommands_ButtonCommandExecuted;
			ButtonCommands.ButtonCommandErrored += OnButtonCommands_ButtonCommandErrored;

			ModalCommands.ModalCommandExecuted += OnModalCommands_ModalCommandExecuted;
			ModalCommands.ModalCommandErrored += OnModalCommands_ModalCommandErrored;

			slashCommands.SlashCommandErrored += OnSlashCommands_SlashCommandErrored;

			// 6. Registering Commands

			#region Villager Application

			Commands.RegisterCommands<PrefixCommandVillagerApplication>();
			ButtonCommands.RegisterButtons<ButtonCommandVillagerApplication>();
			ModalCommands.RegisterModals<ModalCommandVillagerApplication>();

			#endregion

			// 7. Connecting...
			await Client.ConnectAsync();
		}

		private Task OnSlashCommands_SlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs args)
		{
			_logger.LogError(args.Exception,
				$"{ args.Context.Member } has used slash command { args.Context.CommandName } which threw an Exception: {args.Exception.Message}");

			return Task.CompletedTask;
		}

		private Task OnModalCommands_ModalCommandErrored(ModalCommandsExtension sender, ModalCommandErrorEventArgs args)
		{
			_logger.LogError(args.Exception,
				$"{args.Context.Member} has used modal command {args.CommandName} which threw an Exception: {args.Exception.Message}");

			return Task.CompletedTask;
		}

		private Task OnButtonCommands_ButtonCommandErrored(ButtonCommandsExtension sender, ButtonCommandErrorEventArgs args)
		{
			_logger.LogError(args.Exception,
				$"{args.Context.Member} has used button command {args.CommandName} which threw an Exception: {args.Exception.Message}");

			return Task.CompletedTask;
		}

		private Task OnCommands_CommandErrored(CommandsNextExtension sender, CommandErrorEventArgs args)
		{
			_logger.LogError(args.Exception,
				$"{args.Context.Member} has used button command {args.Command.Name} which threw an Exception: {args.Exception.Message}");

			return Task.CompletedTask;
		}

		private Task OnModalCommands_ModalCommandExecuted(ModalCommandsExtension sender, ModalCommandExecutionEventArgs args)
		{
			return Task.CompletedTask;
		}

		private Task OnButtonCommands_ButtonCommandExecuted(ButtonCommandsExtension sender, ButtonCommandExecutionEventArgs args)
		{
			return Task.CompletedTask;
		}

		private Task OnCommands_CommandExecuted(CommandsNextExtension sender, CommandExecutionEventArgs args)
		{
			return Task.CompletedTask;
		}

		private Task OnClient_ModalSubmitted(DiscordClient sender, ModalSubmitEventArgs args)
		{
			return Task.CompletedTask;
		}

		private Task OnClient_Ready(DiscordClient sender, ReadyEventArgs args)
		{
			return Task.CompletedTask;
		}

		private Task OnClient_ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs args)
		{
			return Task.CompletedTask;
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken) => Task.CompletedTask;

		public override async Task StopAsync(CancellationToken cancellationToken)
		{
			await Client.DisconnectAsync();
			Client.Dispose();
			_logger.LogInformation("Konohagakure Bot has stopped...");
		}
	}
}
