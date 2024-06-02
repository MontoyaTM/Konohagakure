Status: 
Tag:
Links:

---
> [!note] 
>  # DiscordBot Class

The DiscordBot class extends the BackgroundService to implement the worker service application. Since DSharpPlus implements the Task-based Asynchronous Pattern, the majority of methods must be executed in methods marked as async so they can be properly awaited.

The DiscordClient is used to setup the configuration for the Discord Bot. The CommandsNextExtension, ButtonCommandsExtension, and ModalCommandsExtension are classes that handles command registration, management, and execution respectively. 

``` run-csharp
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
```

## StartAsync(..)

The StartAsync() is an asynchronous method used to load the Konohagakure Discord Bot with required configuration to connect to the Discord gateway.

``` run-csharp
public override async Task StartAsync(CancellationToken cancellationToken)
{
	_logger.LogInformation("Starting Konohagakure Bot...");
```

1. The first step is to retrieve the configuration detailed stored in the User Secrets.

``` run-csharp
	string botToken = _config.GetConnectionString("Token");
	string botPrefix = _config.GetConnectionString("Prefix");
```

2. Initialize the DiscordClient by creating an instance of the DiscordConfiguration class. In here, you will provide the necessary information in regards to the bot.

- https://dsharpplus.github.io/DSharpPlus/articles/beyond_basics/intents.html

``` run-csharp
Client = new DiscordClient(new DiscordConfiguration()
{
	Intents = DiscordIntents.All,
	Token = botToken,
	TokenType = TokenType.Bot,
	AutoReconnect = true,
	MinimumLogLevel = LogLevel.Debug
});
```

3. Register services for DSharpPlus's DependencyInjection

- https://dsharpplus.github.io/DSharpPlus/articles/commands_next/dependency_injection.html

``` run-csharp
var botServices = new ServiceCollection()
	.AddSingleton<IConfiguration>(_config)
	.AddTransient<IDatabaseRequestData, PostgreSQLRequestData>()
	.AddTransient<IDatabaseProfileData, PostgreSQLProfileData>()
	.AddTransient<IPostgreSQLDataAccess, PostgreSQLDataAccess>()
		.BuildServiceProvider();
```

4. Default timeout for Commands using Interactivty.

``` run-csharp
Client.UseInteractivity(new InteractivityConfiguration()
{
	Timeout = TimeSpan.FromMinutes(5)
});
```

5. Setting up command configuration for Commands, Button Commands, Modal Commands, and Slash Commands.

- https://dsharpplus.github.io/DSharpPlus/articles/commands/introduction.html?tabs=main-method
- https://dsharpplus.github.io/DSharpPlus/articles/advanced_topics/buttons.html
- https://dsharpplus.github.io/DSharpPlus/articles/slash_commands.html

``` run-csharp
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
```

5. Task handlers for various command operations.

``` run-csharp
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
```

6. Registering commands

``` run-csharp
Commands.RegisterCommands<PrefixCommandVillagerApplication>();
ButtonCommands.RegisterButtons<ButtonCommandVillagerApplication>();
ModalCommands.RegisterModals<ModalCommandVillagerApplication>();

Commands.RegisterCommands<PrefixCommandHokageDashboard>();
ButtonCommands.RegisterButtons<ButtonCommandHokageDashboard>();

Commands.RegisterCommands<PrefixCommandRaidDashboard>();
ButtonCommands.RegisterButtons<ButtonCommandRaidDashboard>();

slashCommands.RegisterCommands<SlashCommandProfile>();
Commands.RegisterCommands<PrefixCommandProfile>();
ButtonCommands.RegisterButtons<ButtonCommandProfile>();
ModalCommands.RegisterModals<ModalCommandProfile>();

Commands.RegisterCommands<PrefixCommandProctor>();
ButtonCommands.RegisterButtons<ButtonCommandProctor>();
ModalCommands.RegisterModals<ModalCommandProctor>();
```

7. Connect to the gateway

``` run-csharp
// 7. Connecting...
await Client.ConnectAsync();
```


## OnClient_ComponentInteractionCreated(..)

When component types are interacted with, they invokes a ComponentInteractionCreated which is handled by the OnClient_ComponentInteractionCreated() method. The options listed within the switch statement refer to a dropdown menu for updating a user's <span style="color:rgb(102, 240, 129)">Character Profile</span>.


In the event args, Id will be the id of the button you specified. There's also an Interaction property, which contains the interaction the event created. It's important to respond to an interaction within 3 seconds, or it will time out. Responding after this period will throw a NotFoundException.

- https://dsharpplus.github.io/DSharpPlus/articles/advanced_topics/selects.html

``` run-csharp
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
```

---
References: