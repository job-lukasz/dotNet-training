namespace rpi_dotnet
{
    public enum DeviceType {SENSOR, ACTUATOR};
    public interface IConfiguredDevice
    {
        DeviceType type {get;}
        bool FindDevice();
        event System.EventHandler<MeasuredValueChange> ValueChanged;
        bool Act();
    }
}
