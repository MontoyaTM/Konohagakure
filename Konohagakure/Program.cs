using KonohagakureLibrary.Data;
using KonohagakureLibrary.DBUtil;

namespace Konohagakure
{
	public class Program
	{
		public static void Main(string[] args)
		{
			IHost host = Host.CreateDefaultBuilder(args)
				.ConfigureAppConfiguration((_, config) =>
				{
					config.AddUserSecrets<DiscordBot>();
				})
				.ConfigureServices(services =>
				{
					services.AddHostedService<DiscordBot>()
					.AddTransient<IDatabaseRequestData, PostgreSQLRequestData>()
					.AddTransient<IDatabaseProfileData, PostgreSQLProfileData>()
					.AddTransient<IPostgreSQLDataAccess, PostgreSQLDataAccess>();
				})
				.Build();

			host.Run();
		}
	}
}