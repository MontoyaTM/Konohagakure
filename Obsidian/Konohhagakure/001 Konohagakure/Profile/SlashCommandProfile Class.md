Status: 
Tag:
Links:

---
> [!note] 
>  # SlashCommandProfile Class

The SlashCommandProfile Class extends ApplicationCommandModule Class, which is required for slash commands to be executed. The db variable is used to call the PostgreSQLProfileData Class that implements the IDatabaseProfileData through Dependency Injection.

``` run-csharp
[ModuleLifespan(ModuleLifespan.Transient)]
public class SlashCommandProfile : ApplicationCommandModule
{
	public IDatabaseProfileData _db { get; set; }
```


## DisplayProfile(..)

The DisplayProfile() method displays an embedded message with the user profile from the database. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

Command: <span style="color:rgb(102, 240, 129)">/profile</span>
Roles: Default

![[Profile Command.png| center]]

- Determines whether the applicant's member id exists in the database
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Unable to submit application, please be aware of one application per user."

![[SubmitApplication Reponse.png| center]]

- Determines whether the profile model using the user's member id was retrieved. 
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred embed message. "Failed to retrieved profile!"

If the validation check were valid, an embedded message is returned with data returned from the database as a Profile Model.

![[Profile.png | center]]

``` run-csharp
[SlashCommand("profile", "Displays the user's profile as an embed.")]
[SlashCooldown(2, 30, SlashCooldownBucketType.User)]
public async Task DisplayProfile(InteractionContext ctx)
{
	await ctx.DeferAsync();

	var memberId = ctx.Member.Id;
	var userExist = await _db.UserExistsAsync(memberId);

	if (userExist == false)
	{
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Username}'s profile does not exist!"));
		return;
	}

	var isRetieved = await _db.RetrieveProfileAsync(memberId);

	if (isRetieved.Item1)
	{
		var profile = isRetieved.Item2.First();

		var embedApplication = new DiscordMessageBuilder()
				.AddEmbed(new DiscordEmbedBuilder()
					.WithColor(DiscordColor.SpringGreen)
					.WithTitle($"Leaf Village Application")
					.WithImageUrl(profile.ProfileImage)
					.WithThumbnail(Images.LeafSymbol_URL)
					.AddField("IGN:", $"```{profile.InGameName}```", true)
					.AddField("Organization:", $"```{profile.Organization}```", true)
					.AddField("Organization Rank:", $"```{profile.OrganizationRank}```", true)
					.AddField("Level:", $"```{profile.Level}```", true)
					.AddField("Masteries:", $"```{String.Join(",", profile.Masteries)}```", true)
					.AddField("Clan:", $"```{profile.Clan}```", true)
					.AddField("Proctored Missions:", $"```{profile.ProctoredMissions}```", true)
					.AddField("Raids:", $"```{profile.Raids}```", true)
					.AddField("Fame:", $"```{profile.Fame}```", true)
					);

		await ctx.EditResponseAsync(new DiscordWebhookBuilder(embedApplication));
	}
	else
	{
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Failed to retrieved profile!"));
	}
}
```

## UpdateProfileImage(..)

The UpdateProfileImage() method updates the user profile image from the database. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

Command: <span style="color:rgb(102, 240, 129)">/update_profileimage</span>
Roles: Default


![[UpdateProfileImage.png| center]]

- Determines whether the user's profile image has been updated in the database with the user's member id and image URL.
	- <span style="color:rgb(255, 97, 97)">False</span>: A deferred message is returned. "Failed to updated profile image!"

If the validation check were valid, an embedded message is returned. "Successfully updated profile image!"

![[Pasted image 20240601151652.png | center]]


The updated user profile can be displayed using the /profile command.

![[Updated User Profile.png| center]]

``` run-csharp
[SlashCommand("update_profileimage", "Updates the user's profile image.")]
[SlashCooldown(2, 30, SlashCooldownBucketType.User)]
public async Task UpdateProfileImage(InteractionContext ctx, [Option("Image", "The image you want to upload.")] DiscordAttachment image)
{
	await ctx.DeferAsync(true);

	var MemberID = ctx.Interaction.User.Id;
	var isUpdated = await _db.UpdateProfileImageAsync(MemberID, image.Url);

	if (isUpdated)
	{
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully updated profile image!"));
	}
	else
	{
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to updated profile image!"));
	}
}
```

## GiveFame(..)

The GiveFame() method increments a user's fame stat in their user profile and has a one day cooldown. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

Command: <span style="color:rgb(102, 240, 129)">/give_fame</span>
Roles: Default
![[GiveFame.png| center]]

- Tries to retrieve the user's profile image that is giving out the fame so that it can be used in the embedded message.

- Determines whether the user's fame stat has been incremented (the user receiving the fame). 
	- <span style="color:rgb(255, 97, 97)">False</span>: A deferred message is returned. "Unable to give {User.Username} fame!"

If the validation checks were valid, an embedded message is displayed.

![[Give Fame Response.png| center]]

The updated user profile can be displayed using the /profile command.

![[Updated Profile Fame.png| center]]

``` run-csharp
[SlashCommand("give_fame", "Gives fame to a user: 1 day cooldown.")]
[SlashCooldown(1, 86400, SlashCooldownBucketType.User)]
public async Task GiveFame(InteractionContext ctx, [Option("User", "Discord User to recieve your fame.")] DiscordUser User)
{
	await ctx.DeferAsync();

	var MemberID = ctx.Interaction.User.Id;
	var UserID = User.Id;

	var profileImage = await _db.GetProfileImageAsync(MemberID);
	var isFameUpdated = await _db.UpdateFameAsync(UserID);

	if (isFameUpdated)
	{
		var embedFame = new DiscordMessageBuilder()
			.AddEmbed(new DiscordEmbedBuilder()
				.WithColor(DiscordColor.SpringGreen)
				.WithDescription($"{ctx.Interaction.User.Username} gave fame to {User.Username}!")
				.WithThumbnail(profileImage.Item2.First())
			);

		await ctx.EditResponseAsync(new DiscordWebhookBuilder(embedFame));
	}
	else
	{
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Unable to give {User.Username} fame!"));
	}
}
```

## UpdateOrg(..)

The UpdateOrg() method updates a user's organization and rank. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

Command: <span style="color:rgb(102, 240, 129)">/update_user_prganization</span>
Roles: Hokage or Org Leader

![[Update Org Command.png| center]]

- Determines if the user issuing the command has the required roles: Hokage or Org Leader
	- <span style="color:rgb(255, 97, 97)">False</span>: A deferred message is returned. "You do not have the role to access this command!"

- Determines whether the user's organization and rank has been successfully changed in the database.
	- <span style="color:rgb(255, 97, 97)">False</span>: A deferred message is returned. "Failed to assign {Organization} {Rank} to {User.Username}!"

If the validations check are valid a deferred message will be returned.

![[Update Org Response.png | center]]

The updated user profile can be displayed using the /profile command.

![[Updated Profile.png | center]]

``` run-csharp
[SlashCommand("update_user_organization", "Updates a user's Organization & Rank title on their profile.")]
public async Task UpdateOrg(InteractionContext ctx, [Option("User", "The user you wish to assign Organization and Rank.")] DiscordUser User,

													[Choice("Leaf 12 Guardians", "12 Guardians")]
													[Choice("Leaf Military Police Force", "Leaf Military Police Force")]
													[Choice("Leaf Medical Corp", "Lead Medical Corp")]
													[Choice("Leaf ANBU", "Leaf ANBU")]
													[Option("Organization", "The organization to assign the user.")] string Organization,

													[Option("Rank", "The rank to assign the user.")] string Rank)
{
	await ctx.DeferAsync(true);

	var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Org Leader");

	if (hasRole)
	{
		var Member = (DiscordMember)User;
		var isAssigned = await _db.UpdateOrganizationAsync(Member.Id, Organization, Rank);

		if (isAssigned)
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully assigned {Organization} {Rank} to {User.Username}!"));
		}
		else
		{
			await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to assign {Organization} {Rank} to {User.Username}!"));
		}
	}
	else
	{
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You do not have the role to access this command!"));
	}
}
```

## InsertAlt(..)

The InsertAlt() method is used to add character (alts) in their user profile. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

Command: <span style="color:rgb(102, 240, 129)">/add_alt</span>

![[Add Alt Command.png| center]]


- Determines whether the alt type was successfully added to a list of alts in the database.
	- <span style="color:rgb(255, 97, 97)">False</span>: A deferred message is returned. "Failed to add {Alt} to your alt(s) list!"

If the validation check was valid, a deferred message will be retuned and an embedded message of the user's character list will be sent to the user as a directed message.

![[Add Alt Response.png| center]]

![[Pasted image 20240601161047.png | center]]

``` run-csharp
[SlashCommand("add_alt", "Inserts a character into your alt(s) list. ")]
[SlashCooldown(2, 30, SlashCooldownBucketType.User)]
public async Task InsertAlt(InteractionContext ctx, [Option("IGN", "The ingame name of the character you want to add.")] string Alt)
{
	await ctx.DeferAsync(true);

	var MemberID = ctx.Interaction.User.Id;
	var isUpdated = await _db.AddAltAsync(MemberID, Alt);

	if (isUpdated)
	{
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully added {Alt} to your alt(s) list!"));

		var isRetrieved = await _db.RetrieveAlts(MemberID);

		DiscordMember member = await ctx.Guild.GetMemberAsync(MemberID);

		var embedAlts = new DiscordMessageBuilder()
		.AddEmbed(new DiscordEmbedBuilder()
			.WithColor(DiscordColor.Red)
			.WithTitle($"{ctx.Interaction.User.Username} Alt(s) List:")
			.WithDescription(String.Join(",", isRetrieved.Item2))
			.WithThumbnail(Images.LeafSymbol_URL)
			.WithImageUrl(ctx.Interaction.User.AvatarUrl)
		);

		await member.SendMessageAsync(embedAlts);
	}
	else
	{
		await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to add {Alt} to your alt(s) list!"));
	}
}
```

---
References: