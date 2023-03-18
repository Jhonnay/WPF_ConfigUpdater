

using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.IO;
using System.Collections;


public struct AutoStatus
{
    //State Buffer is 4 Byte aligned 

    //int nDocState; 
    //bool bAllClientsRunning; 
    //bool bIdentical;
    //int nMiniserver;
    //bool m_bUpdating;
    //int m_nCurrentState;
    // add more status fields here

    public int nDocState;
    /*
        #define DS_OFFLINE 0
        #define DS_ONLINE 1
        #define DS_SIMULATION 2
        #define DS_LIVEVIEW 3
     */
    public int bAllClientsRunning; //All clients online 
    public int bIdentical; //Programm identisch 
    public int nMiniservers; //Anzahl Miniserver
    public int m_bUpdating; // boolean
    public int m_nCurrentState;
    /*
    #define AUTO_DISCONNECTED 0
    #define AUTO_IDLE 1
    #define AUTO_CONNECTING 2
    #define AUTO_CONNECTED 3
    #define AUTO_LOADING 10
    #define AUTO_LOADINGCOMPLETE 11
    #define AUTO_SAVING 20
    #define AUTO_SAVINGCOMPLETE 21
    #define AUTO_UPDATING 30
    #define AUTO_UPDATINGCOMPLETE 31
    #define AUTO_NETSEARCH 40
    #define AUTO_LNKSEARCH 41
    #define AUTO_AIRSEARCH 42
    #define AUTO_ENDSEARCH 49 
 */

    public override string ToString()
    {
        return "nDocState = " + nDocState + "," + "bAllClientsRunning = "
            + bAllClientsRunning + "," + "bIdentical = " + bIdentical + "," + "nMiniservers = " + nMiniservers + ","
            + "m_bUpdating = " + m_bUpdating + "," + "m_nCurrentState = " + m_nCurrentState;
    }

}


namespace WPFConfigUpdater
{
    public class UDPListener
    //Udp Listener Thread - https://www.py4u.net/discuss/777848
    {
        private int m_portToListen = 7771;
        private volatile bool listening;
        private Thread m_ListeningThread;
        private AutoStatus autoStatus = new AutoStatus();
        private string versionConfig;
        private ArrayList versionsOfMiniservers = new ArrayList();
        private string projectPath;
        private string configPath;

        public AutoStatus AutoStatus { get => autoStatus; set => autoStatus = value; }
        public string VersionConfig { get => versionConfig; set => versionConfig = value; }
        public ArrayList VersionsMiniservers { get => versionsOfMiniservers; set => versionsOfMiniservers = value; }


        //constructor
        public UDPListener()
        {
            this.listening = false;
        }

        public UDPListener(string projectPath, string configPath)
        {
            this.listening = false;
            this.projectPath = projectPath;
            this.configPath = configPath;

        }

        public void StartListener()
        {
            if (!this.listening)
            {
                //m_ListeningThread = new Thread(ListenForUDPPackages);
                m_ListeningThread = new Thread(new ThreadStart(ListenForUDPPackages));
                m_ListeningThread.IsBackground = true;
                this.listening = true;
                m_ListeningThread.Start();
            }
        }

        public void StopListener()
        {
            this.listening = false;
        }

        public void ListenForUDPPackages()
        {
            UdpClient listener = null;

            try
            {
                listener = new UdpClient(m_portToListen);
            }
            catch (SocketException)
            {
                //do nothing
            }

            if (listener != null)
            {
                IPEndPoint groupEP = new IPEndPoint(IPAddress.Any, m_portToListen);

                try
                {
                    while (this.listening)
                    {
                        byte[] receiveBytes = listener.Receive(ref groupEP);

                        autoStatus.nDocState = receiveBytes[0];
                        autoStatus.bAllClientsRunning = receiveBytes[4];
                        autoStatus.bIdentical = receiveBytes[8];
                        autoStatus.nMiniservers = receiveBytes[12];
                        autoStatus.m_bUpdating = receiveBytes[16];
                        autoStatus.m_nCurrentState = receiveBytes[20];

                        //Working with text files in c#
                        //https://www.ictdemy.com/csharp/files/working-with-text-files-in-csharp-net

                        if (autoStatus.nDocState == 1)
                        {
                             versionsOfMiniservers.Clear();
                             for(int i =0; i< AutoStatus.nMiniservers; i++)
                             {
                             //mVersions[0] = BitConverter.ToUInt32(receiveBytes.Skip(24).Take(4).ToArray(), 0);
                             UInt32 temp = (BitConverter.ToUInt32(receiveBytes.Skip(24 + 4 * i).Take(4).ToArray(), 0)); //Get Firmware of the n-MS
                             versionsOfMiniservers.Add(temp.ToString()) ;
                             }
                            
                            
                            
                            
                            Console.WriteLine(DateTime.UtcNow.ToString() + " - " + autoStatus + ", MS-Version " + versionsOfMiniservers);
                            //System.Diagnostics.Debug.WriteLine(Directory.GetCurrentDirectory() + $"\\Logs\\log.txt");
                            //using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + $"\\Logs\\log.txt", true))
                            //{
                            //    sw.WriteLine(DateTime.UtcNow.ToString() + " - " + autoStatus + ", MS - Versions ");
                            //    foreach(string vers in versionsOfMiniservers)
                            //    {
                            //        sw.Write(vers.ToString() + ", ");
                            //    }
                                
                            //    sw.Flush();
                            //    sw.Close();
                            //}
                            
                        }
                        else
                        {
                            Console.WriteLine(DateTime.UtcNow.ToString() + " - " + autoStatus + ", MS-Version " + " NA");
                            //System.Diagnostics.Debug.WriteLine(Directory.GetCurrentDirectory() + $"\\Logs\\log.txt");
                            //using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + $"\\Logs\\log.txt", true))
                            //{
                            //    sw.WriteLine(DateTime.UtcNow.ToString() + " - " + autoStatus + ", MS-Version " + "NA");
                            //    sw.Flush();
                            //}
                        }

                        //Get File Version --> Config Version Number
                        //https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.fileversioninfo.fileversion?view=net-5.0
                        FileVersionInfo myFileVersionInfo = FileVersionInfo.GetVersionInfo(configPath);
                        Console.WriteLine("Version number: " + myFileVersionInfo.FileVersion);


                        string temp2 = myFileVersionInfo.FileMajorPart.ToString("D2") + myFileVersionInfo.FileMinorPart.ToString("D2") +
                            myFileVersionInfo.FileBuildPart.ToString("D2") + myFileVersionInfo.FilePrivatePart.ToString("D2");
                        Console.WriteLine("Version from Config: " + temp2 + "\n");

                        versionConfig = temp2;
                        //System.Diagnostics.Debug.WriteLine(Directory.GetCurrentDirectory() + $"\\Logs\\log.txt");
                        //using (StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + $"\\Logs\\log.txt", true))
                        //{
                        //    sw.WriteLine("Config Version Number: " + myFileVersionInfo.FileVersion + "\n" +
                        //        "Version from Config: " + temp2 + "\n");
                        //    sw.Flush();
                        //}
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
                finally
                {
                    listener.Close();
                    Console.WriteLine("Done listening for UDP broadcast");
                }
            }
        }
    }
}




