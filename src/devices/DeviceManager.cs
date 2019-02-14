using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using System.Linq;

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
            initListeners();
        }
        private void initListeners(){
            devices.Where((device) => device.type == DeviceType.ACTUATOR)
                   .ToList()
                   .ForEach((device) => this.AddListener(device as IConfiguredActuator));
        }

        public void AddListener(IEventListener listener)
        {
            devices.ForEach((currentDevice) =>
            {
                if(listener != null){
                    currentDevice.ValueChanged += listener.onEvent;
                }
            });
        }
        public void ReadSensors()
        {
            devices.ForEach(device =>
            {
                if(device.type == DeviceType.SENSOR) device.Act();
            });
        }
    }
}
