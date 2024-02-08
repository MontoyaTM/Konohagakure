using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.ButtonCommands;
using DSharpPlus;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.ModalCommands;
using DSharpPlus.ModalCommands.Attributes;
using KonohagakureLibrary.Data;
using KonohagakureLibrary.ImageURLs;
using KonohagakureLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konohagakure.VillagerApplication
{
	[ModuleLifespan(ModuleLifespan.Transient)]
	public class ModalCommandVillagerApplication : ModalCommandModule
	{
		public IDatabaseProfileData _db { get; set; }

		[ModalCommand("VillagerApplication")]
		public async Task SubmitApplication(ModalContext ctx)
		{
			await ctx.Interaction.DeferAsync(true);

			var modalValues = ctx.Values;

			ButtonCommandsExtension buttonCommand = ctx.Client.GetButtonCommands();

			var profileModel = new ProfileModel
			{
				MemberId = ctx.Interaction.User.Id,
				Username = ctx.Interaction.User.Username,
				ServerId = ctx.Interaction.Guild.Id,
				ServerName = ctx.Interaction.Guild.Name,
				AvatarURL = ctx.Interaction.User.AvatarUrl,
				ProfileImage = ctx.Interaction.User.AvatarUrl,
				InGameName = modalValues.ElementAt(0),
				Alts = modalValues.ElementAt(2),
			};

			var applicantExists = await _db.UserExistsAsync(profileModel.MemberId);

			if (applicantExists)
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
																	.WithContent($"Unable to submit application, please be aware of one application per user."));
				return;
			}

			var isApplicationStored = await _db.StoreVillagerApplicationAsync(profileModel);

			if (isApplicationStored)
			{
				var guildChannels = await ctx.Interaction.Guild.GetChannelsAsync();
				var storeApplicationChannel = guildChannels.FirstOrDefault(x => x.Name == "villager-applications");

				if (storeApplicationChannel == null)
				{
					var embedFailed = new DiscordEmbedBuilder()
					{
						Title = "Villager Application Failed",
						Color = DiscordColor.Red,
						Description = "Channel: villager-applications does not exist!"
					};

					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
					return;
				}

				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
																	.WithContent($"Succesfully created a villager application for {ctx.Interaction.User.Username}"));

				var embedApplication = new DiscordMessageBuilder()
					.AddEmbed(new DiscordEmbedBuilder()
						.WithColor(DiscordColor.SpringGreen)
						.WithTitle($"Leaf Village Application")
						.WithImageUrl(ctx.Interaction.User.AvatarUrl)
						.WithThumbnail(Images.LeafSymbol_URL)
						.WithFooter(ctx.Interaction.User.Id.ToString())
						.AddField("IGN:", profileModel.InGameName)
						.AddField("Introduction:", modalValues.ElementAt(1))
						.AddField("Alt(s):", profileModel.Alts))
					.AddComponents(
					new DiscordButtonComponent(ButtonStyle.Primary, buttonCommand.BuildButtonId("btn_AcceptApplicant"), "Accept Applicant"),
					new DiscordButtonComponent(ButtonStyle.Secondary, buttonCommand.BuildButtonId("btn_DeclineApplicant"), "Deny Applicant")
					);

				await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(storeApplicationChannel.Id), embedApplication);
			}
			else
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
																	.WithContent($"Failed to created application for {ctx.Interaction.User.Username}."));
			}
		}
	}
}
