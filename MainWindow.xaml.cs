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
        public string stringApplicationVersion = "V 0.9.0";
        public ObservableCollection<CMiniserver> miniserverList = new ObservableCollection<CMiniserver>();
        public int int_selectedItems_before_Refresh = 0;
        private BackgroundWorker worker_MSUpdate = null;
        public CApplicationUI cApplicationUI = new CApplicationUI();
        public int previousMouseOverIndex;
        public int mouseOverIndex;
        private GridViewColumnHeader listViewSortCol = null;
        private SortAdorner listViewSortAdorner = null;
        private List<CMiniserver> selected_Miniserver_befor_refresh;

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
                textblock_statusbar_config.Text = openFileDialog.FileName;


                if (textblock_statusbar_config.Text != MyConstants.Strings.Statusbar_TextBlockConfig_No_Config_selected)
                {
                    FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(textblock_statusbar_config.Text);
                    Debug.WriteLine("Config Version: " + myFileVersionInfo.FileVersion);


                    for (int i = 0; i < miniserverList.Count; i++)
                    {
                        if (miniserverList[i].MSVersion != MyConstants.Strings.StartUp_Listview_MS_Version)
                        {
                            if (int.Parse(myFileVersionInfo.FileVersion.Replace(".", "")) > int.Parse(miniserverList[i].MSVersion.Replace(".", "")))
                            {
                                miniserverList[i].VersionColor = "orange";
                            }
                            else if (int.Parse(myFileVersionInfo.FileVersion.Replace(".", "")) == int.Parse(miniserverList[i].MSVersion.Replace(".", "")))
                            {
                                miniserverList[i].VersionColor = "green";
                            }
                            else
                            {
                                miniserverList[i].VersionColor = "black";
                            }
                        }
                        else
                        {
                            miniserverList[i].VersionColor = "black";
                        }


                    }
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
                    MessageBox.Show(MyConstants.Strings.MessageBox_OpenConfig_No_Config_selected);
                }
                else if (processes.Count() != 0)
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_ConnectConfigButton_ConfigsOpen_Error_Part1 + processes.Count() + MyConstants.Strings.MessageBox_ConnectConfigButton_ConfigsOpen_Error_Part2);
                }
                else
                {
                    config.ConfigPath = textblock_statusbar_config.Text;
                    config.OpenConfigLoadProject();
                }
            }
            else
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_ConnectConfigButton_AutoUpdate_Block, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            if (textblock_statusbar_config.Text != MyConstants.Strings.Statusbar_TextBlockConfig_No_Config_selected)
            {
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(textblock_statusbar_config.Text);
                Debug.WriteLine("Config Version: " + myFileVersionInfo.FileVersion);

                foreach (CMiniserver ms in listView_Miniserver.SelectedItems) //skip Updates if only 1 MS  is AutoUpdating
                {
                    if (ms.MSStatus == MyConstants.Strings.Listview_MS_Status_AutoUpdate)
                    {
                        skipUpdate_AutoUpdate = true;
                    }
                    if (ms.MSVersion != MyConstants.Strings.StartUp_Listview_MS_Version)
                    {
                        if (int.Parse(ms.MSVersion.Replace(".", "")) >= int.Parse(myFileVersionInfo.FileVersion.Replace(".", "")))
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
                        MessageBox.Show(MyConstants.Strings.MessageBox_ButtonUpdate_OtherUdpateRunning, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
                else
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_ButtonUpdate_ConfigsOpen_Error_Part1 + processes.Count() + MyConstants.Strings.MessageBox_ButtonUpdate_ConfigsOpen_Error_Part2);
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
                    MessageBox.Show(MyConstants.Strings.MessageBox_Update_canceled + (int)e.Result + "/" + int_selectedItems_before_Refresh + MyConstants.Strings.MessageBox_Update_alreadyUpdatedMS);
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
                    MessageBox.Show((int)e.Result + "/" + int_selectedItems_before_Refresh + MyConstants.Strings.MessageBox_Update_alreadyUpdatedMS);
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
                        getMiniserverInformationsWebServices(cMiniserver, out  ret_MsVersion, out var index1, out ret_cLoxAppJson, out updatelevel);

                       
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

                            if (miniserverList[index].MSVersion != MyConstants.Strings.StartUp_Listview_MS_Version)
                            {
                                if (int.Parse(myFileVersionInfo.FileVersion.Replace(".", "")) > int.Parse(miniserverList[index].MSVersion.Replace(".", "")))
                                {
                                    miniserverList.ElementAt(miniserverList.IndexOf(miniserverList[index])).VersionColor = "orange";
                                }
                                else if (int.Parse(myFileVersionInfo.FileVersion.Replace(".", "")) == int.Parse(miniserverList[index].MSVersion.Replace(".", "")))
                                {
                                    miniserverList.ElementAt(miniserverList.IndexOf(miniserverList[index])).VersionColor = "green";
                                }
                                else
                                {
                                    miniserverList.ElementAt(miniserverList.IndexOf(miniserverList[index])).VersionColor = "black";
                                }
                            }
                            else
                            {
                                miniserverList.ElementAt(miniserverList.IndexOf(miniserverList[index])).VersionColor = "black";
                            }
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
                    MessageBox.Show(MyConstants.Strings.MessageBox_UpdateButton_No_Config_selected);
                    
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

                    ArrayList arrlist = update.UpdateMS(worker_MSUpdate);
                    result_MS_Update++;
                    

                    if (cApplicationUI.HandsfreeMode == false && worker_MSUpdate.CancellationPending == false)
                    {
                        string concat_strings = string.Join(" ", arrlist.Cast<string>().ToArray());
                        MessageBox.Show(MyConstants.Strings.MessageBox_Update_Show_all_updatedMS_Versions + concat_strings);
                        
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
            progressbar_ProcessStatus.Value = 0;
            textblock_processStatus.Text = MyConstants.Strings.Statusbar_ProcessStatus_Refresh_inProgress_Text;
            StackPaneButtons.IsEnabled = false; //Disables all containing Buttons
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

            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += worker_DoWork_RefreshMSInformation;
            worker.ProgressChanged += worker_ProgressChanged_RefreshMSInformation;
            worker.RunWorkerCompleted += worker_RunWorkerCompleted_RefreshMSInformation;
            worker.RunWorkerAsync(list);
        }

        private void worker_RunWorkerCompleted_RefreshMSInformation(object sender, RunWorkerCompletedEventArgs e)
        {
            MessageBox.Show((int)e.Result + "/" + int_selectedItems_before_Refresh + MyConstants.Strings.MessageBox_Refresh_Information_pulled);
            textblock_processStatus.Text = MyConstants.Strings.Statusbar_ProcessStatus_Refresh_Information_pulled_Text;
            StackPaneButtons.IsEnabled = true;
            listView_Miniserver.IsEnabled = true;

            if(textblock_statusbar_config.Text != MyConstants.Strings.Statusbar_TextBlockConfig_No_Config_selected)
            {
                FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(textblock_statusbar_config.Text);
                Debug.WriteLine("Config Version: " + myFileVersionInfo.FileVersion);

                foreach (CMiniserver ms in selected_Miniserver_befor_refresh)
                {
                    if (ms.MSVersion != MyConstants.Strings.StartUp_Listview_MS_Version)
                    {
                        if (int.Parse(myFileVersionInfo.FileVersion.Replace(".", "")) > int.Parse(ms.MSVersion.Replace(".", "")))
                        {
                            miniserverList.ElementAt(miniserverList.IndexOf(ms)).VersionColor = "orange";
                        }
                        else if (int.Parse(myFileVersionInfo.FileVersion.Replace(".", "")) == int.Parse(ms.MSVersion.Replace(".", "")))
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

                    getMiniserverInformationsWebServices(ms, out ret_MsVersion, out index, out ret_cLoxAppJson, out updatelevel);   

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


                    progressPercentage = Convert.ToInt32((double)result_MS_Refreshed / list.Count * 100);
                    (sender as BackgroundWorker).ReportProgress(progressPercentage, cMiniserver);
                    System.Threading.Thread.Sleep(100);
                }
                (sender as BackgroundWorker).ReportProgress(100); //Fix even if retreival not possible for some MS --> show 100%
                e.Result = result_MS_Refreshed;
            }
           
        }

        private void getMiniserverInformationsWebServices(CMiniserver ms, out string ret_MsVersion, out int index, out CLoxAppJson ret_cLoxAppJson, out string Updatelevel)
        {
            if (ms.LocalIPAdress != "" && ms.LocalIPAdress != null)
            {
                ret_MsVersion = WebService.sendCommandRest_Version_Local_Gen1(ms.LocalIPAdress, ms.adminUser, ms.adminPassWord, @"/dev/sys/version", "value");
                System.Diagnostics.Debug.WriteLine(ret_MsVersion + "- " + ms.serialNumer);
                index = miniserverList.IndexOf(ms);
                ret_MsVersion = Format_Miniserver_String(ret_MsVersion);
                miniserverList.ElementAt(index).MSVersion = ret_MsVersion;
                ret_cLoxAppJson = WebService.sendCommandRest_LoxAppJson_Local_Gen1(ms.LocalIPAdress, ms.adminUser, ms.adminPassWord, @"/data/LoxAPP3.json");
                Updatelevel = WebService.sendCommandRest_Version_Local_Gen1(ms.LocalIPAdress, ms.adminUser, ms.adminPassWord, @"/dev/cfg/updatelevel", "value");
                
            }
            else
            {
                ret_MsVersion = WebService.sendCommandRest_Version_Remote_Cloud(ms.serialNumer, ms.adminUser, ms.adminPassWord, @"/dev/sys/version", "value");
                System.Diagnostics.Debug.WriteLine(ret_MsVersion + "- " + ms.serialNumer);
                index = miniserverList.IndexOf(ms);
                ret_MsVersion = Format_Miniserver_String(ret_MsVersion);
                miniserverList.ElementAt(index).MSVersion = ret_MsVersion;
                ret_cLoxAppJson = WebService.sendCommandRest_LoxAppJson_Remote_Cloud(ms.serialNumer, ms.adminUser, ms.adminPassWord, @"/data/LoxAPP3.json");
                Updatelevel = WebService.sendCommandRest_Version_Remote_Cloud(ms.serialNumer, ms.adminUser, ms.adminPassWord, @"/dev/cfg/updatelevel", "value");
            }
            
            if( Updatelevel.IndexOf("\"") > 0)
            {
                Updatelevel = Updatelevel.Remove(Updatelevel.IndexOf("\""));
            }

            if( Updatelevel == "-1213")
            {
                Updatelevel = MyConstants.Strings.Listview_Refresh_MS_Configuration_Error;
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
            cmd.StartInfo.Arguments = "/c start " + link;
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
                ApplicationSettings settings = JsonSerializer.Deserialize<ApplicationSettings>(strJson);
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
                config.startConfig(textblock_statusbar_config.Text);
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
            string jsonString = JsonSerializer.Serialize(miniserverList, new JsonSerializerOptions { WriteIndented = true });
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
                    ObservableCollection<CMiniserver> miniservers = JsonSerializer.Deserialize<ObservableCollection<CMiniserver>>(jsonString);

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



            if (updatelevel == "Alpha")
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_UpdateLevelSet_Successfully + updatelevel, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_UpdateLevelSet_Error + updatelevel, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            miniserverList[index].UpdateLevel = updatelevel;
            
        }

        private void ContextUpdateLevelRelease(object sender, RoutedEventArgs e)
        {
            int index = previousMouseOverIndex;
            string link, updatelevel;
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);

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



            if (updatelevel == "Release")
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_UpdateLevelSet_Successfully + updatelevel, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_UpdateLevelSet_Error + updatelevel, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            miniserverList[index].UpdateLevel = updatelevel;

        }
        private void ContextUpdateLevelBeta(object sender, RoutedEventArgs e)
        {
            int index = previousMouseOverIndex;
            string link, updatelevel;
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);

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



            if (updatelevel == "Beta")
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_UpdateLevelSet_Successfully + updatelevel, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_UpdateLevelSet_Error + updatelevel, "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            miniserverList[index].UpdateLevel = updatelevel;
        }
        private void ContextUpdteLevelUpdateToLevel(object sender, RoutedEventArgs e)
        {
            int index = previousMouseOverIndex;
            string link, returnCode;
            CMiniserver currentlySelectedMiniserver = miniserverList.ElementAt(previousMouseOverIndex);

            if(currentlySelectedMiniserver.MSConfiguration == MyConstants.Strings.Listview_Refresh_MS_Configuration_Standalone)
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

        }

        private void MenuItem_Open_Loxone_App(object sender, RoutedEventArgs e)
        {
            //% localappdata %\Programs\kerberos

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
    }
}


