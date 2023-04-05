using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

public static class Defines
{
    public static int AUTO_DISCONNECTED = 0;
    public static int AUTO_IDLE = 1;
    public static int AUTO_CONNECTING = 2;
    public static int AUTO_CONNECTED = 3;
    public static int AUTO_LOADING = 10;
    public static int AUTO_LOADINGCOMPLETE = 11;
    public static int AUTO_SAVING = 20;
    public static int AUTO_SAVINGCOMPLETE = 21;
    public static int AUTO_UPDATING = 30;
    public static int AUTO_UPDATINGCOMPLETE = 31;
    public static int AUTO_NETSEARCH = 40;
    public static int AUTO_LNKSEARCH = 41;
    public static int AUTO_AIRSEARCH = 42;
    public static int AUTO_ENDSEARCH = 49;

    public const int DS_OFFLINE = 0;
    public const int DS_ONLINE = 1;
    public const int DS_SIMULATION = 2;
    public const int DS_LIVEVIEW = 3;


    public const int UPDATE_CYCLE_CHECK_CONNECTION_AND_LOAD_PROJECT = 0;
    public const int UPDATE_CYCLE_MS_UPDATE = 1;
    public const int UPDATE_CYCLE_CHECK_UPDATE = 2;
    public const int UPDATE_CYCLE_CHECK_MS_VERSIONS = 3;
    public const int UPDATE_CYCLE_DISCONNECT_CONFIG = 4;
    public const int UPDATE_CYCLE_CLOSE_CONFIG = 5;

}


namespace WPFConfigUpdater
{
    internal class CConfigMSUpdate
    {
        private static bool stop = false;
        private static int updateCycleState = 0;
        private string msIP;
        private string user;
        private string pw;
        private string projectPath = @"EmptyProject.Loxone";
        private string configPath;
        private string newConfigInstallerPath = null;
        private Config config = null;
        private const string localhost = "127.0.0.1";



        public static int UpdateCycleState { get => updateCycleState; set => updateCycleState = value; }
        public static bool Stop { get => stop; set => stop = value; }
        public string MsIP { get => msIP; set => msIP = value; }
        public string User { get => user; set => user = value; }
        public string Pw { get => pw; set => pw = value; }
        public string ProjectPath { get => projectPath; set => projectPath = value; }
        public string ConfigPath { get => configPath; set => configPath = value; }
        public string NewConfigInstallerPath { get => newConfigInstallerPath; set => newConfigInstallerPath = value; }


        public void OpenConfigLoadProject()
        {
            this.config = new Config();
            config.startConfig(this.configPath, this.projectPath);
            Thread.Sleep(9000); //Wait for Config to start
            String msg = "C," + msIP + "," + user + "," + pw;
            msg = msg.Replace(":", ".");
            System.Diagnostics.Debug.WriteLine("Connecting to MS with: " + msIP + "," + user + "," + pw);
            config.sendCommand(localhost, 7770, msg); //Connect with MS
            Thread.Sleep(7000);
            config.LoadFromMiniserver();

        }

        public void OpenConfigLoadProject_Cancelable(BackgroundWorker worker_Connect_Config, string configLanguage)
        {
            if(worker_Connect_Config.CancellationPending != true)
            {
                this.config = new Config();
                config.startConfig_Language(this.configPath, this.projectPath, configLanguage);
                Thread.Sleep(9000); //Wait for Config to start
            }

            

            if (worker_Connect_Config.CancellationPending != true)
            {
                String msg = "C," + msIP + "," + user + "," + pw;
                msg = msg.Replace(":", ".");
                System.Diagnostics.Debug.WriteLine("Connecting to MS with: " + msIP + "," + user + "," + pw);
                config.sendCommand(localhost, 7770, msg); //Connect with MS
                Thread.Sleep(7000);
            }

            if (worker_Connect_Config.CancellationPending != true)
            {
                config.LoadFromMiniserver();
            }
               
        }



        public ArrayList InstallConfigandUpdateMS( )
        {
            int ret = -1;

            this.config = new Config();
            config.installNewConfig(this.newConfigInstallerPath);
            config.p.WaitForExit();
            config.startConfig(this.configPath, this.projectPath);
            Thread.Sleep(9000); //Wait for Config to start
            String msg = "C," + msIP + "," + user + "," + pw;
            System.Diagnostics.Debug.WriteLine("Connecting to MS with: " + msIP + "," + user + "," + pw);
            config.sendCommand(localhost, 7770, msg); //Connect with MS
            Thread.Sleep(5000);

            UDPListener udpL = new UDPListener(projectPath, configPath);
            System.Diagnostics.Debug.WriteLine("Starting UDP Listener");
            udpL.StartListener();

            Thread.Sleep(3000);
            performMiniserverUpdate(config, udpL);
            config.closeConfig();


            return udpL.VersionsMiniservers; 
        }

        



        public ArrayList UpdateMS(BackgroundWorker worker_MSUpdate, string configLanuage)
        {
            int ret = -1;
            UDPListener udpL = new UDPListener(projectPath, configPath);

            if (worker_MSUpdate.CancellationPending == false)
            {
                this.config = new Config();
                config.startConfig_Language(this.configPath, this.projectPath,configLanuage);
                Thread.Sleep(9000); //Wait for Config to start
                String msg = "C," + msIP + "," + user + "," + pw;
                msg = msg.Replace(":", ".");
                System.Diagnostics.Debug.WriteLine("Connecting to MS with: " + msIP + "," + user + "," + pw);
                config.sendCommand(localhost, 7770, msg); //Connect with MS
                Thread.Sleep(7000);

                
                System.Diagnostics.Debug.WriteLine("Starting UDP Listener");
                udpL.StartListener();



                Thread.Sleep(5000);
                if (performMiniserverUpdate(config, udpL, worker_MSUpdate) == 1)
                {
                    config.closeConfig();
                }
                else
                {
                    if(worker_MSUpdate.CancellationPending == false)
                    {
                        MessageBox.Show("Update takes longer than expected. \nPossible Reasons: 1 or more Miniservers have not updated and are not running on the target version!\n Please Check! The Current Config Session was not terminated!");
                        udpL.StopListener();
                    }
                    else
                    {
                        MessageBox.Show("Update Process was cancelled!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                    }
                    
                    return null;
                }

                udpL.StopListener();
            }

            

            return udpL.VersionsMiniservers;
        }

        private static int performMiniserverUpdate(Config config, UDPListener udpL, BackgroundWorker backgroundWorker)
        {
            int updateCycleState = 0;
            int counterCyclesTime = 0; //After 17 Minutes, stop update Progress in this application and show Messsage Box and DO NOT KILL CONFIG!
            int ret = -1;
            int fails = 0; 



            while (updateCycleState < 6 && backgroundWorker.CancellationPending == false)
            {
                System.Diagnostics.Debug.WriteLine(udpL.AutoStatus);
                switch (updateCycleState)
                {
                    case Defines.UPDATE_CYCLE_CHECK_CONNECTION_AND_LOAD_PROJECT:
                        System.Diagnostics.Debug.WriteLine("Checking if Connection with MS was established and all Clients are also Running.");
                        if (udpL.AutoStatus.nDocState == Defines.DS_ONLINE)
                        {
                            System.Diagnostics.Debug.WriteLine("Miniservers are connected and running. Going to next step - Loading From MS.\n Waiting 1000ms ...");
                            Thread.Sleep(3000);
                            PrintConfigMsVersions(udpL);
                            config.LoadFromMiniserver();
                            Thread.Sleep(7000);
                            updateCycleState++;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Connection not established or not all Miniservers are running.");
                            System.Diagnostics.Debug.WriteLine("DocState = " + udpL.AutoStatus.nDocState + " should be  =  " + Defines.DS_ONLINE);
                            System.Diagnostics.Debug.WriteLine("Sleep 5s");

                            fails++;
                            if(fails == 5)
                            {
                                updateCycleState = 0;
                                fails = 0;
                                config.LoadFromMiniserver();
                            }
                            Thread.Sleep(5000);

                        }
                        
                        break;

                    case Defines.UPDATE_CYCLE_MS_UPDATE:
                        if (udpL.AutoStatus.nDocState == Defines.DS_ONLINE && udpL.AutoStatus.m_nCurrentState == Defines.AUTO_CONNECTED && udpL.AutoStatus.bAllClientsRunning == 1)
                        {
                            System.Diagnostics.Debug.WriteLine("Project successfully loaded from the MS. Next Step update Firmware on all MS.");
                            config.Update();
                            updateCycleState++;
                            Thread.Sleep(10000); //extra time until the update flag is set 
                        }
                        break;

                    case Defines.UPDATE_CYCLE_CHECK_UPDATE:

                        if (udpL.AutoStatus.nDocState == 1 && udpL.AutoStatus.bAllClientsRunning == 1
                            && udpL.AutoStatus.m_bUpdating == 0
                            && udpL.AutoStatus.m_nCurrentState == 3)
                        {
                            System.Diagnostics.Debug.WriteLine("Update Complete. Next Step checking Versions of all Miniservers.");
                            updateCycleState++;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Miniservers are still updating or restarting.");
                        }
                        break;

                    case Defines.UPDATE_CYCLE_CHECK_MS_VERSIONS:
                        bool oneMiniserverDidNotUpdate = false;

                        foreach (string version in udpL.VersionsMiniservers)
                        {
                            if (!udpL.VersionConfig.Equals(version))
                            {
                                oneMiniserverDidNotUpdate = true;
                            }
                        }

                        if (!oneMiniserverDidNotUpdate)
                        {
                            System.Diagnostics.Debug.WriteLine("\n" + DateTime.Now);
                            System.Diagnostics.Debug.WriteLine("Versions of Config and Miniservers match");
                            PrintConfigMsVersions(udpL);
                            updateCycleState++;
                        }
                        break;

                    case Defines.UPDATE_CYCLE_DISCONNECT_CONFIG:
                        //Disconnecting
                        config.Disconnect();

                        //check if disconnected!
                        System.Diagnostics.Debug.WriteLine("Disconnecting Config from Miniserver");
                        updateCycleState++;
                        break;

                    case Defines.UPDATE_CYCLE_CLOSE_CONFIG:
                        config.closeConfig();
                        System.Diagnostics.Debug.WriteLine("\n\n" + "Update Process complete.");
                        updateCycleState++;
                        ret = 1;
                        break;

                }
                System.Diagnostics.Debug.WriteLine("     Waiting 1000ms ...");
                counterCyclesTime += 1;
                if(counterCyclesTime > (60 * 17))
                {
                    updateCycleState = 99; //set > 6 to escape Update While LOOP. 
                }
                Thread.Sleep(1000);
            }
            return ret;
        }

        private static int performMiniserverUpdate(Config config, UDPListener udpL)
        {
            int updateCycleState = 0;
            int counterCyclesTime = 0; //After 17 Minutes, stop update Progress in this application and show Messsage Box and DO NOT KILL CONFIG!
            int ret = -1;
            int fails = 0;



            while (updateCycleState < 6)
            {
                System.Diagnostics.Debug.WriteLine(udpL.AutoStatus);
                switch (updateCycleState)
                {
                    case Defines.UPDATE_CYCLE_CHECK_CONNECTION_AND_LOAD_PROJECT:
                        System.Diagnostics.Debug.WriteLine("Checking if Connection with MS was established and all Clients are also Running.");
                        if (udpL.AutoStatus.nDocState == Defines.DS_ONLINE)
                        {
                            System.Diagnostics.Debug.WriteLine("Miniservers are connected and running. Going to next step - Loading From MS.\n Waiting 1000ms ...");
                            Thread.Sleep(3000);
                            PrintConfigMsVersions(udpL);
                            config.LoadFromMiniserver();
                            Thread.Sleep(7000);
                            updateCycleState++;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Connection not established or not all Miniservers are running.");
                            System.Diagnostics.Debug.WriteLine("DocState = " + udpL.AutoStatus.nDocState + " should be  =  " + Defines.DS_ONLINE);
                            System.Diagnostics.Debug.WriteLine("Sleep 5s");

                            fails++;
                            if (fails == 5)
                            {
                                updateCycleState = 0;
                                fails = 0;
                                config.LoadFromMiniserver();
                            }
                            Thread.Sleep(5000);

                        }

                        break;

                    case Defines.UPDATE_CYCLE_MS_UPDATE:
                        if (udpL.AutoStatus.nDocState == Defines.DS_ONLINE && udpL.AutoStatus.m_nCurrentState == Defines.AUTO_CONNECTED && udpL.AutoStatus.bAllClientsRunning == 1)
                        {
                            System.Diagnostics.Debug.WriteLine("Project successfully loaded from the MS. Next Step update Firmware on all MS.");
                            config.Update();
                            updateCycleState++;
                        }
                        break;

                    case Defines.UPDATE_CYCLE_CHECK_UPDATE:

                        if (udpL.AutoStatus.nDocState == 1 && udpL.AutoStatus.bAllClientsRunning == 1
                            && udpL.AutoStatus.m_bUpdating == 0
                            && udpL.AutoStatus.m_nCurrentState == 3)
                        {
                            System.Diagnostics.Debug.WriteLine("Update Complete. Next Step checking Versions of all Miniservers.");
                            updateCycleState++;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("Miniservers are still updating or restarting.");
                        }
                        break;

                    case Defines.UPDATE_CYCLE_CHECK_MS_VERSIONS:
                        bool oneMiniserverDidNotUpdate = false;

                        foreach (string version in udpL.VersionsMiniservers)
                        {
                            if (!udpL.VersionConfig.Equals(version))
                            {
                                oneMiniserverDidNotUpdate = true;
                            }
                        }

                        if (!oneMiniserverDidNotUpdate)
                        {
                            System.Diagnostics.Debug.WriteLine("\n" + DateTime.Now);
                            System.Diagnostics.Debug.WriteLine("Versions of Config and Miniservers match");
                            PrintConfigMsVersions(udpL);
                            updateCycleState++;
                        }
                        break;

                    case Defines.UPDATE_CYCLE_DISCONNECT_CONFIG:
                        //Disconnecting
                        config.Disconnect();

                        //check if disconnected!
                        System.Diagnostics.Debug.WriteLine("Disconnecting Config from Miniserver");
                        updateCycleState++;
                        break;

                    case Defines.UPDATE_CYCLE_CLOSE_CONFIG:
                        config.closeConfig();
                        System.Diagnostics.Debug.WriteLine("\n\n" + "Update Process complete.");
                        updateCycleState++;
                        ret = 1;
                        break;

                }
                System.Diagnostics.Debug.WriteLine("     Waiting 1000ms ...");
                counterCyclesTime += 1;
                if (counterCyclesTime > (60 * 17))
                {
                    updateCycleState = 99; //set > 6 to escape Update While LOOP. 
                }
                Thread.Sleep(1000);
            }
            return ret;
        }


        public static void PrintConfigMsVersions(UDPListener udpL)
        {
            System.Diagnostics.Debug.WriteLine("Current Config Version: " + udpL.VersionConfig);
            System.Diagnostics.Debug.WriteLine("Current MS-Firmware-Versions: ");

            foreach (String vers in udpL.VersionsMiniservers)
            {
                System.Diagnostics.Debug.Write(vers + ", ");
            }
        }

    }
}
