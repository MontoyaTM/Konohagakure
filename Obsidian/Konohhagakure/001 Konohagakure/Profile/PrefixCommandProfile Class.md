Status: 
Tag:
Links:

---
> [!note] 
>  # PrefixCommandProfile Class

The PrefixCommandProfile Class extends BaseCommandModule Class, which is required for commands to be executed.

``` run-csharp
public class PrefixCommandProfile : BaseCommandModule
{
```


## UpdateProfile(..)

The UpdateProfile() method creates an embedded message with two Discord Select Components and two Discord Button Components. The embedded message is used as a user-interface to allow users to update their profile in the database.

![[Update Character Profile.png| center]]

- The embedded message contains a List of DiscordSelectComponentOptions to allow a user to update their clan.

![[Update Clan.png| center]]

- The embedded message contains a second List of DiscordSelectComponentOptions to allow a user to update their masteries.
![[Update Masteries.png| center]]

``` run-csharp
[Command("character_profile")]
[RequireRoles(RoleCheckMode.Any, "Administrator")]
public async Task UpdateProfile(CommandContext ctx)
{
	ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

	var dropdownClanOptions = new List<DiscordSelectComponentOption>()
	{
		new DiscordSelectComponentOption("Clanless", "Clanless", null, false, new DiscordComponentEmoji(1206664158813233194)),
		new DiscordSelectComponentOption("Sasayaki", "Sasyaki", null, false, new DiscordComponentEmoji(1206664189985165342)),
		new DiscordSelectComponentOption("Muteki", "Muteki", null, false, new DiscordComponentEmoji(1206664227268464710)),
		new DiscordSelectComponentOption("Suwa", "Suwa", null, false, new DiscordComponentEmoji(1206664242103844944)),
		new DiscordSelectComponentOption("Ukiyo", "Ukiyo", null, false, new DiscordComponentEmoji(1206664256666476594)),
		new DiscordSelectComponentOption("Hayashi", "Hayashi", null, false, new DiscordComponentEmoji(1206664266866884638)),

	};
	var createClanDropdown = new DiscordSelectComponent("dpdwn_ClanEmoji", "Update Clan", dropdownClanOptions, false, 1, 1);

	var dropdownMasteryOptions = new List<DiscordSelectComponentOption>()
	{
		new DiscordSelectComponentOption("Fire", "Fire", null, false, new DiscordComponentEmoji(1206665774870306836)),
		new DiscordSelectComponentOption("Wind", "Wind", null, false, new DiscordComponentEmoji(1206665892264550492)),
		new DiscordSelectComponentOption("Lightning", "Lightning", null, false, new DiscordComponentEmoji(1206665815097737256)),
		new DiscordSelectComponentOption("Earth", "Earth", null, false, new DiscordComponentEmoji(1206665803181719682)),
		new DiscordSelectComponentOption("Water", "Water", null, false, new DiscordComponentEmoji(1206665834148270140)),
		new DiscordSelectComponentOption("Medical", "Medical", null, false, new DiscordComponentEmoji(1206665864691064892)),
		new DiscordSelectComponentOption("Weapon Master", "Weapon Master", null, false, new DiscordComponentEmoji(1206665908123209728)),
		new DiscordSelectComponentOption("Taijutsu", "Taijutsu", null, false, new DiscordComponentEmoji(1206665847263985674)),
		new DiscordSelectComponentOption("Gentle Fist", "Gentle Fist", null, false, new DiscordComponentEmoji(1206666003728302110)),
		new DiscordSelectComponentOption("Fan", "Fan", null, false, new DiscordComponentEmoji(1206665929149390919)),
		new DiscordSelectComponentOption("Bubble", "Bubble", null, false, new DiscordComponentEmoji(1206665947176505396))
	};
	var createMasteryDropdown = new DiscordSelectComponent("dpdwn_MasteryEmoji", "Update Mastery(s)", dropdownMasteryOptions, false, 1, 2);

	var embedUpdate = new DiscordMessageBuilder()
		.AddEmbed(new DiscordEmbedBuilder()
			.WithColor(DiscordColor.SpringGreen)
			.WithTitle("Character Profile")
			.AddField("Description:", "Please use the following options below to update your character profile information.")
			.WithImageUrl(Images.CharacterProfile_URL)
			.WithThumbnail(Images.LeafSymbol_URL)
		)
		.AddComponents(createClanDropdown)
		.AddComponents(createMasteryDropdown)
		.AddComponents(
		new DiscordButtonComponent(ButtonStyle.Success, buttonCommand.BuildButtonId("btn_UpdateIGN"), "Update IGN"),
		new DiscordButtonComponent(ButtonStyle.Success, buttonCommand.BuildButtonId("btn_UpdateLevel"), "Update Level"));

	await ctx.Message.DeleteAsync();
	await ctx.Channel.SendMessageAsync(embedUpdate);
}
```

---
References: