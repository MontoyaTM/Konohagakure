Status: 
Tag:
Links:

---
> [!note] 
>  # ModalCommandVillagerApplication Class

The ModalCommandVillagerApplication Class extends ModalCommandModule Class, which is required for modals to be executed. The db variable is used to call the PostgreSQLProfileData Class that implements the IDatabaseProfileData through Dependency Injection.

``` run-csharp
[ModuleLifespan(ModuleLifespan.Transient)]
public class ModalCommandVillagerApplication : ModalCommandModule
{
	public IDatabaseProfileData _db { get; set; }

```

## SubmitApplication(..)

The SubmitApplication() method reads the values from the modal builder in the CreateVillagerApplication() method to determine whether to application can be stored in the database. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

- Determines whether the applicant's member id exists in the database
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Unable to submit application, please be aware of one application per user."

![[SubmitApplication Reponse.png| center]]

- Determines whether the profile model created using the modal builder response was successfully stored in the database.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Failed to create application for {ctx.Interaction.User.Username}."

![[Pasted image 20240601132456.png | center | 400]]

- Determines whether the Discord server contains a channel called "villager-applications" where the applications will be stored.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Channel: villager-applications does not exist!"

If the validations were valid, a deferred embed message will be sent to the applicant. "Successfully created a villager application for {ctx.Interaction.User.Username}."

![[Modal Response.png | center]]


An embed message of the user's villager application will be sent to the villager-application channel for approval.

![[Accept-Deny Application.png | center]]

``` run-csharp
[ModalCommand("VillagerApplication")]
public async Task SubmitApplication(ModalContext ctx)
{
	await ctx.Interaction.DeferAsync(true);

	var modalValues = ctx.Values;

	var data = ctx.Values[2];
	string[] alts = data.Split(",").Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();

	var profileModel = new ProfileModel
	{
		MemberId = ctx.Interaction.User.Id,
		Username = ctx.Interaction.User.Username,
		ServerId = ctx.Interaction.Guild.Id,
		ServerName = ctx.Interaction.Guild.Name,
		AvatarURL = ctx.Interaction.User.AvatarUrl,
		ProfileImage = ctx.Interaction.User.AvatarUrl,
		InGameName = modalValues.ElementAt(0),
		Alts = alts,
		
	};

	var applicantExists = await _db.UserExistsAsync(profileModel.MemberId);

	if (applicantExists)
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
															.WithContent($"Unable to submit application, please be aware of one application per user."));
		return;
	}

	var isApplicationStored = await _db.StoreVillagerApplicationAsync(profileModel);

	if (isApplicationStored)
	{
		var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
		var storeApplicationChannel = guildChannels.FirstOrDefault(x => x.Name == "villager-applications");

		if (storeApplicationChannel == null)
		{
			var embedFailed = new DiscordEmbedBuilder()
			{
				Title = "Villager Application Failed",
				Color = DiscordColor.Red,
				Description = "Channel: villager-applications does not exist!"
			};

			await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
			return;
		}

		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
															.WithContent($"Succesfully created a villager application for {ctx.Interaction.User.Username}"));

		ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

		var embedApplication = new DiscordMessageBuilder()
			.AddEmbed(new DiscordEmbedBuilder()
				.WithColor(DiscordColor.SpringGreen)
				.WithTitle($"Leaf Village Application")
				.WithImageUrl(ctx.Interaction.User.AvatarUrl)
				.WithThumbnail(Images.LeafSymbol_URL)
				.WithFooter(ctx.Interaction.User.Id.ToString())
				.AddField("IGN:", profileModel.InGameName)
				.AddField("Introduction:", modalValues.ElementAt(1))
				.AddField("Alt(s):", modalValues.ElementAt(2))
				)
			.AddComponents(
			new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_AcceptApplicant"), "Accept Applicant"),
			new DiscordButtonComponent(ButtonStyle.Secondary, buttonCommand.BuildButtonId("btn_DeclineApplicant"), "Deny Applicant")
			);

		await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(storeApplicationChannel.Id), embedApplication);
	}
	else
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
															.WithContent($"Failed to created application for {ctx.Interaction.User.Username}."));
	}
}
```

---
References:






















