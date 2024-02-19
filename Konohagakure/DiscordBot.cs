using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.ButtonCommands.EventArgs;
using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.ModalCommands;
using DSharpPlus.ModalCommands.EventArgs;
using DSharpPlus.ModalCommands.Extensions;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using DSharpPlus.SlashCommands.EventArgs;
using Konohagakure.Dashboards.Hokage;
using Konohagakure.Dashboards.Proctor;
using Konohagakure.Dashboards.Raid;
using Konohagakure.Profile;
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
		private readonly IDatabaseProfileData _db;

		private DiscordClient Client;
		private CommandsNextExtension Commands;
		private ButtonCommandsExtension ButtonCommands;
		private ModalCommandsExtension ModalCommands;

		public DiscordBot(ILogger<DiscordBot> logger, IConfiguration config, IDatabaseProfileData _db)
		{
			_logger = logger;
			_config = config;
			this._db = _db;
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
				.AddTransient<IDatabaseRequestData, PostgreSQLRequestData>()
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
				Prefix = botPrefix,
				Services = botServices
				
			};
			ButtonCommands = Client.UseButtonCommands(buttonCommandsConfig);

			var modalCommandsConfig = new ModalCommandsConfiguration()
			{
				Seperator = "-",
				Prefix = botPrefix,
				Services = botServices
			};
			ModalCommands = Client.UseModalCommands(modalCommandsConfig);

			var slashCommandsConfig = new SlashCommandsConfiguration()
			{
				Services = botServices
			};

			var slashCommands = Client.UseSlashCommands(slashCommandsConfig);

			// 5. Creating Task Handlers
			Client.Ready += OnClient_Ready;
			Client.ComponentInteractionCreated += OnClient_ComponentInteractionCreated;
			Client.ModalSubmitted += OnClient_ModalSubmitted;

			Commands.CommandExecuted += OnCommands_CommandExecuted;
			Commands.CommandErrored += OnCommands_CommandErrored;
			
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

			#region Hokage Dashboard

			Commands.RegisterCommands<PrefixCommandHokageDashboard>();
			ButtonCommands.RegisterButtons<ButtonCommandHokageDashboard>();

			#endregion


			#region Raid Dashboard

			Commands.RegisterCommands<PrefixCommandRaidDashboard>();
			ButtonCommands.RegisterButtons<ButtonCommandRaidDashboard>();

			#endregion

			#region Profile

			slashCommands.RegisterCommands<SlashCommandProfile>();
			Commands.RegisterCommands<PrefixCommandProfile>();
			ButtonCommands.RegisterButtons<ButtonCommandProfile>();
			ModalCommands.RegisterModals<ModalCommandProfile>();

			#endregion

			#region Ranked Dashboard

			Commands.RegisterCommands<PrefixCommandProctor>();
			ButtonCommands.RegisterButtons<ButtonCommandProctor>();
			ModalCommands.RegisterModals<ModalCommandProctor>();

			#endregion

			// 7. Connecting...
			await Client.ConnectAsync();
		}

		private async Task OnSlashCommands_SlashCommandErrored(SlashCommandsExtension sender, SlashCommandErrorEventArgs args)
		{
			_logger.LogError(args.Exception,
				$"{ args.Context.Member } has used slash command { args.Context.CommandName } which threw an Exception: {args.Exception.Message}");

			if (args.Exception is SlashExecutionChecksFailedException exception)
			{
				string timeLeft = string.Empty;
				foreach (var check in exception.FailedChecks)
				{
					var cooldown = (SlashCooldownAttribute)check;
					timeLeft = cooldown.GetRemainingCooldown(args.Context).ToString(@"hh\:mm\:ss");
				}

				var coolDownMessage = new DiscordEmbedBuilder()
				{
					Color = DiscordColor.Red,
					Title = "Please wait for the cooldown to end.",
					Description = $"Time: {timeLeft}"
				};

				await args.Context.CreateResponseAsync(coolDownMessage);
			}
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

		private async Task OnClient_ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs args)
		{
			if (args.Interaction.Data.ComponentType == ComponentType.StringSelect)
			{
				switch (args.Interaction.Data.CustomId)
				{
					case "dpdwn_ClanEmoji":
						await args.Interaction.DeferAsync(true);
						string clanSelected = args.Interaction.Data.Values[0];

						var userExists_Clan = await _db.UserExistsAsync(args.Interaction.User.Id);

						if (!userExists_Clan)
						{
							await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("You do not have a profile in the database!"));
							break;
						}

						var isClanUpdated = await _db.UpdateClanAsync(args.User.Id, clanSelected);

						if (isClanUpdated)
						{
							await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully updated your clan!"));
						}
						else
						{
							await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to update your clan!"));
						}
						break;


					case "dpdwn_MasteryEmoji":
						await args.Interaction.DeferAsync(true);
						var masterySelected = args.Interaction.Data.Values.ToArray();

						var userExists_Mastery = await _db.UserExistsAsync(args.Interaction.User.Id);

						if (!userExists_Mastery)
						{
							await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("You do not have a profile in the database!"));
							break;
						}

						var isMasteryUpdated = await _db.UpdateMasteriesAsync(args.User.Id, masterySelected);

						if (isMasteryUpdated)
						{
							await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully updated your masteries!"));
						}
						else
						{
							await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to update your masteries!"));
						}
						break;
				}


			}
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
