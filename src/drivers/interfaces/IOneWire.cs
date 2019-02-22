using System.Collections.Generic;
namespace rpi_dotnet
{
    public interface IOneWire
    {
        List<IOneWireDevice> getDevices();
        IOneWireDevice getDevice(string deviceID);
    }
}