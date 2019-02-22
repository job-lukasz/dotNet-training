namespace rpi_dotnet
{
    public enum Direction
    {
        IN, OUT, Undefined
    }
    public interface IGPIODevice
    {
        string pinAddress { get; }
        bool? lastValue { get; }
        bool setValue(bool value);
        bool getValue();
        bool setDirection(Direction direction);
    }
}