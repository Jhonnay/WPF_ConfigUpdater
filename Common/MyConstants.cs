using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFConfigUpdater.Common
{
    public static class MyConstants
    {
        public static class Strings
        {
            /* Paths */
            public static readonly string Path_ApplicationSettings = $"\\WPF_MinserverUpdater\\ApplicationSettings.json";
            public static readonly string Path_Loxone_Installation = $"\\Loxone";
            public static readonly string Path_Changelog = $"\\Changelog.txt";
            public static readonly string Path_Folder_for_ApplicationData = $"\\WPF_MiniserverUpdater";

            /* Links */
            public static readonly string Link_CloudDNS = "https://dns.loxonecloud.com/";

            /* Processes */
            public static readonly string Process_Loxone = "LoxoneConfig";

            /* ListView */

            public static readonly string StartUp_Listview_MS_Status = "outdated info 😐";
            public static readonly string StartUp_Listview_MS_Version = "TBD";
            public static readonly string Listview_Refresh_MS_Configuration_Error = "error";
            public static readonly string Listview_Refresh_MS_Configuration_Client = "Client";
            public static readonly string Listview_Refresh_MS_Configuration_Standalone = "Standalone";
            public static readonly string Listview_Refresh_MS_Configuration_ClientGateway = "Client/Gateway";
            public static readonly string Listview_Updated_MS_Status = "updated ✅";
            public static readonly string Listview_MS_Status_Updating = "Updating";
            public static readonly string Listview_MS_Status_Updating_Emoji = "Updating ... 🔄";
            public static readonly string Listview_MS_Status_Update_finished = "Update finished";
            public static readonly string Listview_MS_Status_retreiving_information = "retreiving data ⏲";
            public static readonly string Listview_MS_Status_retreiving_information_successfull = "Info up to date ✅";
            public static readonly string Listview_MS_Status_retreiving_information_timeout = "Error/Timeout ⚠";
            public static readonly string Listview_MS_Status_Edited = "Miniserver modified ⚠";
            public static readonly string Listview_ProjectName_Invalid_JSON = "Invalid JSON"; 


            /* Bottom Status Bar */
            public static readonly string Statusbar_ProcessStatus_Refresh_inProgress_Text = "Miniserver Information is retreived ... ";
            public static readonly string Statusbar_ProcessStatus_Refresh_inProgress_MS = "retreiving data of ";
            public static readonly string Statusbar_ProcessStatus_Refresh_Information_pulled_Text = "Informations pulled.";
            public static readonly string Statusbar_ProcessStatus_Update_Canceled = "Update canceled! ⚠";
            public static readonly string Statusbar_ProcessStatus_Updating = "Updating ... 🔄";
            public static readonly string Statusbar_ProcessStatus_Update_Complete = "Miniserver(s) updated";
            public static readonly string Statusbar_ProcessStatus_Update_Complete_show_MS = "Updated - ";
            public static readonly string Statusbar_TextBlockConfig_No_Config_selected = "Current Config: not selected - double click to select";

            


            /* MessageBox */
            public static readonly string MessageBox_OpenConfig_No_Config_selected = "No Config Selected! 🤷‍";
            public static readonly string MessageBox_UpdateButton_No_Config_selected = "No Config Selected! No Update will be performed!";
            public static readonly string MessageBox_ConnectConfigButton_ConfigsOpen_Error_Part1 = "Unable to open and Connect Config! There are ";
            public static readonly string MessageBox_ConnectConfigButton_ConfigsOpen_Error_Part2 = " Config Instances running. \nPlease Save current Projects!";
            public static readonly string MessageBox_ButtonUpdate_OtherUdpateRunning = "The Update Process is still running! Please Cancel the Update!";
            public static readonly string MessageBox_ButtonUpdate_ConfigsOpen_Error_Part1 = "Unable to perform Updates! There are ";
            public static readonly string MessageBox_ButtonUpdate_ConfigsOpen_Error_Part2 = " Config Instances running. In order to perform Miniserver Updates no other Config has to be open! \nPlease Save current Projects!";
            public static readonly string MessageBox_Update_canceled = "Update(s) canceled by user!";
            public static readonly string MessageBox_Update_alreadyUpdatedMS = " Miniserver(s) updated!";
            public static readonly string MessageBox_Update_Show_all_updatedMS_Versions = "Miniserver Updated to - ";
            public static readonly string MessageBox_Refresh_Information_pulled = " Miniserver Information pulled!";
            public static readonly string MessageBox_Refresh_Context_Copy_SNR_Error = "SNR could not be copied to Clipboard! \nPlease select a Miniserver.";
            public static readonly string MessageBox_Changelog_Cannot_be_opened = "Changelog cannot be opened! ";
            public static readonly string MessageBox_Applicationsettings_saved = "Settings saved. ✅";
            public static readonly string MessageBox_Applicationsettings_Not_saved = "Settings were not saved! ⚠";

        }
    }
}
