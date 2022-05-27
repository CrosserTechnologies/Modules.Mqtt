namespace Crosser.Modules.MqttClient.Common
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Crosser.EdgeNode.Common.Abstractions.Utilities.Errors;
    using Crosser.EdgeNode.Common.Utilities.Logger;
    using Crosser.EdgeNode.Flows;
    using Crosser.EdgeNode.Modules.MqttClient.Common.Client;

    /// <summary>
    /// Baseclass for MQTT module implementations
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class MqttClientModule<T> : FlowModule<T> where T : MqttModuleConnectionSettings, new()
    {
        protected IMqttClientService MqttService { get; private set; }
        protected CancellationTokenSource CancellationTokenSource { get; set; }
        protected CancellationToken CancellationToken { get; set; }
        protected EventHandler OnConnected { get; set; }
        protected EventHandler OnDisconnected { get; set; }
        protected bool Stopped { get; set; }

        protected ILog log;

        public MqttClientModule(ILog logger, FlowModuleType moduleType) : base(moduleType)
        {
            this.CancellationTokenSource = new CancellationTokenSource();
            this.CancellationToken = this.CancellationTokenSource.Token;
            this.log = logger;
        }

        public override Task<IError> Initialize()
        {
            return Task.FromResult(this.InitializeMqttService());
        }

        public override async Task<IError> Start()
        {
            this.Stopped = false;
            var connected = await this.MqttService.Connect();
            if (!connected)
            {
                this.SetStatus(Status.Warning, $"Could not connect to MQTT Broker => {this.Settings.Url}:{this.Settings.Port}");
            }

            return await base.Start();
        }

        public override async Task Stop()
        {
            this.Stopped = true;
            this.CancellationTokenSource.Cancel();
            if (this.MqttService != null)
            {
                await this.MqttService.Disconnect();
            }
            await base.Stop();
        }

        private IError InitializeMqttService()
        {
            this.EnsureClientId();
            return this.EnsureMqttService();
        }

        private IError EnsureMqttService()
        {
            try
            {
                if (this.MqttService == null)
                {
                    this.MqttService = new MqttClientService(this.log, this.Settings, this.CancellationToken);
                    this.MqttService.OnConnected += (sender, e) =>
                    {
                        if (this.OnConnected != null)
                        {
                            this.OnConnected.Invoke(this, EventArgs.Empty);
                        }
                    };
                    this.MqttService.OnDisconnected += (sender, e) =>
                    {
                        if (this.OnDisconnected != null)
                        {
                            this.OnDisconnected.Invoke(this, EventArgs.Empty);
                        }
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                return new Error($"Failed to create MQTT service: {ex.Message}");
            }
        }

        private void EnsureClientId()
        {
            if (string.IsNullOrEmpty(this.Settings.ClientId))
            {
                this.Settings.ClientId = System.Guid.NewGuid().ToString();
            }
        }
    }
}