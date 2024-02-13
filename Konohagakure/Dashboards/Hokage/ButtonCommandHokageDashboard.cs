using DSharpPlus.ButtonCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using KonohagakureLibrary.Data;
using KonohagakureLibrary.ImageURLs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konohagakure.Dashboards.Hokage
{
	[ModuleLifespan(ModuleLifespan.Transient)]
	public class ButtonCommandHokageDashboard : ButtonCommandModule
	{
		public IDatabaseHokageData _db { get; set; }

		[ButtonCommand("btn_DeleteApplication")]
		public async Task DeleteApplication(ButtonContext ctx)
		{
			await ctx.Interaction.DeferAsync(true);

			var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council");

			if (hasRole)
			{
				var guildChannels = await ctx.Guild.GetChannelsAsync();
				var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "hokage-records");

				if (recordsChannel == null)
				{
					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("Channel: Record Channel does not exist!"));
					return;
				}

				var interactivity = ctx.Client.GetInteractivity();
				var isMemberIdRetrieved = await RetrieveMemberID(ctx, interactivity);

				if (isMemberIdRetrieved.Item1)
				{
					ulong memberID = isMemberIdRetrieved.Item2;

					var isDeleted = await _db.DeleteApplication(memberID);

					if (isDeleted)
					{
						var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

						var embedDeleted = new DiscordMessageBuilder()
						.AddEmbed(new DiscordEmbedBuilder()
							.WithColor(DiscordColor.SpringGreen)
							.WithTitle("Leaf Village — Delete Application")
							.WithDescription($"Successfully deleted **{isMemberIdRetrieved.Item4.Username}**'s villager application from the database.")
							.WithFooter("• " + dateTime + "     • Executed by: " + ctx.Interaction.User.Username)
						);

						await ctx.Interaction.EditFollowupMessageAsync(isMemberIdRetrieved.Item3.Id, new DiscordWebhookBuilder()
							.WithContent($"Successfully deleted {isMemberIdRetrieved.Item4.Username}'s villager application!"));

						await ctx.Client.SendMessageAsync(recordsChannel, embedDeleted);

					}
					else
					{
						await ctx.Interaction.EditFollowupMessageAsync(isMemberIdRetrieved.Item3.Id, new DiscordWebhookBuilder()
							.WithContent($"Failed to delete villager application!"));
					}
				}
				else
				{
					return;
				}
			}
			else
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
					.WithContent($"Unable to delete application for {ctx.Interaction.User.Username}, please check required roles."));
			}
		}

		[ButtonCommand("btn_GetAltList")]
		public async Task GetAltList(ButtonContext ctx)
		{
			await ctx.Interaction.DeferAsync(true);

			var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council");

			if (hasRole)
			{
				var guildChannels = await ctx.Guild.GetChannelsAsync();
				var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "hokage-records");

				if (recordsChannel == null)
				{
					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("Channel: Record Channel does not exist!"));
					return;
				}

				var interactivity = ctx.Client.GetInteractivity();
				var isMemberIdRetrieved = await RetrieveMemberID(ctx, interactivity);

				if (isMemberIdRetrieved.Item1)
				{
					ulong memberID = isMemberIdRetrieved.Item2;

					var isAltsRetrieved = await _db.RetrieveAlts(memberID);

					if (isAltsRetrieved.Item1)
					{
						var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

						var embedAlts = new DiscordMessageBuilder()
						.AddEmbed(new DiscordEmbedBuilder()
							.WithColor(DiscordColor.SpringGreen)
							.WithTitle($"{isMemberIdRetrieved.Item4.Username} Alt(s) List:")
							.WithDescription(String.Join(",", isAltsRetrieved.Item2))
							.WithThumbnail(Images.LeafSymbol_URL)
							.WithImageUrl(isMemberIdRetrieved.Item4.AvatarUrl)
							.WithFooter("• " + dateTime + "     • Executed by: " + ctx.Interaction.User.Username)
						);

						await ctx.Interaction.EditFollowupMessageAsync(isMemberIdRetrieved.Item3.Id, new DiscordWebhookBuilder()
							.WithContent($"Successfully retrieved {isMemberIdRetrieved.Item4.Username}'s alt(s) list!"));

						await ctx.Client.SendMessageAsync(recordsChannel, embedAlts);
					}
					else
					{
						await ctx.Interaction.EditFollowupMessageAsync(isMemberIdRetrieved.Item3.Id, new DiscordWebhookBuilder()
							.WithContent($"Unable to retrieve alts!"));
						return;
					}
				}
				else
				{
					return;
				}
			}
			else
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
					.WithContent($"Unable to retrieve alts for {ctx.Interaction.User.Username}, please check required roles."));
			}
		}

		public async Task<(bool, ulong, DiscordMessage, DiscordMember?)> RetrieveMemberID(ButtonContext ctx, InteractivityExtension interactivity)
		{
			var embedMessage = new DiscordEmbedBuilder()
			{
				Color = DiscordColor.SpringGreen,
				Title = "Enter the MemberID of the applicant as the next message in this channel."
			};

			var embedMemberID = await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedMessage));
			var memberIdResponse = await interactivity.WaitForMessageAsync(x => x.Author.Id == ctx.Interaction.User.Id, TimeSpan.FromMinutes(2));

			DiscordMember? member;
			ulong memberID;

			if (ulong.TryParse(memberIdResponse.Result.Content, out memberID))
			{
				await ctx.Channel.DeleteMessageAsync(memberIdResponse.Result);
			}
			else
			{
				await ctx.Interaction.EditFollowupMessageAsync(embedMemberID.Id, new DiscordWebhookBuilder()
					.WithContent("Unable to convert your response to a numerical value, please check your submission!"));
				await ctx.Channel.DeleteMessageAsync(memberIdResponse.Result);
				return (false, 0, embedMemberID, null);
			}

			try
			{
				member = await ctx.Guild.GetMemberAsync(memberID);
			}
			catch (Exception ex)
			{
				await ctx.Interaction.EditFollowupMessageAsync(embedMemberID.Id, new DiscordWebhookBuilder()
							.WithContent("MemberID does not match any user inside this server, please check your response!"));
				return (false, 0, embedMemberID, null);
			}

			return (true, memberID, embedMemberID, member);
		}
	}
}
