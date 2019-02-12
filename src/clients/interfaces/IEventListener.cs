using System;
namespace rpi_dotnet
{
    public class MeasuredValueChange : EventArgs
    {
        public readonly float NewValue;
        public readonly string SpaceID;
        public readonly string DeviceID;
        public MeasuredValueChange(float newValue, string spaceID, string deviceID)
        {
            NewValue = newValue;
            SpaceID = spaceID;
            DeviceID = deviceID;
        }
    }
    public interface IEventListener
    {
        void onEvent(object sender, MeasuredValueChange value);
    }
}