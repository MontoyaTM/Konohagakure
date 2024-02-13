using DSharpPlus;
using DSharpPlus.ButtonCommands.Extensions;
using DSharpPlus.ButtonCommands;
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
using DSharpPlus.CommandsNext.Attributes;

namespace Konohagakure.Profile
{
	[ModuleLifespan(ModuleLifespan.Transient)]
	public class ModalCommandProfile : ModalCommandModule
	{
		public IDatabaseProfileData _db { get; set; }

		[ModalCommand("ProfileLevel")]
		public async Task UpdateProfileLevel(ModalContext ctx)
		{
			await ctx.Interaction.DeferAsync(true);

			var modalValues = ctx.Values;
	
			var isLevel = await LevelCheck(ctx, modalValues);

			if (!isLevel.Item1)
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
					.WithContent($"Failed to update level for {ctx.Interaction.User.Username} as level field must within 1-60!"));
				return;
			}

			var isUpdated = await _db.UpdateLevelAsync(ctx.Interaction.User.Id, isLevel.Item2);

			if (isUpdated)
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successful updated your level!."));
			}
			else
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to update your level!."));
			}

		}

		[ModalCommand("ProfileInGameName")]
		public async Task UpdateProfileIGN(ModalContext ctx)
		{
			await ctx.Interaction.DeferAsync(true);

			var modalValues = ctx.Values;

			var isUpdated = await _db.UpdateIngameName(ctx.Interaction.User.Id, modalValues[0]);

			if (isUpdated)
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Successful updated your IGN!."));
			}
			else
			{
				await ctx.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().WithContent($"Failed to update your IGN!."));
			}

		}

		public async Task<(bool, int)> LevelCheck(ModalContext ctx, string[] modalValues)
		{
			int level;
			try
			{
				level = int.Parse(modalValues.ElementAt(0));
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return (false, -1);
			}


			if (level <= 0 || level > 60)
			{
				return (false, -1);
			}

			return (true, level);
		}
	}
}
