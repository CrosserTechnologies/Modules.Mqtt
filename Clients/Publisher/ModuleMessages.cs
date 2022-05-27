namespace Crosser.Modules.MqttClient.Publisher
{
    internal static class ModuleMessages
    {
        public const string CONNECTED = "Connected to MQTT broker";
        public const string DISCONNECTED = "Disconnected from MQTT broker";
        public const string RECEIVED_DATA_WHEN_NOT_CONNECTED = "MQTT Publisher received data but the connection to the MQTT Broker is not open";
        public const string INVALID_DATA = "Failed to get a valid MQTT message to send";
        public const string MISSING_TOPIC = "Topic was neither found in module settings nor in the incomming message";
        public const string PROCESSING_ERROR = "Error when processing message in";
    }
}
