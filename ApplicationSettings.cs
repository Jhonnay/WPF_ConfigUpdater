using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WPFConfigUpdater
{
    public class ApplicationSettings
    {
        private bool bUseDefaultConfiguration;
        private string strDefaultConfigurationPath;
        private bool bUseDefaultConfig;
        private string strDefaultConfigPath;

        public ApplicationSettings()
        {
            BUseDefaultConfiguration = false;
            StrDefaultConfigurationPath = "";
            BUseDefaultConfig = false;
            StrDefaultConfigPath = "";
        }

        public bool BUseDefaultConfiguration { get => bUseDefaultConfiguration; set => bUseDefaultConfiguration = value; }
        public string StrDefaultConfigurationPath { get => strDefaultConfigurationPath; set => strDefaultConfigurationPath = value; }
        public bool BUseDefaultConfig { get => bUseDefaultConfig; set => bUseDefaultConfig = value; }
        public string StrDefaultConfigPath { get => strDefaultConfigPath; set => strDefaultConfigPath = value; }
    }
}
