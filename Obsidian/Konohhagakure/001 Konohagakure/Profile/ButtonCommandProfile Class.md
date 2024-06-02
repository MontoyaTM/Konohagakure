Status: 
Tag:
Links:

---
> [!note] 
>  # ButtonCommandProfile Class

The ButtonCommandProfile Class extends BaseCommandModule Class, which is required for commands to be executed. The db variable is used to call the PostgreSQLProfileData Class that implements the IDatabaseProfileData through Dependency Injection.

``` run-csharp
[ModuleLifespan(ModuleLifespan.Transient)]
public class ButtonCommandProfile : ButtonCommandModule
{
	public IDatabaseProfileData _db { get; set; }

```


## UpdateLevel(..)

The UpdateLevel() provides a validation check to determine if the user's member id is found in the database. If it is found, the method creates a Discord Modal with a TextInputComponents to update a user's profile level. 

- <span style="color:rgb(255, 97, 97)">False</span>: An embed message is returned. "You do not have a profile in the database!"

![[Update Character Profile.png| center]]

![[Update Profile Level.png| center]]

``` run-csharp
[ButtonCommand("btn_UpdateLevel")]
public async Task UpdateLevel(ButtonContext ctx)
{
	var userExists = await _db.UserExistsAsync(ctx.Interaction.User.Id);

	if (userExists)
	{
		var modalUpdateLevel = ModalBuilder.Create("ProfileLevel")
		.WithTitle("Update Profile — Character Level")
		.AddComponents(new TextInputComponent("Level:", "ProfileLevel", "Level (1-60)", null, true, TextInputStyle.Short));

		await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalUpdateLevel);
	}
	else
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("You do not have a profile in the database!"));
	}
}
```

## UpdateIGN(..)

The UpdateIGN() provides a validation check to determine if the user's member id is found in the database. If it is found, the method creates a Discord Modal with a TextInputComponents to update a user's profile in-game name. 

- <span style="color:rgb(255, 97, 97)">False</span>: An embed message is returned. "You do not have a profile in the database!"

![[Update Character Profile.png| center]]

![[Update Profile IGN.png| center]]

``` run-csharp
[ButtonCommand("btn_UpdateIGN")]
public async Task UpdateIGN(ButtonContext ctx)
{
	var userExists = await _db.UserExistsAsync(ctx.Interaction.User.Id);

	if (userExists)
	{
		var modalUpdateLevel = ModalBuilder.Create("ProfileInGameName")
		.WithTitle("Update Profile — Character IGN")
		.AddComponents(new TextInputComponent("IGN:", "ProfileIGN", "Ingame Name", null, true, TextInputStyle.Short));

		await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalUpdateLevel);
	}
	else
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("You do not have a profile in the database!"));
	}
```

---
References: