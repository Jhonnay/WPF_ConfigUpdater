using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace WPFConfigUpdater.Common
{
    public static class MyConstants
    {
        public static class Strings
        {
            /* Paths */
            public static readonly string Path_ApplicationSettings = $"\\WPF_MiniserverUpdater\\ApplicationSettings.json";
            public static readonly string Path_Loxone_Installation = $"\\Loxone";
            public static readonly string Path_Changelog = $"\\Changelog.txt";
            public static readonly string Path_Folder_for_ApplicationData = $"\\WPF_MiniserverUpdater";
            public static readonly string Path_Folder_for_Loxone_App = $"\\Programs\\kerberos\\Loxone.exe";

            /* Links */
            public static readonly string Link_CloudDNS = "https://dns.loxonecloud.com/";
            public static readonly string Link_CloudDNS_simplified = "dns.loxonecloud.com/";
            public static readonly string Link_LPH = "https://lph.loxone.com/main.php?snr=";
            public static readonly string Link_CrashLog_Server = "https://gitlab.loxone.com:8443/crashstats/miniservercrashes.php?mac=";

            /* Processes */
            public static readonly string Process_Loxone_Config = "LoxoneConfig";
            public static readonly string Process_Loxone_App = "Loxone";

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
            public static readonly string Listview_MS_Status_AutoUpdate = "Auto Update...";
            public static readonly string Listview_MS_Refresh_canceled = "canceled";


            /* Bottom Status Bar */
            public static readonly string Statusbar_ProcessStatus_Refresh_inProgress_Text = "Miniserver Information is retreived ... ";
            public static readonly string Statusbar_ProcessStatus_Refresh_inProgress_MS = "retreiving data of ";
            public static readonly string Statusbar_ProcessStatus_Refresh_Information_pulled_Text = "Information pulled.";
            public static readonly string Statusbar_ProcessStatus_Update_Canceled = "Update canceled! ⚠";
            public static readonly string Statusbar_ProcessStatus_Updating = "Updating ... 🔄";
            public static readonly string Statusbar_ProcessStatus_Update_Complete = "Miniserver(s) updated";
            public static readonly string Statusbar_ProcessStatus_Update_Complete_show_MS = "Updated - ";
            public static readonly string Statusbar_ProcessStatus_Update_failed = "Update failed!";
            public static readonly string Statusbar_TextBlockConfig_No_Config_selected = "Current Config: not selected - double click to select";
            public static readonly string Statusbar_ProcessStatus_Refresh_Canceled = "Refresh Canceled!";


            /* Create MS Dialog */
            public static readonly string MS_Default_partial_SN = "504F94";
            public static readonly string MS_Default_dummy_SN = "504F94A00000";

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
            public static readonly string MessageBox_Refresh_Context_Copy_Username_Error = "Username could not be copied to Clipboard! \nPlease select a Miniserver.";
            public static readonly string MessageBox_Changelog_Cannot_be_opened = "Changelog cannot be opened! ";
            public static readonly string MessageBox_Applicationsettings_saved = "Settings saved. ✅";
            public static readonly string MessageBox_Applicationsettings_Not_saved = "Settings were not saved! ⚠";
            public static readonly string MessageBox_Applicationsettings_Configuration_not_found = "Configuration JSON not found! ⚠";
            public static readonly string MessageBox_ApplicationSettings_No_Paths_Set = "If a CheckBox is checked, a Path has to be set! ";
            public static readonly string MessageBox_CraeteMSDialog_Fields_Not_Filled = "Sorry my dude 🤷‍.Please fill in all fields!";
            public static readonly string MessageBox_CraeteMSDialog_Fields_SerialNumber_Length = "Sorry my dude 🤷‍.Please check SN length!";
            public static readonly string MessageBox_UpdateLevelSet_Successfully = "Updatelevel was successfully set to: ";
            public static readonly string MessageBox_UpdateLevelSet_Error = "Updatelevel is still: ";
            public static readonly string MessageBox_AutoUpdate_Not_Possible = "AutoUpdate not possible for " + Listview_Refresh_MS_Configuration_ClientGateway;
            public static readonly string MessageBox_AutoUpdate_seleceted_Not_Possible = MessageBox_AutoUpdate_Not_Possible + "\n\nPlease select only standalone Miniserver.";
            public static readonly string MessageBox_AutoUpdate_Startet = "Autoupdate started! \nPlease be patient. \nUI will not be updated on finish!";
            public static readonly string MessageBox_AutoUpdate_Not_Startet = "Autoupdate not started! WebService Error!";
            public static readonly string MessageBox_UpdateButton_AutoUpdate_Block = "Updates aborted! Please Select Miniserver that do not auto update.";
            public static readonly string MessageBox_ConnectConfigButton_AutoUpdate_Block = "Autoupdate! Connecting aborted!";
            public static readonly string MessageBox_App_already_open = "Loxone App already open!";
            public static readonly string MessageBox_App_not_installed = "Loxone App not installed! Could not find: %localappdata%\\Programs\\kerberos\\Loxone.exe";
            public static readonly string MessageBox_FTP_Local_IP_not_defined = "FTP without Local IP does not work!";
            public static readonly string MessageBox_FTP_Local_Ping_failed = "Ping to MS failed! Check local IP and if you are connected to the right network!";
            public static readonly string MessageBox_FTP_Local_authentification_failed = "Check MS Credentials!";
            public static readonly string MessageBox_UpdateButton_MS_updated_OR_higher_Version = "Selected Miniserver(s) are already on higher version.";
            public static readonly string MessageBox_ConnecConfig_Button_Canceled = "Connecting to Config canceled and Config terminated";
            public static readonly string MessageBox_Button_Refresh_canceled = "Refresh canceled!";
            public static readonly string MessageBox_ConfigExe_Not_Valid = "Please select a real Config EXE! \nPath will not be used!";
            public static readonly string MessageBox_Backgroundworker_Connect_Config_already_running = "There is already a Connect Config Task running! \nPlease try again later. ";
            public static readonly string MessageBox_Miniserver_Json_not_valid = "Please select a valid Miniserver Json Configuration!";
            public static readonly string MessageBox_AutoUpdate_Already_running = "AutoUpdate already running.\nPlease wait until Update is done.";
            public static readonly string MessageBox_AutoUpdate_selected_Already_running = MessageBox_AutoUpdate_Already_running + "\n\nPlease select only Miniserver that do not Autoupdate.";
            public static readonly string MessageBox_AutoUpdate_selected_ERROR_nothing_selected = "Please select Miniservers";
            public static readonly string MessageBox_AutoUpdate_selected_Error_which_could_not_update = "These Miniserver could not start AutoUpdate: ";
            public static readonly string MessageBox_CheckUpdate_no_Internet = "Check your internet connection!";
            public static readonly string MessageBox_CheckUpdate_no_Update_available = "No new updates available. 😕";
            /* WebServices */
            public static readonly string WebService_Success_Code = "200";

        }
    }
}
