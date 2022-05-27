namespace Crosser.Modules.MqttClient.Publisher
{
    using Crosser.EdgeNode.Common.Abstractions.Models;
    using Crosser.EdgeNode.Common.Abstractions.Utilities.Errors;
    using Crosser.EdgeNode.Common.Utilities.Logger;
    using Crosser.EdgeNode.Flows;
    using Crosser.Modules.MqttClient.Common;
    using System;
    using System.Text;
    using System.Threading.Tasks;


    public class MqttPublisherClientModule : MqttClientModule<MqttPublisherClientModuleSettings>
    {
        public override string UserFriendlyName => "MQTT Client Publisher";
        private const string TOPIC = "topic";
        private const string MESSAGE = "message";

        public MqttPublisherClientModule(ILog logger) : base(logger, FlowModuleType.Output) { }

        public override async Task<IError> Start()
        {
            base.OnDisconnected += this.OnClientDisconnected;
            base.OnConnected += this.OnClientConnected;
            return await base.Start();
        }

        public override async Task Stop()
        {
            base.OnDisconnected -= this.OnClientDisconnected;
            base.OnConnected -= this.OnClientConnected;
            await base.Stop();
        }

        private void OnClientConnected(object sender, EventArgs e)
        {
            this.SetStatus(Status.Ok, ModuleMessages.CONNECTED);
            this.MqttService.Subscribe(this.Settings.Topic, this.Settings.QoS);
        }

        private void OnClientDisconnected(object sender, EventArgs e)
        {
            if (this.Stopped == false)
            {
                this.SetStatus(Status.Warning, ModuleMessages.DISCONNECTED);
            }
        }

        protected override async Task MessageReceived(IFlowMessage msg)
        {
            try
            {
                if (this.MqttService == null || !this.MqttService.IsConnected())
                {
                    var error = ModuleMessages.RECEIVED_DATA_WHEN_NOT_CONNECTED;
                    this.SetStatus(Status.Warning, error);
                    msg.SetError(error);
                    await this.Next(msg);
                    return;
                }

                var topic = this.GetTopic(msg);
                var (message, valid, messageAsText) = this.GetMessage(msg);

                if (valid is false)
                {
                    throw new ArgumentException(ModuleMessages.INVALID_DATA);
                }

                msg[TOPIC] = topic;
                msg[MESSAGE] = messageAsText;
                await this.Publish(topic, message);

                msg.SetSuccess();
            }
            catch (Exception ex)
            {
                this.SetStatus(Status.Error, $"{ModuleMessages.PROCESSING_ERROR} {this.Name}: {ex.Message}");
                msg.SetError(ex.Message);
            }
            finally
            {
                await this.Next(msg);
            }
        }

        private string GetTopic(IFlowMessage msg)
        {
            if (!string.IsNullOrEmpty(this.Settings.Topic))
            {
                return this.Settings.Topic;
            }

            if (msg.Has<string>(TOPIC))
            {
                return msg.Get<string>(TOPIC);
            }

            throw new Exception(ModuleMessages.MISSING_TOPIC);
        }

        private (byte[] message, bool valid, string messageAsText) GetMessage(IFlowMessage msg)
        {
            try
            {
                string json;
                if (msg.Has(this.Settings.SourceProperty))
                {
                    var m = msg.Get<object>(this.Settings.SourceProperty);
                    switch (m)
                    {
                        case IFlowMessage fm:
                            json = fm.ToJSON();
                            return (Encoding.UTF8.GetBytes(json), true, json);
                        case string s:
                            return (Encoding.UTF8.GetBytes(s), true, s);
                        case byte[] b:
                            return (b, true, Encoding.UTF8.GetString(b));
                        default:
                            switch (this.Settings.DataFormat)
                            {
                                case DataFormat.JSON:
                                    json = EdgeNode.Common.Utilities.Json.JsonSerializer.Serialize(m);
                                    return (Encoding.UTF8.GetBytes(json), true, json);
                            }
                            return (new byte[0], true, string.Empty);
                    }
                }

                return (new byte[0], true, string.Empty);
            }
            catch (Exception ex)
            {
                this.log.Error(ex, ModuleMessages.INVALID_DATA);
                return (null, false, null);
            }
        }

        private async Task Publish(string topic, byte[] message)
        {
            await this.MqttService.Publish(topic, message, this.Settings.QoS, this.Settings.Retain);
        }
    }
}
