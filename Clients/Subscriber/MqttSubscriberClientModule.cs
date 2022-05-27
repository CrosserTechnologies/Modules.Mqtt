namespace Crosser.Modules.MqttClient.Subscriber
{
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using Crosser.EdgeNode.Common.Abstractions.Models;
    using Crosser.EdgeNode.Common.Abstractions.Utilities.Errors;
    using Crosser.EdgeNode.Common.Utilities.Logger;
    using Crosser.EdgeNode.Flows;
    using Crosser.EdgeNode.Flows.Models.Abstractions.Models;
    using Crosser.Modules.MqttClient.Common;
    using MQTTnet;

    public class MqttSubscriberClientModule : MqttClientModule<MqttSubscriberClientModuleSettings>
    {
        private const string DATA = "data";
        private const string TOPIC = "topic";
        private const string RETAIN = "retain";
        private const string QOS = "qos";
        private const string DUPFLAG = "dupFlag";
        public override string UserFriendlyName => "MQTT Client Subscriber";

        public MqttSubscriberClientModule(ILog logger) : base(logger, FlowModuleType.Input)
        {

        }

        protected override async Task MessageReceived(IFlowMessage msg)
        {
            var raw = msg.Get<byte[]>(DATA);
            msg.Remove(DATA);

            switch (this.Settings.DataFormat)
            {
                case DataFormat.JSON:
                    var jsonValue = JsonSerializer.Deserialize(Encoding.UTF8.GetString(raw));
                    msg.SetValue(this.Settings.TargetProperty, jsonValue, true);
                    break;
                case DataFormat.Raw:
                    msg.Set(this.Settings.TargetProperty, raw, true);
                    break;
                default:
                    this.SetStatus(Status.Warning, $"{ModuleMessages.MISSING_FORMAT} {this.Settings.DataFormat}");
                    break;
            }

            await this.Next(msg);
        }

        private async Task OnMessageReceived(object sender, MqttApplicationMessage e)
        {
            try
            {
                IFlowMessage message = new FlowMessage();
                message[DATA] = e.Payload;
                message[TOPIC] = e.Topic;
                message[RETAIN] = e.Retain;
                message[QOS] = (int)e.QualityOfServiceLevel;
                message[DUPFLAG] = e.Dup;

                await this.Receive(message);
            }
            catch (Exception ex)
            {
                var error = ModuleMessages.FAILED_TO_RECEIVE;
                this.log.Error(ex, error);
                this.SetStatus(Status.Error, error);
            }
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

        public override async Task<IError> Start()
        {
            this.MqttService.OnMessageReceived += async (object sender, MqttApplicationMessage e) => await this.OnMessageReceived(sender, e);
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
    }
}