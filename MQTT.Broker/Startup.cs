using System.Net;
using System.Net.Sockets;
using MQTT.Broker.Controllers;
using MQTTnet.AspNetCore;
using Serilog;

namespace MQTT.Broker;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateLogger();
        
        // var host = Dns.GetHostEntry(Dns.GetHostName());
        // IPAddress addr = new IPAddress(new byte[]{127, 0, 0, 1});
        // foreach (var ip in host.AddressList)
        // {
        //     if (ip.AddressFamily == AddressFamily.InterNetwork)
        //     {
        //         Console.WriteLine("IP Address = " + ip.ToString());
        //         addr = ip;
        //     }
        // }
        services.AddHostedMqttServer(
            optionsBuilder =>
            {
                //optionsBuilder.WithDefaultEndpoint();
                optionsBuilder.WithDefaultEndpointBoundIPAddress(IPAddress.Any);
            });
        
        services.AddMqttConnectionHandler();
        services.AddConnections();

        services.AddSingleton<MqttController>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, MqttController mqttController)
    {
        app.UseMqttServer(
            server =>
            {
                server.ValidatingConnectionAsync += mqttController.ValidateConnection;
                server.ClientConnectedAsync += mqttController.OnClientConnected;
                server.InterceptingPublishAsync += mqttController.ServerOnInterceptingPublishAsync;
            });
        
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();
    }
}