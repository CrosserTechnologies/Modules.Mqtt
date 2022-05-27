namespace Crosser.Modules.MqttClient.Publisher
{

    using Crosser.EdgeNode.Common.Abstractions.Utilities.Validation;
    using Crosser.Modules.MqttClient.Common;
    using MQTTnet.Protocol;
    using Newtonsoft.Json;
    using NJsonSchema.Annotations;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;

    public class MqttPublisherClientModuleSettings : MqttModuleConnectionSettings
    {
        [JsonSchemaExtensionData("x-sortOrder", 0)]
        [Display(Name = "Source Property", Description = "The property that contains the MQTT message to send")]
        [DefaultValue("data")]
        [Required]
        [MinLength(1)]
        [MaxLength(256)]
        public string SourceProperty { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 1)]
        [Display(Name = "Topic", Description = "MQTT Topic, wildcard is allowed. Wildcards: <code>'+' is single level, '#' is multi level</code>")]
        [DefaultValue("")]
        [MinLength(0)]
        [MaxLength(256)]
        public string Topic { get; set; }

        // x-sortOrder for Settings from MqttModuleConnectionSettings are between 10 and 19
        [JsonSchemaExtensionData("x-sortOrder", 20)]
        [Display(Name = "QoS", Description = "Quality of Service")]
        [DefaultValue(MqttQualityOfServiceLevel.AtMostOnce)]
        public MqttQualityOfServiceLevel QoS { get; set; }

        [JsonSchemaExtensionData("x-sortOrder", 21)]
        [Display(Name = "Check to `retain` message")]
        [DefaultValue(false)]
        public bool Retain { get; set; }


        [JsonSchemaExtensionData("x-sortOrder", 22)]
        [Display(Name = "Message Format", Description = "If you pass an object in the `message` property it can be formated to JSON.<br/>Note that this setting is ignored if you pass string or byte[] as message.")]
        [DefaultValue(DataFormat.JSON)]
        public DataFormat DataFormat { get; set; }

        public override void Validate(SettingsValidator validator)
        {
            validator.Validate(nameof(this.Topic), this.Topic).NotNull().MinLength(0).MaxLength(256);
            validator.Validate(nameof(this.SourceProperty), this.SourceProperty).NotNull().MinLength(1).MaxLength(256);

            if (this.Topic.Contains("#") || this.Topic.Contains("+"))
            {
                validator.AddError(nameof(this.Topic), "Topic cant contain '#' or '+'");
            }
            base.Validate(validator);
        }

    }
}