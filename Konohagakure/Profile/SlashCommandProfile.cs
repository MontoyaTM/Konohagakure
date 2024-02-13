using DSharpPlus.CommandsNext.Attributes;
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
	[ModuleLifespan(ModuleLifespan.Transient)]
	public class SlashCommandProfile : ApplicationCommandModule
	{
		public IDatabaseProfileData _db { get; set; }
		public IDatabaseHokageData _db2 { get; set; }

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

		[SlashCommand("update_user_organization", "Updates a user's Organization & Rank title on their profile.")]
		public async Task UpdateOrg(InteractionContext ctx, [Option("User", "The user you wish to assign Organization and Rank.")] DiscordUser User,

															[Choice("Leaf 12 Guardians", "12 Guardians")]
															[Choice("Leaf Military Police Force", "Leaf Military Police Force")]
															[Choice("Leaf Medical Corp", "Lead Medical Corp")]
															[Choice("Leaf ANBU", "Leaf ANBU")]
															[Option("Organization", "The organization to assign the user.")] string Organization,

															[Option("Rank", "The rank to assign the user.")] string Rank)
		{
			await ctx.DeferAsync(true);

			var hasRole = ctx.Member.Roles.Any(x => x.Name == "Hokage" || x.Name == "Org Leader");

			if (hasRole)
			{
				var Member = (DiscordMember)User;
				var isAssigned = await _db.UpdateOrganizationAsync(Member.Id, Organization, Rank);

				if (isAssigned)
				{
					await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully assigned {Organization} {Rank} to {User.Username}!"));
				}
				else
				{
					await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to assign {Organization} {Rank} to {User.Username}!"));
				}
			}
			else
			{
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent("You do not have the role to access this command!"));
			}
		}

		[SlashCommand("add_alt", "Inserts a character into your alt(s) list. ")]
		[SlashCooldown(2, 30, SlashCooldownBucketType.User)]
		public async Task InsertAlt(InteractionContext ctx, [Option("IGN", "The ingame name of the character you want to add.")] string Alt)
		{
			await ctx.DeferAsync(true);

			var MemberID = ctx.Interaction.User.Id;
			var isUpdated = await _db.AddAltAsync(MemberID, Alt);

			if (isUpdated)
			{
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Successfully added {Alt} to your alt(s) list!"));

				var isRetrieved = await _db2.RetrieveAlts(MemberID);

				DiscordMember member = await ctx.Guild.GetMemberAsync(MemberID);

				var embedAlts = new DiscordMessageBuilder()
				.AddEmbed(new DiscordEmbedBuilder()
					.WithColor(DiscordColor.Red)
					.WithTitle($"{ctx.Interaction.User.Username} Alt(s) List:")
					.WithDescription(String.Join(",", isRetrieved.Item2))
					.WithThumbnail(Images.LeafSymbol_URL)
					.WithImageUrl(ctx.Interaction.User.AvatarUrl)
				);

				await member.SendMessageAsync(embedAlts);
			}
			else
			{
				await ctx.EditResponseAsync(new DiscordWebhookBuilder().WithContent($"Failed to add {Alt} to your alt(s) list!"));
			}
		}
	}
}
