using System;
using System.Collections.Generic;
using System.Threading;
namespace rpi_dotnet
{

    class Program
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly List<DeviceConfiguration> tempDeviceConfigurations = new List<DeviceConfiguration>{
            new DeviceConfiguration("28-000006cc02c9","salon"),
            new DeviceConfiguration("28-000006cc00ce","korytarz")
        }; //TODO: should be configured from iotHUB

        static void Main(string[] args)
        {

            var temperatureReporter = new TempReporter(tempDeviceConfigurations);

            log.Info("Start main program loop");
            while (true)
            {
                log.Info("Start loop iteration");
                temperatureReporter.Report();
                Thread.Sleep(10 * 1000);
            }
        }
    }
}

//TECHNICAL DEBT
//TODO: Error handling - more precise
//TODO: Parameters
//TODO: Remove hardcoded values
