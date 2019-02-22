using System;
namespace rpi_dotnet
{
    public class ConfiguredPompActuator : IConfiguredActuator
    {
        public readonly string spaceID;
        public event EventHandler<MeasuredValueChange> ValueChanged;
        private GPIO actuator;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public DeviceType type { private set; get; }
        public ConfiguredPompActuator(string pinAddress, string spaceId)
        {
            this.type = DeviceType.ACTUATOR;
            this.actuator = new GPIO(pinAddress);
            this.spaceID = spaceId;
        }
        public bool FindDevice()
        {
            return actuator.SetValue(false);
        }
        public void onEvent(object sender, MeasuredValueChange value)
        {
            if (value.MeasureName == "indoorSensor")
            {
                if (value.SpaceID == this.spaceID)
                {
                    setState(value.NewValue == 1);
                }
            }
        }
        public bool Act()
        {
            if (actuator.lastValue != null)
            {
                OnValueChanged(new MeasuredValueChange("pompWorks", (float)actuator.lastValue, spaceID, actuator.deviceID));
                return true;
            }
            return false;
        }

        private void setState(bool shouldWork)
        {
            if (actuator.lastValue != (shouldWork ? 1 : 0))
            {
                actuator.SetValue(shouldWork);
                Act();
            }
        }

        private void OnValueChanged(MeasuredValueChange value)
        {
            ValueChanged?.Invoke(this, value);
        }
    }
}
