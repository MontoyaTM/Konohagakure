using DSharpPlus.ButtonCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Net.Models;
using KonohagakureLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konohagakure.Dashboards.Raid
{
	[ModuleLifespan(ModuleLifespan.Transient)]
	public class ButtonCommandRaidDashboard : ButtonCommandModule
	{
		public IDatabaseProfileData _db { get; set; }

		[ButtonCommand("btn_VillageRaid")]
		public async Task RaidCounter(ButtonContext ctx)
		{
			await ctx.Interaction.DeferAsync(true);
			
			var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council" || x.Name == "Raid Leader");

			if (hasRole)
			{
				var guildChannels = await ctx.Guild.GetChannelsAsync();
				var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");
				var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "raid-records");

				if (raidChannel == null)
				{
					var embedFailed = new DiscordEmbedBuilder()
					{
						Title = "Raid++ Failed",
						Color = DiscordColor.Red,
						Description = "Channel: Village Raid does not exist!"
					};

					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
					return;
				}

				if (recordsChannel == null)
				{
					var embedFailed = new DiscordEmbedBuilder()
					{
						Title = "Raid++ Failed",
						Color = DiscordColor.Red,
						Description = "Channel: raid-records does not exist!"
					};

					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
					return;
				}

				if (raidChannel.Users.Count() == 0)
				{
					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"There are no users in the Village Raid voice channel!"));
					return;
				}

				List<DiscordMember> Members = raidChannel.Users.ToList();

				string[] username = new string[Members.Count];

				var i = 0;

				foreach (var member in Members)
				{
					var userExists = await _db.UserExistsAsync(member.Id);

					if (userExists)
					{
						var isUpdated = await _db.UpdateRaidAsync(member.Id);

						username[i] = member.Username;
						i++;
					}
				}

				var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

				var embedRaid = new DiscordMessageBuilder()
					.AddEmbed(new DiscordEmbedBuilder()
						.WithColor(DiscordColor.SpringGreen)
						.WithTitle("Leaf Village — Raid++")
						.WithDescription(string.Join("\n", username))
						.WithFooter("• " + dateTime + "     • Raid Leader: " + ctx.Interaction.User.Username)
					);

				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully incremented all Member's Raids stat!"));

				await ctx.Client.SendMessageAsync(await ctx.Client.GetChannelAsync(recordsChannel.Id), embedRaid);

			}
			else
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
					.WithContent($"Unable to use button command Raid++ for {ctx.Interaction.User.Username}, please check required roles."));
			}
		}

		[ButtonCommand("btn_RetrieveMasteries")]
		public async Task RetrieveMasteries(ButtonContext ctx)
		{
			await ctx.Interaction.DeferAsync(true);

			var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council" || x.Name == "Raid Leader");

			if (hasRole)
			{
				var guildChannels = await ctx.Guild.GetChannelsAsync();
				var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");
				var recordsChannel = guildChannels.FirstOrDefault(x => x.Name == "war-room");

				if (raidChannel == null)
				{
					var embedFailed = new DiscordEmbedBuilder()
					{
						Title = "Raid Composition Failed",
						Color = DiscordColor.Red,
						Description = "Channel: Village Raid voice channel does not exist!"
					};

					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
					return;
				}

				if (recordsChannel == null)
				{
					var embedFailed = new DiscordEmbedBuilder()
					{
						Title = "Raid Composition Failed",
						Color = DiscordColor.Red,
						Description = "Channel: war-room channel does not exist!"
					};

					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
					return;
				}

				if (raidChannel.Users.Count() == 0)
				{
					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"There are no users in the Village Raid voice channel!"));
					return;
				}

				var Members = raidChannel.Users.ToList();
				List<ulong> MemberIDs = new List<ulong>();

				foreach (var member in Members)
				{
					MemberIDs.Add(member.Id);
				}

				var isRetrieved = await _db.GetRaidMasteries(MemberIDs);

				if (isRetrieved.Item1)
				{
					var dateTime = DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");

					var embedMasteries = new DiscordMessageBuilder()
						.AddEmbed(new DiscordEmbedBuilder()
							.WithColor(DiscordColor.SpringGreen)
							.WithTitle("Leaf Village — Raid Composition")
							.WithDescription(string.Join("\n", isRetrieved.Item2))
							.WithFooter("• " + dateTime + "     • Raid Leader: " + ctx.Interaction.User.Username)
						);
					await ctx.Client.SendMessageAsync(recordsChannel, embedMasteries);
					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
						.WithContent($"Successfully retrieved masteries for those inside voice channel!"));
				}
				else
				{
					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
						.WithContent($"Unable to retrieve masteries!"));
				}
			}
			else
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
					.WithContent($"Unable to use button command Retrieve Masteries for {ctx.Interaction.User.Username}, please check required roles."));
			}
		}

		[ButtonCommand("btn_VoiceChannel")]
		public async Task RaidVoiceChannel(ButtonContext ctx)
		{
			await ctx.Interaction.DeferAsync(true);

			var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Council" || x.Name == "Raid Leader");

			if (hasRole)
			{
				var guildChannels = await ctx.Guild.GetChannelsAsync();
				var raidLobby = guildChannels.FirstOrDefault(x => x.Name == "Raid Lobby");
				var raidChannel = guildChannels.FirstOrDefault(x => x.Name == "Village Raid");

				if (raidLobby == null)
				{
					var embedFailed = new DiscordEmbedBuilder()
					{
						Title = "Village Raid Failed",
						Color = DiscordColor.Red,
						Description = "Channel: Raid Lobby does not exist!"
					};

					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
					return;
				}

				if (raidChannel == null)
				{
					var embedFailed = new DiscordEmbedBuilder()
					{
						Title = "Villager Raid Failed",
						Color = DiscordColor.Red,
						Description = "Channel: Village Raid does not exist!"
					};

					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embedFailed));
					return;
				}

				var Members = raidLobby.Users.ToList();

				if (Members.Count == 0)
				{
					await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
						.WithContent($"There are no users in the Village Lobby voice channel!"));
					return;
				}

				foreach (var member in Members)
				{
					if (member.VoiceState != null)
					{
						await member.ModifyAsync(delegate (MemberEditModel Edit)
						{
							Edit.VoiceChannel = raidChannel;
						});
					}
				}
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successfully moved all users to the Village Raid!"));

			}
			else
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
					.WithContent($"Unable to use button command Voice Channel for {ctx.Interaction.User.Username}, please check required roles."));
			}
		}
	}
}
