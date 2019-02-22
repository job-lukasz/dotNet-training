using System;
namespace rpi_dotnet
{
    public class ConfiguredTempSensor : ConfiguredSensor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private OneWire w1bus;
        private string deviceUUID;
        public ConfiguredTempSensor(string deviceId, string spaceId, OneWire OneWireBus = null) : base(spaceId, "heatWaterTemp")
        {
            deviceUUID = deviceId;
            w1bus = OneWireBus ?? new OneWire();
        }
        public override bool FindDevice()
        {
            var foundedDevice = w1bus.getDevice(deviceUUID);
            if (foundedDevice != null && foundedDevice is DS18B20)
            {
                sensor = (DS18B20)foundedDevice;
                log.Info($"Sensor found for device: {sensor.deviceID}");
                return true;
            }
            else
            {
                sensor = null;
                log.Warn($"Sensor not found for device: {sensor.deviceID}");
            }
            return false;
        }
    }
}
