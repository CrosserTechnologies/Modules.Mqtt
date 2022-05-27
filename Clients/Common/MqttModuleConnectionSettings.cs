namespace Crosser.Modules.MqttClient.Common
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Security.Authentication;
    using System.Security.Cryptography.X509Certificates;
    using Crosser.EdgeNode.Common.Abstractions.Utilities.Validation;
    using Crosser.EdgeNode.Flows.Abstractions;
    using NJsonSchema.Annotations;

    public class MqttModuleConnectionSettings : FlowModuleSettings
    {
        [JsonSchemaExtensionData("x-sortOrder", 10)]
        [Display(Name = "URL", Description = "The URL of the MQTT broker to use")]
        [DefaultValue("")]
        [Required]
        [MinLength(1)]
        [MaxLength(256)]
        public string Url { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 11)]
        [Display(Name = "Port", Description = "Normally 1883/8883 depending of security")]
        [DefaultValue(1883)]
        [Range(1, 99999)]
        public int Port { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 12)]
        [Display(Name = "Use TLS")]
        [DefaultValue(false)]
        public bool UseTLS { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 13)]
        [Display(Name = "SSL Protocol", Description = "The SSL protocol to use")]
        [DefaultValue(SslProtocols.Tls12)]
        public SslProtocols SslProtocol { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 14)]
        [Display(Name = "Allow Untrusted Certificates", Description = "For example when the server has a self-signed certificate")]
        [DefaultValue(false)]
        public bool AllowUntrustedCertificates { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 15)]
        [Display(Name = "ClientId", Description = "The ClientId to use, if empty a random id will be generated and used.")]
        [DefaultValue("")]
        [MinLength(0)]
        [MaxLength(256)]
        public string ClientId { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 16)]
        [DisplayAttribute(Name = "CA Certificate")]
        [JsonSchemaExtensionDataAttribute(Credential.ATTRIBUTE, Credential.Types.Certificate)]
        public Guid? CaCertificateCredential { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 17)]
        [DisplayAttribute(Name = "Client Certificate")]
        [JsonSchemaExtensionDataAttribute(Credential.ATTRIBUTE, Credential.Types.Certificate)]
        public Guid? ClientCertificateCredential { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 18)]
        [DisplayAttribute(Name = "User Credential")]
        [JsonSchemaExtensionDataAttribute(Credential.ATTRIBUTE, Credential.Types.UsernameAndPassword)]
        public Guid? UsernamePasswordCredential { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 99)]
        [DisplayAttribute(Name = "Last Will Testament")]
        public LastWillTestament LastWillTestament { get; set; }

        public override void Validate(SettingsValidator validator)
        {
            validator.Validate(nameof(this.Url), this.Url).NotNull().MinLength(1).MaxLength(256);
            validator.Validate(nameof(this.Port), this.Port).MinValue(1).MaxValue(99999);
            validator.Validate(nameof(this.ClientId), this.ClientId).NotNull().MinLength(0).MaxLength(256);

            if (this.LastWillTestament.UseLastWillTestament)
            {
                validator.Validate(nameof(this.LastWillTestament.Topic), this.LastWillTestament.Topic).NotNull().MinLength(1).MaxLength(256);
                validator.Validate(nameof(this.LastWillTestament.Payload), this.LastWillTestament.Payload).MinLength(0).MaxLength(2048);
            }
        }

        public bool HasCaCertificate()
        {
            return this.CaCertificateCredential.HasValue;
        }
        public bool HasClientCertificate()
        {
            return this.ClientCertificateCredential.HasValue;
        }
        public bool HasUsernamePassword()
        {
            return this.UsernamePasswordCredential.HasValue;
        }

        public CredentialWithUsernamePassword GetUsernamePassword()
        {
            return this.Credentials[this.UsernamePasswordCredential.Value].ToCredential<CredentialWithUsernamePassword>();
        }

        public X509Certificate2 GetCaCertificate()
        {
            return this.Credentials[this.CaCertificateCredential.Value].ToCredential<CredentialWithCertificate>().X509Certificate2;
        }

        public X509Certificate2 GetClientCertificate()
        {
            return this.Credentials[this.ClientCertificateCredential.Value].ToCredential<CredentialWithCertificate>().X509Certificate2;
        }
    }
}