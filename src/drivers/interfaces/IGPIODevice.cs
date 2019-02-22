namespace rpi_dotnet
{
    public enum Direction
    {
        IN, OUT, Undefined
    }
    public interface IGPIODevice : ISensorDevice
    {
        bool SetValue(bool value);
        bool SetDirection(Direction direction);
    }
}