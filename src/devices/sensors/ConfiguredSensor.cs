using System;
namespace rpi_dotnet
{
    public abstract class ConfiguredSensor : IConfiguredDevice
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public event EventHandler<MeasuredValueChange> ValueChanged;
        public readonly string spaceID;
        public DeviceType type { private set; get; }
        protected ISensorDevice sensor;
        private string measureName;
        public ConfiguredSensor(string spaceID, string measureName)
        {
            this.spaceID = spaceID;
            this.measureName = measureName;
            this.type = DeviceType.SENSOR;
        }
        public bool Act()
        {
            if (sensor != null)
            {
                var oldValue = sensor.lastValue;
                try
                {
                    var newValue = sensor.GetValue();
                    if (newValue != oldValue)
                    {
                        OnValueChanged(new MeasuredValueChange(measureName, newValue, spaceID, sensor.deviceID));
                        return true;
                    }
                }
                catch (Exception err)
                {
                    log.Error($"Something wnet wrong during get value from device. Address: {sensor.deviceID}");
                    log.Error(err);
                }
            }
            return false;
        }

        public abstract bool FindDevice();
        private void OnValueChanged(MeasuredValueChange value)
        {
            ValueChanged?.Invoke(this, value);
        }
    }
}