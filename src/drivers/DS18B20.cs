using System.Text.RegularExpressions;

namespace rpi_dotnet
{
    public class DS18B20 : IOneWireDevice
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IFileWrapper file;

        public DS18B20(string deviceID, IFileWrapper fileWrapper = null)
        {
            file = fileWrapper ?? new FileWrapper();
            this.deviceID = deviceID;
        }

        public string deviceID{ get; private set;}
        public float? lastMeasure {get; private set;}

        public float Measure()
        {
            log.Debug($"Get measure for device: {deviceID}");
            var rawOutput = file.Read($"{OneWire.path}/{deviceID}/w1_slave");
            log.Debug($"Read raw data: {rawOutput}");
            var measure = new DS18B20Measure(rawOutput);
            if (measure.crcStatus)
            {
                lastMeasure = measure.temp;
                return (float)lastMeasure; //corrected measure date could not be null
            }
            else throw new System.Exception();
        }

        private class DS18B20Measure
        {
            public float temp;
            public bool crcStatus;
            public DS18B20Measure(string rawOutput)
            {
                temp = float.Parse(Regex.Match(rawOutput, @"t=(\d+)").Groups[1].Value) / 1000;
                crcStatus = rawOutput.Contains("YES");
            }
        }
    }
}
