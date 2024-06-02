Status: 
Tag:
Links:

---
> [!note] 
>  # IPostgreSQLDataAccess Interface

The IPostgreSQLDataAccess Interface is used to show the required methods needed for the database to implement. This allows for the use of different database.

``` run-csharp
public interface IPostgreSQLDataAccess
{
	Task<List<T>> LoadData<T, U>(string sqlStatement, U parameters, string connectionStringName);
	Task SaveData<T>(string sqlStatement, T parameters, string connectionStringName);
	Task<long> GetCount<T>(string sqlStatement, T parameters, string connectionStringName);
}
```

---
References: