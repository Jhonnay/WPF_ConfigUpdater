

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using static WPFConfigUpdater.MainWindow;

namespace WPFConfigUpdater
{
    public class Config
    {
        static string miniserver_ip;
        static string user;
        static string pw;
        static int port_receive;
        static int port_send;
        public static  List<string> LanguageList  = new () {
            "CAT", "CHS", "CSY", "DEU", "ENG", "ENU", "ESN", "FRA", "ITA", "HUN", "NLD", "NOR", "PLK", "ROM", "RUS", "SKY", "TRK"
            }; //Credit @Jan Posedel

    public Process p;

        public Config()
        {
            port_receive = 7700;
            port_send = 7771;
        }

        public Config(string msIp, string userName, string password) : this()
        {
            miniserver_ip = msIp;
            user = userName;
            pw = password;


        }


        public void startConfig()
        {
            p = new Process();
            p.StartInfo.FileName = "C:\\Program Files (x86)\\Loxone\\LoxoneALPHA_12.2.10.6\\LoxoneConfig.exe";
            p.StartInfo.Arguments = @"/auto C:\Users\musatbe\source\repos\ConfigAutoUpdate\ConfigAutoUpdate\resources\Kraftwerk.Loxone";
            //p.StartInfo.Arguments = @"/auto";
            p.Start();
        }

        public void startConfig(string configPath)
        {
            try
            {
                p = new Process();
                p.StartInfo.FileName = configPath;
                p.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        public void startConfig_Language(string configPath, string Language)
        {

            try
            {
                if (Language != null)
                {
                    p = new Process();
                    p.StartInfo.FileName = configPath;
                    p.StartInfo.Arguments = @"/Language=" + LanguageList.ElementAt(int.Parse(Language));
                    //p.StartInfo.Arguments = @"/auto";
                    p.Start();
                }
                else
                {
                    startConfig(configPath);
                }
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        public void startConfig(string configPath, string projectPath)
        {
            try
            {
                p = new Process();
                p.StartInfo.FileName = configPath;
                p.StartInfo.Arguments = @"/auto " + projectPath;
                //p.StartInfo.Arguments = @"/auto";
                p.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }

        public void startConfig_Language(string configPath, string projectPath, string Language)
        {
            try
            {
                if(Language != null)
                {
                    p = new Process();
                    p.StartInfo.FileName = configPath;
                    p.StartInfo.Arguments = @"/auto " + projectPath + " /Language=" + Config.LanguageList.ElementAt(int.Parse(Language));
                    p.Start() ;
                }
                else
                {
                    Language = "ENU";
                    p = new Process();
                    p.StartInfo.FileName = configPath;
                    p.StartInfo.Arguments = @"/auto " + projectPath + " /Language=" + Language;
                    p.Start();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }


        public void installNewConfig(string configInstallerPath)
        {
            try
            {
                p = new Process();
                p.StartInfo.FileName = configInstallerPath;
                p.StartInfo.Arguments = @"/silent";
                p.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            
        }


        public void closeConfig()
        {
            System.Diagnostics.Debug.WriteLine(p.ProcessName + " <- Task!");
            p.Kill();
            System.Diagnostics.Debug.WriteLine("Task killed!");
        }

        public void sendCommand(string ip, int port, string msg)
        {
            UdpClient:
            UdpClient udpClient = new UdpClient();

            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(ip), port);


            try
            {

                udpClient.Connect(ep);
                Byte[] sendBytes = Encoding.ASCII.GetBytes(msg);

                if (udpClient.Send(sendBytes, sendBytes.Length) != 0)
                {
                    udpClient.Close();
                    System.Diagnostics.Debug.WriteLine("Command : '" + msg + "' was successfully sent");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Error Sendig command to Config.");
                }

            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Thread.Sleep(1000);
                Debug.WriteLine(ex.Message);
                MessageBox.Show("Debug: \n " + ex.Message + "\nTrying again. ");

                goto UdpClient;
            }    
            
        }

        public void Disconnect()
        {
            sendCommand("127.0.0.1", 7770, "D");
        }

        internal void Update()
        {
            sendCommand("127.0.0.1", 7770, "U");
        }

        internal void LoadFromMiniserver()
        {
            sendCommand("127.0.0.1", 7770, "L");
        }
    }
}




