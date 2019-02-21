using CommandLine;
class Options {
  [Option('i', "iotHub", Required = true, HelpText = "IotHub authenitcation string")]
  public string iotHubAuthString { get; set; }

}
