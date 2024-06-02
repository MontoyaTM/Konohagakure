
The PrefixCommandProctor Class extends BaseCommandModule Class, which is required for commands to be executed.

``` run-csharp
public class PrefixCommandProctor : BaseCommandModule
{
```

## RankedDashboard()

The RPRequest() method creates an embedded message with one Discord Button Components to be allow a user to request an RP event.

Command: <span style="color:rgb(102, 240, 129)">/rprequest</span>
Roles: Administrator

![[RP Request.png| center]]


``` run-csharp
[Command("rprequest")]
[RequireRoles(RoleCheckMode.Any, "Administrator")]
[Description("Displays an Embed for requesting an RP mission.")]
public async Task RPRequest(CommandContext ctx)
{
	ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

	var embedSupportForm = new DiscordMessageBuilder()
		.AddEmbed(new DiscordEmbedBuilder()
			.WithColor(DiscordColor.SpringGreen)
			.WithTitle("RP Request")
			.WithDescription(" ")
			.WithImageUrl(Images.RankedNPC_URL)
			.WithThumbnail(Images.LeafSymbol_URL)
			.AddField("IGN:", "The in game name of the character that is requesting the rp mission.")
			.AddField("RP Mission:", "The RP mission number you are requesting. You can only request one RP mission at a time! \n\n Ex. RP I, RP II, RP II, ...")
			.AddField("Timezone:", "Timezone more suitable for completion.")
			.AddField("Attendees:", "The person(s) who are going to be attending the RP mission. If you are missing members, a discussion channel will be created for players to reach out. " +
									"\n\nEx. IGN, Ninja2, Ninja3")
			.WithFooter("A Ranked Ninjas will be reaching out to your request in the channel discussion created. Please be aware that RP missions are done in a 3 man team.")
			)
		.AddComponents(new DiscordComponent[]
		{
			new DiscordButtonComponent(ButtonStyle.Success, buttonCommand.BuildButtonId("btn_CreateRequest"), "Create RP Request")
		});

	await ctx.Channel.SendMessageAsync(embedSupportForm);
	await ctx.Message.DeleteAsync();
}
```

---
References: