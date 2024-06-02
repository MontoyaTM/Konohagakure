Status: 
Tag:
Links: 

---
> [!note] 
>  # ButtonCommandRaidDashboard Class

The ButtonCommandRaidDashboard Class extends ButtonCommandModule Class, which is required for commands to be executed. The db variable is used to call the PostgreSQLProfileData Class that implements the IDatabaseProfileData through Dependency Injection.

``` run-csharp
[ModuleLifespan(ModuleLifespan.Transient)]
public class ButtonCommandRaidDashboard : ButtonCommandModule
{
	public IDatabaseProfileData _db { get; set; }
```


## RaidCounter(..)

The RaidCounter() method is used to increment all raid stats for the user profile in the database that matches the member id given. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

![[Raid Dashboard.png| center]]

- Determines whether the user that pressed the Raid++ button has the Hokage, Council, or Raid Leader roles.
	- <span style="color:rgb(255, 97, 97)">False</span>: Return a deferred message. "Unable to use button command Raid++ for {ctx.Interaction.User.Username}, please check required roles."

- Determines whether the Discord server contains a voice channel called "Village Raid".
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Channel: Village Raid does not exist!"

- Determines whether the Discord server contains a channel called "raid-records" where the logs will be stored.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Channel: raid-records does not exist!"

- Determine whether there are users in the Village Raid channel.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred message. "There are no users in the Village Raid voice channel!"

The method create a list of DiscordMembers to have as a reference for member ids so that it can check against the database for user profiles. If the user profile was found, it would increment their raid stat by one. A deferred message will be returned. "Successfully incremented all Member's Raids stat!"

A deferred embedded message will be sent to the raid-records to store as a log for command execution.

![[Raid++.png| center]]

``` run-csharp
[ButtonCommand("btn_VillageRaid")]
public async Task RaidCounter(ButtonContext ctx)
{
	await ctx.Interaction.DeferAsync(true);
	
	var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council" || x.Name == "Raid Leader");

	if (hasRole)
	{
		var guildChannels = await ctx.Guild.GetChannelsAsync();
		var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");
		var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "raid-records");

		if (raidChannel == null)
		{
			var embedFailed = new DiscordEmbedBuilder()
			{
				Title = "Raid++ Failed",
				Color = DiscordColor.Red,
				Description = "Channel: Village Raid does not exist!"
			};

			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
			return;
		}

		if (recordsChannel == null)
		{
			var embedFailed = new DiscordEmbedBuilder()
			{
				Title = "Raid++ Failed",
				Color = DiscordColor.Red,
				Description = "Channel: raid-records does not exist!"
			};

			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
			return;
		}

		if (raidChannel.Users.Count() == 0)
		{
			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"There are no users in the Village Raid voice channel!"));
			return;
		}

		List<DiscordMember> Members = raidChannel.Users.ToList();

		string[] username = new string[Members.Count];

		var i = 0;

		foreach (var member in Members)
		{
			var userExists = await _db.UserExistsAsync(member.Id);

			if (userExists)
			{
				var isUpdated = await _db.UpdateRaidAsync(member.Id);

				username[i] = member.Username;
				i++;
			}
		}

		var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

		var embedRaid = new DiscordMessageBuilder()
			.AddEmbed(new DiscordEmbedBuilder()
				.WithColor(DiscordColor.SpringGreen)
				.WithTitle("Leaf Village — Raid++")
				.WithDescription(string.Join("\n", username))
				.WithFooter("• " + dateTime + "     • Raid Leader: " + ctx.Interaction.User.Username)
			);

		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully incremented all Member's Raids stat!"));

		await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(recordsChannel.Id), embedRaid);

	}
	else
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
			.WithContent($"Unable to use button command Raid++ for {ctx.Interaction.User.Username}, please check required roles."));
	}
}
```

## RetrieveMasteries() 

The RetrieveMasteries() method is used to retrieve all masteries from the user profile in the database that matches the member id given. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

![[Raid Dashboard.png| center]]

- Determines whether the user that pressed the Raid Composition button has the Hokage, Council, or Raid Leader roles.
	- <span style="color:rgb(255, 97, 97)">False</span>: Return a deferred message. "Unable to use button command Retrieve Masteries for {ctx.Interaction.User.Username}, please check required roles."

- Determines whether the Discord server contains a voice channel called "Village Raid".
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Channel: Village Raid voice channel does not exist!"

- Determines whether the Discord server contains a channel called "war-room" where the logs will be stored.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Channel: war-room channel does not exist!"

The method create a list of DiscordMembers to have as a reference for member ids so that it can check against the database for user profiles. If the user profile was found, it will receive the masteries of each user in the voice channel.

![[Pasted image 20240601201758.png | center]]

``` run-csharp
[ButtonCommand("btn_RetrieveMasteries")]
public async Task RetrieveMasteries(ButtonContext ctx)
{
	await ctx.Interaction.DeferAsync(true);

	var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council" || x.Name == "Raid Leader");

	if (hasRole)
	{
		var guildChannels = await ctx.Guild.GetChannelsAsync();
		var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");
		var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "war-room");

		if (raidChannel == null)
		{
			var embedFailed = new DiscordEmbedBuilder()
			{
				Title = "Raid Composition Failed",
				Color = DiscordColor.Red,
				Description = "Channel: Village Raid voice channel does not exist!"
			};

			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
			return;
		}

		if (recordsChannel == null)
		{
			var embedFailed = new DiscordEmbedBuilder()
			{
				Title = "Raid Composition Failed",
				Color = DiscordColor.Red,
				Description = "Channel: war-room channel does not exist!"
			};

			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
			return;
		}

		if (raidChannel.Users.Count() == 0)
		{
			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"There are no users in the Village Raid voice channel!"));
			return;
		}

		var Members = raidChannel.Users.ToList();
		List<ulong> MemberIDs = new List<ulong>();

		foreach (var member in Members)
		{
			MemberIDs.Add(member.Id);
		}

		var isRetrieved = await _db.GetRaidMasteries(MemberIDs);

		if (isRetrieved.Item1)
		{
			var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

			var embedMasteries = new DiscordMessageBuilder()
				.AddEmbed(new DiscordEmbedBuilder()
					.WithColor(DiscordColor.SpringGreen)
					.WithTitle("Leaf Village — Raid Composition")
					.WithDescription(string.Join("\n", isRetrieved.Item2))
					.WithFooter("• " + dateTime + "     • Raid Leader: " + ctx.Interaction.User.Username)
				);
			await ctx.Client.SendMessageAsync(recordsChannel, embedMasteries);
			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
				.WithContent($"Successfully retrieved masteries for those inside voice channel!"));
		}
		else
		{
			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
				.WithContent($"Unable to retrieve masteries!"));
		}
	}
	else
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
			.WithContent($"Unable to use button command Retrieve Masteries for {ctx.Interaction.User.Username}, please check required roles."));
	}
}
```


## RaidVoiceChannel(..)

The ## RaidVoiceChannel() method is used to move all members whos user profile in the database matches the member id given. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

![[Raid Dashboard.png| center]]

- Determines whether the user that pressed the Raid Voice Channel button has the Hokage, Council, or Raid Leader roles.
	- <span style="color:rgb(255, 97, 97)">False</span>: Return a deferred message. "Unable to use button command Voice Channel for {ctx.Interaction.User.Username}, please check required roles."

- Determines whether the Discord server contains a voice channel called "Raid Lobby".
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Channel: Raid Lobby does not exist!"

- Determines whether the Discord server contains a voice channel called "Village Raid".
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Channel: Village Raid voice channel does not exist!"

- Determine whether there are users in the Village Raid channel.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred message. "There are no users in the Village Raid voice channel!"

The method create a list of DiscordMembers to have as a reference for member ids so that it can send the users in the Raid Lobby voice channel to the Village Raid voice channel. A deferred message will be returned. "Successfully moved all users to the Village Raid!"

![[Raid Lobby.png| center]]

![[Village Raid Channel.png| center]]

![[Voice Channel Confirmation.png| center]]

``` run-csharp
[ButtonCommand("btn_VoiceChannel")]
public async Task RaidVoiceChannel(ButtonContext ctx)
{
	await ctx.Interaction.DeferAsync(true);

	var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council" || x.Name == "Raid Leader");

	if (hasRole)
	{
		var guildChannels = await ctx.Guild.GetChannelsAsync();
		var raidLobby = guildChannels.FirstOrDefault(x => x.Name == "Raid Lobby");
		var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");

		if (raidLobby == null)
		{
			var embedFailed = new DiscordEmbedBuilder()
			{
				Title = "Village Raid Failed",
				Color = DiscordColor.Red,
				Description = "Channel: Raid Lobby does not exist!"
			};

			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
			return;
		}

		if (raidChannel == null)
		{
			var embedFailed = new DiscordEmbedBuilder()
			{
				Title = "Villager Raid Failed",
				Color = DiscordColor.Red,
				Description = "Channel: Village Raid does not exist!"
			};

			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
			return;
		}

		var Members = raidLobby.Users.ToList();

		if (Members.Count == 0)
		{
			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
				.WithContent($"There are no users in the Village Lobby voice channel!"));
			return;
		}

		foreach (var member in Members)
		{
			if (member.VoiceState != null)
			{
				await member.ModifyAsync(delegate (MemberEditModel Edit)
				{
					Edit.VoiceChannel = raidChannel;
				});
			}
		}
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully moved all users to the Village Raid!"));

	}
	else
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
			.WithContent($"Unable to use button command Voice Channel for {ctx.Interaction.User.Username}, please check required roles."));
	}
}
```

---
References: