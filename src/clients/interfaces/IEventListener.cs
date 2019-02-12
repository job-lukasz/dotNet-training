using System;
namespace rpi_dotnet
{
    public class MeasuredValueChange : EventArgs
    {
        public readonly string MeasureName;
        public readonly float NewValue;
        public readonly string SpaceID;
        public readonly string DeviceID;
        public MeasuredValueChange(string measureName, float newValue, string spaceID, string deviceID)
        {
            MeasureName = measureName;
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