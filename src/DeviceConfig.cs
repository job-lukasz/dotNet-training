namespace rpi_dotnet
{
    public class DeviceConfiguration
    {
        public string deviceID;
        public string spaceID;
        public IOneWireDevice sensor;
        public DeviceConfiguration(string deviceId, string spaceId)
        {
            this.deviceID = deviceId;
            this.spaceID = spaceId;
        }
    }
}