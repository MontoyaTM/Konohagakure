Status: 
Tag:
Links:

---
> [!note] 
>  # ModalCommandProfile Class

The ModalCommandProfile Class extends ModalCommandModule Class, which is required for modals to be executed. The db variable is used to call the PostgreSQLProfileData Class that implements the IDatabaseProfileData through Dependency Injection.

``` run-csharp
[ModuleLifespan(ModuleLifespan.Transient)]
public class ModalCommandProfile : ModalCommandModule
{
	public IDatabaseProfileData _db { get; set; }

```


## UpdateProfileLevel(..)

The UpdateProfileLevel() method updates a user's profile level in the database. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

- Determines if the level is a valid integer between 1-60.
	- <span style="color:rgb(255, 97, 97)">False</span>: A deferred embed message is returned. "Failed to update level for {ctx.Interaction.User.Username} as level field must within 1-60!"

- Determines if the level chosen is updated within the database.
	- <span style="color:rgb(255, 97, 97)">False</span>: A deferred embed message is returned. "Failed to update your level!"

If the checks were valid, an embed message will be returned. "Successful updated your level!."

``` run-csharp
[ModalCommand("ProfileLevel")]
public async Task UpdateProfileLevel(ModalContext ctx)
{
	await ctx.Interaction.DeferAsync(true);

	var modalValues = ctx.Values;
	
	var isLevel = await LevelCheck(ctx, modalValues);

	if (!isLevel.Item1)
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
			.WithContent($"Failed to update level for {ctx.Interaction.User.Username} as level field must within 1-60!"));
		return;
	}

	var isUpdated = await _db.UpdateLevelAsync(ctx.Interaction.User.Id, isLevel.Item2);

	if (isUpdated)
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successful updated your level!."));
	}
	else
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to update your level!."));
	}

}
```


## UpdateProfileIGN(..)

The UpdateProfileIGN() method updates a user's profile in-game name in the database. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

- Determines if the name chosen is updated within the database.
	- <span style="color:rgb(255, 97, 97)">False</span>: A deferred embed message is returned. "Failed to update your IGN!"

If the checks were valid, an embed message will be returned. "Successful updated your IGN!."

``` run-csharp
[ModalCommand("ProfileInGameName")]
public async Task UpdateProfileIGN(ModalContext ctx)
{
	await ctx.Interaction.DeferAsync(true);

	var modalValues = ctx.Values;

	var isUpdated = await _db.UpdateIngameName(ctx.Interaction.User.Id, modalValues[0]);

	if (isUpdated)
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successful updated your IGN!."));
	}
	else
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to update your IGN!."));
	}

}
```


## LevelCheck(..)

The LevelCheck() method whether the modal value for the level is a valid integer between 1-60.

``` run-csharp
public async Task<(bool, int)> LevelCheck(ModalContext ctx, string[] modalValues)
{
	int level;
	try
	{
		level = int.Parse(modalValues.ElementAt(0));
	}
	catch (Exception ex)
	{
		Console.WriteLine(ex.Message);
		return (false, -1);
	}


	if (level <= 0 || level > 60)
	{
		return (false, -1);
	}

	return (true, level);
}
```

---
References: