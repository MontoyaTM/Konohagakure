using KonohagakureLibrary.DBUtil;
using KonohagakureLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonohagakureLibrary.Data
{
	public class PostgreSQLRequestData : IDatabaseRequestData
	{
		private readonly IPostgreSQLDataAccess _db;
		private const string connectionStringName = "PostgreSQL";

		public PostgreSQLRequestData(IPostgreSQLDataAccess db)
		{
			_db = db;
		}

		public async Task<bool> StoreRPRequestAsync(RequestModel request)
		{
			try
			{
				string sql = "INSERT INTO konohagakure.requestdata (requestid, memberid, username, serverid, servername, ingamename, mission, attendees, timezone) " +
							$"VALUES (@requestid, @memberid, @username, @serverid, @servername, @ingamename, @mission, @attendees, @timezone);";

				await _db.SaveData(sql, new
				{
					requestid = (long)request.RequestId,
					memberid = (long)request.MemberId,
					username = request.Username,
					serverid = (long)request.ServerId,
					servername = request.ServerName,
					ingamename = request.InGameName,
					mission = request.Mission,
					attendees = request.Attendees,
					timezone = request.Timezone
				}, connectionStringName);

				return true;
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}

		public async Task<bool> DeleteRequestAsync(ulong RequestId)
		{
			try
			{
				string sql = "DELETE FROM konohagakure.requestdata " +
							$"WHERE requestid = @requestid;";

				await _db.SaveData(sql, new { requestid = (long)RequestId }, connectionStringName);

				return true;

			} catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return false;
			}
		}
		

	}
}
