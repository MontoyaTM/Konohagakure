Status: 
Tag:
Links:

---
> [!note] 
>  # PostgreSQLRequestData Class

The PostgreSQLRequestData Class implements the IDatabaseRequestData Interface class and its methods. The config variable is used to retrieve the needed configuration using Dependency Injection.

``` run-csharp
public class PostgreSQLRequestData : IDatabaseRequestData
{
	private readonly IPostgreSQLDataAccess _db;
	private const string connectionStringName = "PostgreSQL";

	public PostgreSQLRequestData(IPostgreSQLDataAccess db)
	{
		_db = db;
	}
```

## StoreRPRequestAsync(..)

The StoreRPRequestAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to store a RequestModel to the database.

``` run-csharp
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
```

## DeleteRequestAsync(..)

The DeleteRequestAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to delete a RequestModel in the database using the request id.

``` run-csharp
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
```

---
References: