
namespace KonohagakureLibrary.DBUtil
{
	public interface IPostgreSQLDataAccess
	{
		Task<List<T>> LoadData<T, U>(string sqlStatement, U parameters, string connectionStringName);
		Task SaveData<T>(string sqlStatement, T parameters, string connectionStringName);
		Task<long> GetCount<T>(string sqlStatement, T parameters, string connectionStringName);
	}
}