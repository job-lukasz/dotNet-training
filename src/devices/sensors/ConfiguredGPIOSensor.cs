using System;
namespace rpi_dotnet
{
    public class ConfiguredGPIOSensor : ConfiguredSensor
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public ConfiguredGPIOSensor(string pinAddress, string spaceId) : base(spaceId, "indoorSensor")
        {
            this.sensor = new GPIO(pinAddress);
        }
        public ConfiguredGPIOSensor(IGPIODevice sensor, string spaceId) : base(spaceId, "indoorSensor")
        {
            this.sensor = sensor;
        }
        public override bool FindDevice()
        {
            try
            {
                return (this.sensor as IGPIODevice).SetDirection(Direction.IN);
            }
            catch (Exception err)
            {
                log.Error($"Error occure during set Pin direction for: {sensor.deviceID} ");
                log.Error(err);
                this.sensor = null;
            }
            return false;
        }

    }
}
