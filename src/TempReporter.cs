using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace rpi_dotnet
{
    public class TempReporter
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private Dictionary<string, string> config;
        private InfluxClient client;
        private Dictionary<string, DS18B20> tempSensors = new Dictionary<string, DS18B20>();
        public TempReporter(Dictionary<string, string> configuration, OneWire w1Bus = null, InfluxClient influxClient = null)
        {
            config = configuration;
            client = influxClient ?? new InfluxClient("home-server.local", "homeTest"); //TODO: move this hardcoded data
            var w1bus = new OneWire();
            var devices = w1bus.getDevices();
            devices.ForEach((device) =>
            {
                if (device is DS18B20) tempSensors.Add(device.deviceID, (DS18B20)device);
            });
        }

        public async Task<bool> Report()
        {
            var temperatures = ReadTemp();
            var success = true;
            foreach (KeyValuePair<string, string> configuredDevice in config)
            {
                if (temperatures.ContainsKey(configuredDevice.Key))
                {
                    var spaceId = configuredDevice.Value;
                    try
                    {
                        var successfull = await client.addMeasure("temperature", spaceId, temperatures[configuredDevice.Key]);
                        if(!successfull){
                            success = false;
                            log.Warn($"Something went wrong during sent data to DB. Device: {configuredDevice.Key}");
                        }
                    }
                    catch (Exception err)
                    {
                        log.Error($"Error occure during sent data to DB. Device: {configuredDevice.Key}");
                        log.Error(err);
                    }
                }
            }
            return success;
        }
        private Dictionary<string, float> ReadTemp()
        {
            var temperatures = new Dictionary<string, float>();
            foreach (KeyValuePair<string, string> configuredDevice in config)
            {
                if (tempSensors.ContainsKey(configuredDevice.Key))
                {
                    try
                    {
                        temperatures.Add(configuredDevice.Key, tempSensors[configuredDevice.Key].Measure());
                    }
                    catch (Exception err)
                    {
                        log.Error($"Exception during reading value for device: {configuredDevice.Key}");
                    }
                }
                else
                {
                    log.Warn($"Device {configuredDevice.Key} from config is not connected");
                }
            }
            return temperatures;
        }
    }
}