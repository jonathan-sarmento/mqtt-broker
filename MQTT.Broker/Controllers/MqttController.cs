using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using MQTTnet;
using MQTTnet.Packets;
using MQTTnet.Server;
using Newtonsoft.Json;
using Serilog;

namespace MQTT.Broker.Controllers;


public class MqttController
{
    public MqttController()
    {
        
    }

    public Task OnClientConnected(ClientConnectedEventArgs eventArgs)
    {
        return Task.CompletedTask;
    }
    
    public Task ServerOnInterceptingPublishAsync(InterceptingPublishEventArgs arg)
    {
        var context = arg.ApplicationMessage;
        var payload = arg.ApplicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(arg.ApplicationMessage?.Payload);
        
        Log.Logger.Information(
            "TimeStamp: {TimeStamp} -- Message: ClientId = {ClientId}, Topic = {Topic}, Payload = {Payload}, QoS = {Qos}, Retain-Flag = {RetainFlag}",
            DateTime.Now,
            arg.ClientId,
            context.Topic,
            payload,
            context.QualityOfServiceLevel,
            context.Retain);
        
        return Task.CompletedTask;
    }

    public Task ValidateConnection(ValidatingConnectionEventArgs eventArgs)
    {
        Console.WriteLine($"Client '{eventArgs.ClientId}' wants to connect. Accepting!");
        Log.Logger.Information(
            "New connection: ClientId = {ClientId}, Endpoint = {Endpoint}",
            eventArgs.ClientId,
            eventArgs.Endpoint);
        
        return Task.CompletedTask;
    }
    
    public Task ServerOnInterceptingInboundPacketAsync(InterceptingPacketEventArgs arg)
    {
        var json = JsonConvert.SerializeObject(arg.Packet);
        var applicationMessage = JsonConvert.DeserializeObject<MqttApplicationMessage>(json);
        
        var context = applicationMessage;
        var payload = applicationMessage?.Payload == null ? null : Encoding.UTF8.GetString(applicationMessage.Payload);
        
        Log.Logger.Information(
            "TimeStamp: {TimeStamp} -- Message: ClientId = {ClientId}, Topic = {Topic}, Payload = {Payload}, QoS = {Qos}, Retain-Flag = {RetainFlag}",
            DateTime.Now,
            arg.ClientId,
            context?.Topic,
            payload,
            context?.QualityOfServiceLevel,
            context?.Retain);
        
        return Task.CompletedTask;
    }
}