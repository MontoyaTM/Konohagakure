Status: 
Tag:
Links:

---
> [!note] 
>  # Main

Konohagakure Discord Bot is a .NET <span style="color:rgb(102, 240, 129)">worker service console application</span> that takes advantage of <span style="color:rgb(102, 240, 129)">Dependency Injection</span>. The project uses <span style="color:rgb(102, 240, 129)">User Secrets</span> to retrieve connection strings to the <span style="color:rgb(102, 240, 129)">PostgreSQL database</span>, <span style="color:rgb(102, 240, 129)">Discord Bot Token</span>, and the default <span style="color:rgb(102, 240, 129)">prefix</span> for the Discord Bot commands.

``` run-csharp
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
```

---
References: