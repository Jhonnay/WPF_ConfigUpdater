﻿using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WPFConfigUpdater.Common;
using static WPFConfigUpdater.MainWindow;

namespace WPFConfigUpdater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private  async void  Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow window = new MainWindow();

            //if (e.Args.Length != 0)
            //{
            //    MessageBox.Show(e.Args[0]); 
            //}

            ApplicationSettingsDialog dialog;
            if (File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + MyConstants.Strings.Path_ApplicationSettings))
            {
                string strJson = File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + MyConstants.Strings.Path_ApplicationSettings);
                ApplicationSettings settings = JsonSerializer.Deserialize<ApplicationSettings>(strJson);
                
                if(File.Exists(settings.StrDefaultConfigurationPath) && settings.BUseDefaultConfiguration)
                {
                    string strJsonMiniservers = File.ReadAllText(settings.StrDefaultConfigurationPath);


                    try
                    {
                        ObservableCollection<CMiniserver> miniservers = JsonSerializer.Deserialize<ObservableCollection<CMiniserver>>(strJsonMiniservers);

                        if (miniservers != null && miniservers.Count != 0)
                        {
                            for (int i = 0; i < miniservers.Count; i++)
                            {
                                miniservers[i].MSStatus = MyConstants.Strings.StartUp_Listview_MS_Status;
                                miniservers[i].MSVersion = MyConstants.Strings.StartUp_Listview_MS_Version;
                                miniservers[i].VersionColor = "black";
                            }

                            window.miniserverList = miniservers;
                            window.listView_Miniserver.ItemsSource = window.miniserverList;
                            window.ListView_GridView_Autoresize();



                            //MenuItem_MSVersionRefresh_Click(sender, e); //automatically refesh not wanted. 

                        }
                    }catch (Exception ex)
                    {
                        MessageBox.Show(MyConstants.Strings.MessageBox_Miniserver_Json_not_valid, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }



                }
                else
                {
                    if (settings.BUseDefaultConfiguration)
                    {
                        MessageBox.Show(MyConstants.Strings.MessageBox_Applicationsettings_Configuration_not_found, "Exception Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                   
                }


                if (File.Exists(settings.StrDefaultConfigPath) && settings.BUseDefaultConfig)
                {
                    if (WPFConfigUpdater.MainWindow.checkConfigEXE(settings.StrDefaultConfigPath))
                    {
                        window.textblock_statusbar_config.Text = settings.StrDefaultConfigPath;
                    }
                    else
                    {
                        window.textblock_statusbar_config.Text = MyConstants.Strings.Statusbar_TextBlockConfig_No_Config_selected;
                        MessageBox.Show(MyConstants.Strings.MessageBox_ConfigExe_Not_Valid, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                    
                }
                else
                {
                    window.textblock_statusbar_config.Text = MyConstants.Strings.Statusbar_TextBlockConfig_No_Config_selected;
                }



            }
            window.listView_Miniserver.SelectionMode = SelectionMode.Multiple;
            

            window.Show();

            window.Help_Check_for_Updates_On_Startup();

            //bool updatesAvailable = await window.check_Update_needed();
            //{
            //    if (updatesAvailable)
            //    {
            //        MessageBox.Show("Update available!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            //    }
            //}

        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            if(e.Exception.InnerException != null)
            {
                MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message + "\nInner Exception: " + e.Exception.InnerException.Message + "\nCongrats! You found an Bug! You can now go home.", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message +  "\nCongrats! You found an Bug! You can now go home.", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

           
            e.Handled = true;
            System.Windows.Application.Current.Shutdown();
        }
    }
}
