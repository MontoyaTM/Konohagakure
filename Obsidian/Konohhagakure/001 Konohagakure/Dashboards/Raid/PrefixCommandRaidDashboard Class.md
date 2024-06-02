Status: 
Tag:
Links: 

---
> [!note] 
>  # PrefixCommandRaidDashboard Class


The PrefixCommandRaidDashboard Class extends BaseCommandModule Class, which is required for commands to be executed.

``` run-csharp
public class PrefixCommandRaidDashboard : BaseCommandModule
{
```

## RaidDashboard(..)

The RaidDashboard() method creates an embedded message with three Discord Button Components to be display as a graphical interface.

Command: <span style="color:rgb(102, 240, 129)">/raid_dashboard</span>
Roles: Administrator


![[Raid Dashboard.png| center]]

``` run-csharp
[Command("raid_dashboard")]
[RequireRoles(RoleCheckMode.Any, "Administrator")]
[Description("Leaf Raid Dashboard used to manage village raids.")]
public async Task RaidDashboard(CommandContext ctx)
{
	ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

	var embedDashboard = new DiscordMessageBuilder()
		.AddEmbed(new DiscordEmbedBuilder()
			.WithColor(DiscordColor.SpringGreen)
			.WithTitle("Leaf Village Raid Dashboard")
			.WithThumbnail(Images.LeafSymbol_URL)
			.WithImageUrl(Images.RaidDashboard_URL)
			.AddField("Raid++", "Increments the Raid stat for each user in the Village Raid voice channel.")
			.AddField("Raid Composition:", "Return a list of masteries for each user in the Village Raid voice channel.")
			.AddField("Voice Channel:", "Moves all users in the Raid Lobby voice channel to the Village Raid voice channel.")
		)
		.AddComponents(new DiscordComponent[]
		{
			new DiscordButtonComponent(ButtonStyle.Success, buttonCommand.BuildButtonId("btn_VillageRaid"), "Raid++"),
			new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_RetrieveMasteries"), "Raid Composition"),
			new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_VoiceChannel"), "Voice Channel")
		});

	await ctx.Message.DeleteAsync();
	await ctx.Channel.SendMessageAsync(embedDashboard);
}
```

---
References: