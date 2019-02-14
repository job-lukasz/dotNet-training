using System;
namespace rpi_dotnet
{
    public class ConfiguredTempSensor : IConfiguredDevice
    {
        public readonly string deviceID;
        public readonly string spaceID;
        public event EventHandler<MeasuredValueChange> ValueChanged;
        private IOneWireDevice sensor;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private OneWire w1bus;

        public DeviceType type {private set; get;}
        public ConfiguredTempSensor(string deviceId, string spaceId, OneWire OneWireBus = null)
        {
            this.type = DeviceType.SENSOR;
            this.deviceID = deviceId;
            this.spaceID = spaceId;
            w1bus = OneWireBus ?? new OneWire();
        }
        public bool FindDevice()
        {
            var foundedDevice = w1bus.getDevice(deviceID);
            if (foundedDevice != null && foundedDevice is DS18B20)
            {
                sensor = (DS18B20)foundedDevice;
                log.Info($"Sensor found for device: {deviceID}");
                return true;
            }
            else
            {
                sensor = null;
                log.Warn($"Sensor not found for device: {deviceID}");
            }
            return false;
        }
        public bool Act()
        {
            if (sensor != null)
            {
                var oldValue = sensor.lastValue;
                try
                {
                    var newValue = sensor.Measure();
                    if (sensor.lastValue != oldValue)
                    {
                        OnValueChanged(new MeasuredValueChange("heatWaterTemp", newValue, spaceID, deviceID));
                        return true;
                    }
                }
                catch (Exception err)
                {
                    log.Error($"Something wnet wrong during measure. Device : {sensor.deviceID}");
                    log.Error(err);
                }
            }
            return false;
        }

        private void OnValueChanged(MeasuredValueChange value)
        {
            ValueChanged?.Invoke(this, value);
        }
    }
}
