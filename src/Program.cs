using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using CommandLine;

namespace rpi_dotnet
{

    class Program
    {
        public static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly List<IConfiguredDevice> tempDeviceConfigurations = new List<IConfiguredDevice>{
            new ConfiguredTempSensor("28-000006cc02c9","salon"),
            new ConfiguredTempSensor("28-000006cc00ce","korytarz"),
            new ConfiguredGPIOSensor("23", "salon"),
            new ConfiguredPompActuator("44", "salon")
        };
        //TODO: should be configured from iotHUB

        static void Main(string[] args)
        {
            Options options;
            CommandLine.Parser.Default.ParseArguments<Options>(args)
            .WithParsed<Options>(opts => options = opts)
            .WithNotParsed<Options>((errs) => HandleParseError(errs));

            var deviceManager = new DeviceManager(tempDeviceConfigurations);
            var influxListener = new InfluxListener(new InfluxClient("http://home-server.local:8086", "homeTest"));
            deviceManager.AddListener(influxListener);
            log.Info("Start main program loop");
            while (true)
            {
                log.Info("Start loop iteration");
                deviceManager.ReadSensors();
                Thread.Sleep(10 * 1000);
            }
        }
        static void HandleParseError(IEnumerable<Error> errs)
        {
            if (errs.Any())
            {
                log.Error(errs);
            }
        }
    }
}

//TECHNICAL DEBT
//TODO: Error handling - more precise
//TODO: Parameters
//TODO: Remove hardcoded values
