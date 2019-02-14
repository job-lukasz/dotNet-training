using System;
namespace rpi_dotnet
{
    public class GPIO
    {
        public enum Direction
        {
            IN, OUT, Undefined
        }

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
#if DEBUG
        private static readonly string gpioPath = "./sys/class/gpio";
#else
        private static readonly string gpioPath = "/sys/class/gpio";
#endif
        private Direction direction = Direction.Undefined;
        private bool isExported = false;
        IFileWrapper file;
        public string pinAddress { private set; get; }
        public bool? lastValue { get; private set; }
        public GPIO(string address, IFileWrapper fileWrapper = null)
        {
            file = fileWrapper ?? new FileWrapper();
            pinAddress = address;
        }

        public bool setValue(bool value)
        {
            log.Debug($"Try to set value: {value} on pin: {pinAddress}");
            try
            {
                if (exportPin() && setDirection(Direction.OUT))
                {
                    file.Write($"{gpioPath}/gpio{pinAddress}/value", value ? "1" : "0");
                    lastValue = value;
                }
            }
            catch (System.Exception err)
            {
                log.Error($"Unalble to set value: {value} on pin: {pinAddress}");
                log.Error(err);
                lastValue = null;
                return false;
            }
            return true;
        }

        public bool getValue()
        {
            log.Debug($"Try to get value from: {gpioPath}/gpio{pinAddress}/value");
            try
            {
                if (exportPin() && setDirection(Direction.IN))
                {
                    lastValue = file.Read($"{gpioPath}/gpio{pinAddress}/value") == "1";
                    return (bool)lastValue;
                }
                return false;
            }
            catch (System.Exception err)
            {
                log.Error($"Unalble to get value from pin: {pinAddress}");
                log.Error(err);
                return false;
            }
        }

        public bool setDirection(Direction direction)
        {
            if (this.direction != direction)
            {
                log.Debug($"Try to set direction: {direction} on pin: {pinAddress}");
                try
                {
                    file.Write($"{gpioPath}/gpio{pinAddress}/direction", direction.ToString());
                    this.direction = direction;
                }
                catch (System.Exception err)
                {
                    log.Error($"Unalble to set direction: {direction} on pin: {pinAddress}");
                    log.Error(err);
                    return false;
                }
            }
            return true;
        }

        private bool exportPin()
        {
            if(!isExported){
                log.Debug($"Try to export pin: {pinAddress}");
                try
                {
                    file.Write($"{gpioPath}/export", pinAddress);
                    isExported = true;
                }
                catch (System.Exception err)
                {
                    log.Error($"Unalble to export pin: {pinAddress}");
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
