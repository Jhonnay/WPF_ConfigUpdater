using System.Collections.Generic;

//CTRL + M plus CTRL + O  - Collapse All
//TODO: Version number  13.2.11.11 is displayed in Version Collumn as 13.2.1111
//TODO: Handling when unable to connect because already someone connected to Miniserver
namespace WPFConfigUpdater
{

    public partial class MainWindow
    {
        public class CMiniserverListConfigPath {

            private List<CMiniserver> listMiniservers;
            private string configPath;

            public string ConfigPath { get => configPath; set => configPath = value; }
            public List<CMiniserver> ListMiniservers { get => listMiniservers; set => listMiniservers = value; }
        }
    }

}
