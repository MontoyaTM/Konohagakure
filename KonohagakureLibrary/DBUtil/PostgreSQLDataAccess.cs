using Dapper;
using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonohagakureLibrary.DBUtil
{
	public class PostgreSQLDataAccess : IPostgreSQLDataAccess
	{
		private readonly IConfiguration _config;

		public PostgreSQLDataAccess(IConfiguration config)
		{
			_config = config;
		}

		public async Task<List<T>> LoadData<T, U>(string sqlStatement, U parameters, string connectionStringName)
		{
			string connectionString = _config.GetConnectionString(connectionStringName);

			using (IDbConnection connection = new NpgsqlConnection(connectionString))
			{
				List<T> rows = (List<T>)await connection.QueryAsync<T>(sqlStatement, parameters);
				return rows;
			}
		}

		public async Task SaveData<T>(string sqlStatement, T parameters, string connectionStringName)
		{
			string connectionString = _config.GetConnectionString(connectionStringName);

			using (IDbConnection connection = new NpgsqlConnection(connectionString))
			{
				await connection.ExecuteAsync(sqlStatement, parameters);
			};

		}
	}
}
