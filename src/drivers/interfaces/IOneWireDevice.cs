namespace rpi_dotnet
{
    public interface IOneWireDevice
    {
        string deviceID { get; set; }
        float Measure();
    }
}
