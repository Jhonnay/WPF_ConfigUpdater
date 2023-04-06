using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WPFConfigUpdater.Common;

namespace WPFConfigUpdater
{
    /// <summary>
    /// Interaktionslogik für Window1.xaml
    /// </summary>
    public partial class CreateMiniserverDialog : Window
    {
        public CreateMiniserverDialog()
        {
            InitializeComponent();
            //MyDialogTextBoxMS_Password.Text = "temp";
            //MyDialogTextBoxMS_User.Text = "temp";
            MyDialogTextBoxMS_SerialNumber.Text = MyConstants.Strings.MS_Default_partial_SN;
        }

        public CreateMiniserverDialog(string serialNumber, string user, string password)
        {
            InitializeComponent();
            //MyDialogPasswordBoxMS_Password;
            MyDialogTextBoxMS_User.Text = user;
            MyDialogTextBoxMS_SerialNumber.Text = serialNumber;
        }

        private void btnDialogOk_Click(object sender, RoutedEventArgs e)
        {
             MyDialogTextBoxMS_SerialNumber.Text =  MyDialogTextBoxMS_SerialNumber.Text.Replace(":", ""); //remove semicolons because it produces errors. 

            if (MyDialogPasswordBoxMS_Password.Password != "" &&
                MyDialogTextBoxMS_User.Text != "" &&
                MyDialogTextBoxMS_SerialNumber.Text != "")
            {
                if (MyDialogTextBoxMS_SerialNumber.Text.Length == MyConstants.Strings.MS_Default_dummy_SN.Length)
                {
                    this.DialogResult = true;
                }
                else{
                    MessageBox.Show(MyConstants.Strings.MessageBox_CraeteMSDialog_Fields_SerialNumber_Length);
                    this.DialogResult = false;
                }
            }
            else
            {
                MessageBox.Show(MyConstants.Strings.MessageBox_CraeteMSDialog_Fields_Not_Filled);
                this.DialogResult = false;
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            MyDialogTextBoxMS_SerialNumber.SelectAll();
            MyDialogTextBoxMS_SerialNumber.Focus();
        }

        public WPFConfigUpdater.MainWindow.CMiniserver Answer
        {
            get
            {
                WPFConfigUpdater.MainWindow.CMiniserver cMiniserver = new MainWindow.CMiniserver();
                cMiniserver.adminPassWord = MyDialogPasswordBoxMS_Password.Password;
                cMiniserver.adminUser = MyDialogTextBoxMS_User.Text;
                cMiniserver.serialNumer = MyDialogTextBoxMS_SerialNumber.Text;
                cMiniserver.LocalIPAdress = MyDialogTextBoxMS_LocalIP.Text;

                return cMiniserver;
            }
        }
    }
}
