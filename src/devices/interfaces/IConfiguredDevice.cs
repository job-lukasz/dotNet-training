namespace rpi_dotnet
{
    public interface IConfiguredDevice
    {
        bool FindDevice();
        event System.EventHandler<MeasuredValueChange> ValueChanged;
        bool Measure();
    }
}