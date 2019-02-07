using System.Collections.Generic;
using System;
using System.Threading.Tasks;

namespace rpi_dotnet
{
    public class TempReporter //TODO add UT
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private List<DeviceConfiguration> configuredDevices;
        private InfluxClient client;
        public TempReporter(List<DeviceConfiguration> configuration, OneWire w1Bus = null, InfluxClient influxClient = null)
        {
            configuredDevices = configuration;
            client = influxClient ?? new InfluxClient("home-server.local", "homeTest"); //TODO: move this hardcoded data
            initDevices();
        }

        public bool Report()
        {
            var success = true;
            if (ReadTemp()) //report only when all devices readed succesfully
            {
                configuredDevices.ForEach(async (device) =>
                {
                    try
                    {
                        var successfull = await client.addMeasure("temperature", device.spaceID, device.sensor.lastMeasure);
                        if (!successfull)
                        {
                            success = false;
                            log.Warn($"Something went wrong during sent data to DB. Device: {device.deviceID}");
                        }
                    }
                    catch (Exception err)
                    {
                        success = false;
                        log.Error($"Error occure during sent data to DB. Device: {device.deviceID}");
                        log.Error(err);
                    }
                });
            }
            return success;
        }
        private void initDevices()
        {
            var w1bus = new OneWire();
            var devices = w1bus.getDevices();
            configuredDevices.ForEach((currentDevice) =>
            {
                var foundedDevice = w1bus.getDevice(currentDevice.deviceID);
                if (foundedDevice != null && foundedDevice is DS18B20)
                {
                    currentDevice.sensor = (DS18B20)foundedDevice;
                    log.Info($"Sensor found for device: {currentDevice.deviceID}");
                }
                else
                {
                    currentDevice.sensor = null;
                    log.Warn($"Sensor not found for device: {currentDevice.deviceID}");
                }
            });
        }
        private bool ReadTemp()
        {
            var success = true;
            configuredDevices.ForEach(device =>
            {
                try
                {
                    device.sensor.Measure();
                }
                catch (Exception err)
                {
                    log.Error($"Exception during reading value for device: {device.deviceID}");
                    log.Error(err);
                    success = false;
                }
            });
            return success;
        }
    }
}