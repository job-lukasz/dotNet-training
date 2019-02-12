using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace rpi_dotnet
{
    public class DeviceManager //TODO add UT
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<IConfiguredDevice> devices;
        public DeviceManager(List<IConfiguredDevice> configuration)
        {
            devices = configuration;
            InitDevices();
        }

        private void InitDevices()
        {
            devices.ForEach((currentDevice) =>
            {
                currentDevice.FindDevice();
            });
        }

        public void AddListener(IEventListener listener)
        {
            devices.ForEach((currentDevice) =>
            {
                currentDevice.ValueChanged += listener.onEvent;
            });
        }
        public void ReadSensors()
        {
            devices.ForEach(device =>
            {
                device.Measure();
            });
        }
    }
}