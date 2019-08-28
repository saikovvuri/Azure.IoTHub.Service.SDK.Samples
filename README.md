# Azure IoT Hub Service SDK Samples

Purpose is to demonstrate using Service SDK to

- Apply configuration (deployment manifest) to an IoT Edge Device
  
- Query previously applied configuration for success/failures

## Environment Settings

Environment Settings act differently depending upon the IDE that is being used.

_VSCode_
The best method to work with environment settings here for command line is to put environment settings in a .env file and reference it with the vscode `launch.json` file.

IOTHUB_CONNECTIONSTRING="connectionstring"
DEVICE="deviceid"

If using Visual Studio then the environment variables are pulled out of the `Properties/launchSettings.json`
