using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;



namespace WPFConfigUpdater
{
    /// <summary>
    /// Interaktionslogik für ApplicationSettingsDialog.xaml
    /// </summary>
    public partial class ApplicationSettingsDialog : Window
    {
        ApplicationSettings settings = new ApplicationSettings();
        public ApplicationSettingsDialog()
        {
            InitializeComponent();
        }

        public ApplicationSettingsDialog(ApplicationSettings applicationSettings)
        {
            
            InitializeComponent();
            checkbox_use_default_configpath_on_startup.IsChecked = applicationSettings.BUseDefaultConfig;
            checkbox_use_default_configuration_on_startup.IsChecked = applicationSettings.BUseDefaultConfiguration;
            textbox_configuration_path.Text = applicationSettings.StrDefaultConfigurationPath;
            textbox_config_path.Text = applicationSettings.StrDefaultConfigPath;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
            bool bool_Dialog_can_be_closed= true;

            if(checkbox_use_default_configuration_on_startup.IsChecked == true && textbox_configuration_path.Text == "" ||
                checkbox_use_default_configpath_on_startup.IsChecked == true && textbox_config_path.Text == "not set")
            {
                bool_Dialog_can_be_closed = false;
            }

            if (checkbox_use_default_configpath_on_startup.IsChecked == true && textbox_config_path.Text == "" ||
                checkbox_use_default_configpath_on_startup.IsChecked == true && textbox_config_path.Text == "not set")
            {
                bool_Dialog_can_be_closed = false;
            }

            if(bool_Dialog_can_be_closed == true)
            {
                settings.BUseDefaultConfig = (bool)checkbox_use_default_configpath_on_startup.IsChecked;
                settings.BUseDefaultConfiguration = (bool) checkbox_use_default_configuration_on_startup.IsChecked;
                settings.StrDefaultConfigurationPath = textbox_configuration_path.Text;
                settings.StrDefaultConfigPath = textbox_config_path.Text;

                string jsonString = JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true });
                this.DialogResult = true;

                if(!File.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MinserverUpdater" + $"\\ApplicationSettings.json"))
                {
                    Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MinserverUpdater");
                }

                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MinserverUpdater" + $"\\ApplicationSettings.json",jsonString);
                
                this.Close();


            }
            else
            {
                MessageBox.Show("If a CheckBox is checked, a Path has to be set! ", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            
        }
        private void btn_click_open_default_configuration (object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "JSON file (*.json)|*.json";

            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MinserverUpdater"))
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MinserverUpdater";
            }
            else
            {
                Directory.CreateDirectory(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MinserverUpdater");
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + $"\\WPF_MinserverUpdater";
            }

            if (openFileDialog.ShowDialog() == true)
            {
                textbox_configuration_path.Text = openFileDialog.FileName;
            }

        }

        private void btn_click_open_default_config (object sender, RoutedEventArgs e) 
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Exe file (*.exe)|*.exe";

            if (Directory.Exists(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + $"\\Loxone"))
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86) + $"\\Loxone";
            }
            else
            {
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
            }

            if (openFileDialog.ShowDialog() == true)
            {
                textbox_config_path.Text = openFileDialog.FileName;
            }

        }

        public string Answer
        {
            get
            {
                

                return textbox_config_path.Text;
            }
        }

    }
}
