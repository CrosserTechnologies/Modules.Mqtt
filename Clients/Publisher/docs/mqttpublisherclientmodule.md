# MQTT Client Publisher

The module is used to `publish` messages to external MQTT brokers.

This module is designed to be at the end of a flow.

## Settings

|                              |                          |                                                                                          |               |
| ---------------------------- | ------------------------ | ---------------------------------------------------------------------------------------- | ------------- |
| **Name**                     | **Requirements**         | **Purpose**                                                                              | **Default**   |
| Source Property              | Length: 1-256            | The property that contains the MQTT message to send                                      | message       |
| Topic\*                      | Length: 0-256            | The topic to use in the message                                                          |               |
| URL                          | Length: 1-256            | The MQTT broker to use.                                                                  |               |
| Port                         |                          | The port to use on the broker.                                                           | 1883          |
| Use TLS                      |                          |                                                                                          | false         |
| SSL Protocol                 | The SSL protocol to use. |                                                                                          | Tls12         |
| Allow Untrusted Certificates |                          | For example when the server has a self-signed certificate.                               | false         |
| ClientId                     | Length: 0-256            | If this is empty a random id will be generated and used                                  |               |
| QoS                          |                          | At least once, At most once, Exactly once                                                | At least once |
| Message Format               | `RAW`, `JSON`            | If an object is passed in this sets the text-format to use when sending the MQTT message | JSON          |
| Use LWT                      | Boolean                  | Check to use last will testament                                                         | false         |
| LWT Topic                    | Length: 0-256            | The last will topic                                                                      |               |
| LWT Payload                  | Length: 0-2048           | The last will payload                                                                    |               |
| LWT QoS                      |                          | The last will quality of service                                                         | AtLeastOnce   |
| LWT Retain                   |                          | Check to retain the last will testament                                                  | false         |

### Credential

This module contains an option to select credentials to use in the module. All credentials supported by the module are presented in a drop-down.

## Input

All settings marked with a `*` can be passed in on the incoming message. So for example we can set the `topic` by passing in a message like this.

`{topic:'foo/bar'}`

Note that the value in settings has higher priority than the value in the message. If a topic is neither set on the module settings nor in the incomming message an error will be set on the module.

## Output

The module will, in case of success, send a message as output containing the topic used, the message sent and a `crosser.success` property. In case of an error, any information about the error will be sent as output message.
