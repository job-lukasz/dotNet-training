using System.Collections.Generic;
using System.Linq;

namespace rpi_dotnet
{
    public class OneWire
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#if DEBUG
        public static readonly string path = "./sys/bus/w1/devices/";
#else
        public static readonly string path = "/sys/bus/w1/devices/";
#endif
        private IFileWrapper file;
        public OneWire(IFileWrapper fileWrapper = null)
        {
            file = fileWrapper ?? new FileWrapper();
        }

        public List<IOneWireDevice> getDevices()
        {
            List<IOneWireDevice> devices = new List<IOneWireDevice>();
            var devicesNames = file.ListDirectories(path).Where((value) => !value.StartsWith("w1")).ToList();
            devicesNames.ForEach((deviceName) =>
            {
                log.Debug($"Found new device: {deviceName}");
                if (deviceName.StartsWith("28"))
                {
                    log.Debug($"Device: {deviceName} is DS18B20 sensor");
                    devices.Add(new DS18B20(deviceName));
                }
            });
            return devices;
        }

        public IOneWireDevice getDevice(string deviceID)
        {
            var devices = getDevices();
            return devices.Find((device) => device.deviceID == deviceID);
        }
    }
}
