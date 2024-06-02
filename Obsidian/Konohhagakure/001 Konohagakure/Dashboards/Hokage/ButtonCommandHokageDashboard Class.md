Status: 
Tag:
Links: 

---
> [!note] 
>  # ButtonCommandHokageDashboard Class

The ButtonCommandHokageDashboard Class extends ButtonCommandModule Class, which is required for button commands to be executed. The db variable is used to call the PostgreSQLProfileData Class that implements the IDatabaseProfileData through Dependency Injection.

``` run-csharp
[ModuleLifespan(ModuleLifespan.Transient)]
public class ButtonCommandHokageDashboard : ButtonCommandModule
{
	public IDatabaseProfileData _db { get; set; }
```


## DeleteApplication(..)

The DeleteApplication() method is used to delete a user profile in the database that matches the member id given. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

![[Hokage Dashboard.png| center]]

- Determines whether the user that pressed the Delete Application button has the Hokage or Council roles.
	- <span style="color:rgb(255, 97, 97)">False</span>: Return a deferred message. "Unable to delete application for {ctx.Interaction.User.Username}, please check required roles."

- Determines whether the Discord server contains a channel called "hokage-records" where the logs will be stored.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Channel: Record Channel does not exist!"

- Determines whether the member id retrieved is valid using the RetrieveMemberID() method.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred message. "Failed to delete villager application!"

- Determines whether the user profile matching the member id was successfully deleted.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred message. "Failed to delete villager application!"

If all validation check were valid, a deferred embedded message will be sent to the hokage-records channel so that it can be logged and a separate message will be displayed letting the user know; "Successfully deleted {isMemberIdRetrieved.Item4.Username}'s villager application!"

``` run-csharp
[ButtonCommand("btn_DeleteApplication")]
public async Task DeleteApplication(ButtonContext ctx)
{
	await ctx.Interaction.DeferAsync(true);

	var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council");

	if (hasRole)
	{
		var guildChannels = await ctx.Guild.GetChannelsAsync();
		var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "hokage-records");

		if (recordsChannel == null)
		{
			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("Channel: Record Channel does not exist!"));
			return;
		}

		var interactivity = ctx.Client.GetInteractivity();
		var isMemberIdRetrieved = await RetrieveMemberID(ctx, interactivity);

		if (isMemberIdRetrieved.Item1)
		{
			ulong memberID = isMemberIdRetrieved.Item2;

			var isDeleted = await _db.DeleteApplication(memberID);

			if (isDeleted)
			{
				var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

				var embedDeleted = new DiscordMessageBuilder()
				.AddEmbed(new DiscordEmbedBuilder()
					.WithColor(DiscordColor.SpringGreen)
					.WithTitle("Leaf Village — Delete Application")
					.WithDescription($"Successfully deleted **{isMemberIdRetrieved.Item4.Username}**'s villager application from the database.")
					.WithFooter("• " + dateTime + "     • Executed by: " + ctx.Interaction.User.Username)
				);

				await ctx.Interaction.EditFollowupMessageAsync(isMemberIdRetrieved.Item3.Id, new DiscordWebhookBuilder()
					.WithContent($"Successfully deleted {isMemberIdRetrieved.Item4.Username}'s villager application!"));

				await ctx.Client.SendMessageAsync(recordsChannel, embedDeleted);

			}
			else
			{
				await ctx.Interaction.EditFollowupMessageAsync(isMemberIdRetrieved.Item3.Id, new DiscordWebhookBuilder()
					.WithContent($"Failed to delete villager application!"));
			}
		}
		else
		{
			return;
		}
	}
	else
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
			.WithContent($"Unable to delete application for {ctx.Interaction.User.Username}, please check required roles."));
	}
}
```


## GetAltList(..)

The GetAltList() method is used to retrieve a user profile in the database that matches the member id given. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:


![[Hokage Dashboard.png| center]]

- Determines whether the user that pressed the Retrieve Alt(s) button has the Hokage or Council roles.
	- <span style="color:rgb(255, 97, 97)">False</span>: Return a deferred message. "Unable to delete application for {ctx.Interaction.User.Username}, please check required roles."

- Determines whether the Discord server contains a channel called "hokage-records" where the logs will be stored.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Channel: Record Channel does not exist!"

- Determines whether the member id retrieved is valid using the RetrieveMemberID() method.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred message. "Failed to delete villager application!"

- Determines whether the user profile matching the member id was able to successfully retrieve the character list.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred message. "Unable to retrieve alts!"

