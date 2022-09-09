
using MQTTnet.AspNetCore;

namespace MQTT.Broker;
    
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
                webBuilder.UseKestrel(
                    o =>
                    {
                        // This will allow MQTT connections based on TCP port 1883.
                        o.ListenAnyIP(Int16.Parse(Environment.GetEnvironmentVariable("PORT") ?? "1883"), l => l.UseMqtt());
                    });
                webBuilder.UseStartup<Startup>();
            });
}