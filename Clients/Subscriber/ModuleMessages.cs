namespace Crosser.Modules.MqttClient.Subscriber
{
    internal static class ModuleMessages
    {
        public const string MISSING_FORMAT = "Format not implemented:";
        public const string FAILED_TO_RECEIVE = "Could not write to internal channel";
        public const string CONNECTED = "Connected to MQTT broker";
        public const string DISCONNECTED = "Disconnected from MQTT broker";
    }
}