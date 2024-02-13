using DSharpPlus;
using DSharpPlus.ButtonCommands;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.ModalCommands;
using KonohagakureLibrary.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konohagakure.Profile
{
	[ModuleLifespan(ModuleLifespan.Transient)]
	public class ButtonCommandProfile : ButtonCommandModule
	{
		public IDatabaseProfileData _db { get; set; }

		[ButtonCommand("btn_UpdateLevel")]
		public async Task UpdateLevel(ButtonContext ctx)
		{
			var userExists = await _db.UserExistsAsync(ctx.Interaction.User.Id);

			if (userExists)
			{
				var modalUpdateLevel = ModalBuilder.Create("ProfileLevel")
				.WithTitle("Update Profile — Character Level")
				.AddComponents(new TextInputComponent("Level:", "ProfileLevel", "Level (1-60)", null, true, TextInputStyle.Short));

				await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalUpdateLevel);
			}
			else
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("You do not have a profile in the database!"));
			}
		}

		[ButtonCommand("btn_UpdateIGN")]
		public async Task UpdateIGN(ButtonContext ctx)
		{
			var userExists = await _db.UserExistsAsync(ctx.Interaction.User.Id);

			if (userExists)
			{
				var modalUpdateLevel = ModalBuilder.Create("ProfileInGameName")
				.WithTitle("Update Profile — Character IGN")
				.AddComponents(new TextInputComponent("IGN:", "ProfileIGN", "Ingame Name", null, true, TextInputStyle.Short));

				await ctx.Interaction.CreateResponseAsync(InteractionResponseType.Modal, modalUpdateLevel);
			}
			else
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent("You do not have a profile in the database!"));
			}
		}
	}
}

