Status: 
Tag:
Links: 

---
> [!note] 
>  # PrefixCommandHokageDashboard Class


The PrefixCommandHokageDashboard Class extends BaseCommandModule Class, which is required for commands to be executed. 

``` run-csharp
public class PrefixCommandHokageDashboard : BaseCommandModule
{
```


## HokageDashboard(..)

The HokageDashboard() method creates an embedded message with two Discord Button Components to be display as a graphical interface.

Command: <span style="color:rgb(102, 240, 129)">/hokage_dashboard</span>
Roles: Administrator

![[Hokage Dashboard.png| center]]

``` run-csharp
[Command("hokage_dashboard")]
[RequireRoles(RoleCheckMode.Any, "Administrator")]
[Description("Hokage Dashboard used to manage village affairs.")]
public async Task HokageDashboard(CommandContext ctx)
{
	ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

	var embedDashboard = new DiscordMessageBuilder()
		.AddEmbed(new DiscordEmbedBuilder()
			.WithColor(DiscordColor.SpringGreen)
			.WithTitle("Leaf Village Hokage Dashboard")
			.WithThumbnail(Images.LeafSymbol_URL)
			.WithImageUrl(Images.HokageNPC_URL)
			.AddField("Delete Application", "Enter the Member ID of the applicant you wish to delete. This will remove the data stored in the database of the user. However, this will allow the individual to create a new villager application.")
			.AddField("Retrieve Alt(s)", "Enter the Member ID of the user you want to retrieve a list of alt(s).")
		)
		.AddComponents(new DiscordComponent[]
		{
			new DiscordButtonComponent(ButtonStyle.Danger, buttonCommand.BuildButtonId("btn_DeleteApplication"), "Delete Application"),
			new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_GetAltList"), "Retrieve Alt(s)")
		});

	await ctx.Message.DeleteAsync();
	await ctx.Channel.SendMessageAsync(embedDashboard);
}
```

---
Reference: