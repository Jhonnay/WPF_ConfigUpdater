//CTRL + M plus CTRL + O  - Collapse All
//TODO: Version number  13.2.11.11 is displayed in Version Collumn as 13.2.1111
//TODO: Handling when unable to connect because already someone connected to Miniserver
namespace WPFConfigUpdater
{
    public class CLoxAppJson
    {
        public string serialNr { get; set; }
        public string msName { get; set; }
        public string projectName { get; set; }
        public string localUrl { get; set; }
        public string remoteUrl { get; set; }
        public int tempUnit { get; set; }
        public string currency { get; set; }
        public string squareMeasure { get; set; }
        public string location { get; set; }
        public double latitude { get; set; }

        public int gatewayType { get; set; }
        public double longitude { get; set; }
        public int altitude { get; set; }
        public string languageCode { get; set; }
        public string heatPeriodStart { get; set; }
        public string heatPeriodEnd { get; set; }
        public string coolPeriodStart { get; set; }
        public string coolPeriodEnd { get; set; }
        public string catTitle { get; set; }
        public string roomTitle { get; set; }
        public int miniserverType { get; set; }
        public string deviceMonitor { get; set; }
        public CurrentUser currentUser { get; set; }
    }

}
