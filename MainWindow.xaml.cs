using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using System.Collections;
using Microsoft.VisualBasic;
using static WPFConfigUpdater.MainWindow;
using System.Threading;
using WPFConfigUpdater.Common;
using System.Reflection;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Web;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.IO.Compression;
using System.Net;
using System.Diagnostics.Eventing.Reader;
using System.Printing;




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
        public string stringApplicationVersion = "0.9.6";
        public ObservableCollection<CMiniserver> miniserverList = new ObservableCollection<CMiniserver>();
        public int int_selectedItems_before_Refresh = 0;
        private BackgroundWorker worker_MSUpdate = null;
        private BackgroundWorker worker_Connect_Config = null;
        BackgroundWorker worker_Refresh_MS_Information = null;
        public CApplicationUI cApplicationUI = new CApplicationUI();
        public int previousMouseOverIndex;
        public int mouseOverIndex;
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        private List<CMiniserver> selected_Miniserver_befor_refresh;
        private List<string> languageList = null;
        string token = @"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";
        string url_github_Latest = @"https://api.github.com/repos/Jhonnay/WPF_ConfigUpdater/releases/latest";
        string UpdateVersion = null;

        public List<string> LanguageList { get => languageList; set => languageList = value; }

        public MainWindow()
        {
            InitializeComponent();
            worker_MSUpdate = new BackgroundWorker();
            worker_MSUpdate.WorkerReportsProgress = true;
            worker_MSUpdate.WorkerSupportsCancellation = true;
            worker_MSUpdate.DoWork += worker_DoWork_UpdateMSButton;
            worker_MSUpdate.ProgressChanged += worker_ProgressChanged_UpdateMSButton;
            worker_MSUpdate.RunWorkerCompleted += worker_RunWorkerCompleted_UpdateMSButton;

            worker_Connect_Config = new BackgroundWorker();
            worker_Connect_Config.WorkerSupportsCancellation = true;
            worker_Connect_Config.DoWork += worker_DoWork_OpenConfig;
            worker_Connect_Config.RunWorkerCompleted += worker_RunWorkerCompleted_OpenConfig;


            worker_Refresh_MS_Information = new BackgroundWorker();
            worker_Refresh_MS_Information.WorkerReportsProgress = true;
            worker_Refresh_MS_Information.WorkerSupportsCancellation = true;
            worker_Refresh_MS_Information.DoWork += worker_DoWork_RefreshMSInformation;
            worker_Refresh_MS_Information.ProgressChanged += worker_ProgressChanged_RefreshMSInformation;
            worker_Refresh_MS_Information.RunWorkerCompleted += worker_RunWorkerCompleted_RefreshMSInformation;


            listView_Miniserver.ItemsSource = miniserverList;
            LanguageList = Config.LanguageList;

            CheckBoxDisableUpdateDialogs.DataContext = cApplicationUI;
            RefreshButton.IsEnabled = false;
            UpdateButton.IsEnabled = false;
            RemoveMSButton.IsEnabled = false;

            
            //btn_klick_me.AddHandler(FrameworkElement.MouseDownEvent, new MouseButtonEventHandler(Button_MouseUp), true);
            //btn_klick_me.Click += test;
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
                    
                    miniserverList.ElementAt(index).MSConfiguration = MyConstants.Strings.Listview_Refresh_MS_Configuration_Error;
                }
                else if(ret_cLoxAppJson.gatewayType == 0)
                {
                    miniserverList.ElementAt(index).MSConfiguration = MyConstants.Strings.Listview_Refresh_MS_Configuration_Client;
                }
                else
                {
                    miniserverList.ElementAt(index).MSConfiguration = MyConstants.Strings.Listview_Refresh_MS_Configuration_ClientGateway;
                }

                miniserverList.ElementAt(index).MSProject = ret_cLoxAppJson.projectName;
            }
        }

        private string Format_Miniserver_String (string unformatedVersionString)
        {
            string formatedVersionString = "0.0.0.0";

            if(unformatedVersionString == "error")
            {
                return formatedVersionString; //e.g. Gen1 not reachable
            }

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
                    if(sub == "00")
                    {
                        sb.Append("0");
                    }
                    else
                    {
                        string temp = sub.TrimStart('0');
                        sb.Append(temp);
                    }

                    sb.Append('.');

                }
                sb.Remove(sb.Length-1,1);
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
            if(Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + MyConstants.Strings.Path_Loxone_Installation))
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + MyConstants.Strings.Path_Loxone_Installation;
            }
            else
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            }
            
            openFileDialog.Filter = "Executeable (*.exe)|*.exe";
            if(openFileDialog.ShowDialog() == true)
            {
                
                if (checkConfigEXE(openFileDialog.FileName)){
                    textblock_statusbar_config.Text = openFileDialog.FileName;
                    if (textblock_statusbar_config.Text != MyConstants.Strings.Statusbar_TextBlockConfig_No_Config_selected)
                    {
                        List<CMiniserver> list = new List<CMiniserver>();
                        for (int i = 0; i < miniserverList.Count; i++)
                        {
                            list.Add(miniserverList[i]);
                        }

                        FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(textblock_statusbar_config.Text);
                        Debug.WriteLine("Config Version: " + myFileVersionInfo.FileVersion);

                        colorMiniserverVersions(list, myFileVersionInfo);
                    }
                }
                else
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_ConfigExe_Not_Valid
                        , "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                
            }
        }

        private void listview_button_OpenConfig_Click(object sender, RoutedEventArgs e)
        {
            if (miniserverList[mouseOverIndex].MSStatus != MyConstants.Strings.Listview_MS_Status_AutoUpdate)
            {
                var processes = Process.GetProcessesByName(MyConstants.Strings.Process_Loxone_Config);

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


                if (textblock_statusbar_config.Text == MyConstants.Strings.Statusbar_TextBlockConfig_No_Config_selected)
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_OpenConfig_No_Config_selected, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (processes.Count() != 0)
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_ConnectConfigButton_ConfigsOpen_Error_Part1 + processes.Count() + MyConstants.Strings.MessageBox_ConnectConfigButton_ConfigsOpen_Error_Part2
                        , "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    config.ConfigPath = textblock_statusbar_config.Text;
                    //config.OpenConfigLoadProject();
                    if (!worker_Connect_Config.IsBusy)
                    {
                        worker_Connect_Config.RunWorkerAsync(config);
                    }
                    else
                    {
                        MessageBox.Show(MyConstants.Strings.MessageBox_Backgroundworker_Connect_Config_already_running , "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    
                }
            }
            else
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_ConnectConfigButton_AutoUpdate_Block, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }      
        }

        private void worker_DoWork_OpenConfig(object sender, DoWorkEventArgs e)
        {
            CConfigMSUpdate config = (CConfigMSUpdate)e.Argument; //workerdata
            config.OpenConfigLoadProject_Cancelable(worker_Connect_Config, miniserverList.ElementAt(mouseOverIndex).ConfigLanguage);
            if(worker_Connect_Config.CancellationPending)
            {
                e.Cancel = true;
            }
        }

        private void worker_RunWorkerCompleted_OpenConfig (object sender, RunWorkerCompletedEventArgs e)
        {
            if(e.Cancelled)
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_ConnecConfig_Button_Canceled, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                
                //Kill open configs
                var processes = Process.GetProcessesByName(MyConstants.Strings.Process_Loxone_Config);

                foreach (var instance in processes)
                {
                    instance.Kill();
                }

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
            var processes = Process.GetProcessesByName(MyConstants.Strings.Process_Loxone_Config);
            int index = previousMouseOverIndex;
            bool skipUpdate_AutoUpdate = false;
            bool skipUpdate_MS_updated_or_higher_Version = false;
            
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                listView_Miniserver.Items.SortDescriptions.Clear();

            }


            if (textblock_statusbar_config.Text != MyConstants.Strings.Statusbar_TextBlockConfig_No_Config_selected)
            {
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(textblock_statusbar_config.Text);
                Debug.WriteLine("Config Version: " + myFileVersionInfo.FileVersion);

                foreach (CMiniserver ms in listView_Miniserver.SelectedItems) 
                {
                    if (ms.MSStatus == MyConstants.Strings.Listview_MS_Status_AutoUpdate)//skip Updates if only 1 MS  is AutoUpdating
                    {
                        skipUpdate_AutoUpdate = true;
                    }
                    if (ms.MSVersion != MyConstants.Strings.StartUp_Listview_MS_Version)
                    {
                        if (checkIfUpdateNecessary( ms,  myFileVersionInfo))
                        {
                            skipUpdate_MS_updated_or_higher_Version = true;
                        }
                    }

                }

            }

            if (!skipUpdate_AutoUpdate && !skipUpdate_MS_updated_or_higher_Version)
            {
                if (processes.Count() == 0)
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

                    //BackgroundWorker worker_Refresh_MS_Information = new BackgroundWorker();
                    //worker_Refresh_MS_Information.WorkerReportsProgress = true;
                    //worker_Refresh_MS_Information.DoWork += worker_DoWork_UpdateMSButton;
                    //worker_Refresh_MS_Information.ProgressChanged += worker_ProgressChanged_UpdateMSButton;
                    //worker_Refresh_MS_Information.RunWorkerCompleted += worker_RunWorkerCompleted_UpdateMSButton;
                    if (!worker_MSUpdate.IsBusy)
                    {
                        listView_Miniserver.IsEnabled = false;
                        worker_MSUpdate.RunWorkerAsync(workerdata);
                    }
                    else
                    {
                        MessageBox.Show(MyConstants.Strings.MessageBox_ButtonUpdate_OtherUdpateRunning, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_ButtonUpdate_ConfigsOpen_Error_Part1 + processes.Count() + MyConstants.Strings.MessageBox_ButtonUpdate_ConfigsOpen_Error_Part2
                        , "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            else
            {
                if (skipUpdate_AutoUpdate)
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_UpdateButton_AutoUpdate_Block, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                if (skipUpdate_MS_updated_or_higher_Version)
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_UpdateButton_MS_updated_OR_higher_Version, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                
            }
        }

        private void worker_RunWorkerCompleted_UpdateMSButton(object? sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled)
            {
                try
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_Update_canceled + (int)e.Result + "/" + int_selectedItems_before_Refresh + MyConstants.Strings.MessageBox_Update_alreadyUpdatedMS
                        , "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }catch(InvalidOperationException ex)
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_Update_canceled, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                textblock_processStatus.Text = MyConstants.Strings.Statusbar_ProcessStatus_Update_Canceled;
                foreach (CMiniserver ms in listView_Miniserver.SelectedItems)
                {
                    if(miniserverList.ElementAt(miniserverList.IndexOf(ms)).MSStatus != MyConstants.Strings.Listview_Updated_MS_Status)
                    {
                        miniserverList.ElementAt(miniserverList.IndexOf(ms)).MSStatus = MyConstants.Strings.Statusbar_ProcessStatus_Update_Canceled;
                    }
                       
                   
                }

                //(sender as BackgroundWorker).ReportProgress(100);
                progressbar_ProcessStatus.Value = 100;
                ListView_GridView_Autoresize();

            }
            else
            {
                if(cApplicationUI.HandsfreeMode == false)
                {
                    MessageBox.Show((int)e.Result + "/" + int_selectedItems_before_Refresh + MyConstants.Strings.MessageBox_Update_alreadyUpdatedMS
                        , "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                
                if((int)e.Result == 0){
                    textblock_processStatus.Text = MyConstants.Strings.Statusbar_ProcessStatus_Update_failed;
                }else {
                    textblock_processStatus.Text = MyConstants.Strings.Statusbar_ProcessStatus_Update_Complete;
                }

                
            }
            progressbar_ProcessStatus.Value = 100;
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
                    if(cMiniserver.MSStatus == MyConstants.Strings.Listview_MS_Status_Updating)
                    {
                        miniserverList[index].MSStatus = MyConstants.Strings.Listview_MS_Status_Updating_Emoji;
                        textblock_processStatus.Text = MyConstants.Strings.Statusbar_ProcessStatus_Updating + cMiniserver.serialNumer;
                        //listView_Miniserver.IsEnabled = false;
                    }
                    if(cMiniserver.MSStatus == MyConstants.Strings.Listview_MS_Status_Update_finished)
                    {
                        RoutedEventArgs routedEventArgs = new RoutedEventArgs();

                        CLoxAppJson ret_cLoxAppJson;
                        string ret_MsVersion;
                        string updatelevel;
                        getMiniserverInformationsWebServices(cMiniserver,  worker_Refresh_MS_Information, out  ret_MsVersion, out var index1, out ret_cLoxAppJson, out updatelevel);

                       
                        if (ret_cLoxAppJson.gatewayType == 0)
                        {
                            miniserverList[index].MSConfiguration = MyConstants.Strings.Listview_Refresh_MS_Configuration_Standalone;
                        }
                        else
                        {
                            miniserverList[index].MSConfiguration = MyConstants.Strings.Listview_Refresh_MS_Configuration_ClientGateway;
                        }

                        miniserverList[index].MSProject = ret_cLoxAppJson.projectName + "/" + ret_cLoxAppJson.localUrl;
                        miniserverList[index].MSVersion = ret_MsVersion;
                        miniserverList[index].MSStatus = MyConstants.Strings.Listview_Updated_MS_Status;
                        miniserverList[index].UpdateLevel = updatelevel;
                        textblock_processStatus.Text =  MyConstants.Strings.Statusbar_ProcessStatus_Update_Complete_show_MS + cMiniserver.serialNumer;

                        if (textblock_statusbar_config.Text != MyConstants.Strings.Statusbar_TextBlockConfig_No_Config_selected)
                        {
                            FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(textblock_statusbar_config.Text);
                            Debug.WriteLine("Config Version: " + myFileVersionInfo.FileVersion);

                            List<CMiniserver> list = new List<CMiniserver>();
                            list.Add(miniserverList[index]);
                            colorMiniserverVersions(list, myFileVersionInfo);
                        }

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
                if (workerdata.ConfigPath == MyConstants.Strings.Statusbar_TextBlockConfig_No_Config_selected)
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_UpdateButton_No_Config_selected
                        , "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    
                }
                else
                {
                    int progressPercentage = Convert.ToInt32((double)result_MS_Update / list.Count * 100);
                    if(progressPercentage <= 99)
                    {
                        progressPercentage += 1;
                    }
                    string progressText = MyConstants.Strings.Listview_MS_Status_Updating;
                    ms.MSStatus = progressText;
                    (sender as BackgroundWorker).ReportProgress(progressPercentage, ms);
                    update.ConfigPath = workerdata.ConfigPath;
                    if (worker_MSUpdate.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }

                    ArrayList arrlist = update.UpdateMS(worker_MSUpdate, ms.ConfigLanguage);
                    result_MS_Update++;
                    

                    if (cApplicationUI.HandsfreeMode == false && worker_MSUpdate.CancellationPending == false)
                    {
                        string concat_strings = string.Join(" ", arrlist.Cast<string>().ToArray());
                        MessageBox.Show(MyConstants.Strings.MessageBox_Update_Show_all_updatedMS_Versions + concat_strings
                            , "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        
                    }

                    if (worker_MSUpdate.CancellationPending == true)
                    {
                        e.Cancel = true;
                        return;
                    }

                    progressPercentage = Convert.ToInt32((double)result_MS_Update / list.Count * 100);

                    progressText = MyConstants.Strings.Listview_MS_Status_Update_finished;
                    ms.MSStatus = progressText;
                    (sender as BackgroundWorker).ReportProgress(progressPercentage, ms);
                }
                
            }
            
            e.Result = result_MS_Update;
        }

        


        private void Button_RefreshMS_Click(object sender, RoutedEventArgs e)
        {
            if (listViewSortCol != null)
            {
                AdornerLayer.GetAdornerLayer(listViewSortCol).Remove(listViewSortAdorner);
                listView_Miniserver.Items.SortDescriptions.Clear();

            }

            progressbar_ProcessStatus.Value = 0;
            textblock_processStatus.Text = MyConstants.Strings.Statusbar_ProcessStatus_Refresh_inProgress_Text;
            //StackPaneButtons.IsEnabled = false; //Disables all containing Buttons
            

            Task.Run(() =>
            {

                // Update UI elements on main UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    UpdateButton.IsEnabled = false;
                    RefreshButton.IsEnabled = false;
                    RemoveMSButton.IsEnabled = false;
                    InsertMSButton.IsEnabled = false;
                    CancelUpdateButton.IsEnabled = true;
                });
            });



            int_selectedItems_before_Refresh = listView_Miniserver.SelectedItems.Count;

            List<CMiniserver> list = new List<CMiniserver> { };
            foreach(CMiniserver ms in listView_Miniserver.SelectedItems)
            {
                ms.MSStatus = MyConstants.Strings.StartUp_Listview_MS_Version;
                list.Add(ms);
            }
            listView_Miniserver.IsEnabled = false;

            Thread.Sleep(200);

            selected_Miniserver_befor_refresh = list;

            
            worker_Refresh_MS_Information.RunWorkerAsync(list);
        }

        public void colorMiniserverVersions(List<CMiniserver> selected_miniservers, FileVersionInfo myFileVersionInfo)
        {
            Debug.WriteLine("Config Version: " + myFileVersionInfo.FileVersion);
            string correctedConfigVersion = addLeadingZeoresToVersionNumber(myFileVersionInfo.FileVersion).Replace(".", "");
            

            foreach (CMiniserver ms in selected_miniservers)
            {
                if (ms.MSVersion != MyConstants.Strings.StartUp_Listview_MS_Version)
                {
                    string correctedMiniserverVersion = addLeadingZeoresToVersionNumber(ms.MSVersion).Replace(".", "");
                    if (int.Parse(correctedConfigVersion) > int.Parse(correctedMiniserverVersion))
                    {
                        miniserverList.ElementAt(miniserverList.IndexOf(ms)).VersionColor = "orange";
                    }
                    else if (int.Parse(correctedConfigVersion) == int.Parse(correctedMiniserverVersion))
                    {
                        miniserverList.ElementAt(miniserverList.IndexOf(ms)).VersionColor = "green";
                    }
                    else
                    {
                        miniserverList.ElementAt(miniserverList.IndexOf(ms)).VersionColor = "black";
                    }
                }
                else
                {
                    miniserverList.ElementAt(miniserverList.IndexOf(ms)).VersionColor = "black";
                }
            }

        }
        
        public static bool checkConfigEXE(string configFilePath)
        {
            if(FileVersionInfo.GetVersionInfo(configFilePath).FileVersion != null 
                && FileVersionInfo.GetVersionInfo(configFilePath).ProductName == "LoxoneConfig" )
            {
                return true;
            }
            else
            {
                return false;
            }
           
        }
        public bool checkIfUpdateNecessary(CMiniserver ms, FileVersionInfo myFileVersionInfo)
        {
            string msVersionString = ms.MSVersion;
            string correctedMiniserverVersion = addLeadingZeoresToVersionNumber(msVersionString).Replace(".", "");
            string correctedConfigVersion = addLeadingZeoresToVersionNumber(myFileVersionInfo.FileVersion).Replace(".", "");


            if (int.Parse(correctedMiniserverVersion) > int.Parse(correctedConfigVersion))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        private static string addLeadingZeoresToVersionNumber(string msVersionString)
        {
            if(msVersionString != null)
            {
                string[] segments = msVersionString.Split('.');

                for (int i = 0; i < segments.Length; i++)
                {
                    if (segments[i].Length == 1)
                    {
                        segments[i] = "0" + segments[i];
                    }
                }
                string correctedVresion = string.Join(".", segments);
                return correctedVresion;
            }
            return msVersionString;
        }

        private void worker_RunWorkerCompleted_RefreshMSInformation(object sender, RunWorkerCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                MessageBox.Show((int)e.Result + "/" + int_selectedItems_before_Refresh + MyConstants.Strings.MessageBox_Refresh_Information_pulled
                    , "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                textblock_processStatus.Text = MyConstants.Strings.Statusbar_ProcessStatus_Refresh_Information_pulled_Text;
            }
            else
            {
                textblock_processStatus.Text = MyConstants.Strings.Statusbar_ProcessStatus_Refresh_Canceled;
            }

            Task.Run(() =>
            {

                // Update UI elements on main UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    StackPaneButtons.IsEnabled = true;
                    InsertMSButton.IsEnabled = true;
                    listView_Miniserver.IsEnabled = true;
                });
            });

            

            if(textblock_statusbar_config.Text != MyConstants.Strings.Statusbar_TextBlockConfig_No_Config_selected && !e.Cancelled)
            {
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(textblock_statusbar_config.Text);
                colorMiniserverVersions(selected_Miniserver_befor_refresh, myFileVersionInfo);
            }
            else if(e.Cancelled)
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_Button_Refresh_canceled, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }            
        }

        private void worker_ProgressChanged_RefreshMSInformation(object sender, ProgressChangedEventArgs e)
        {
            progressbar_ProcessStatus.Value = e.ProgressPercentage;
            
            if (e.UserState != null)
            {

                CMiniserver cMiniserver = (CMiniserver)e.UserState;
                textblock_processStatus.Text =  MyConstants.Strings.Statusbar_ProcessStatus_Refresh_inProgress_MS + cMiniserver.serialNumer;
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
                        cMiniserver.MSStatus = MyConstants.Strings.Listview_MS_Status_retreiving_information;
                    }
                    else if (cMiniserver.MSVersion != "0.0.0.0")
                    {
                        cMiniserver.MSStatus = MyConstants.Strings.Listview_MS_Status_retreiving_information_successfull;
                        
                    }
                    else
                    {
                        cMiniserver.MSStatus = MyConstants.Strings.Listview_MS_Status_retreiving_information_timeout;
                       
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
                    string updatelevel; 
                    int index = -1;
                    CLoxAppJson ret_cLoxAppJson;
                    if (worker_Refresh_MS_Information.CancellationPending)
                    {
                        e.Cancel = true;
                        return; 
                    }

                    getMiniserverInformationsWebServices(ms, worker_Refresh_MS_Information, out ret_MsVersion, out index, out ret_cLoxAppJson, out updatelevel);

                    CMiniserver cMiniserver = ms;
                    cMiniserver.MSVersion = ret_MsVersion;
                    cMiniserver.UpdateLevel = updatelevel;

                    if (ret_MsVersion == "0.0.0.0")
                    {

                        cMiniserver.MSConfiguration = MyConstants.Strings.Listview_Refresh_MS_Configuration_Error;
                    }
                    else if (ret_cLoxAppJson.gatewayType == 0)
                    {
                        cMiniserver.MSConfiguration = MyConstants.Strings.Listview_Refresh_MS_Configuration_Standalone;
                        result_MS_Refreshed++;
                    }
                    else
                    {
                        cMiniserver.MSConfiguration = MyConstants.Strings.Listview_Refresh_MS_Configuration_ClientGateway;
                        result_MS_Refreshed++;
                    }

                    cMiniserver.MSProject = ret_cLoxAppJson.projectName + "/" + ret_cLoxAppJson.localUrl;

                    if (worker_Refresh_MS_Information.CancellationPending)
                    {
                        e.Cancel = true;
                        miniserverList[index].MSStatus = "canceled";
                        (sender as BackgroundWorker).ReportProgress(100); 
                        return;
                    }

                    progressPercentage = Convert.ToInt32((double)result_MS_Refreshed / list.Count * 100);
                    (sender as BackgroundWorker).ReportProgress(progressPercentage, cMiniserver);
                    System.Threading.Thread.Sleep(100);
                }
                (sender as BackgroundWorker).ReportProgress(100); //Fix even if retreival not possible for some MS --> show 100%
                e.Result = result_MS_Refreshed;
            }
           
        }

        private void getMiniserverInformationsWebServices(CMiniserver ms, BackgroundWorker worker_Refresh_MS_Information,  out string ret_MsVersion, out int index, out CLoxAppJson ret_cLoxAppJson, out string Updatelevel)
        {
            ret_cLoxAppJson = new CLoxAppJson();
            ret_cLoxAppJson.projectName = "";
            Updatelevel = "canceled";
            ret_MsVersion = "0.0.0.0";
            index = miniserverList.IndexOf(ms);

            if (ms.LocalIPAdress != "" && ms.LocalIPAdress != null)
            {
                
                if (!worker_Refresh_MS_Information.CancellationPending)
                {
                    ret_MsVersion = WebService.sendCommandRest_Version_Local_Gen1(ms.LocalIPAdress, ms.adminUser, ms.adminPassWord, @"/dev/sys/version", "value");
                    System.Diagnostics.Debug.WriteLine(ret_MsVersion + "- " + ms.serialNumer);
                    index = miniserverList.IndexOf(ms);
                    ret_MsVersion = Format_Miniserver_String(ret_MsVersion);
                    miniserverList.ElementAt(index).MSVersion = ret_MsVersion;
                }

                if (!worker_Refresh_MS_Information.CancellationPending)
                {
                    ret_cLoxAppJson = WebService.sendCommandRest_LoxAppJson_Local_Gen1(ms.LocalIPAdress, ms.adminUser, ms.adminPassWord, @"/data/LoxAPP3.json");
                }

                if (!worker_Refresh_MS_Information.CancellationPending)
                {
                    Updatelevel = WebService.sendCommandRest_Version_Local_Gen1(ms.LocalIPAdress, ms.adminUser, ms.adminPassWord, @"/dev/cfg/updatelevel", "value");
                }                
            }
            else
            {
                if (!worker_Refresh_MS_Information.CancellationPending)
                {
                    ret_MsVersion = WebService.sendCommandRest_Version_Remote_Cloud(ms.serialNumer, ms.adminUser, ms.adminPassWord, @"/dev/sys/version", "value");
                    System.Diagnostics.Debug.WriteLine(ret_MsVersion + "- " + ms.serialNumer);
                    index = miniserverList.IndexOf(ms);
                    ret_MsVersion = Format_Miniserver_String(ret_MsVersion);
                    miniserverList.ElementAt(index).MSVersion = ret_MsVersion;
                }

                if (!worker_Refresh_MS_Information.CancellationPending)
                {
                    ret_cLoxAppJson = WebService.sendCommandRest_LoxAppJson_Remote_Cloud(ms.serialNumer, ms.adminUser, ms.adminPassWord, @"/data/LoxAPP3.json");
                }

                if (!worker_Refresh_MS_Information.CancellationPending)
                {
                    Updatelevel = WebService.sendCommandRest_Version_Remote_Cloud(ms.serialNumer, ms.adminUser, ms.adminPassWord, @"/dev/cfg/updatelevel", "value");
                }
                   
            }

            if (!worker_Refresh_MS_Information.CancellationPending)
            {
                if (Updatelevel.IndexOf("\"") > 0)
                {
                    Updatelevel = Updatelevel.Remove(Updatelevel.IndexOf("\""));
                }

                if (Updatelevel == "-1213")
                {
                    Updatelevel = MyConstants.Strings.Listview_Refresh_MS_Configuration_Error;
                }
            }
        }

        private void listView_Miniserver_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            if(listView.SelectedIndex == -1)
            {
                Task.Run(() =>
                {

                    // Update UI elements on main UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        RefreshButton.IsEnabled = false;
                        UpdateButton.IsEnabled = false;
                        RemoveMSButton.IsEnabled = false;
                    });
                });
                
            }
            else
            {
                Task.Run(() =>
                {

                    // Update UI elements on main UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        RefreshButton.IsEnabled = true;
                        UpdateButton.IsEnabled = true;
                        RemoveMSButton.IsEnabled = true;
                        InsertMSButton.IsEnabled = true;
                    });
                });
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
                result.MSStatus = MyConstants.Strings.StartUp_Listview_MS_Status;
                result.MSVersion = MyConstants.Strings.StartUp_Listview_MS_Version;
                result.VersionColor = "Black";
                result.ConfigLanguage = "5";
                miniserverList.Add(result);
            }
        }

        private void Button_CancelUpdate_Click(object sender, RoutedEventArgs e)
        {
            worker_MSUpdate.CancelAsync();
            worker_Connect_Config.CancelAsync();
            worker_Refresh_MS_Information.CancelAsync();
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
                MessageBox.Show(MyConstants.Strings.MessageBox_Refresh_Context_Copy_SNR_Error, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
                link = MyConstants.Strings.Link_CloudDNS + snr + "/dev/fsget/log/def.log";
            }

            OpenLinkinDefaultBrowser(link);
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
                result.MSStatus = MyConstants.Strings.Listview_MS_Status_Edited;
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
            if(File.Exists(Directory.GetCurrentDirectory() + MyConstants.Strings.Path_Changelog))
            {
                System.Diagnostics.Process.Start("notepad.exe", Directory.GetCurrentDirectory() + MyConstants.Strings.Path_Changelog);

            }
            else
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_Changelog_Cannot_be_opened, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            string link =  MyConstants.Strings.Link_CloudDNS + currentlySelectedMiniserver.serialNumer;

            OpenLinkinDefaultBrowser(link);

        }

        private static void OpenLinkinDefaultBrowser(string link)
        {
            Process cmd = new Process();
            cmd.StartInfo.FileName = "cmd.exe";
            cmd.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            cmd.StartInfo.Arguments = "/c start \"\" " + link;
            cmd.StartInfo.CreateNoWindow = true;

            cmd.Start();
        }

        

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            //Starts default Browser
            int index = mouseOverIndex;

            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(mouseOverIndex);
            string link = "www.loxone.com";

            if (currentlySelectedMiniserver.LocalIPAdress != "" && currentlySelectedMiniserver.LocalIPAdress != null)
            {
                link = @"http://" + currentlySelectedMiniserver.LocalIPAdress;
            }
            else
            {
                link = MyConstants.Strings.Link_CloudDNS + currentlySelectedMiniserver.serialNumer;
            }



            OpenLinkinDefaultBrowser(link);
        }

        private void menuItem_Settings(object sender, RoutedEventArgs e)
        {
            ApplicationSettingsDialog dialog;
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + MyConstants.Strings.Path_ApplicationSettings))
            {
                string strJson = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + MyConstants.Strings.Path_ApplicationSettings);
                ApplicationSettings settings = System.Text.Json.JsonSerializer.Deserialize<ApplicationSettings>(strJson);
                dialog= new ApplicationSettingsDialog(settings);
            }
            else
            {
                dialog = new ApplicationSettingsDialog();
            }
            

            if (dialog.ShowDialog() == true)
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_Applicationsettings_saved, "Settings Dialog", MessageBoxButton.OK, MessageBoxImage.Information);
                textblock_statusbar_config.Text = dialog.Answer;

            }
            else
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_Applicationsettings_Not_saved, "Settings Dialog", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ContentControl_textblock_statusbar_menuItem_Start_Config(object sender, RoutedEventArgs e)
        {
            if (textblock_statusbar_config.Text != MyConstants.Strings.Statusbar_TextBlockConfig_No_Config_selected)
            {
                Config config = new Config();
                config.startConfig_Language(textblock_statusbar_config.Text,Config.LanguageList.IndexOf("ENU").ToString()); //english
            }
            else
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_OpenConfig_No_Config_selected, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
                
        }

        private void ContentControl_textblock_statusbar_menuItem_Kill_Configs(object sender, RoutedEventArgs e)
        {
            var processes = Process.GetProcessesByName(MyConstants.Strings.Process_Loxone_Config);

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
            string jsonString = System.Text.Json.JsonSerializer.Serialize(miniserverList, new JsonSerializerOptions { WriteIndented = true });
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "JSON file (*.json)|*.json";

            if (!Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + MyConstants.Strings.Path_Folder_for_ApplicationData))
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + MyConstants.Strings.Path_Folder_for_ApplicationData);
                
            }
           

            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + MyConstants.Strings.Path_Folder_for_ApplicationData;


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
            openFileDialog.Title = "Select a Configuration File with Minservers";

            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + MyConstants.Strings.Path_Folder_for_ApplicationData)) 
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + MyConstants.Strings.Path_Folder_for_ApplicationData;
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
                    ObservableCollection<CMiniserver> miniservers = System.Text.Json.JsonSerializer.Deserialize<ObservableCollection<CMiniserver>>(jsonString);

                    if (miniservers != null && miniservers.Count != 0)
                    {
                        for (int i = 0; i < miniservers.Count; i++)
                        {
                            miniservers[i].MSStatus = MyConstants.Strings.StartUp_Listview_MS_Status;
                            miniservers[i].MSVersion = MyConstants.Strings.StartUp_Listview_MS_Version;
                            miniservers[i].VersionColor = "black";
                        }

                        miniserverList = miniservers;
                        listView_Miniserver.ItemsSource = miniserverList;
                        //MenuItem_MSVersionRefresh_Click(sender, e);
                        ListView_GridView_Autoresize();
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_Miniserver_Json_not_valid, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            previousMouseOverIndex = mouseOverIndex;
            Debug.WriteLine("Tunnel! Selected Index = " + mouseOverIndex + " previous = " + previousMouseOverIndex);
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
            //previousMouseOverIndex = mouseOverIndex;
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

        private void ContextUpdateLevelSetAlpha(object sender, RoutedEventArgs e)
        {
            int index = previousMouseOverIndex;
            string link,updatelevel; 
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);


            Task.Run(() =>
            {
                if (currentlySelectedMiniserver.LocalIPAdress != "" && currentlySelectedMiniserver.LocalIPAdress != null)
                {

                    WebService.sendCommandRest_Version_Local_Gen1(currentlySelectedMiniserver.LocalIPAdress, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel/alpha", "value");
                    updatelevel = WebService.sendCommandRest_Version_Local_Gen1(currentlySelectedMiniserver.LocalIPAdress, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel", "value");
                }
                else
                {
                    WebService.sendCommandRest_Version_Remote_Cloud(currentlySelectedMiniserver.serialNumer, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel/alpha", "value");
                    updatelevel = WebService.sendCommandRest_Version_Remote_Cloud(currentlySelectedMiniserver.serialNumer, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel", "value");
                }

                if (updatelevel.IndexOf("\"") > 0)
                {
                    updatelevel = updatelevel.Remove(updatelevel.IndexOf("\""));
                }
                else if (updatelevel == "-1213")
                {
                    updatelevel = MyConstants.Strings.Listview_Refresh_MS_Configuration_Error;
                }



                if (updatelevel == "Alpha")
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_UpdateLevelSet_Successfully + updatelevel, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_UpdateLevelSet_Error + updatelevel, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                miniserverList[index].UpdateLevel = updatelevel;
                // ...

                // Update UI elements on main UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Update UI elements
                    // ...
                });
            });

            
            
        }

        private void ContextUpdateLevelRelease(object sender, RoutedEventArgs e)
        {
            int index = previousMouseOverIndex;
            string link, updatelevel;
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);


            Task.Run(() =>
            {
                // WPF commands to be executed in separate thread
                if (currentlySelectedMiniserver.LocalIPAdress != "" && currentlySelectedMiniserver.LocalIPAdress != null)
                {

                    WebService.sendCommandRest_Version_Local_Gen1(currentlySelectedMiniserver.LocalIPAdress, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel/default", "value");
                    updatelevel = WebService.sendCommandRest_Version_Local_Gen1(currentlySelectedMiniserver.LocalIPAdress, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel", "value");
                }
                else
                {
                    WebService.sendCommandRest_Version_Remote_Cloud(currentlySelectedMiniserver.serialNumer, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel/default", "value");
                    updatelevel = WebService.sendCommandRest_Version_Remote_Cloud(currentlySelectedMiniserver.serialNumer, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel", "value");
                }

                if (updatelevel.IndexOf("\"") > 0)
                {
                    updatelevel = updatelevel.Remove(updatelevel.IndexOf("\""));
                }
                else if (updatelevel == "-1213")
                {
                    updatelevel = MyConstants.Strings.Listview_Refresh_MS_Configuration_Error;
                }


                if (updatelevel == "Release")
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_UpdateLevelSet_Successfully + updatelevel, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_UpdateLevelSet_Error + updatelevel, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                miniserverList[index].UpdateLevel = updatelevel;

                // Update UI elements on main UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Update UI elements
                    // ...
                });
            });


            

        }
        private void ContextUpdateLevelBeta(object sender, RoutedEventArgs e)
        {
            int index = previousMouseOverIndex;
            string link, updatelevel;
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);

            Task.Run(() =>
            {
                // WPF commands to be executed in separate thread
                if (currentlySelectedMiniserver.LocalIPAdress != "" && currentlySelectedMiniserver.LocalIPAdress != null)
                {

                    WebService.sendCommandRest_Version_Local_Gen1(currentlySelectedMiniserver.LocalIPAdress, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel/beta", "value");
                    updatelevel = WebService.sendCommandRest_Version_Local_Gen1(currentlySelectedMiniserver.LocalIPAdress, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel", "value");
                }
                else
                {
                    WebService.sendCommandRest_Version_Remote_Cloud(currentlySelectedMiniserver.serialNumer, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel/beta", "value");
                    updatelevel = WebService.sendCommandRest_Version_Remote_Cloud(currentlySelectedMiniserver.serialNumer, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel", "value");
                }

                if (updatelevel.IndexOf("\"") > 0)
                {
                    updatelevel = updatelevel.Remove(updatelevel.IndexOf("\""));
                }
                else if (updatelevel == "-1213")
                {
                    updatelevel = MyConstants.Strings.Listview_Refresh_MS_Configuration_Error;
                }



                if (updatelevel == "Beta")
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_UpdateLevelSet_Successfully + updatelevel, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_UpdateLevelSet_Error + updatelevel, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                miniserverList[index].UpdateLevel = updatelevel;

                // Update UI elements on main UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Update UI elements
                    // ...
                });
            });


            
        }
        private void ContextUpdteLevelUpdateToLevel(object sender, RoutedEventArgs e)
        {
            int index = previousMouseOverIndex;
            string link, returnCode;
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);

            if (currentlySelectedMiniserver.MSStatus != MyConstants.Strings.Listview_MS_Status_AutoUpdate)
            {
                Task.Run(() =>
                {
                    // WPF commands to be executed in separate thread
                    if (currentlySelectedMiniserver.MSConfiguration == MyConstants.Strings.Listview_Refresh_MS_Configuration_Standalone)
                    {
                        if (currentlySelectedMiniserver.LocalIPAdress != "" && currentlySelectedMiniserver.LocalIPAdress != null)
                        {

                            returnCode = WebService.sendCommandRest_Version_Local_Gen1(currentlySelectedMiniserver.LocalIPAdress, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/sys/autoupdate", "Code");
                            //updatelevel = WebService.sendCommandRest_Version_Local_Gen1(currentlySelectedMiniserver.LocalIPAdress, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel", "value");
                        }
                        else
                        {
                            returnCode = WebService.sendCommandRest_Version_Remote_Cloud(currentlySelectedMiniserver.serialNumer, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/sys/autoupdate", "Code");
                            //updatelevel = WebService.sendCommandRest_Version_Remote_Cloud(currentlySelectedMiniserver.serialNumer, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/cfg/updatelevel", "value");
                        }

                        if (returnCode.IndexOf("\"") > 0)
                        {
                            returnCode = returnCode.Remove(returnCode.IndexOf("\""));
                        }

                        if (returnCode == MyConstants.Strings.WebService_Success_Code) //WebService Successful
                        {
                            MessageBox.Show(MyConstants.Strings.MessageBox_AutoUpdate_Startet, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                            miniserverList[index].MSStatus = MyConstants.Strings.Listview_MS_Status_AutoUpdate;
                        }
                        else
                        {
                            MessageBox.Show(MyConstants.Strings.MessageBox_AutoUpdate_Not_Startet, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }

                    }
                    else
                    {
                        MessageBox.Show(MyConstants.Strings.MessageBox_AutoUpdate_Not_Possible, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                    // Update UI elements on main UI thread
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        // Update UI elements
                        // ...
                    });
                });

            }
            else
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_AutoUpdate_Already_running, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }


            }

        private void MenuItem_Open_Loxone_App(object sender, RoutedEventArgs e)
        {
            var processes = Process.GetProcessesByName(MyConstants.Strings.Process_Loxone_App);

            if(processes.Count() != 0)
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_App_already_open, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if(File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)+ MyConstants.Strings.Path_Folder_for_Loxone_App))
                {
                    Process process = new Process();
                    process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + MyConstants.Strings.Path_Folder_for_Loxone_App;
                    process.Start();
                }
                else
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_App_not_installed, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
                
        }

        private void MenuItem_Loxone_App_Kill_Instances(object sender, RoutedEventArgs e)
        {
            var processes = Process.GetProcessesByName(MyConstants.Strings.Process_Loxone_App);

            foreach(var process in processes)
            {
                process.Kill();
            }
        }

        private void MenuItem_Click_Loxone_App_Debug(object sender, RoutedEventArgs e)
        {
            var processes = Process.GetProcessesByName(MyConstants.Strings.Process_Loxone_App);

            if (processes.Count() != 0)
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_App_already_open, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + MyConstants.Strings.Path_Folder_for_Loxone_App))
                {
                    Process process = new Process();
                    process.StartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + MyConstants.Strings.Path_Folder_for_Loxone_App;
                    process.StartInfo.Arguments = " --debug";
                    process.Start();
                }
                else
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_App_not_installed, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private void ContextMenu_FTP(object sender, RoutedEventArgs e)
        {
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);

            if (currentlySelectedMiniserver.LocalIPAdress != "" && currentlySelectedMiniserver.LocalIPAdress != null)
            {
                string link = @"explorer ftp://" + currentlySelectedMiniserver.adminUser + ":" + currentlySelectedMiniserver.adminPassWord + "@" + currentlySelectedMiniserver.LocalIPAdress ;

                Ping myPing = new Ping();
                PingReply reply = myPing.Send(currentlySelectedMiniserver.LocalIPAdress, 1000);
                if (reply != null && reply.Status.ToString() != "TimedOut")
                {
                    string ret_MsVersion = "0.0.0.0";
                    ret_MsVersion = WebService.sendCommandRest_Version_Local_Gen1(currentlySelectedMiniserver.LocalIPAdress, currentlySelectedMiniserver.adminUser, currentlySelectedMiniserver.adminPassWord, @"/dev/sys/version", "value");

                    if (ret_MsVersion != "-1213")
                    {
                        OpenLinkinDefaultBrowser(link);
                    }
                    else
                    {
                        MessageBox.Show(MyConstants.Strings.MessageBox_FTP_Local_authentification_failed, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }

                }
                else
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_FTP_Local_Ping_failed, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            else
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_FTP_Local_IP_not_defined, "Information", MessageBoxButton.OK, MessageBoxImage.Information);       
            }
        }

        private void ContextMenu_LPH(object sender, RoutedEventArgs e)
        {
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);
            string link = MyConstants.Strings.Link_LPH + currentlySelectedMiniserver.serialNumer;

            OpenLinkinDefaultBrowser(link);
        }

        private void ContextMenu_Copy_Password(object sender, RoutedEventArgs e)
        {
            int index = previousMouseOverIndex;
            if (index != -1)
            {
                string password = miniserverList.ElementAt(previousMouseOverIndex).adminPassWord;
                Clipboard.SetText(password);
            }
            else
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_Refresh_Context_Copy_SNR_Error, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ContextMenu_CrashLogServer(object sender, RoutedEventArgs e)
        {
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);
            string link = MyConstants.Strings.Link_CrashLog_Server + currentlySelectedMiniserver.serialNumer;

            OpenLinkinDefaultBrowser(link);
        }

        private void ContextMenu_Project_Copy_Local_IP(object sender, RoutedEventArgs e)
        {
            int index = previousMouseOverIndex;

            string localIP_from_project = miniserverList.ElementAt(index).MSProject;
            if(localIP_from_project != null)
            {
                localIP_from_project = localIP_from_project.Remove(0, localIP_from_project.IndexOf("/") + 1);

                Clipboard.SetText(localIP_from_project);
            }
        }

        private void ContextMenu_Project_Copy_Local_IP_to_Collumn(object sender, RoutedEventArgs e)
        {
            int index = previousMouseOverIndex;

            string localIP_from_project = miniserverList.ElementAt(index).MSProject;
            if( localIP_from_project != null )
            {
                localIP_from_project = localIP_from_project.Remove(0, localIP_from_project.IndexOf("/") + 1);
                miniserverList[index].LocalIPAdress = localIP_from_project;
            }
        }

        private void ContextMenu_LoxoneApp_Connect_MS(object sender, RoutedEventArgs e)
        {
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);
            //MenuItem_Loxone_App_Kill_Instances(sender,e);
            string link = "loxone://ms?host=192.168.178.62&usr=admin&pwd=SlavaDomnului2021!";




            if (currentlySelectedMiniserver.LocalIPAdress != "" && currentlySelectedMiniserver.LocalIPAdress != null)
            {
                link = "loxone://ms?host=" + currentlySelectedMiniserver.LocalIPAdress +
                    "&usr=" + currentlySelectedMiniserver.adminUser +
                    "&pwd=" + currentlySelectedMiniserver.adminPassWord;

            }
            else
            {
                link = "loxone://ms?mac=" + currentlySelectedMiniserver.serialNumer +
                    "&usr=" + currentlySelectedMiniserver.adminUser +
                    "&pwd=" + currentlySelectedMiniserver.adminPassWord; 
            }



            //OpenLinkinDefaultBrowser(HttpUtility.UrlEncode(link));
            OpenLinkinDefaultBrowser(link);
        }

        private void Application_Help_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = true;
        }

        private void Application_Help_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Window webbrowserWindow = new Window();
            webbrowserWindow.Title = "Documentation (BETA)";
            //webbrowserWindow.Padding = new Thickness(10);

            WebBrowser webBrowser = new WebBrowser();
            webBrowser.Source = new Uri(System.IO.Path.Combine(Directory.GetCurrentDirectory(), "resources\\Documentation.html"));
            webbrowserWindow.Width = 1000;
            webbrowserWindow.Height = 800;
            webbrowserWindow.Padding= new Thickness(10);

            webbrowserWindow.Content = webBrowser;
            webbrowserWindow.Show();

            //webBrowser.ContextMenu.IsEnabled = false;
        }

        private void ContextMenu_Autoupdate_selected(object sender, RoutedEventArgs e)
        {
            string returnCode;
           

            List<CMiniserver> list = listView_Miniserver.SelectedItems.Cast<CMiniserver>().ToList();

            bool not_standalone = false;
            bool already_autoupdate = false;

            if(listView_Miniserver.SelectedItems.Count <= 0)
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_AutoUpdate_selected_ERROR_nothing_selected, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            foreach(CMiniserver miniserver in list)
            {
                if(miniserver.MSStatus == MyConstants.Strings.Listview_MS_Status_AutoUpdate)
                {
                    already_autoupdate = true;
                }
                if(miniserver.MSConfiguration == MyConstants.Strings.Listview_Refresh_MS_Configuration_ClientGateway)
                {
                    not_standalone = true;
                }
            }

            if(not_standalone)
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_AutoUpdate_seleceted_Not_Possible, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if(already_autoupdate)
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_AutoUpdate_selected_Already_running, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }





            Task.Run(() =>
            {
                List<string> unsuccessful_autoupdate_SNR = new List<string>();

                foreach (CMiniserver miniserver in list)
                {
                    if (miniserver.LocalIPAdress != "" && miniserver.LocalIPAdress != null)
                    {

                        returnCode = WebService.sendCommandRest_Version_Local_Gen1(miniserver.LocalIPAdress, miniserver.adminUser, miniserver.adminPassWord, @"/dev/sys/autoupdate", "Code");
                        
                    }
                    else
                    {
                        returnCode = WebService.sendCommandRest_Version_Remote_Cloud(miniserver.serialNumer, miniserver.adminUser, miniserver.adminPassWord, @"/dev/sys/autoupdate", "Code");
                        
                    }

                    if (returnCode.IndexOf("\"") > 0)
                    {
                        returnCode = returnCode.Remove(returnCode.IndexOf("\""));
                    }

                    if (returnCode == MyConstants.Strings.WebService_Success_Code)
                    {
                        miniserverList[miniserverList.IndexOf(miniserver)].MSStatus = MyConstants.Strings.Listview_MS_Status_AutoUpdate;
                        }
                    else
                    {
                        unsuccessful_autoupdate_SNR.Add(miniserver.serialNumer);
                    }

                }

                if(unsuccessful_autoupdate_SNR.Count>0)
                {
                    string message = MyConstants.Strings.MessageBox_AutoUpdate_selected_Error_which_could_not_update + "\n";
                    foreach(string str in unsuccessful_autoupdate_SNR)
                    {
                        message = message + str + "\n";
                    }

                    MessageBox.Show(message, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

                // Update UI elements on main UI thread
                Application.Current.Dispatcher.Invoke(() =>
                {
                    // Update UI elements
                    // ...
                });
            });
        }

        public async void Help_Check_for_Updates(object sender, RoutedEventArgs e)
        {
            bool updatesAvailable = await check_Update_needed();
            {
                if(updatesAvailable)
                {
                    string message = "Theres is a newer Version available! Do you want to download and install?" + "\nCurrent Version: " + stringApplicationVersion + "\nAvailable Version: " + UpdateVersion;
                    if (MessageBox.Show(message, "Install Update?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        //do no stuff
                        string downloadPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads\MiniserverUpdater.msi";
                        await InstallLatestRelease(url_github_Latest, downloadPath, token);
                    }
                    

                }else
                {
                    MessageBox.Show("No new updates available. ", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }

            }
            

        }

        public async void Help_Check_for_Updates_On_Startup()
        {
            bool updatesAvailable = await check_Update_needed();
            {
                if (updatesAvailable)
                {
                    string message = "Theres is a newer Version available! Do you want to download and install?" + "\nCurrent Version: " + stringApplicationVersion + "\nAvailable Version: " + UpdateVersion;
                    if (MessageBox.Show(message, "Install Update?", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                    {
                        //do no stuff
                        string downloadPath = Environment.GetEnvironmentVariable("USERPROFILE") + @"\Downloads\MiniserverUpdater.msi";
                        await InstallLatestRelease(url_github_Latest, downloadPath, token);
                    }
                }
            }
        }


        public static async Task InstallLatestRelease(string url, string downloadPath, string personalAccessToken)
        {
            try
            {
                // Create an HttpClient instance
                using (HttpClient client = new HttpClient())
                {
                    // Set the user agent and accept header
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));

                    // Set the authorization header if a personal access token is provided
                    if (!string.IsNullOrEmpty(personalAccessToken))
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", personalAccessToken);
                    }

                    // Send a GET request to the URL and deserialize the response
                    HttpResponseMessage response = await client.GetAsync(url);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    dynamic release = JsonConvert.DeserializeObject(responseBody);

                    var assests = release.assets;
                    string downloadUrl = release.assets[0].browser_download_url;

                    // Download the asset to the specified path

                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", personalAccessToken);
                    using (HttpResponseMessage downloadResponse = await client.GetAsync(downloadUrl))
                    using (Stream downloadStream = await downloadResponse.Content.ReadAsStreamAsync())
                    using (FileStream fileStream = new FileStream(downloadPath, FileMode.Create))
                    {
                        using (Stream stream = await client.GetStreamAsync(downloadUrl))
                        {
                            await stream.CopyToAsync(fileStream);
                        }
                    }


                    ProcessStartInfo startInfo = new ProcessStartInfo();
                    startInfo.FileName = downloadPath;
                    startInfo.UseShellExecute = true;
                    Process.Start(startInfo);
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error downloading latest release: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"error extracting contents of downloaded release: {ex.Message}");
            }





        }

        public  async Task<bool> check_Update_needed()
        {
            HttpClient client = new HttpClient();

            try
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; AcmeInc/1.0)");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                HttpResponseMessage response = await client.GetAsync(url_github_Latest);

                string responseBody = await response.Content.ReadAsStringAsync();
                dynamic release = JsonConvert.DeserializeObject(responseBody);

                //gets latest release Tag in format "v0.x.x-alpha" and compares. 
                string latestVersion = release.tag_name;
                latestVersion = latestVersion.Replace("v", "");
                latestVersion = latestVersion.Remove(latestVersion.IndexOf("-"));
                Version currentVersion = new Version(stringApplicationVersion); // Replace with your current version
                Version latestVersionObj = new Version(latestVersion);

                if (latestVersionObj.CompareTo(currentVersion) > 0)
                {
                    UpdateVersion = latestVersion;
                    return true;
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"Error downloading latest release: {ex.Message}");
            }
            catch (IOException ex)
            {
                Console.WriteLine($"error extracting contents of downloaded release: {ex.Message}");
            }



            return false;
        }
    }
    
}


