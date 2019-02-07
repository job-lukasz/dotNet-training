namespace rpi_dotnet
{
    public interface IOneWireDevice
    {
        string deviceID { get; }
        float Measure();
        float lastMeasure { get; }
    }
}
