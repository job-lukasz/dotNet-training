using System.Text.RegularExpressions;

namespace rpi_dotnet
{
    public class DS18B20 : IOneWireDevice
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly string w1busPath = "/sys/bus/w1/devices";

        class DS18B20Measure
        {
            public float temp;
            public bool crcStatus;
            public DS18B20Measure(string rawOutput)
            {

                temp = float.Parse(Regex.Match(rawOutput, @"t=(\d+)").Groups[1].Value) / 1000;
                crcStatus = rawOutput.Contains("YES");
            }
        }
        private string _deviceID;
        private IFileWrapper file;
        public DS18B20(string deviceID, IFileWrapper fileWrapper = null)
        {
            file = fileWrapper ?? new FileWrapper();
            this._deviceID = deviceID;
        }

        public string deviceID
        {
            get { return _deviceID; }
            private set { _deviceID = value; }
        }
        public float Measure()
        {
            log.Debug($"Get measure for device: {_deviceID}");
            var rawOutput = file.Read($"{w1busPath}/{_deviceID}/w1_slave");
            log.Debug($"Read raw data: '''{rawOutput}'''");
            var measure = new DS18B20Measure(rawOutput);
            if (measure.crcStatus) return measure.temp;
            else throw new System.Exception();
        }
    }
}