If all validation check were valid, a deferred embedded message will be sent to the hokage-records channel so that it can be logged and a separate message will be displayed letting the user know; "Successfully retrieved {isMemberIdRetrieved.Item4.Username}'s alt(s) list!"

``` run-csharp
[ButtonCommand("btn_GetAltList")]
public async Task GetAltList(ButtonContext ctx)
{
	await ctx.Interaction.DeferAsync(true);

	var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council");

	if (hasRole)
	{
		var guildChannels = await ctx.Guild.GetChannelsAsync();
		var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "hokage-records");

		if (recordsChannel == null)
		{
			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("Channel: Record Channel does not exist!"));
			return;
		}

		var interactivity = ctx.Client.GetInteractivity();
		var isMemberIdRetrieved = await RetrieveMemberID(ctx, interactivity);

		if (isMemberIdRetrieved.Item1)
		{
			ulong memberID = isMemberIdRetrieved.Item2;

			var isAltsRetrieved = await _db.RetrieveAlts(memberID);

			if (isAltsRetrieved.Item1)
			{
				var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

				var embedAlts = new DiscordMessageBuilder()
				.AddEmbed(new DiscordEmbedBuilder()
					.WithColor(DiscordColor.SpringGreen)
					.WithTitle($"{isMemberIdRetrieved.Item4.Username} Alt(s) List:")
					.WithDescription(String.Join(",", isAltsRetrieved.Item2))
					.WithThumbnail(Images.LeafSymbol_URL)
					.WithImageUrl(isMemberIdRetrieved.Item4.AvatarUrl)
					.WithFooter("• " + dateTime + "     • Executed by: " + ctx.Interaction.User.Username)
				);

				await ctx.Interaction.EditFollowupMessageAsync(isMemberIdRetrieved.Item3.Id, new DiscordWebhookBuilder()
					.WithContent($"Successfully retrieved {isMemberIdRetrieved.Item4.Username}'s alt(s) list!"));

				await ctx.Client.SendMessageAsync(recordsChannel, embedAlts);
			}
			else
			{
				await ctx.Interaction.EditFollowupMessageAsync(isMemberIdRetrieved.Item3.Id, new DiscordWebhookBuilder()
					.WithContent($"Unable to retrieve alts!"));
				return;
			}
		}
		else
		{
			return;
		}
	}
	else
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
			.WithContent($"Unable to retrieve alts for {ctx.Interaction.User.Username}, please check required roles."));
	}
}
```


## RetrieveMemberID()

The RetrieveMemberID() method is used to retrieve a DiscordMember based on the user id given. The method begins by displaying an embed message notifying the user to enter the applicants member id. The method provides some validation handling:

- Determines whether the input received is a valid numerical value.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred message. "Unable to convert your response to a numerical value, please check your submission!"

- Determines whether the member id given corresponded with a user profile in the database.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred message. "MemberID does not match any user inside this server, please check your response!"

If the validation checks were valid, the method returns true, memberid, embedMemberID, member. 

``` run-csharp
public async Task<(bool, ulong, DiscordMessage, DiscordMember?)> RetrieveMemberID(ButtonContext ctx, InteractivityExtension interactivity)
{
	var embedMessage = new DiscordEmbedBuilder()
	{
		Color = DiscordColor.SpringGreen,
		Title = "Enter the MemberID of the applicant as the next message in this channel."
	};

	var embedMemberID = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedMessage));
	var memberIdResponse = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Interaction.User.Id, TimeSpan.FromMinutes(2));

	DiscordMember? member;
	ulong memberID;

	if (ulong.TryParse(memberIdResponse.Result.Content, out memberID))
	{
		await ctx.Channel.DeleteMessageAsync(memberIdResponse.Result);
	}
	else
	{
		await ctx.Interaction.EditFollowupMessageAsync(embedMemberID.Id, new DiscordWebhookBuilder()
			.WithContent("Unable to convert your response to a numerical value, please check your submission!"));
		await ctx.Channel.DeleteMessageAsync(memberIdResponse.Result);
		return (false, 0, embedMemberID, null);
	}

	try
	{
		member = await ctx.Guild.GetMemberAsync(memberID);
	}
	catch (Exception ex)
	{
		await ctx.Interaction.EditFollowupMessageAsync(embedMemberID.Id, new DiscordWebhookBuilder()
					.WithContent("MemberID does not match any user inside this server, please check your response!"));
		return (false, 0, embedMemberID, null);
	}

	return (true, memberID, embedMemberID, member);
}
```

---
References: