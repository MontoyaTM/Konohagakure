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
					services.AddHostedService<DiscordBot>();
				})
				.Build();

			host.Run();
		}
	}
}