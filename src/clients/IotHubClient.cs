using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
namespace rpi_dotnet
{
    public class IotHubClient
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private DeviceClient client;
        public IotHubClient(string connectionString)
        {
            client = DeviceClient.CreateFromConnectionString(connectionString, TransportType.Mqtt);
        }
        public async Task<bool> addMeasure(string measureName, string spaceID, double value)
        {
            var JSON = new
            {
                spaceID,
                value,
                measureName,
                utcDate = DateTime.UtcNow.Ticks
            };
            return await addMeasure(JSON);
        }
        public async Task<bool> addMeasure(object JSONObject)
        {
            try
            {
                var messageString = JsonConvert.SerializeObject(JSONObject);
                var message = new Message(Encoding.ASCII.GetBytes(messageString));
                log.Info($"Send IOTHub message: {messageString}");
                await client.SendEventAsync(message);
            }
            catch (Exception exception)
            {
                log.Error($"Something goes wrong during sent message to iot hub");
                log.Error(exception.ToString());
                return await Task.FromResult(false);
            }
            return await Task.FromResult(true);
        }
    }
}