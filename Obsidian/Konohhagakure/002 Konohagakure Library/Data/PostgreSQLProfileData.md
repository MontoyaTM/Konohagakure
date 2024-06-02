Status: 
Tag:
Links:

---
> [!note] 
>  # PostgreSQLProfileData Class

The PostgreSQLProfileData Class implements the IDatabaseProfileData Interface class and its methods. The config variable is used to retrieve the needed configuration using Dependency Injection.

``` run-csharp
public class PostgreSQLProfileData : IDatabaseProfileData
{
	private readonly IPostgreSQLDataAccess _db;
	private const string connectionStringName = "PostgreSQL";

	public PostgreSQLProfileData(IPostgreSQLDataAccess db)
	{
		_db = db;
	}
```

## UserExistsAsync(..)

The UserExistsAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to get the number of records associated with the statement. The method is used to determine whether the user exists in the database using the member id.

``` run-csharp
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
```

## isEmptyAsync()

The isEmptyAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to get the number of records associated with the statement. The method is used to determine whether the user exists in the database using the server id.

``` run-csharp
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
```

## StoreVillagerApplicationAsync(..)

The StoreVillagerApplicationAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to store a ProfileModel into the database.

``` run-csharp
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
```

## RetrieveProfileAsync(..)

The RetrieveProfileAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to retrieve a ProfileModel from the database using a member id.

``` run-csharp
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
```

## UpdateProfileImageAsync(..)

The UpdateProfileImageAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to update the profile image column based on a given member id.

``` run-csharp
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
```

## GetProfileImageAsync(..)

The GetProfileImageAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to retrieve the profile image based on a given member id.

``` run-csharp
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
```

## UpdateFameAsync(..)

The UpdateFameAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to update the profile fame based on a given member id.

``` run-csharp
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
```

## UpdateClanAsync(..)

The UpdateClanAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to update the profile clan based on a given member id.

``` run-csharp
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
```

## UpdateMasteriesAsync(..)

The UpdateMasteriesAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to update the profile masteries based on a given member id.

``` run-csharp
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
```

## UpdateOrganizationAsync(..)

The UpdateOrganizationAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to update the profile organization based on a given member id.

``` run-csharp
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
```

## UpdateLevelAsync(..)

The UpdateLevelAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to update the profile level based on a given member id.

``` run-csharp
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
```

## UpdateIngameName(..)

The UpdateIngameName() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to update the profile in-game name based on a given member id.

``` run-csharp
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
```

## AddAltAsync(..)

The AddAltAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to update the profile alts based on a given member id.

``` run-csharp
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
```

## UpdateRaidAsync(..)

The UpdateRaidAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to update the profile raid based on a given member id.

``` run-csharp
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
```

## GetRaidMasteries(..)

The GetRaidMasteries() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to retrieve the profile masteries based on a given list of member ids.

``` run-csharp
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
```

## DeleteApplication(..)

The DeleteApplication() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to delete a user profile based on a given member id.

``` run-csharp
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
```

## RetrieveAlts(..)

The RetrieveAlts() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to retrieve a profile alts based on a given member id.

``` run-csharp
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
```

## UpdateProctoredMissionsAsync(..)

The UpdateProctoredMissionsAsync() method is used to pass a SQL statement to PostgreSQLDataAccess where it will try to update a profile proctored mission based on a given member id.

``` run-csharp
public async Task<bool> UpdateProctoredMissionsAsync(ulong MemberId)
{
	try
	{
		string sql = "UPDATE konohagakure.profiledata " +
					$"SET proctoredmissions = proctoredmissions + 1 " +
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
```

---
Reference: