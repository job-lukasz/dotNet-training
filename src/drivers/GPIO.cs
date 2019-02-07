using System;
namespace rpi_dotnet
{
    public class GPIO
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private static readonly string gpioPath = "/sys/class/gpio";

        IFileWrapper file;
        string pinAddress;
        public enum Direction
        {
            IN, OUT
        }

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
                    file.Write($"{gpioPath}/gpio{pinAddress}/value", value.ToString());
                }
            }
            catch (System.Exception err)
            {
                log.Error(err);
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
                    return file.Read($"{gpioPath}/gpio{pinAddress}/value") == "1";
                }
                return false;
            }
            catch (System.Exception err)
            {
                log.Error(err);
                return false;
            }
        }

        private bool setDirection(Direction direction)
        {
            log.Debug($"Try to set direction: {direction} on pin: {pinAddress}");
            try
            {
                file.Write($"{gpioPath}/gpio{pinAddress}/direction", direction.ToString());
            }
            catch (System.Exception err)
            {
                log.Error(err);
                return false;
            }
            return true;
        }

        private bool exportPin()
        {
            log.Debug($"Try to export pin: {pinAddress}");
            try
            {
                file.Write($"{gpioPath}/export", pinAddress);
            }
            catch (System.Exception err)
            {
                log.Error(err);
                return false;
            }
            return true;
        }
    }
}
