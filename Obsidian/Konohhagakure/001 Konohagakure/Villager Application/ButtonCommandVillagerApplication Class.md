Status: 
Tag:
Links: 

---
> [!note] 
>  # ButtonCommandVillagerApplication Class

The ButtonCommandVillagerApplication Class extends BaseCommandModule Class, which is required for commands to be executed. The db variable is used to call the PostgreSQLProfileData Class that implements the IDatabaseProfileData through Dependency Injection.

``` run-csharp
[ModuleLifespan(ModuleLifespan.Transient)]
public class ButtonCommandVillagerApplication : ButtonCommandModule
{
	public IDatabaseProfileData _db { get; set; }
```

## CreateVillagerApplication(..)

The CreateVillagerApplication() method creates a Discord Modal with three TextInputComponents. These three components are used to retain the response from the applicants. 

![[Villager Application Modal.png | center]]

``` run-csharp
	[ButtonCommand("btn_CreateApplication")]
	public async Task CreateVillagerApplication(ButtonContext ctx)
	{
		var modalVillagerApplication = ModalBuilder.Create("VillagerApplication")
			.WithTitle("Konohagakure — Villager Application")
			.AddComponents(new TextInputComponent("IGN:", "ingamenameTextBox", "Name of Character", null, true, TextInputStyle.Short))
			.AddComponents(new TextInputComponent("Introduction:", "introductionTextBox", "Introduce youself & your reason for joining", null, true, TextInputStyle.Paragraph))
			.AddComponents(new TextInputComponent("Alt(s):", "altsTextBox", "IGN, Alt1, Alt2, ... \nPlease be sure to separate your alt(s) with a comma!", null, true, TextInputStyle.Paragraph));
	
		await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalVillagerApplication);
	}
```

## AcceptVillager(..)

The AcceptVillager() method gives the applicant access to the Discord server by granting them the role of <span style="color:rgb(102, 240, 129)">Genin</span>. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

![[Accept-Deny Application.png | center]]

- Determines whether the user that pressed the button has the Hokage or Council role.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Unable to accept villager for {ctx.Interaction.User.Username}, please check required roles."

- Determines whether the Discord server contains a channel called "application-accepted" where the accepted applications will be stored.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Channel: application-accepted does not exist, please create the channel to store records."

If the checks were valid, it parses the applicant's member id from the footer of the embed message and grants the applicant genin role. The reference to the user application is sent to the application-accepted channel to be displayed an an embedded message for storage of application.


![[Application-Accepted.png | center]] ![[Accept-Application Response.png | center]]

``` run-csharp
	[ButtonCommand("btn_AcceptApplicant")]
	public async Task AcceptVillager(ButtonContext ctx)
	{
		await ctx.Interaction.DeferAsync(true);
	
		var hasRoles = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council");
	
		if (hasRoles)
		{
			var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
			var acceptedChannel = guildChannels.FirstOrDefault(x => x.Name == "application-accepted");
	
			if (acceptedChannel == null)
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
					.WithContent($"Channel: application-accepted does not exist, please create the channel to store records."));
				return;
			}
	
			var userApplication = ctx.Message.Embeds.First();
			var villagerRole = ctx.Guild.Roles.FirstOrDefault(x => x.Value.Name == "Villager");
	
			var memberID = ulong.Parse(userApplication.Footer.Text);
			DiscordMember member = await ctx.Guild.GetMemberAsync(memberID);
			await member.GrantRoleAsync(villagerRole.Value);
	
			await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(acceptedChannel.Id), userApplication);
	
			await ctx.Message.DeleteAsync();
	
			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
								.WithContent($"Successfully granted Genin role to {member.Username}!"));
		}
		else
		{
			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
				.WithContent($"Unable to accept villager for {ctx.Interaction.User.Username}, please check required roles."));
		}
	}
```


## DeclineVillager(..)

The DeclineVillager() method denies the applicant access to the server and sends them a response for rejecting their application. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

![[Accept-Deny Application.png | center]]

- Determines whether the user that pressed the button has the Hokage or Council role.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message will be outputted. "Unable to accept villager for {ctx.Interaction.User.Username}, please check required roles."

- Determines whether the Discord server contains a channel called "application-denied" where the denied applications will be stored.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Channel: application-denied does not exist, please create the channel to store records."

If the checks were valid, it parses the applicant's member id from the footer of the embed message to store a reference to the applicant.

An embedded message is shown to the user who pressed the button so that a reason for denying the application can be stored. The response stored will be sent to the applicant to notify them of the decision.

![[Application-Denied Reason.png | center]] ![[Application-Denied Confirmation.png | center]]![[Application-Denied Response.png | center]]

The reference to the user application is sent to the application-denied channel to be displayed an an embedded message.

![[Application-Denied.png | center]] 

``` run-csharp
	[ButtonCommand("btn_DeclineApplicant")]
	public async Task DeclineVillager(ButtonContext ctx)
	{
		await ctx.Interaction.DeferAsync(true);
	
		var hasLMPFRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council");
	
		if (hasLMPFRole)
		{
			var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
			var deniedChannel = guildChannels.FirstOrDefault(x => x.Name == "application-denied");
	
			if (deniedChannel == null)
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
					.WithContent($"Channel: application-denied does not exist, please create the channel to store records."));
				return;
			}
	
			var embedMessage = ctx.Message.Embeds.First();
	
			var memberID = ulong.Parse(embedMessage.Footer.Text);
			DiscordMember member = await ctx.Guild.GetMemberAsync(memberID);
	
			var embedReason = new DiscordEmbedBuilder()
			{
				Color = DiscordColor.SpringGreen,
				Title = "Please enter the reason for denying application as the next message."
			};
	
			var followupMessage = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedReason));
	
			var interactivity = ctx.Client.GetInteractivity();
			var reason = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Interaction.User.Id, TimeSpan.FromMinutes(5));
	
			var embedDenied = new DiscordMessageBuilder()
			.AddEmbed(new DiscordEmbedBuilder()
				.WithColor(DiscordColor.Red)
				.WithTitle($"Application Denied")
				.WithImageUrl(member.AvatarUrl)
				.WithThumbnail(Images.LeafSymbol_URL)
				.WithDescription($"{reason.Result.Content}")
			);
	
			await member.SendMessageAsync(embedDenied);
			await ctx.Message.DeleteAsync();
	
			await ctx.Interaction.EditFollowupMessageAsync(followupMessage.Id, new DiscordWebhookBuilder().WithContent($"Your response was sent to {member.Username}."));
	
			await ctx.Channel.DeleteMessageAsync(reason.Result);
	
			var embedFields = embedMessage.Fields;
			var embedFieldLists = new List<string>();
	
			foreach (var field in embedFields)
			{
				embedFieldLists.Add(field.Value);
			}
	
			var embedApplicationDenied = new DiscordMessageBuilder()
			.AddEmbed(new DiscordEmbedBuilder()
				.WithColor(DiscordColor.Red)
				.WithTitle($"Leaf Village Application")
				.WithImageUrl(member.AvatarUrl)
				.WithThumbnail(Images.LeafSymbol_URL)
				.AddField("IGN:", embedFieldLists[0])
				.AddField("Introduction:", embedFieldLists[1])
				.AddField("Alt(s):", embedFieldLists[2])
				.WithFooter($"{memberID}\nApplication Denied: \n{reason.Result.Content}\n")
				);
	
			await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(deniedChannel.Id), embedApplicationDenied);
		}
		else
		{
			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Unable to deny applicant for {ctx.Interaction.User.Username}, please check required roles."));
		}
	}
```

---
References: