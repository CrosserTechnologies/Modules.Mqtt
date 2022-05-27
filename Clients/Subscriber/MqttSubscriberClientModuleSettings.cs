namespace Crosser.Modules.MqttClient.Subscriber
{
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using Crosser.EdgeNode.Common.Abstractions.Utilities.Validation;
    using Crosser.Modules.MqttClient.Common;
    using MQTTnet.Protocol;
    using Newtonsoft.Json;
    using NJsonSchema.Annotations;

    public class MqttSubscriberClientModuleSettings : MqttModuleConnectionSettings
    {
        [JsonSchemaExtensionData("x-sortOrder", 0)]
        [Display(Name = "Topic", Description = "MQTT Topic, wildcard is allowed. Wildcards: <code>'+' is single level, '#' is multi level</code>")]
        [DefaultValue("")]
        [Required]
        [MinLength(1)]
        [MaxLength(256)]
        public string Topic { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 1)]
        [Display(Name = "Target Property", Description = "The property that will contain the result")]
        [DefaultValue("data")]
        [Required]
        [MinLength(1)]
        [MaxLength(64)]
        public string TargetProperty { get; set; }

        // x-sortOrder for Settings from MqttModuleConnectionSettings are between 10 and 19
        [JsonSchemaExtensionData("x-sortOrder", 20)]
        [Display(Name = "QoS", Description = "Quality of Service")]
        [DefaultValue(MqttQualityOfServiceLevel.AtMostOnce)]
        public MqttQualityOfServiceLevel QoS { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 21)]
        [Display(Name = "Output Format", Description = "How to format the data on the payload")]
        [DefaultValue(DataFormat.JSON)]
        public DataFormat DataFormat { get; set; }

        public override void Validate(SettingsValidator validator)
        {
            validator.Validate(nameof(this.Topic), this.Topic).NotNull().MinLength(1).MaxLength(256);
            validator.Validate(nameof(this.TargetProperty), this.TargetProperty).NotNull().MinLength(1).MaxLength(64);

            base.Validate(validator);
        }

    }
}