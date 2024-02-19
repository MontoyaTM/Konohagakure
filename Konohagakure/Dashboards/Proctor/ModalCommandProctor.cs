using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.ButtonCommands;
using DSharpPlus.Entities;
using DSharpPlus;
using DSharpPlus.ModalCommands;
using DSharpPlus.ModalCommands.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KonohagakureLibrary.ImageURLs;
using KonohagakureLibrary.Models;
using KonohagakureLibrary.Data;
using DSharpPlus.CommandsNext.Attributes;

namespace Konohagakure.Dashboards.Proctor
{
	[ModuleLifespan(ModuleLifespan.Transient)]
	public class ModalCommandProctor : ModalCommandModule
	{
		public IDatabaseRequestData _dbRequest { get; set; }
		public IDatabaseProfileData _dbProfile { get; set; }

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

		public ulong CreateID()
		{
			var random = new Random();

			ulong minValue = 10000000000000000;
			ulong maxValue = 99999999999999999;

			ulong randomNumber = ((ulong)random.Next((int)(minValue >> 32), int.MaxValue) << 32) | ((ulong)random.Next());
			ulong result = randomNumber % (maxValue - minValue + 1) + minValue;

			return result;
		}
	}
}
