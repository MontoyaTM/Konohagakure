Status: 
Tag:
Links: 

---
> [!note] 
>  # ButtonCommandProctor Class

The ButtonCommandProctor Class extends ButtonCommandModule Class, which is required for commands to be executed. The dbProfile variable is used to call the PostgreSQLProfileData Class that implements the IDatabaseProfileData through Dependency Injection. Likewise the dbRequest variable is used to call the PostgreSQLRequestData Class that implements the IDatabaseRequestData.

``` run-csharp
[ModuleLifespan(ModuleLifespan.Transient)]
public class ButtonCommandProctor : ButtonCommandModule
{
	public IDatabaseRequestData _dbRequest { get; set; }
	public IDatabaseProfileData _dbProfile { get; set; }
```


## CreateRPRequest(..)

The CreateRPRequest()method creates a Discord Modal with four TextInputComponents. These four components are used to retain the response from the user. 

![[RP Request Modal.png| center]]

``` run-csharp
[ButtonCommand("btn_CreateRequest")]
public async Task CreateRPRequest(ButtonContext ctx)
{
	var modalRPRequest = ModalBuilder.Create("RPRequestModal")
		.WithTitle("Leaf Village RP Request")
		.AddComponents(new TextInputComponent("IGN:", "ingamenameTextBoxRP", "Name of Character", null, true, TextInputStyle.Short))
		.AddComponents(new TextInputComponent("RP Mission", "missionTextBoxRP", "RP Mission (I-VII)", null, true, TextInputStyle.Short))
		.AddComponents(new TextInputComponent("Timezone:", "timezoneTextBoxRP", "Timezone", null, true, TextInputStyle.Short))
		.AddComponents(new TextInputComponent("Attendees:", "attendeesTextBoxRP", "IGN, Ninja 2, Ninja 3", null, true, TextInputStyle.Short));

	await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalRPRequest);

}
```


## CloseRPRequest(..)

The CloseRPRequest() is used to close an RP request application record from the database. The method will also increment the proctor's missions stat upon completion. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

![[Close RP Request.png| center]]

- Determines whether the user that pressed the Delete Application button has the Chunin, Jonin, or Specialized Jonin roles.
	- <span style="color:rgb(255, 97, 97)">False</span>: Return a deferred message. "Unable to close request for {ctx.Interaction.User.Username}, please check required roles."

- Determines whether the Discord server contains a channel called "rp-records" where the logs will be stored.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Channel: rp-records does not exist, please create the channel to store records."


The method will override the permissions required to type in the current channel. Initially, any user above genin rank will be able to type if the created channel. However, once overridden, it will only be accessible to Chunin, Jonin, or Specialized Jonin rank.

The method displays multiple embedded message to retrieve additional data in regards to logging the response.

- Displays an embedded message. "Please provide a (screenshot and a detailed description) of your RP Mission as the next message in this channel."

![[RP Response 1.png | center]]

- Displays an embedded message. "Please write the names of the 3 Ninjas who participated in the RP Mission as the next message in this channel. Ex. Ninja1, Ninja2, Ninja3"

![[RP Response 2.png | center]]

- Displays an embedded message. "Please write the date the RP Mission was completed as the next message in the channel. " + "\n\nmm/day/yyyy"

![[RP Response 3.png | center]]

The method uses the information gathered from the response to provide an embedded message displayed in the rp-records channel for storage and deletes the current channel.

![[RP Request Log.png| center]]

``` run-csharp
[ButtonCommand("btn_CloseRequest")]
public async Task CloseRPRequest(ButtonContext ctx)
{
	await ctx.Interaction.DeferAsync();

	var hasRankedRole = ctx.Member.Roles.Any(x => x.Name == "Chunin" || x.Name == "Jonin" || x.Name == "Specialized Jonin");

	if (hasRankedRole)
	{
		var interactivity = ctx.Client.GetInteractivity();

		var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
		var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "rp-records");

		if (recordsChannel == null)
		{
			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
				.WithContent($"Channel: rp-records does not exist, please create the channel to store records."));
			return;
		}

		var embedMessage = ctx.Message.Embeds.First();
		var villagerRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name == "Villager");

		var OverWriteBuilderList = new DiscordOverwriteBuilder[] { new DiscordOverwriteBuilder(villagerRole.Value).Deny(Permissions.SendMessages) };
		await ctx.Channel.ModifyAsync(x => x.PermissionOverwrites = OverWriteBuilderList);

		var embedMessage1 = new DiscordEmbedBuilder()
		{
			Color = DiscordColor.SpringGreen,
			Title = "Please provide a (screenshot and a detailed description) of your RP Mission as the next message in this channel."
		};

		var followupMessage_1 = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedMessage1));

		var details = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Interaction.User.Id, TimeSpan.FromMinutes(5));

		if (details.Result.Attachments.Count == 0)
		{
			await ctx.Interaction.EditFollowupMessageAsync(followupMessage_1.Id, new DiscordWebhookBuilder().WithContent("There was no screenshot, try again!"));
			return;
		}

		DiscordAttachment image = details.Result.Attachments[0];

		HttpClient client = new HttpClient();
		Stream stream = await client.GetStreamAsync(image.Url);

		using (var fileStream = new FileStream("TempImages/output.png", FileMode.Create))
		{
			stream.CopyTo(fileStream);
		}

		FileStream file = new FileStream("TempImages/output.png", FileMode.Open, FileAccess.Read);

		var embedImage = new DiscordMessageBuilder()
			.AddFile(file);

		var embedMessage2 = new DiscordEmbedBuilder()
		{
			Color = DiscordColor.SpringGreen,
			Title = "Please write the names of the 3 Ninjas who participated in the RP Mission as the next message in this channel. " +
					"\n\n Ex. Ninja1, Ninja2, Ninja3"
		};
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedMessage2));
		var attendees = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Interaction.User.Id, TimeSpan.FromMinutes(5));

		var embedMessage3 = new DiscordEmbedBuilder()
		{
			Color = DiscordColor.SpringGreen,
			Title = "Please write the date the RP Mission was completed as the next message in the channel. " +
					"\n\nmm/day/yyyy"
		};
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedMessage3));
		var datetime = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Interaction.User.Id, TimeSpan.FromMinutes(5));

		var embedFields = embedMessage.Fields;
		var profileImage = await _dbProfile.GetProfileImageAsync(ctx.Interaction.User.Id);
		var embedFieldLists = new List<string>();

		foreach (var field in embedFields)
		{
			embedFieldLists.Add(field.Value);
		}

		var embedRPRecord = new DiscordMessageBuilder()
			.AddEmbed(new DiscordEmbedBuilder()
				.WithColor(DiscordColor.SpringGreen)
				.WithTitle($"RP Mission Request Record")
				.WithImageUrl(profileImage.Item2[0])
				.WithThumbnail(Images.LeafSymbol_URL)
				.AddField("Proctor:", ctx.Interaction.User.Username, true)
				.AddField("RP Mission:", embedFieldLists[1], true)
				.AddField("Date/Time:", datetime.Result.Content)
				.AddField("Attendees", attendees.Result.Content)
				.AddField("Details:", details.Result.Content)
				.WithFooter($"{ctx.Interaction.User.Username} had successfully proctored an RP mission and has incremented their proctored missions stat! Congratulations!")
			);

		await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(recordsChannel.Id), embedRPRecord);
		await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(recordsChannel.Id), embedImage);

		await _dbProfile.UpdateProctoredMissionsAsync(ctx.Interaction.User.Id);

		var requestId = ulong.Parse(embedMessage.Footer.Text);
		await _dbRequest.DeleteRequestAsync(requestId);

		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Your response was sent to the rp-records channel."));

		await ctx.Channel.DeleteAsync();
	}
	else
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to close request for {ctx.Interaction.User.Username}, please check required roles."));
	}
}
```

---
References: