using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.ButtonCommands;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KonohagakureLibrary.ImageURLs;

namespace Konohagakure.Dashboards.Raid
{
	public class PrefixCommandRaidDashboard : BaseCommandModule
	{
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
	}
}
