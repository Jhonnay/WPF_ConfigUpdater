using Microsoft.VisualBasic;
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
using WPFConfigUpdater.Common;
using static WPFConfigUpdater.MainWindow;

namespace WPFConfigUpdater
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
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
                
                if(File.Exists(settings.StrDefaultConfigurationPath))
                {
                    string strJsonMiniservers = File.ReadAllText(settings.StrDefaultConfigurationPath);

                    ObservableCollection<CMiniserver> miniservers = JsonSerializer.Deserialize<ObservableCollection<CMiniserver>>(strJsonMiniservers);

                    if (miniservers != null && miniservers.Count != 0)
                    {
                        for (int i = 0; i < miniservers.Count; i++)
                        {
                            miniservers[i].MSStatus = MyConstants.Strings.StartUp_Listview_MS_Status;
                            miniservers[i].MSVersion = MyConstants.Strings.StartUp_Listview_MS_Version;
                        }

                        window.miniserverList = miniservers;
                        window.listView_Miniserver.ItemsSource = window.miniserverList;
                        window.textblock_statusbar_config.Text = settings.StrDefaultConfigPath;
                        //MenuItem_MSVersionRefresh_Click(sender, e);
                        window.ListView_GridView_Autoresize();
                    }
                }
                else
                {
                    MessageBox.Show(MyConstants.Strings.MessageBox_Applicationsettings_Configuration_not_found, "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
            window.listView_Miniserver.SelectionMode = SelectionMode.Multiple;
            window.Show();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception just occurred: " + e.Exception.Message + "\nCongrats! You found an Bug! You can now go home.", "Exception Error", MessageBoxButton.OK, MessageBoxImage.Error);
            e.Handled = true;
            System.Windows.Application.Current.Shutdown();
        }
    }
}
