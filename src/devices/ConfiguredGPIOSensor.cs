using System;
namespace rpi_dotnet
{
    public class ConfiguredGPIOSensor : IConfiguredDevice
    {
        public readonly string spaceID;
        public event EventHandler<MeasuredValueChange> ValueChanged;
        private GPIO sensor;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public DeviceType type {private set; get;}
        public ConfiguredGPIOSensor(string pinAddress, string spaceId)
        {   
            this.type = DeviceType.SENSOR;
            this.sensor = new GPIO(pinAddress);
            this.spaceID = spaceId;
        }
        public bool FindDevice()
        {
            try
            {
                return this.sensor.setDirection(GPIO.Direction.IN);
            }
            catch (Exception err)
            {
                log.Error($"Error occure during set Pin direction for: {sensor.pinAddress} ");
                log.Error(err);
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
                    var newValue = sensor.getValue();
                    if (sensor.lastValue != oldValue)
                    {
                        var value = newValue ? 1 : 0;
                        OnValueChanged(new MeasuredValueChange("indoorSensor", value, spaceID, sensor.pinAddress));
                        return true;
                    }
                }
                catch (Exception err)
                {
                    log.Error($"Something wnet wrong during get value from gpio. Address: {sensor.pinAddress}");
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
