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
using static System.Net.Mime.MediaTypeNames;
using KonohagakureLibrary.ImageURLs;

namespace Konohagakure.VillagerApplication
{
	public class PrefixCommandVillagerApplication : BaseCommandModule
	{
		[Command("villager_application")]
		[RequireRoles(RoleCheckMode.Any, "Administrator")]
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
	}
}
