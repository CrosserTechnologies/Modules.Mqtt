# MQTT Client Subscriber

The module is used to `subscribe` to MQTT messages from an external MQTT broker.

This module is designed to be located at the start of a flow.

## Settings

|                              |                                              |                                                                                      |               |
| ---------------------------- | -------------------------------------------- | ------------------------------------------------------------------------------------ | ------------- |
| **Name**                     | **Requirements**                             | **Purpose**                                                                          | **Default**   |
| Topic\*                      | Length: 1-256                                | The topic to subscribe to.                                                           |               |
| Target Property              | Length: 1-64                                 | The property that will contain the result                                            | data          |
| URL                          | Length: 1-256                                | The MQTT broker to use.                                                              |               |
| Port                         |                                              | The port to use on the broker.                                                       | 1883          |
| Use TLS                      |                                              |                                                                                      | false         |
| SSL Protocol                 | The SSL protocol to use.                     |                                                                                      | Tls12         |
| Allow Untrusted Certificates |                                              | For example when the server has a self-signed certificate.                           | false         |
| ClientId                     | Length: 0-256                                | If this is empty a random id will be generated and used                              |               |
| QoS                          |                                              | At least once, At most once, Exactly once                                            | At least once |
| Output Format                | `Raw` (no formatting) <br>`Format from JSON` | Will convert the `data` property on the input to an object from the specified format | `Json`        |
| Use LWT                      | Boolean                                      | Check to use last will testament                                                     | false         |
| LWT Topic                    | Length: 0-256                                | The last will topic                                                                  |               |
| LWT Payload                  | Length: 0-2048                               | The last will payload                                                                |               |
| LWT QoS                      |                                              | The last will quality of service                                                     | AtLeastOnce   |
| LWT Retain                   |                                              | Check to retain the last will testament                                              | false         |

The `Topic`setting accepts MQTT wildcards:

`+` is single-level

`#` is multi-level

### Credential

This module contains an option to select credentials to use in the module. All credentials supported by the module are presented in a drop-down.

## Input

There is no input for this module

## Output

The output is a MQTT message with properties specified below.

<table>
  <tr>
   <td><strong>Name</strong>
   </td>
   <td><strong>Requirements</strong>
   </td>
   <td><strong>Purpose</strong>
   </td>
   <td><strong>Default</strong>
   </td>
  </tr>
  <tr>
   <td>data
   </td>
   <td>
   </td>
   <td>The actual MQTT message represented as a byte[]
   </td>
   <td>
   </td>
  </tr>
  <tr>
   <td>topic
   </td>
   <td>
   </td>
   <td>The topic for the published message
   </td>
   <td>
   </td>
  </tr>
  <tr>
   <td>retain
   </td>
   <td>
   </td>
   <td>Flag for knowing if the message is retained
   </td>
   <td>
   </td>
  </tr>
  <tr>
   <td>qos
   </td>
   <td>
   </td>
   <td>The quality of service for the message
   </td>
   <td>
   </td>
  </tr>
  <tr>
   <td>dupFlag
   </td>
   <td>
   </td>
   <td>True if the message has been sent before
   </td>
   <td>
   </td>
</table>
