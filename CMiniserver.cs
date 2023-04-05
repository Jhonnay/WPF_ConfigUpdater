using System.ComponentModel;

//CTRL + M plus CTRL + O  - Collapse All
//TODO: Version number  13.2.11.11 is displayed in Version Collumn as 13.2.1111
//TODO: Handling when unable to connect because already someone connected to Miniserver
namespace WPFConfigUpdater
{

    public partial class MainWindow
    {
        public class CMiniserver : INotifyPropertyChanged
        {
            public string serialNumer { get; set; }
            public string adminUser { get; set; }
            public string adminPassWord { get; set; }

            private string msVersion;
            private string msStatus;

            private string updatelevel;

            private string versionColor;

            private string configLanguage;


            public string ConfigLanguage
            {
                get { return this.configLanguage; }
                set
                {
                    if (this.configLanguage != value)
                    {
                        this.configLanguage = value;
                        this.NotifyPropertychanged("ConfigLanguage");
                    }
                }
            }

            public string VersionColor
            {
                get { return this.versionColor; }
                set
                {
                    if (this.versionColor != value)
                    {
                        this.versionColor = value;
                        this.NotifyPropertychanged("VersionColor");
                    }
                }
            }

            public string UpdateLevel
            {
                get { return this.updatelevel; }
                set
                {
                    if (this.updatelevel != value)
                    {
                        this.updatelevel = value;
                        this.NotifyPropertychanged("UpdateLevel");
                    }
                }
            }

            public string MSStatus
            {
                get { return this.msStatus; }
                set
                {
                    if (this.msStatus != value)
                    {
                        this.msStatus = value;
                        this.NotifyPropertychanged("MSStatus");
                    }
                }
            }

            public string MSVersion 
            {
                get { return this.msVersion; }
                set
                {
                    if(this.msVersion!= value)
                    {
                        this.msVersion = value;
                        this.NotifyPropertychanged("MSVersion");
                    }
                } 
            }

            private string msProject;

            public string MSProject
            {
                get { return this.msProject; }
                set
                {
                    if (this.msProject != value)
                    {
                        this.msProject = value;
                        this.NotifyPropertychanged("MSProject");
                    }
                }
            }

            private string msConfiguration;
            public string MSConfiguration
            {
                get { return this.msConfiguration; }
                set
                {
                    if (this.msConfiguration != value)
                    {
                        this.msConfiguration = value;
                        this.NotifyPropertychanged("MSConfiguration");
                    }
                }
            }

            private string localIPAdress;
            public string LocalIPAdress {
                get { return this.localIPAdress; }
                set
                {
                    if (this.localIPAdress != value)
                    {
                        this.localIPAdress = value;
                        this.NotifyPropertychanged("LocalIPAdress");
                    }
                }
            }

            



            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertychanged(string newVersion)
            {
                if(this.PropertyChanged != null)
                {
                    this.PropertyChanged(this,new PropertyChangedEventArgs(newVersion));
                } 
            }

            


            public string getCloudDNSLink()
            {
                string address = "https://dns.loxonecloud.com/" + this.serialNumer;
                return address;

            }

            public override string? ToString()
            {
                return "CMiniserver: " + this.serialNumer + ", " + this.MSVersion;  
            }
        }
    }

}
