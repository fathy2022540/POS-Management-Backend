using JRM.API;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }
    public static IHostBuilder CreateHostBuilder(string[] args) =>

      Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {

            webBuilder.UseStartup<Startup>();
            webBuilder.ConfigureKestrel(options =>
          {
              options.Limits.MaxRequestBodySize = 2147483648; // Set the desired maximum request body size in bytes
          });

        });
}
