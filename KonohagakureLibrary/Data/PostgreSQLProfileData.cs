using KonohagakureLibrary.DBUtil;
using KonohagakureLibrary.Models;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
							$"WHERE memberid = @MemberId;";

				var results = await _db.LoadData<dynamic, dynamic>(sql, new { MemberId }, connectionStringName);

				if (results.Count() == 0)
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
				return false;
			}
		}

		public async Task<bool> isEmptyAsync(ulong ServerId)
		{
			try
			{
				string sql = "SELECT COUNT(*) " +
							 "FROM konohagakure.profiledata " +
							$"WHERE serverid = @ServerId;";

				var results = await _db.LoadData<dynamic, dynamic>(sql, new { ServerId }, connectionStringName);

				if (results.Count() == 0)
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
				string queryableAlts = QueryableConversion(profile.Alts);
				string queryableMasteries = QueryableConversion(profile.Masteries);

				string sql = "INSERT INTO konohagakure.profiledata (memberid, username, serverid, servername, avatarurl, profileimage, ingamename, level, clan, organization, organizationrank, raids, fame, proctoredmissions, masteries, alts) " +
						$"VALUES ('{profile.MemberId}', '{profile.Username}', '{profile.ServerId}', '{profile.ServerName}', '{profile.AvatarURL}', '{profile.ProfileImage}', '{profile.InGameName}', " +
						$"'{profile.Level}', '{profile.Clan}', '{profile.Organization}', '{profile.OrganizationRank}', '{profile.Raids}', '{profile.Fame}', '{profile.ProctoredMissions}', ARRAY[{queryableMasteries}], ARRAY[{queryableAlts}]);";

				await _db.SaveData(sql, profile, connectionStringName);

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}


		public string QueryableConversion(string profileData)
		{
			string[] array = profileData.Split(",").Select(x => x.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
			string value = string.Join(",", array);
			string queryable = string.Join(",", value.Split(",").Select(x => string.Format("'{0}'", x)));

			return queryable;
		}
	}
}
