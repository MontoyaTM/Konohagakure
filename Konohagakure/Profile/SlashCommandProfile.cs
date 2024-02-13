using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using KonohagakureLibrary.Data;
using KonohagakureLibrary.DBUtil;
using KonohagakureLibrary.ImageURLs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Konohagakure.Profile
{
	public class SlashCommandProfile : ApplicationCommandModule
	{
		public IDatabaseProfileData _db { get; set; }

		[SlashCommand("profile", "Displays the user's profile as an embed.")]
		[SlashCooldown(2, 30, SlashCooldownBucketType.User)]
		public async Task DisplayProfile(InteractionContext ctx)
		{
			await ctx.DeferAsync();

			var memberId = ctx.Member.Id;
			var userExist = await _db.UserExistsAsync(memberId);

			if (userExist == false)
			{
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"{ctx.User.Username}'s profile does not exist!"));
				return;
			}

			var isRetieved = await _db.RetrieveProfileAsync(memberId);

			if (isRetieved.Item1)
			{
				var profile = isRetieved.Item2.First();

				var embedApplication = new DiscordMessageBuilder()
						.AddEmbed(new DiscordEmbedBuilder()
							.WithColor(DiscordColor.SpringGreen)
							.WithTitle($"Leaf Village Application")
							.WithImageUrl(profile.ProfileImage)
							.WithThumbnail(Images.LeafSymbol_URL)
							.AddField("IGN:", $"```{profile.InGameName}```", true)
							.AddField("Organization:", $"```{profile.Organization}```", true)
							.AddField("Organization Rank:", $"```{profile.OrganizationRank}```", true)
							.AddField("Level:", $"```{profile.Level}```", true)
							.AddField("Masteries:", $"```{String.Join(",", profile.Masteries)}```", true)
							.AddField("Clan:", $"```{profile.Clan}```", true)
							.AddField("Proctored Missions:", $"```{profile.ProctoredMissions}```", true)
							.AddField("Raids:", $"```{profile.Raids}```", true)
							.AddField("Fame:", $"```{profile.Fame}```", true)
							);

				await ctx.EditResponseAsync(new DiscordWebhookBuilder(embedApplication));
			}
			else
			{
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("Failed to retrieved profile!"));
			}
		}

		[SlashCommand("update_profileimage", "Updates the user's profile image.")]
		[SlashCooldown(2, 30, SlashCooldownBucketType.User)]
		public async Task UpdateProfileImage(InteractionContext ctx, [Option("Image", "The image you want to upload.")] DiscordAttachment image)
		{
			await ctx.DeferAsync(true);

			var MemberID = ctx.Interaction.User.Id;
			var isUpdated = await _db.UpdateProfileImageAsync(MemberID, image.Url);

			if (isUpdated)
			{
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully updated profile image!"));
			}
			else
			{
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to updated profile image!"));
			}
		}

		[SlashCommand("give_fame", "Gives fame to a user: 1 day cooldown.")]
		[SlashCooldown(1, 86400, SlashCooldownBucketType.User)]
		public async Task GiveFame(InteractionContext ctx, [Option("User", "Discord User to recieve your fame.")] DiscordUser User)
		{
			await ctx.DeferAsync();

			var MemberID = ctx.Interaction.User.Id;
			var UserID = User.Id;

			var profileImage = await _db.GetProfileImageAsync(MemberID);
			var isFameUpdated = await _db.UpdateFameAsync(UserID);

			if (isFameUpdated)
			{
				var embedFame = new DiscordMessageBuilder()
					.AddEmbed(new DiscordEmbedBuilder()
						.WithColor(DiscordColor.SpringGreen)
						.WithDescription($"{ctx.Interaction.User.Username} gave fame to {User.Username}!")
						.WithThumbnail(profileImage.Item2.First())
					);

				await ctx.EditResponseAsync(new DiscordWebhookBuilder(embedFame));
			}
			else
			{
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Unable to give {User.Username} fame!"));
			}
		}

	}
}
