namespace rpi_dotnet
{
    public interface ISensorDevice
    {
        string deviceID { get; }
        float GetValue();
        float? lastValue { get; }
    }
}