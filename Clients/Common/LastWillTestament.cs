namespace Crosser.Modules.MqttClient.Common
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using MQTTnet.Protocol;
    using NJsonSchema.Annotations;

    public class LastWillTestament
    {
        [JsonSchemaExtensionData("x-sortOrder", 0)]
        [Display(Name = "Use LWT", Description = "Check to use last will testament")]
        [DefaultValue(false)]
        public bool UseLastWillTestament { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 1)]
        [Display(Name = "Topic", Description = "The last will topic")]
        [DefaultValue("")]
        [MinLength(0)]
        [MaxLength(256)]
        public string Topic { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 2)]
        [Display(Name = "Payload", Description = "The last will payload")]
        [DefaultValue("")]
        [MinLength(0)]
        [MaxLength(2048)]
        public string Payload { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 3)]
        [Display(Name = "QoS", Description = "The last will quality of service")]
        [DefaultValue(MqttQualityOfServiceLevel.AtLeastOnce)]
        public MqttQualityOfServiceLevel QualityOfServiceLevel { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 4)]
        [Display(Name = "Retain", Description = "Check to retain the last will testament")]
        [DefaultValue(false)]
        public bool Retain { get; set; }
    }
}