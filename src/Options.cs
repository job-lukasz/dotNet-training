using CommandLine;
class Options {
  [Option('i', "iotHub", Required = true, HelpText = "IotHub authenitcation string")]
  public string iotHubAuthString { get; set; }
  [Option('s', "influxServer", Required = true, HelpText = "influx server address")]
  public string influxServer { get; set; }
  [Option('d', "influxDB", Required = true, HelpText = "influx server database name")]
  public string influxDB { get; set; }
}
