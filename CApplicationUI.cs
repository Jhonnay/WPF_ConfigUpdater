using System.ComponentModel;

//CTRL + M plus CTRL + O  - Collapse All
//TODO: Version number  13.2.11.11 is displayed in Version Collumn as 13.2.1111
//TODO: Handling when unable to connect because already someone connected to Miniserver
namespace WPFConfigUpdater
{

    public partial class MainWindow
    {
        public class CApplicationUI : INotifyPropertyChanged
        {
            private bool handsfreeMode;

            public bool HandsfreeMode
            {
                get { return this.handsfreeMode; }
                set
                {
                    if (this.handsfreeMode != value)
                    {
                        this.handsfreeMode = value;
                        this.NotifyPropertychanged("HandsfreeMode");
                    }
                }
            }

            public event PropertyChangedEventHandler PropertyChanged;

            private void NotifyPropertychanged(string newVersion)
            {
                if (this.PropertyChanged != null)
                {
                    this.PropertyChanged(this, new PropertyChangedEventArgs(newVersion));
                }
            }

        }
    }

}
