# Modules.Mqtt

## Crosser EdgeNode MQTT Modules

This repository contains Crosser Modules for MQTT communication.
Currently there are two modules

- MQTT Client - Subscriber
- MQTT Client - Publisher

These can be used in https://cloud.crosser.io to get data into flows (subscriber) or to send data to MQTT brokers (publisher)

## Build/Pack Subscriber

Navigate to `Clients/Subscriber`

Build: `dotnet build -c release`
Pack: `dotnet pack -c release`

## Build/Pack Publisher

Navigate to `Clients/Subscriber`

Build: `dotnet build -c release`
Pack: `dotnet pack -c release`

## Registering on Nuget

If you want to register custom versions of these modules on Nuget to be able to register them in your Crosser account at https://cloud.crosser.io you
will need to change the name of the module, but it is also recomened to change the namespace to align it with the name of your project/company.
