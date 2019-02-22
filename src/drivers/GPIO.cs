using System;
namespace rpi_dotnet
{
    public class GPIO : IGPIODevice
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#if DEBUG
        private static readonly string gpioPath = "./sys/class/gpio";
#else
        private static readonly string gpioPath = "/sys/class/gpio";
#endif
        private Direction direction = Direction.Undefined;
        private bool isExported = false;
        IFileWrapper file;
        public string deviceID { private set; get; }
        public float? lastValue { get; private set; }
        public GPIO(string address, IFileWrapper fileWrapper = null)
        {
            file = fileWrapper ?? new FileWrapper();
            deviceID = address;
        }

        public bool SetValue(bool value)
        {
            log.Debug($"Try to set value: {value} on pin: {deviceID}");
            try
            {
                if (exportPin() && SetDirection(Direction.OUT))
                {
                    file.Write($"{gpioPath}/gpio{deviceID}/value", value ? "1" : "0");
                    lastValue = value ? 1 : 0;
                }
            }
            catch (System.Exception err)
            {
                log.Error($"Unalble to set value: {value} on pin: {deviceID}");
                log.Error(err);
                lastValue = null;
                return false;
            }
            return true;
        }

        public float GetValue()
        {
            log.Debug($"Try to get value from: {gpioPath}/gpio{deviceID}/value");
            try
            {
                if (exportPin() && SetDirection(Direction.IN))
                {
                    lastValue = file.Read($"{gpioPath}/gpio{deviceID}/value") == "1" ? 1 : 0;
                    return (float)lastValue;
                }
                return 0;
            }
            catch (System.Exception err)
            {
                log.Error($"Unalble to get value from pin: {deviceID}");
                log.Error(err);
                return 0;
            }
        }

        public bool SetDirection(Direction direction)
        {
            if (this.direction != direction)
            {
                log.Debug($"Try to set direction: {direction} on pin: {deviceID}");
                try
                {
                    file.Write($"{gpioPath}/gpio{deviceID}/direction", direction.ToString());
                    this.direction = direction;
                }
                catch (System.Exception err)
                {
                    log.Error($"Unalble to set direction: {direction} on pin: {deviceID}");
                    log.Error(err);
                    return false;
                }
            }
            return true;
        }

        private bool exportPin()
        {
            if (!isExported)
            {
                log.Debug($"Try to export pin: {deviceID}");
                try
                {
                    file.Write($"{gpioPath}/export", deviceID);
                    isExported = true;
                }
                catch (System.Exception err)
                {
                    log.Error($"Unalble to export pin: {deviceID}");
                    log.Error(err);
                    isExported = false;
                    return false;
                }
                return true;
            }
            return true;
        }
    }
}
