Status: 
Tag:
Links: 

---
> [!note] 
>  # PrefixCommandVillagerApplication Class

The PrefixCommandVillagerApplication Class extends BaseCommandModule Class, which is required for commands to be executed.

``` run-csharp
public class PrefixCommandVillagerApplication : BaseCommandModule
{
```

## VillagerApplication(..)

The VillagerApplication() method displays an embed message for the <span style="color:rgb(102, 240, 129)">Villager Application</span>, along with a Discord Button component that will be used to display a Discord Modal component.

Command: <span style="color:rgb(102, 240, 129)">/villager_application</span>
Roles: Administrator

![[Villager Application.png | center]]

``` run-csharp
    [Command("villager_application")]
	[RequireRoles(RoleCheckMode.Any, "Administrator")]
	[Description("Displays an embed message for villager applications.")]
	public async Task VillagerApplication(CommandContext ctx)
	{
		ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();			

		var embedApplication = new DiscordMessageBuilder()
			.AddEmbed(new DiscordEmbedBuilder()
				.WithColor(DiscordColor.SpringGreen)
				.WithTitle("Villager Application")
				.WithImageUrl(Images.VillagerApplication_URL)
				.WithThumbnail(Images.LeafSymbol_URL)
				.AddField("IGN:", "The in game name for the character you are currently maining.")
				.AddField("Introduction:", "A short introduction about yourself and why you want to be apart of the Leaf community.")
				.AddField("Alt(s):", "An entire list of characters that you play on or have access to, including the IGN of the character for this application. Make sure to separate your alt(s) with a comma!" +
									 "\n\n Ex: IGN, Alt1, Alt2, ...")
				.WithFooter("Please be aware of one application per user, you will not be able to edit your application once submitted!")
			).AddComponents
			(
				new DiscordButtonComponent(ButtonStyle.Success, buttonCommand.BuildButtonId("btn_CreateApplication"), "Create Application")
			);

		await ctx.Message.DeleteAsync();
		await ctx.Channel.SendMessageAsync(embedApplication);
	}
```


## ApplicantInfo(..)

The ApplicantInfo() method displays an embed message as a reference when choosing either button when an villager application is sent. The two Discord Button components do not have any functionality and are for display purposes.

Command: <span style="color:rgb(102, 240, 129)">/application_information</span>
Roles: Administrator

![[ApplicantInfo.png | center]]

``` run-csharp
[Command("application_information")]
[RequireRoles(RoleCheckMode.Any, "Administrator")]
[Description("Displays an Embed used as a guide to accepting and denying applications.")]
public async Task ApplicationInfo(CommandContext ctx)
{
	ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

	var embedApplication = new DiscordMessageBuilder()
			.AddEmbed(new DiscordEmbedBuilder()
				.WithColor(DiscordColor.SpringGreen)
				.WithTitle($"Button Information")
				.WithThumbnail(Images.LeafSymbol_URL)
				.AddField("Accept Applicant:", "Pressing this button will grant the user Genin role and their application will be sent to the application-accepted channel for storage. ")
				.AddField("Deny Applicant:", "Pressing this button will display an embed message asking you to type the reason for denying the applicant. " +
											 "Your response will then be sent to the user and their application will be sent to the application-denied channel.")
				.WithFooter("The buttons are for display purposes, they do not have functionality.")
			).AddComponents(new DiscordComponent[]
			{
				new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("displayOnly1"), "Accept Applicant"),
				new DiscordButtonComponent(ButtonStyle.Secondary, buttonCommand.BuildButtonId("displayOnly2"), "Deny Applicant")
			});
	await ctx.Message.DeleteAsync();
	await ctx.Channel.SendMessageAsync(embedApplication);
}
```

---
References: