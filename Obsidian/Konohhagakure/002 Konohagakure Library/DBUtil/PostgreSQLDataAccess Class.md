Status: 
Tag:
Links:

---
> [!note] 
>  # PostgreSQLDataAccess Class

The PostgreSQLDataAccess Class implements the IPostgreSQLDataAccess Interface class and its methods. The config variable is used to retrieve the needed configuration using Dependency Injection.

``` run-csharp
public class PostgreSQLDataAccess : IPostgreSQLDataAccess
{
	private readonly IConfiguration _config;

	public PostgreSQLDataAccess(IConfiguration config)
	{
		_config = config;
	}
```

## LoadData(..)

The LoadData() method is used to retrieve data from the database using a NpgsqlConnection based on the parameters given.

``` run-csharp
public async Task<List<T>> LoadData<T, U>(string sqlStatement, U parameters, string connectionStringName)
{
	string connectionString = _config.GetConnectionString(connectionStringName);

	using (IDbConnection connection = new NpgsqlConnection(connectionString))
	{
		List<T> rows = (List<T>)await connection.QueryAsync<T>(sqlStatement, parameters);
		return rows;
	}
}
```

## SaveData(..)

The SaveData() method is used to save data to the database using a NpgsqlConnection based on the parameters given.

``` run-csharp
public async Task SaveData<T>(string sqlStatement, T parameters, string connectionStringName)
{
	string connectionString = _config.GetConnectionString(connectionStringName);

	using (IDbConnection connection = new NpgsqlConnection(connectionString))
	{
		await connection.ExecuteAsync(sqlStatement, parameters);
	};
}
```

## GetCount(..)

The GetCount() method is used to retrieve the number of records in the database using a NpgsqlConnection based on the parameters given.

``` run-csharp
public async Task<long> GetCount<T>(string sqlStatement,T parameters, string connectionStringName)
{
	string connectionString = _config.GetConnectionString(connectionStringName);

	using (IDbConnection connection = new NpgsqlConnection(connectionString))
	{
		long count = (long) await connection.ExecuteScalarAsync(sqlStatement, parameters);
		return count;
	};
}
```

---
References: