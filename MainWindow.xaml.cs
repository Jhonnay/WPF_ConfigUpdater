using Microsoft.Win32;
using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Text.Json;
using System.Collections;
using Microsoft.VisualBasic;
using static WPFConfigUpdater.MainWindow;
using System.Threading;

//CTRL + M plus CTRL + O  - Collapse All
//TODO: Version number  13.2.11.11 is displayed in Version Collumn as 13.2.1111
//TODO: Handling when unable to connect because already someone connected to Miniserver
namespace WPFConfigUpdater
{
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string stringApplicationVersion = "V 0.8.3";
        public ObservableCollection<CMiniserver> miniserverList = new ObservableCollection<CMiniserver>();
        public int int_selectedItems_before_Refresh = 0;
        private BackgroundWorker worker_MSUpdate = null;
        public CApplicationUI cApplicationUI = new CApplicationUI();
        public int previousMouseOverIndex;
        public int mouseOverIndex;
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;

        public MainWindow()
        {

            InitializeComponent();
            worker_MSUpdate = new BackgroundWorker();
            worker_MSUpdate.WorkerReportsProgress = true;
            worker_MSUpdate.WorkerSupportsCancellation = true;
            worker_MSUpdate.DoWork += worker_DoWork_UpdateMSButton;
            worker_MSUpdate.ProgressChanged += worker_ProgressChanged_UpdateMSButton;
            worker_MSUpdate.RunWorkerCompleted += worker_RunWorkerCompleted_UpdateMSButton;

            listView_Miniserver.ItemsSource = miniserverList;
            CheckBoxDisableUpdateDialogs.DataContext = cApplicationUI;
            RefreshButton.IsEnabled = false;
            UpdateButton.IsEnabled = false;
            RemoveMSButton.IsEnabled = false;

            //btn_klick_me.AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(Button_MouseUp), true);
            //btn_klick_me.Click += test;
        }

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
        


        public class CMiniserver : INotifyPropertyChanged
        {
            public string serialNumer { get; set; }
            public string adminUser { get; set; }
            public string adminPassWord { get; set; }

            private string msVersion;
            private string msStatus;
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

        public class CMiniserverListConfigPath {

            private List<CMiniserver> listMiniservers;
            private string configPath;

            public string ConfigPath { get => configPath; set => configPath = value; }
            public List<CMiniserver> ListMiniservers { get => listMiniservers; set => listMiniservers = value; }
        }

        


        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ListView_SelectionChanged_1(object sender, SelectionChangedEventArgs e)
        {

        }

        private void btn_klick_me_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Double Click!");
            //btn_klick_me.Click -= test; //Unsubscribe 
            
            
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void MenuItem_MSVersionRefresh_Click(object sender, RoutedEventArgs e)
        {
            foreach (CMiniserver ms in miniserverList)
            {
                string ret_MsVersion="0.0.0.0";
                int index = -1;
                CLoxAppJson ret_cLoxAppJson;

                if (ms.LocalIPAdress != "" || ms.LocalIPAdress != null)
                {
                    ret_MsVersion = WebService.sendCommandRest_Version_Local_Gen1(ms.LocalIPAdress, ms.adminUser, ms.adminPassWord, @"/dev/sys/version", "value");
                    System.Diagnostics.Debug.WriteLine(ret_MsVersion + "- " + ms.serialNumer);
                    index = miniserverList.IndexOf(ms);
                    ret_MsVersion = Format_Miniserver_String(ret_MsVersion);
                    miniserverList.ElementAt(index).MSVersion = ret_MsVersion;
                    ret_cLoxAppJson = WebService.sendCommandRest_LoxAppJson_Local_Gen1(ms.serialNumer, ms.adminUser, ms.adminPassWord, @"/data/LoxAPP3.json");
                }
                else
                {
                    ret_MsVersion = WebService.sendCommandRest_Version_Remote_Cloud(ms.serialNumer, ms.adminUser, ms.adminPassWord, @"/dev/sys/version", "value");
                    System.Diagnostics.Debug.WriteLine(ret_MsVersion + "- " + ms.serialNumer);
                    index = miniserverList.IndexOf(ms);
                    ret_MsVersion = Format_Miniserver_String(ret_MsVersion);
                    miniserverList.ElementAt(index).MSVersion = ret_MsVersion;
                    ret_cLoxAppJson = WebService.sendCommandRest_LoxAppJson_Remote_Cloud(ms.serialNumer, ms.adminUser, ms.adminPassWord, @"/data/LoxAPP3.json");
                }
                
                
                
                if (ret_MsVersion == "0.0.0.0")
                {
                    
                    miniserverList.ElementAt(index).MSConfiguration = "error";
                }
                else if(ret_cLoxAppJson.gatewayType == 0)
                {
                    miniserverList.ElementAt(index).MSConfiguration = "Client";
                }
                else
                {
                    miniserverList.ElementAt(index).MSConfiguration = "Client/Gateway";
                }

                miniserverList.ElementAt(index).MSProject = ret_cLoxAppJson.projectName;
            }
        }

        private string Format_Miniserver_String (string unformatedVersionString)
        {
            string formatedVersionString = "0.0.0.0";
            if(Int32.Parse(unformatedVersionString) > 10101010)
            {
                StringBuilder sb = new StringBuilder();

                for (int i = 0; i < unformatedVersionString.Length; i++)
                {
                    if (i % 2 == 0 && i != 0)
                    {
                        sb.Append('.');
                    }
                    sb.Append(unformatedVersionString[i]);
                }

                string[] subs_strings = sb.ToString().Split('.');
                string sub_last = subs_strings.Last();
                sb = new StringBuilder();
                foreach (string sub in subs_strings)
                {
                    string temp = sub.TrimStart('0');
                    sb.Append(temp);
                    if (sub != sub_last)
                    {
                        sb.Append('.');

                    }
                }
                formatedVersionString = sb.ToString();
            }

            return formatedVersionString;
        }

        private string Remove_Trailing_Zeros_from_Formated_Version_String(string VersionStringwithTrailingZeroes)
        {
            string cleanString = VersionStringwithTrailingZeroes;
            StringBuilder sb = new StringBuilder();
            sb.Append(VersionStringwithTrailingZeroes[0]);

            for (int i = 1; i < VersionStringwithTrailingZeroes.Length; i++) { 
            
                if (VersionStringwithTrailingZeroes[i] != '0' && VersionStringwithTrailingZeroes[i - 1] != '.')
                {
                    sb.Append(VersionStringwithTrailingZeroes[i]);
                }
            }

            cleanString = sb.ToString();
            return cleanString;
        }


        private void ContentControl_textblock_statusbar_config_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if(Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + $"\\Loxone"))
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + $"\\Loxone";
            }
            else
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            }
            
            openFileDialog.Filter = "Executeable (*.exe)|*.exe";
            if(openFileDialog.ShowDialog() == true)
            {
                textblock_statusbar_config.Text = openFileDialog.FileName;
            }
        }

        private void listview_button_OpenConfig_Click(object sender, RoutedEventArgs e)
        {
                var processes = Process.GetProcessesByName("LoxoneConfig");

                string serialnumber = miniserverList.ElementAt(mouseOverIndex).serialNumer;
                string adminUser = miniserverList.ElementAt(mouseOverIndex).adminUser;
                string adminPassword = miniserverList.ElementAt(mouseOverIndex).adminPassWord;

                CConfigMSUpdate config = new CConfigMSUpdate();
                config.User = adminUser;
                config.Pw = adminPassword;

                if (miniserverList.ElementAt(mouseOverIndex).LocalIPAdress != "" && miniserverList.ElementAt(mouseOverIndex).LocalIPAdress != null)
                {
                    config.MsIP = miniserverList.ElementAt(mouseOverIndex).LocalIPAdress;
                }
                else
                {
                    config.MsIP = serialnumber;
                }


                if (textblock_statusbar_config.Text == "Current Config: not selected - double click to select")
                {
                    MessageBox.Show("No Config Selected! 🤷‍");
                }
                else if (processes.Count() != 0)
                {
                    MessageBox.Show("Unable to perform Updates! There are " + processes.Count() + " Config Instances running. In order to perform Miniserver Updates no other Config has to be open! " +
                        "\nPlease Save current Projects!");
                }
                else
                {
                    config.ConfigPath = textblock_statusbar_config.Text;
                    config.OpenConfigLoadProject();
                }         
        }

        private void SelectCurrentItem(object sender, MouseButtonEventArgs e)
        {
            //By this Code I got my `ListView` row Selected.
            ListViewItem item = (ListViewItem)sender;
            item.IsSelected = true;

        }

       

        private void Button_Update_Click(object sender, RoutedEventArgs e)
        {
            var processes = Process.GetProcessesByName("LoxoneConfig");
            if(processes.Count() == 0)
            {
                UpdateButton.IsEnabled = false;
                RefreshButton.IsEnabled = false;
                RemoveMSButton.IsEnabled = false;
                InsertMSButton.IsEnabled = false;



                int_selectedItems_before_Refresh = listView_Miniserver.SelectedItems.Count;
                List<CMiniserver> list = new List<CMiniserver> { };
                foreach (CMiniserver ms in listView_Miniserver.SelectedItems)
                {
                    list.Add(ms);
                }
                CMiniserverListConfigPath workerdata = new CMiniserverListConfigPath();
                workerdata.ListMiniservers = list;
                workerdata.ConfigPath = textblock_statusbar_config.Text;

                //BackgroundWorker worker = new BackgroundWorker();
                //worker.WorkerReportsProgress = true;
                //worker.DoWork += worker_DoWork_UpdateMSButton;
                //worker.ProgressChanged += worker_ProgressChanged_UpdateMSButton;
                //worker.RunWorkerCompleted += worker_RunWorkerCompleted_UpdateMSButton;
                if (!worker_MSUpdate.IsBusy)
                {
                    listView_Miniserver.IsEnabled = false;
                    worker_MSUpdate.RunWorkerAsync(workerdata);
                }
                else
                {
                    MessageBox.Show("The Update Process is still running! Please Cancel the Update!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show("Unable to perform Updates! There are " + processes.Count() + " Config Instances running. In order to perform Miniserver Updates no other Config has to be open! " +
                    "\nPlease Save current Projects!");
            }

        }

        private void worker_RunWorkerCompleted_UpdateMSButton(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                try
                {
                    MessageBox.Show("Updates canceled by user! " + (int)e.Result + "/" + int_selectedItems_before_Refresh + " Miniserver(s) updated!");
                }catch(InvalidOperationException ex)
                {
                    MessageBox.Show("Update canceled by user!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                textblock_processStatus.Text = "Update canceled! ⚠";
                foreach (CMiniserver ms in listView_Miniserver.SelectedItems)
                {
                    if(miniserverList.ElementAt(miniserverList.IndexOf(ms)).MSStatus != "updated ✅")
                    miniserverList.ElementAt(miniserverList.IndexOf(ms)).MSStatus = "Update canceled! ⚠";
                }

                //(sender as BackgroundWorker).ReportProgress(100);
                progressbar_ProcessStatus.Value = 100;
                ListView_GridView_Autoresize();

            }
            else
            {
                if(cApplicationUI.HandsfreeMode == false)
                {
                    MessageBox.Show((int)e.Result + "/" + int_selectedItems_before_Refresh + " Miniserver(s) updated!");
                }
                
                textblock_processStatus.Text = "Miniserver(s) updated.";
            }
            UpdateButton.IsEnabled = true;
            RefreshButton.IsEnabled = true;
            RemoveMSButton.IsEnabled = true;
            InsertMSButton.IsEnabled = true;
            listView_Miniserver.IsEnabled = true;
        }

        private void worker_ProgressChanged_UpdateMSButton(object? sender, ProgressChangedEventArgs e)
        {
            progressbar_ProcessStatus.Value = e.ProgressPercentage;

           

            if (e.UserState != null)
            {

                CMiniserver cMiniserver = (CMiniserver)e.UserState;
                textblock_processStatus.Text = cMiniserver.MSStatus;
                int index = -1;

                for (int i = 0; i <= miniserverList.Count; i++)
                {
                    if (miniserverList.ElementAt(i).serialNumer == cMiniserver.serialNumer)
                    {
                        index = i;
                        i = miniserverList.Count + 1; //Abbruch der For-Schleife, weil Index anhand der MS SNR gefunden.
                    }
                }
                if (index > -1)
                {
                    if(cMiniserver.MSStatus == "Updating")
                    {
                        miniserverList[index].MSStatus = "updating ... 🔄";
                        textblock_processStatus.Text = "Updating ... 🔄" + cMiniserver.serialNumer;
                        //listView_Miniserver.IsEnabled = false;
                    }
                    if(cMiniserver.MSStatus == "Update finished")
                    {
                        RoutedEventArgs routedEventArgs = new RoutedEventArgs();

                        CLoxAppJson ret_cLoxAppJson;
                        string ret_MsVersion; 
                        getMiniserverInformationsWebServices(cMiniserver, out  ret_MsVersion, out var index1, out ret_cLoxAppJson);

                       
                        if (ret_cLoxAppJson.gatewayType == 0)
                        {
                            miniserverList[index].MSConfiguration = "Standalone";
                        }
                        else
                        {
                            miniserverList[index].MSConfiguration = "Client/Gateway";
                        }

                        miniserverList[index].MSProject = ret_cLoxAppJson.projectName + "/" + ret_cLoxAppJson.localUrl;
                        miniserverList[index].MSVersion = ret_MsVersion;
                        miniserverList[index].MSStatus = "updated ✅";
                        textblock_processStatus.Text = "Updated - " + cMiniserver.serialNumer;
                        ListView_GridView_Autoresize();
                        
                    }
                    
                }


            }
        }

        private void worker_DoWork_UpdateMSButton(object sender, DoWorkEventArgs e)
        {
            CMiniserverListConfigPath workerdata = (CMiniserverListConfigPath)e.Argument;
            var list = workerdata.ListMiniservers;

            (sender as BackgroundWorker).ReportProgress(1); //show 1% when pressing button

            int result_MS_Update = 0;

            foreach (CMiniserver ms in list)
            {
                if (worker_MSUpdate.CancellationPending == true)
                {
                    e.Cancel = true;
                    return;
                }

                CConfigMSUpdate update = new CConfigMSUpdate();
                if(ms.LocalIPAdress != null && ms.LocalIPAdress != "")
                {
                    update.MsIP = ms.LocalIPAdress;
                }
                else
                {
                    update.MsIP = ms.serialNumer;
                }
               
                update.User = ms.adminUser;
                update.Pw = ms.adminPassWord;
                if (workerdata.ConfigPath == "Current Config: not selected - double click to select")
                {
                    MessageBox.Show("No Config Selected! No Update will be performed!");
                }
                else
                {
                    int progressPercentage = Convert.ToInt32((double)result_MS_Update / list.Count * 100);
                    if(progressPercentage <= 99)
                    {
                        progressPercentage += 1;
                    }
                    string progressText = "Updating";
                    ms.MSStatus = progressText;
                    (sender as BackgroundWorker).ReportProgress(progressPercentage, ms);
                    update.ConfigPath = workerdata.ConfigPath;
                    if (worker_MSUpdate.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }

                    ArrayList arrlist = update.UpdateMS(worker_MSUpdate);
                    result_MS_Update++;
                    

                    if (cApplicationUI.HandsfreeMode == false && worker_MSUpdate.CancellationPending == false)
                    {
                        string concat_strings = string.Join(" ", arrlist.Cast<string>().ToArray());
                        MessageBox.Show("Miniserver Updated to - " + concat_strings);
                        
                    }

                    if (worker_MSUpdate.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }

                    progressPercentage = Convert.ToInt32((double)result_MS_Update / list.Count * 100);

                    progressText = "Update finished";
                    ms.MSStatus = progressText;
                    (sender as BackgroundWorker).ReportProgress(progressPercentage, ms);
                }
                
            }
            
            e.Result = result_MS_Update;
        }

        private void Button_RefreshMS_Click(object sender, RoutedEventArgs e)
        {
            progressbar_ProcessStatus.Value = 0;
            textblock_processStatus.Text = "Miniserver Information is retreived ... ";
            StackPaneButtons.IsEnabled = false; //Disables all containing Buttons
            int_selectedItems_before_Refresh = listView_Miniserver.SelectedItems.Count;

            List<CMiniserver> list = new List<CMiniserver> { };
            foreach(CMiniserver ms in listView_Miniserver.SelectedItems)
            {
                ms.MSStatus = "TBD";
                list.Add(ms);
            }
            listView_Miniserver.IsEnabled = false;

            Thread.Sleep(200);

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork_RefreshMSInformation;
            worker.ProgressChanged += worker_ProgressChanged_RefreshMSInformation;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted_RefreshMSInformation;
            worker.RunWorkerAsync(list);
        }

        private void worker_RunWorkerCompleted_RefreshMSInformation(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show((int)e.Result  + "/" + int_selectedItems_before_Refresh + " Miniserver Information pulled!");
            textblock_processStatus.Text = "Informations pulled.";
            StackPaneButtons.IsEnabled = true;
            listView_Miniserver.IsEnabled = true;
        }

        private void worker_ProgressChanged_RefreshMSInformation(object sender, ProgressChangedEventArgs e)
        {
            progressbar_ProcessStatus.Value = e.ProgressPercentage;
            
            if (e.UserState != null)
            {

                CMiniserver cMiniserver = (CMiniserver)e.UserState;
                textblock_processStatus.Text = "retreiving data of " + cMiniserver.serialNumer;
                int index = -1;

                for (int i = 0; i <= miniserverList.Count; i++)
                {
                    if (miniserverList.ElementAt(i).serialNumer == cMiniserver.serialNumer)
                    {
                        index = i;
                        i = miniserverList.Count + 1; //Abbruch der For-Schleife, weil Index anhand der MS SNR gefunden.
                    }
                }
                if (index > -1)
                {
                    if (cMiniserver.MSVersion == "1.0.0.0")
                    {
                        cMiniserver.MSStatus = "retreiving data ⏲";
                    }
                    else if (cMiniserver.MSVersion != "0.0.0.0")
                    {
                        cMiniserver.MSStatus = "Info up to date ✅";
                        
                    }
                    else
                    {
                        cMiniserver.MSStatus = "Error/Timeout ⚠";
                       
                    }
                    miniserverList[index] = cMiniserver;
                    ListView_GridView_Autoresize();
                }
            }
            
        }

        public void ListView_GridView_Autoresize()
        {
            foreach (GridViewColumn c in listview_gridview.Columns)
            {
                // Code below was found in GridViewColumnHeader.OnGripperDoubleClicked() event handler (using Reflector)
                // i.e. it is the same code that is executed when the gripper is double clicked
                // if (adjustAllColumns || App.StaticGabeLib.FieldDefsGrid[colNum].DispGrid)
                if (double.IsNaN(c.Width))
                {
                    c.Width = c.ActualWidth;
                }
                c.Width = double.NaN;
            }
        }

        private void worker_DoWork_RefreshMSInformation(object sender, DoWorkEventArgs e)
        {
            var list =  (List<CMiniserver>) e.Argument;
            int result_MS_Refreshed = 0;
            int progressPercentage = 0;

            (sender as BackgroundWorker).ReportProgress(1); //show 1% after clicking refresh

            if (list != null)
            {
                foreach (CMiniserver ms in list)
                {
                    ms.MSVersion = "1.0.0.0"; //special condition --> during fetching data to show "retreiving..."
                    (sender as BackgroundWorker).ReportProgress(progressPercentage + 1, ms);
                    string ret_MsVersion = "0.0.0.0";
                    int index = -1;
                    CLoxAppJson ret_cLoxAppJson;
                    getMiniserverInformationsWebServices(ms, out ret_MsVersion, out index, out ret_cLoxAppJson);

                    CMiniserver cMiniserver = ms;
                    cMiniserver.MSVersion = ret_MsVersion;


                    if (ret_MsVersion == "0.0.0.0")
                    {

                        cMiniserver.MSConfiguration = "error";
                    }
                    else if (ret_cLoxAppJson.gatewayType == 0)
                    {
                        cMiniserver.MSConfiguration = "Standalone";
                        result_MS_Refreshed++;
                    }
                    else
                    {
                        cMiniserver.MSConfiguration = "Client/Gateway";
                        result_MS_Refreshed++;
                    }

                    cMiniserver.MSProject = ret_cLoxAppJson.projectName + "/" + ret_cLoxAppJson.localUrl;


                    progressPercentage = Convert.ToInt32((double)result_MS_Refreshed / list.Count * 100);
                    (sender as BackgroundWorker).ReportProgress(progressPercentage, cMiniserver);
                    System.Threading.Thread.Sleep(100);
                }
                (sender as BackgroundWorker).ReportProgress(100); //Fix even if retreival not possible for some MS --> show 100%
                e.Result = result_MS_Refreshed;
            }
           
        }

        private void getMiniserverInformationsWebServices(CMiniserver ms, out string ret_MsVersion, out int index, out CLoxAppJson ret_cLoxAppJson)
        {
            if (ms.LocalIPAdress != "" && ms.LocalIPAdress != null)
            {
                ret_MsVersion = WebService.sendCommandRest_Version_Local_Gen1(ms.LocalIPAdress, ms.adminUser, ms.adminPassWord, @"/dev/sys/version", "value");
                System.Diagnostics.Debug.WriteLine(ret_MsVersion + "- " + ms.serialNumer);
                index = miniserverList.IndexOf(ms);
                ret_MsVersion = Format_Miniserver_String(ret_MsVersion);
                miniserverList.ElementAt(index).MSVersion = ret_MsVersion;
                ret_cLoxAppJson = WebService.sendCommandRest_LoxAppJson_Local_Gen1(ms.LocalIPAdress, ms.adminUser, ms.adminPassWord, @"/data/LoxAPP3.json");
            }
            else
            {
                ret_MsVersion = WebService.sendCommandRest_Version_Remote_Cloud(ms.serialNumer, ms.adminUser, ms.adminPassWord, @"/dev/sys/version", "value");
                System.Diagnostics.Debug.WriteLine(ret_MsVersion + "- " + ms.serialNumer);
                index = miniserverList.IndexOf(ms);
                ret_MsVersion = Format_Miniserver_String(ret_MsVersion);
                miniserverList.ElementAt(index).MSVersion = ret_MsVersion;
                ret_cLoxAppJson = WebService.sendCommandRest_LoxAppJson_Remote_Cloud(ms.serialNumer, ms.adminUser, ms.adminPassWord, @"/data/LoxAPP3.json");
            }
        }

        private void listView_Miniserver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if(listView.SelectedIndex == -1)
            {
                RefreshButton.IsEnabled = false;
                UpdateButton.IsEnabled = false;
                RemoveMSButton.IsEnabled = false;
            }
            else
            {
                RefreshButton.IsEnabled = true;
                UpdateButton.IsEnabled = true;
                RemoveMSButton.IsEnabled = true;
            }
            

        }


        private void Button_RemoveMS_Click(object sender, RoutedEventArgs e)
        {
            var list = listView_Miniserver.SelectedItems;
            CMiniserver[] array = new CMiniserver[listView_Miniserver.SelectedItems.Count];
            
            listView_Miniserver.SelectedItems.CopyTo(array, 0);


            if (list.Count != 0)
            {
                foreach (CMiniserver ms in array)
                {
                    miniserverList.Remove(ms);
                }
            }
            
        }

        private void Button_InsertMS_Click(object sender, RoutedEventArgs e)
        {
            CreateMiniserverDialog dialog = new CreateMiniserverDialog();
            CMiniserver result = new CMiniserver();

            if (dialog.ShowDialog() == true)
            {
                result = dialog.Answer;
                miniserverList.Add(result);

            }
               
        }

        private void Button_CancelUpdate_Click(object sender, RoutedEventArgs e)
        {
            worker_MSUpdate.CancelAsync();
        }

        private void ContextMenu_CopySNR_Click(object sender, RoutedEventArgs e)
        {
            int index = previousMouseOverIndex;
            if(index != -1)
            {
                string snr = miniserverList.ElementAt(previousMouseOverIndex).serialNumer;
                Clipboard.SetText(snr);
            }
            else
            {
                MessageBox.Show("SNR could not be copied to Clipboard! \nPlease select a Miniserver.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            
        }

        private void Contextmenu_DefLog_click(object sender, RoutedEventArgs e)
        {
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);
            

            string snr = currentlySelectedMiniserver.serialNumer;
            string link = "";

            if (currentlySelectedMiniserver.LocalIPAdress != "" && currentlySelectedMiniserver.LocalIPAdress != null)
            {
                link = @"http://" + currentlySelectedMiniserver.LocalIPAdress + "/dev/fsget/log/def.log";
            }
            else
            {
                link = "https://dns.loxonecloud.com/" + snr + "/dev/fsget/log/def.log";
            }

            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = "/c start " + link;
            cmd.StartInfo.CreateNoWindow = true;

            cmd.Start();
        }

        private void Contextmenu_EditMiniserver_click(object sender, RoutedEventArgs e)
        {
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);
            CreateMiniserverDialog dialog = new CreateMiniserverDialog(currentlySelectedMiniserver.serialNumer,currentlySelectedMiniserver.adminUser,currentlySelectedMiniserver.adminPassWord);
            CMiniserver result = new CMiniserver();
            int index = previousMouseOverIndex;

            if (dialog.ShowDialog() == true)
            {
                result = dialog.Answer;
                result.MSStatus = "Miniserver modified ⚠";
                miniserverList[index] = result;
            }

        }

        private void MenuItem_Programm_Version_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(stringApplicationVersion);
        }

        private void MenuItem_Changelog_Click(object sender, RoutedEventArgs e)
        {
            //String stringChangelogContent = File.ReadAllText(@"Changelog.txt");
            //MessageBox.Show(stringChangelogContent);
            if(File.Exists(Directory.GetCurrentDirectory() + $"\\Changelog.txt"))
            {
                System.Diagnostics.Process.Start("notepad.exe",@"Changelog.txt");

            }
            else
            {
                MessageBox.Show("Changelog cannot be opened! ", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            
        }

        private void ContextMenu_internWI_Click(object sender, RoutedEventArgs e)
        {
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);
            string link = @"http://" + currentlySelectedMiniserver.LocalIPAdress;

            OpenLinkinDefaultBrowser(link);
        }
        private void ContextMenu_externWI_Click(object sender, RoutedEventArgs e)
        {
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);
            string link = "https://dns.loxonecloud.com/" + currentlySelectedMiniserver.serialNumer;

            OpenLinkinDefaultBrowser(link);

        }

        private static void OpenLinkinDefaultBrowser(string link)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = "/c start " + link;
            cmd.StartInfo.CreateNoWindow = true;

            cmd.Start();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            //Starts default Browser
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = "/c start " + e.Uri.AbsoluteUri;
            cmd.StartInfo.CreateNoWindow = true;

            cmd.Start();
        }

        private void menuItem_Settings(object sender, RoutedEventArgs e)
        {
            ApplicationSettingsDialog dialog;
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MinserverUpdater\\ApplicationSettings.json"))
            {
                string strJson = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MinserverUpdater\\ApplicationSettings.json");
                ApplicationSettings settings = JsonSerializer.Deserialize<ApplicationSettings>(strJson);
                dialog= new ApplicationSettingsDialog(settings);
            }
            else
            {
                dialog = new ApplicationSettingsDialog();
            }
            

            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show("Settings saved. ✅", "Settings Dialog", MessageBoxButton.OK, MessageBoxImage.Information);
                textblock_statusbar_config.Text = dialog.Answer;

            }
            else
            {
                MessageBox.Show("Settings were not saved! ⚠", "Settings Dialog", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContentControl_textblock_statusbar_menuItem_Start_Config(object sender, RoutedEventArgs e)
        {
            if (textblock_statusbar_config.Text != "Current Config: not selected - double click to select")
            {
                Config config = new Config();
                config.startConfig(textblock_statusbar_config.Text);
            }
            else
            {
                MessageBox.Show("No Config selected! 🤷‍", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
                
        }

        private void ContentControl_textblock_statusbar_menuItem_Kill_Configs(object sender, RoutedEventArgs e)
        {
            var processes = Process.GetProcessesByName("LoxoneConfig");

            foreach(var instance in processes)
            {
                instance.Kill();
            }

        }

        private void CommandBinding_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {

        }

        private void Application_Exit_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Application_Exit_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            
            System.Windows.Application.Current.Shutdown();
            
        }

        private void Application_Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Application_Save_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            string jsonString = JsonSerializer.Serialize(miniserverList, new JsonSerializerOptions { WriteIndented = true });
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON file (*.json)|*.json";

            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MiniserverUpdater"))
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MiniserverUpdater";
            }
            else
            {
                saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }


            if (saveFileDialog.ShowDialog() == true)
            {
                File.WriteAllText(saveFileDialog.FileName, jsonString);
            }
        }

        private void Application_Open_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Application_Open_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON file (*.json)|*.json";

            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MiniserverUpdater"))
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MiniserverUpdater";
            }
            else
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            }


            if (openFileDialog.ShowDialog() == true)
            {
                String jsonString = File.ReadAllText(openFileDialog.FileName);
                try
                {
                    ObservableCollection<CMiniserver> miniservers = JsonSerializer.Deserialize<ObservableCollection<CMiniserver>>(jsonString);

                    if (miniservers != null && miniservers.Count != 0)
                    {
                        for (int i = 0; i < miniservers.Count; i++)
                        {
                            miniservers[i].MSStatus = "outdated info 😐";
                            miniservers[i].MSVersion = "TBD";
                        }

                        miniserverList = miniservers;
                        listView_Miniserver.ItemsSource = miniserverList;
                        //MenuItem_MSVersionRefresh_Click(sender, e);
                        ListView_GridView_Autoresize();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void Application_New_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Application_New_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            miniserverList.Clear();
        }

        private void Application_SelectAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if(listView_Miniserver.IsEnabled == true)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void Application_SelectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            listView_Miniserver.SelectAll();
            listView_Miniserver.Focus();
            //gridview_lv.Focus();
        }

        private void Application_DeselectAll_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (listView_Miniserver.IsEnabled == true)
            {
                e.CanExecute = true;
            }
            else
            {
                e.CanExecute = false;
            }
        }

        private void Application_DeselectAll_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            listView_Miniserver.UnselectAll();
            listView_Miniserver.Focus();
        }

        private void OnListViewItemPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            Debug.WriteLine("Tunnel! Selected Index = " + mouseOverIndex);
            //listView_Miniserver.UnselectAll();
            //listView_Miniserver.focus

            e.Handled = true;
        }

        private void ListViewItem_DragOver(object sender, DragEventArgs e)
        {
            Debug.WriteLine(listView_Miniserver.SelectedIndex.ToString());
            e.Handled = true;
        }

        private void ListViewItem_MouseEnter(object sender, MouseEventArgs e)
        {
            e.Handled = true;
            ListViewItem item = (ListViewItem) sender;
            CMiniserver cMiniserver = (CMiniserver)item.Content;
            //listView_Miniserver.items
            previousMouseOverIndex = mouseOverIndex;
            mouseOverIndex = miniserverList.IndexOf(cMiniserver);
            Debug.WriteLine(item + " | selected index:  " + mouseOverIndex);
        }

        private void listview_Miniserver_ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            GridViewColumnHeader column = (sender as GridViewColumnHeader);
            string sortBy = column.Tag.ToString();
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                listView_Miniserver.Items.SortDescriptions.Clear();
            }

            ListSortDirection newDir = ListSortDirection.Ascending;
            if (listViewSortCol == column && listViewSortAdorner.Direction == newDir)
                newDir = ListSortDirection.Descending;

            listViewSortCol = column;
            listViewSortAdorner = new SortAdorner(listViewSortCol, newDir);
            AdornerLayer.GetAdornerLayer(listViewSortCol).Add(listViewSortAdorner);
            listView_Miniserver.Items.SortDescriptions.Add(new SortDescription(sortBy, newDir));
        }
    }

        //private void CheckBoxEnableMultiSelect_Click(object sender, RoutedEventArgs e)
        //{
        //    if(CheckBoxEnableMultiSelect.IsChecked == true)
        //    {
        //        listView_Miniserver.SelectionMode = SelectionMode.Multiple;
        //    }
        //    else
        //    {
        //        listView_Miniserver.SelectionMode = SelectionMode.Extended;
        //    }
        //}
    


    public static class CustomCommands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
            (
                "Exit",
                "Exit",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F4, ModifierKeys.Alt)
                }
            );

        public static readonly RoutedUICommand DeselectAll = new RoutedUICommand
            (
                "DeselectAll",
                "DeselectAll",
                typeof(CustomCommands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.D, ModifierKeys.Control)
                }
            );

        //Define more commands here, just like the one above
    }

    public class UriConverter : MarkupExtension, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string address = string.Empty;
               
            address = "https://dns.loxonecloud.com/" + value;

            Uri path = new Uri(@address);
            return path;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }

        public override object ProvideValue(System.IServiceProvider serviceProvider)
        {
            return this;
        }
    }

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

    public class CurrentUser
    {
        public string name { get; set; }
        public string uuid { get; set; }
        public bool isAdmin { get; set; }
        public bool changePassword { get; set; }
        public long userRights { get; set; }
    }

    public class WebService
    {
        public static string sendCommand(string ms_SNR, string user, string password, string command, string interestedValue)
        {
            
            HttpWebRequest request;
            string ret = "-9999";

            string testResult = @"https://dns.loxonecloud.com/" + ms_SNR + command;       //Request is created
            System.Diagnostics.Debug.WriteLine(testResult);
            System.Diagnostics.Debug.WriteLine(user+ "&" + password);

            request = (HttpWebRequest)WebRequest.Create(testResult);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Credentials = new NetworkCredential(user, password);
            request.AllowAutoRedirect = false;
            

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) 
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    //Read Characters from string and removes not needed ones
                    string receivedData = reader.ReadToEnd();
                    receivedData = receivedData.Substring(receivedData.IndexOf(interestedValue) + interestedValue.Count() + 2);

                    receivedData = receivedData.Remove(1); //remove everyting after the value
                                                           //ret_MsVersion = Convert.ToInt32(receivedData);
                    ret = receivedData;

                }
            }
            catch (WebException e)
            {
                ret = e.Message; 
            }
            
            return ret;
        }

        public static string sendCommandRest_Version_Remote_Cloud(string ms_SNR, string user, string password, string command, string interestedValue)
        {
            string receivedData = "-1213";
            string url = @"https://dns.loxonecloud.com/" + ms_SNR + command;
            RestClient client = new RestClient(url);
            client.UseXml();
            client.Options.FollowRedirects = true;
            client.Authenticator = new HttpBasicAuthenticator(user, password);
            var response = client.Execute(new RestRequest());
            if (response.StatusCode == HttpStatusCode.Unauthorized )
            {
                client = new RestClient(response.ResponseUri.AbsoluteUri);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    receivedData = response.Content.ToString();
                    receivedData = receivedData.Substring(receivedData.IndexOf(interestedValue) + interestedValue.Count() + 2);
                    receivedData = receivedData.Remove(8);
                }
            }
            else if (response.StatusCode == HttpStatusCode.RedirectKeepVerb){
                client = new RestClient((string)response.Headers.ElementAt(5).Value);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    receivedData = response.Content.ToString();
                    receivedData = receivedData.Substring(receivedData.IndexOf(interestedValue) + interestedValue.Count() + 2);
                    receivedData = receivedData.Remove(8);
                }
            }

            System.Diagnostics.Debug.WriteLine(url);

            return receivedData;
        }

        public static string sendCommandRest_Version_Local_Gen1(string localIP, string user, string password, string command, string interestedValue)
        {
            string receivedData = "-1213";
            string url = @"http://" + localIP  +  command;
            RestClient client = new RestClient(url);
            client.UseXml();
            client.Options.FollowRedirects = true;
            client.Authenticator = new HttpBasicAuthenticator(user, password);
            var response = client.Execute(new RestRequest());
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                client = new RestClient(response.ResponseUri.AbsoluteUri);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    receivedData = response.Content.ToString();
                    receivedData = receivedData.Substring(receivedData.IndexOf(interestedValue) + interestedValue.Count() + 2);
                    receivedData = receivedData.Remove(8);
                }
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    receivedData = response.Content.ToString();
                    receivedData = receivedData.Substring(receivedData.IndexOf(interestedValue) + interestedValue.Count() + 2);
                    receivedData = receivedData.Remove(8);
                }
            }

            System.Diagnostics.Debug.WriteLine(url);

            return receivedData;
        }

        public static CLoxAppJson sendCommandRest_LoxAppJson_Remote_Cloud(string ms_SNR, string user, string password, string command)
        {
            CLoxAppJson cLoxAppJson = new CLoxAppJson();
            string receivedData = "-1213";
            string url = @"https://dns.loxonecloud.com/" + ms_SNR + command;
            RestClient client = new RestClient(url);
            client.UseXml();
            client.Options.FollowRedirects = true;
            client.Authenticator = new HttpBasicAuthenticator(user, password);
            var response = client.Execute(new RestRequest());
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                client = new RestClient(response.ResponseUri.AbsoluteUri);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    receivedData = response.Content.ToString();
                    try
                    {
                        var jsondata = JsonDocument.Parse(receivedData);
                        string msInfo = jsondata.RootElement.GetProperty("msInfo").ToString();
                        cLoxAppJson = JsonSerializer.Deserialize<CLoxAppJson>(msInfo);
                    }
                    catch(Exception ex)
                    {
                        cLoxAppJson.projectName = "Invalid JSON";
                    }
                    
                }
            }
            else if (response.StatusCode == HttpStatusCode.RedirectKeepVerb)
            {
                client = new RestClient((string)response.Headers.ElementAt(5).Value);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    receivedData = response.Content.ToString();
                    try
                    {
                        var jsondata = JsonDocument.Parse(receivedData);
                        string msInfo = jsondata.RootElement.GetProperty("msInfo").ToString();
                        cLoxAppJson = JsonSerializer.Deserialize<CLoxAppJson>(msInfo);
                    }
                    catch (Exception ex)
                    {
                        cLoxAppJson.projectName = "Invalid JSON";
                    }

                }
            }

            System.Diagnostics.Debug.WriteLine(url);

            return cLoxAppJson;
        }

        public static CLoxAppJson sendCommandRest_LoxAppJson_Local_Gen1(string localIP, string user, string password, string command)
        {
            CLoxAppJson cLoxAppJson = new CLoxAppJson();
            string receivedData = "-1213";
            string url = @"http://" + localIP  + command;
            RestClient client = new RestClient(url);
            client.UseXml();
            client.Options.FollowRedirects = true;
            client.Authenticator = new HttpBasicAuthenticator(user, password);
            var response = client.Execute(new RestRequest());
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                client = new RestClient(response.ResponseUri.AbsoluteUri);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    receivedData = response.Content.ToString();
                    try
                    {
                        var jsondata = JsonDocument.Parse(receivedData);
                        string msInfo = jsondata.RootElement.GetProperty("msInfo").ToString();
                        cLoxAppJson = JsonSerializer.Deserialize<CLoxAppJson>(msInfo);
                    }
                    catch (Exception ex)
                    {
                        cLoxAppJson.projectName = "Invalid JSON";
                    }
                }
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    receivedData = response.Content.ToString();
                    try
                    {
                        var jsondata = JsonDocument.Parse(receivedData);
                        string msInfo = jsondata.RootElement.GetProperty("msInfo").ToString();
                        cLoxAppJson = JsonSerializer.Deserialize<CLoxAppJson>(msInfo);
                    }
                    catch (Exception ex)
                    {
                        cLoxAppJson.projectName = "Invalid JSON";
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine(url);

            return cLoxAppJson;
        }
    }

    public class SortAdorner : Adorner
    {
        private static Geometry ascGeometry =
            Geometry.Parse("M 0 4 L 3.5 0 L 7 4 Z");

        private static Geometry descGeometry =
            Geometry.Parse("M 0 0 L 3.5 4 L 7 0 Z");

        public ListSortDirection Direction { get; private set; }

        public SortAdorner(UIElement element, ListSortDirection dir)
            : base(element)
        {
            this.Direction = dir;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            if (AdornedElement.RenderSize.Width < 20)
                return;

            TranslateTransform transform = new TranslateTransform
                (
                    AdornedElement.RenderSize.Width - 15,
                    (AdornedElement.RenderSize.Height - 5) / 2
                );
            drawingContext.PushTransform(transform);

            Geometry geometry = ascGeometry;
            if (this.Direction == ListSortDirection.Descending)
                geometry = descGeometry;
            drawingContext.DrawGeometry(Brushes.Black, null, geometry);

            drawingContext.Pop();
        }
    }

}
