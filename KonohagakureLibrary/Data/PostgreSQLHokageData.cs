using KonohagakureLibrary.DBUtil;
using KonohagakureLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonohagakureLibrary.Data
{
	public class PostgreSQLHokageData : IDatabaseHokageData
	{
		private readonly IPostgreSQLDataAccess _db;
		private const string connectionStringName = "PostgreSQL";

		public PostgreSQLHokageData(IPostgreSQLDataAccess db)
		{
			_db = db;
		}

		public async Task<bool> DeleteApplication(ulong MemberId)
		{
			try
			{
				string sql = "DELETE FROM konohagakure.profiledata " +
								 $"WHERE memberid = @memberid;";

				await _db.SaveData(sql, new { memberid = (long)MemberId }, connectionStringName);

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<(bool, string[])> RetrieveAlts(ulong MemberId)
		{
			try
			{
				string sql = "SELECT p.alts " +
							 "FROM konohagakure.profiledata p " +
							$"WHERE memberid = @memberid;";

				var result = await _db.LoadData<ProfileModel, dynamic>(sql, new { memberid = (long)MemberId }, connectionStringName);
				

				return (true, result[0].Alts);
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return (false, null);
			}
		}
	}
}
