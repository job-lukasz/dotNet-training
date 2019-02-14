using System;
namespace rpi_dotnet
{
    class InfluxListener: IEventListener
    {
        private InfluxClient client;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public InfluxListener(InfluxClient influxClient)
        {
            client = influxClient;

        }
        public async void onEvent(object sender, MeasuredValueChange value)
        {
            try
            {
                {
                    var successfull = await client.addMeasure(value.MeasureName, value.SpaceID, value.NewValue);
                    if (!successfull)
                    {
                        log.Warn($"Something went wrong during sent data to DB. Device: {value.DeviceID}");
                    }
                }
            }
            catch (Exception err)
            {
                log.Error($"Error occure during sent data to DB. Device: {value.DeviceID}");
                log.Error(err);
            }
        }
    }
}
