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
            public static readonly string ApplicationSettingsPath = $"\\WPF_MinserverUpdater\\ApplicationSettings.json";
            public static readonly string Path_Loxone_Installation = $"\\Loxone";

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



            /* Bottom Status Bar */
            public static readonly string Statusbar_ProcessStatus_Refresh_Text = "Miniserver Information is retreived ... ";
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

        }
    }
}
