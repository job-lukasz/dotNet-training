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
            client = influxClient ?? new InfluxClient("http://home-server.local:8086", "homeTest"); //TODO: move this hardcoded data
            initDevices();
        }

        public bool Report()
        {
            var success = true;
            if (ReadTemp()) //report only when all found devices read succesfully
            {
                configuredDevices.ForEach(async (device) =>
                {
                    try
                    {
                        if (device.sensor != null && device.sensor.lastMeasure != null)
                        {
                            var successfull = await client.addMeasure("temperature", device.spaceID, (double)device.sensor.lastMeasure);
                            if (!successfull)
                            {
                                success = false;
                                log.Warn($"Something went wrong during sent data to DB. Device: {device.deviceID}");
                            }
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
            var isSuccess = true;
            configuredDevices.ForEach(device =>
            {
                try
                {
                    if (device.sensor != null) device.sensor.Measure();
                }
                catch (Exception err)
                {
                    log.Error($"Exception during reading value for device: {device.deviceID}");
                    log.Error(err);
                    isSuccess = false;
                }
            });
            return isSuccess;
        }
    }
}