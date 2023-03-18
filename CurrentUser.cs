//CTRL + M plus CTRL + O  - Collapse All
//TODO: Version number  13.2.11.11 is displayed in Version Collumn as 13.2.1111
//TODO: Handling when unable to connect because already someone connected to Miniserver
namespace WPFConfigUpdater
{
    public class CurrentUser
    {
        public string name { get; set; }
        public string uuid { get; set; }
        public bool isAdmin { get; set; }
        public bool changePassword { get; set; }
        public long userRights { get; set; }
    }

}
