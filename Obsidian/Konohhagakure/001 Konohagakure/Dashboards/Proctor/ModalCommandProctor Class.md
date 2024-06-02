Status: 
Tag:
Links: 

---
> [!note] 
>  # ModalCommandProctor Class


The ModalCommandProctor Class extends ModalCommandModule Class, which is required for modals to be executed. The dbProfile variable is used to call the PostgreSQLProfileData Class that implements the IDatabaseProfileData through Dependency Injection. Likewise the dbRequest variable is used to call the PostgreSQLRequestData Class that implements the IDatabaseRequestData.

``` run-csharp
[ModuleLifespan(ModuleLifespan.Transient)]
public class ModalCommandProctor : ModalCommandModule
{
	public IDatabaseRequestData _dbRequest { get; set; }
	public IDatabaseProfileData _dbProfile { get; set; }
```

## SubmitRPRequest(..)

The SubmitRPRequest() method reads the values from the modal builder in the CreateRPRequest() method to determine whether to application can be stored in the database. The method begins by creating a deferred response to the interaction, which is done to handle the wait time of the method. The method provides some validation handling:

- Determines whether the RequestModel created using the values from the modal builder can be stored in the database.
	- <span style="color:rgb(255, 97, 97)">False</span>: Returns a deferred message. Failed to created request for {ctx.Interaction.User.Username}, try again.

- Determines if the user's profile image is retrievable from the database.

The method creates a new channel that is only accessible to Chunin rank and above so that anyone of rank can proctor the mission. The method creates an embedded message with information gathered from the modal builder into the created channel.

![[Close RP Request.png| center]]

``` run-csharp
[ModalCommand("RPRequestModal")]
public async Task SubmitRPRequest(ModalContext ctx)
{
	await ctx.Interaction.DeferAsync(true);

	var modalValues = ctx.Values;

	ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

	var requestInfo = new RequestModel
	{
		RequestId = CreateID(),
		MemberId = ctx.Interaction.User.Id,
		Username = ctx.Interaction.User.Username,
		ServerId = ctx.Interaction.Guild.Id,
		ServerName = ctx.Interaction.Guild.Name,
		InGameName = modalValues.ElementAt(0),
		Mission = modalValues.ElementAt(1),
		Timezone = modalValues.ElementAt(2),
		Attendees = modalValues.ElementAt(3)
	};

	var isStored = await _dbRequest.StoreRPRequestAsync(requestInfo);
	var isRetrieved = await _dbProfile.GetProfileImageAsync(ctx.Interaction.User.Id);

	if (isStored)
	{
		var createDiscussionChannel = await ctx.Interaction.Guild.CreateChannelAsync($"{requestInfo.InGameName}: {requestInfo.Mission}", ChannelType.Text, ctx.Interaction.Channel.Parent);

		var embedRequest = new DiscordMessageBuilder()
			.AddEmbed(new DiscordEmbedBuilder()
				.WithColor(DiscordColor.SpringGreen)
							.WithTitle($"RP Mission Request for {requestInfo.InGameName}")
							.WithImageUrl(isRetrieved.Item2[0])
							.WithThumbnail(Images.LeafSymbol_URL)
							.AddField("IGN:", requestInfo.InGameName, true)
							.AddField("RP Mission:", requestInfo.Mission, true)
							.AddField("Timezone:", requestInfo.Timezone, true)
							.AddField("Attendees:", requestInfo.Attendees)
							.WithFooter($"{requestInfo.RequestId}"))
				.AddComponents(
					new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_CloseRequest"), "Close Request")
				);
		await createDiscussionChannel.SendMessageAsync(embedRequest);
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Succesfully created request for {ctx.Interaction.User.Username}"));
	}
	else
	{
		await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to created request for {ctx.Interaction.User.Username}, try again."));
	}
}
```

``` run-csharp
public ulong CreateID()
{
	var random = new Random();

	ulong minValue = 10000000000000000;
	ulong maxValue = 99999999999999999;

	ulong randomNumber = ((ulong)random.Next((int)(minValue >> 32), int.MaxValue) << 32) | ((ulong)random.Next());
	ulong result = randomNumber % (maxValue - minValue + 1) + minValue;

	return result;
}
```

---
References:
