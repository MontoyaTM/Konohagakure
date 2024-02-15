using KonohagakureLibrary.DBUtil;
using KonohagakureLibrary.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KonohagakureLibrary.Data
{
	public class PostgreSQLProfileData : IDatabaseProfileData
	{
		private readonly IPostgreSQLDataAccess _db;
		private const string connectionStringName = "PostgreSQL";

		public PostgreSQLProfileData(IPostgreSQLDataAccess db)
		{
			_db = db;
		}

		public async Task<bool> UserExistsAsync(ulong MemberId)
		{
			try
			{

				string sql = "SELECT COUNT(*) " +
							 "FROM konohagakure.profiledata " +
							$"WHERE memberid = @memberId;";

				var count = await _db.GetCount(sql, new { memberId = (long)MemberId }, connectionStringName);

				if (count == 0)
				{
					return false;
				}
				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<bool> isEmptyAsync(ulong ServerId)
		{
			try
			{
				string sql = "SELECT COUNT(*) " +
							 "FROM konohagakure.profiledata " +
							$"WHERE serverid = @serverId;";

				var count = await _db.GetCount(sql, new { serverId = (long)ServerId }, connectionStringName);

				if (count == 0)
				{
					return false;
				}
				else
				{
					return true;
				}

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return true;
			}
		}

		public async Task<bool> StoreVillagerApplicationAsync(ProfileModel profile)
		{

			try
			{
				string sql = "INSERT INTO konohagakure.profiledata (memberid, username, serverid, servername, avatarurl, profileimage, ingamename, level, clan, organization, organizationrank, raids, fame, proctoredmissions, masteries, alts) " +
						$"VALUES (@memberid, @username, @serverid, @servername, @avatarurl, @profileimage, @ingamename, @level, @clan, @organization, @organizationrank, @raids, @fame, @proctoredmissions, @masteries, @alts);";
				await _db.SaveData(sql,
								   new
								   {
									   memberid = (long)profile.MemberId,
									   username = profile.Username,
									   serverid = (long)profile.ServerId,
									   servername = profile.ServerName,
									   avatarurl = profile.AvatarURL,
									   profileimage = profile.ProfileImage,
									   ingamename = profile.InGameName,
									   level = profile.Level,
									   clan = profile.Clan,
									   organization = profile.Organization,
									   organizationrank = profile.OrganizationRank,
									   raids = profile.Raids,
									   fame = profile.Fame,
									   proctoredmissions = profile.ProctoredMissions,
									   masteries = profile.Masteries,
									   alts = profile.Alts
								   },
								   connectionStringName);

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<(bool, List<ProfileModel>)> RetrieveProfileAsync(ulong MemberId)
		{
			try
			{
				string sql = "SELECT p.ingamename, p.level, p.masteries, p.clan, p.organization, p.organizationrank, p.raids, p.fame, p.avatarurl, p.profileimage, p.proctoredmissions " +
							 "FROM konohagakure.profiledata p " +
							$"WHERE memberid = @memberId;";

				var profile = await _db.LoadData<ProfileModel, dynamic>(sql, new { memberId = (long)MemberId }, connectionStringName);

				return (true, profile);
			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return (false, null);
			}
		}

		public async Task<bool> UpdateProfileImageAsync(ulong MemberId, string ImageURL)
		{
			try
			{
				string sql = "UPDATE konohagakure.profiledata " +
								  $"SET profileimage = @ImageURL " +
								  $"WHERE memberid = @memberId;";

				await _db.SaveData(sql, new { memberId = (long)MemberId, ImageURL }, connectionStringName);

				return true;
			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<(bool, List<string>)> GetProfileImageAsync(ulong MemberID)
		{
			try
			{
				string sql = "SELECT p.profileimage " +
							 "FROM konohagakure.profiledata p " +
							$"WHERE memberid = @memberId;";

				var result = await _db.LoadData<string, dynamic>(sql, new { memberId = (long)MemberID }, connectionStringName);

				return (true, result);

			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return (false, null);
			}
		}

		public async Task<bool> UpdateFameAsync(ulong MemberID)
		{
			try
			{
				string sql = "UPDATE konohagakure.profiledata " +
								"SET fame = fame + 1 " +
								$"WHERE memberid = @memberId;";

				await _db.SaveData(sql, new { memberId = (long)MemberID }, connectionStringName);

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<bool> UpdateClanAsync(ulong MemberID, string Clan)
		{
			try
			{
				string sql = "UPDATE konohagakure.profiledata " +
								  $"SET  clan = @Clan " +
								  $"WHERE memberid = @memberId;";

				await _db.SaveData(sql, new { memberId = (long)MemberID, Clan }, connectionStringName);

				return true;

			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<bool> UpdateMasteriesAsync(ulong MemberID, string[] Masteries)
		{
			try
			{
				string sql = "UPDATE konohagakure.profiledata " +
							$"SET  masteries = @Masteries " +
							$"WHERE memberid = @memberId;";

				await _db.SaveData(sql, new { memberId = (long)MemberID, Masteries }, connectionStringName);

				return true;

			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<bool> UpdateOrganizationAsync(ulong MemberID, string Organization, string Rank)
		{
			try
			{
				string sql = "UPDATE konohagakure.profiledata " +
							$"SET  organization = @Organization , organizationrank = @Rank " +
							$"WHERE memberid = @memberId;";

				await _db.SaveData(sql, new { memberId = (long)MemberID, Organization, Rank }, connectionStringName);

				return true;

			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<bool> UpdateLevelAsync(ulong MemberID, int Level)
		{
			try
			{
				string sql = "UPDATE konohagakure.profiledata " +
							$"SET  level = @Level " +
							$"WHERE memberid = @memberId;";

				await _db.SaveData(sql, new { memberId = (long)MemberID, Level }, connectionStringName);

				return true;

			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<bool> UpdateIngameName(ulong MemberID, string IngameName)
		{
			try
			{
				string sql = "UPDATE konohagakure.profiledata " +
							$"SET ingamename = @IngameName " +
							$"WHERE memberid = @memberId;";

				await _db.SaveData(sql, new { memberId = (long)MemberID, IngameName }, connectionStringName);

				return true;

			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<bool> AddAltAsync(ulong MemberID, string Alt)
		{
			try
			{
				string sql = "UPDATE konohagakure.profiledata " +
							$"SET alts = ARRAY_APPEND(alts, @Alt) " +
							$"WHERE memberid = @memberId;";

				await _db.SaveData(sql, new { memberId = (long)MemberID, Alt }, connectionStringName);

				return true;

			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<bool> UpdateRaidAsync(ulong MemberID)
		{
			try
			{
				string sql = "UPDATE konohagakure.profiledata " +
							$"SET raids = raids + 1 " +
							$"WHERE memberid = @memberId;";

				await _db.SaveData(sql, new { memberId = (long)MemberID }, connectionStringName);
				
				return true;

			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<(bool, List<string>)> GetRaidMasteries(List<ulong> MemberIDs)
		{
			try
			{
				List<string> output = new List<string>(MemberIDs.Count);
				List<ProfileModel> models = new List<ProfileModel>();

				var count = 0;
                foreach (var id in MemberIDs)
                {
                    string sql = "SELECT p.ingamename, p.masteries " +
								   "FROM konohagakure.profiledata p " +
								  $"WHERE p.memberid = @memberId;";

					models = await _db.LoadData<ProfileModel, dynamic>(sql, new { memberId = (long)MemberIDs[count] }, connectionStringName);
				};

				foreach (var model in models)
				{
					output.Add($"{model.InGameName} — **{string.Join("/", model.Masteries)}**");
				}

				return (true, output);

            } catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return (false, null);
			}

		}

	}
}
