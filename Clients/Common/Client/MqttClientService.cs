namespace Crosser.EdgeNode.Modules.MqttClient.Common.Client
{
    using System.Collections.Generic;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using MQTTnet.Client.Connecting;
    using MQTTnet.Client.Options;
    using MQTTnet.Client;
    using MQTTnet.Protocol;
    using MQTTnet;
    using Crosser.EdgeNode.Common.Utilities.Logger;
    using System.Text;
    using Crosser.Modules.MqttClient.Common;

    public class MqttClientService : IMqttClientService
    {
        public MqttModuleConnectionSettings MqttModuleConnectionSettings { get; set; }
        public IMqttClient MqttClient { get; private set; }
        public EventHandler OnConnected { get; set; }
        public EventHandler OnDisconnected { get; set; }
        public EventHandler<MqttApplicationMessage> OnMessageReceived { get; set; }

        private readonly CancellationToken CancellationToken;
        private bool ShutDown = false;
        private readonly ILog log;

        public MqttClientService(ILog logger, MqttModuleConnectionSettings settings, CancellationToken cancellationToken)
        {
            this.CancellationToken = cancellationToken;
            this.MqttModuleConnectionSettings = settings;
            this.log = logger;

            var factory = new MqttFactory();
            this.MqttClient = factory.CreateMqttClient();

            this.MqttClient.UseDisconnectedHandler(async e =>
            {
                if (this.OnDisconnected != null)
                {
                    this.OnDisconnected.Invoke(this, EventArgs.Empty);
                }

                if (this.ShutDown)
                {
                    return;
                }

                await Task.Delay(TimeSpan.FromSeconds(5));

                if (await this.Connect())
                {
                    this.log.Debug($"MQTT client reconnected...");
                }
            });

            this.MqttClient.UseApplicationMessageReceivedHandler(e =>
            {
                if (this.OnMessageReceived != null)
                {
                    this.OnMessageReceived.Invoke(this, e.ApplicationMessage);
                }
            });
        }
        public async Task<bool> Connect()
        {
            try
            {
                var result = await this.MqttClient.ConnectAsync(this.CreateClientConfiguration(), this.CancellationToken);
                if (result.ResultCode == MqttClientConnectResultCode.Success)
                {
                    if (this.OnConnected != null)
                    {
                        this.OnConnected.Invoke(this, EventArgs.Empty);
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                this.log.Error(ex, $"MQTT client could not connect to broker: {ex.Message}");
                return false;
            }
        }

        public async Task Disconnect()
        {
            this.ShutDown = true;
            await this.MqttClient.DisconnectAsync();
        }

        public async Task Publish(string topic, byte[] msg, MqttQualityOfServiceLevel qos, bool retain)
        {
            var message = new MqttApplicationMessageBuilder()
                        .WithTopic(topic)
                        .WithPayload(msg)
                        .WithQualityOfServiceLevel(qos)
                        .WithRetainFlag(retain)
                        .Build();

            await this.MqttClient.PublishAsync(message);
        }

        public async Task Subscribe(string topic, MqttQualityOfServiceLevel qos)
        {
            await this.MqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic(topic).WithQualityOfServiceLevel(qos).Build());
        }

        public bool IsConnected()
        {
            return this.MqttClient.IsConnected;
        }

        private IMqttClientOptions CreateClientConfiguration()
        {
            var optionsBuilder = new MqttClientOptionsBuilder();
            optionsBuilder.WithClientId(this.MqttModuleConnectionSettings.ClientId);
            optionsBuilder.WithTcpServer(this.MqttModuleConnectionSettings.Url, this.MqttModuleConnectionSettings.Port);

            if (this.MqttModuleConnectionSettings.LastWillTestament.UseLastWillTestament)
            {
                var lastWIll = new MqttApplicationMessage()
                {
                    Topic = this.MqttModuleConnectionSettings.LastWillTestament.Topic,
                    Payload = Encoding.UTF8.GetBytes(this.MqttModuleConnectionSettings.LastWillTestament.Payload),
                    QualityOfServiceLevel = this.MqttModuleConnectionSettings.LastWillTestament.QualityOfServiceLevel,
                    Retain = this.MqttModuleConnectionSettings.LastWillTestament.Retain
                };
                optionsBuilder.WithWillMessage(lastWIll);
            }

            if (this.MqttModuleConnectionSettings.UseTLS)
            {
                optionsBuilder.WithTls(this.TlsOptionsBuilder());
            }
            if (this.MqttModuleConnectionSettings.HasUsernamePassword())
            {
                var userCredential = this.MqttModuleConnectionSettings.GetUsernamePassword();
                optionsBuilder.WithCredentials(userCredential.Username, userCredential.Password);
            }

            return optionsBuilder.Build();
        }

        private MqttClientOptionsBuilderTlsParameters TlsOptionsBuilder()
        {
            var tlsOptions = new MqttClientOptionsBuilderTlsParameters();
            if (this.MqttModuleConnectionSettings.UseTLS)
            {
                tlsOptions.UseTls = true;
                tlsOptions.SslProtocol = this.MqttModuleConnectionSettings.SslProtocol;
            }

            if (this.MqttModuleConnectionSettings.AllowUntrustedCertificates)
            {
                tlsOptions.AllowUntrustedCertificates = true;
                tlsOptions.IgnoreCertificateChainErrors = true;
                tlsOptions.IgnoreCertificateRevocationErrors = true;
                tlsOptions.CertificateValidationHandler = (MqttClientCertificateValidationCallbackContext ctx) =>
                {
                    return true;
                };
            }

            if (this.MqttModuleConnectionSettings.HasCaCertificate() || this.MqttModuleConnectionSettings.HasClientCertificate())
            {
                var certs = new List<X509Certificate>();
                if (this.MqttModuleConnectionSettings.HasCaCertificate())
                {
                    certs.Add(this.MqttModuleConnectionSettings.GetCaCertificate());
                }
                if (this.MqttModuleConnectionSettings.HasClientCertificate())
                {
                    certs.Add(this.MqttModuleConnectionSettings.GetClientCertificate());
                }
                tlsOptions.Certificates = certs;
            }

            return tlsOptions;
        }

    }
}