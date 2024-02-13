using DSharpPlus.ButtonCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using DSharpPlus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.ButtonCommands.Extensions;
using KonohagakureLibrary.ImageURLs;

namespace Konohagakure.Dashboards.Hokage
{
	public class PrefixCommandHokageDashboard : BaseCommandModule
	{
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
	}
}
