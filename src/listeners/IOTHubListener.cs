using System;
namespace rpi_dotnet
{
    class IOTHubListener : IEventListener
    {
        private IotHubClient client;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public IOTHubListener(IotHubClient iotHubClient)
        {
            client = iotHubClient;

        }
        public async void onEvent(object sender, MeasuredValueChange value)
        {
            try
            {
                {
                    await client.addMeasure(value.MeasureName, value.SpaceID, value.NewValue);
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
