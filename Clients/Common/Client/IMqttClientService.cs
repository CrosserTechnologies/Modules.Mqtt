namespace Crosser.EdgeNode.Modules.MqttClient.Common.Client
{
    using System;
    using MQTTnet.Client;
    using MQTTnet;
    using System.Threading.Tasks;
    using MQTTnet.Protocol;
    using Crosser.Modules.MqttClient.Common;

    public interface IMqttClientService
    {
        IMqttClient MqttClient { get; }
        MqttModuleConnectionSettings MqttModuleConnectionSettings { get; set; }
        EventHandler OnConnected { get; set; }
        EventHandler OnDisconnected { get; set; }
        EventHandler<MqttApplicationMessage> OnMessageReceived { get; set; }
        Task<bool> Connect();
        Task Disconnect();
        bool IsConnected();
        Task Publish(string topic, byte[] msg, MqttQualityOfServiceLevel qos, bool retain);
        Task Subscribe(string topic, MqttQualityOfServiceLevel qos);
    }
}