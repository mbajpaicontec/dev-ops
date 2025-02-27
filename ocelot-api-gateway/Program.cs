using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Ocelot.Middleware;
using Ocelot.DependencyInjection;
using Microsoft.AspNetCore.Builder;

namespace OcelotApiGateway
{
  public class Program
  {
    public static void Main(string[] args)
    {
      BuildWebHost(args).Run();
    }

    public static IWebHost BuildWebHost(string[] args)
    {
      return WebHost.CreateDefaultBuilder(args)
              .UseKestrel(options =>
              {
                // Configure Kestrel to listen on HTTPS
                options.ListenAnyIP(443, listenOptions =>
                {
                  listenOptions.UseHttps("/app/crt/ocelot.pfx", "Mybash@1234");
                });

                // Optionally, listen on HTTP as well
                options.ListenAnyIP(80);
              })
              .UseUrls("http://0.0.0.0:80;https://0.0.0.0:443") // Listen on both HTTP and HTTPS
              .UseStartup<Startup>()
              .ConfigureAppConfiguration((hostingContext, config) =>
              {
                config.AddJsonFile("/app/ocelot1.json")
                      .AddEnvironmentVariables();
              })
              .ConfigureServices(s =>
              {
                s.AddOcelot();
              })
              .Configure(app =>
              {
                app.UseStaticFiles();
                app.UseOcelot().Wait();
              })
              .ConfigureLogging((hostingContext, logging) =>
              {
                logging.AddConsole();
              })
              .UseIISIntegration()
              .Build();
    }
  }
}
