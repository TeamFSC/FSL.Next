using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.IO;
using iNKORE.UI.WPF.Modern;
using Newtonsoft.Json;

namespace FSL.Next
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Pages.Mainpage mainpage = new();
        Pages.Accounts accounts = new();

        public MainWindow()
        {
            InitializeComponent();

            if( !File.Exists("./config/accounts.fsl") || !File.Exists("./config/settings.fsl" ) )
            {
                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("启动器检测到配置文件缺失，\n如果您是第一次使用，请先配置启动选项。\n感谢您选择FSL！", "欢迎使用FSL.Next！");
                Directory.CreateDirectory("./config");

                /* 
                 * 有人说不写注释就很难理解
                 * 此处判断配置文件是否存在，并且根据情况创建
                 * 可以避免误杀另一个配置文件的情况！
                */

                if(!File.Exists("./config/accounts.fsl"))
                {
                    File.CreateText("./config/accounts.fsl");
                }

                if (!File.Exists("./config/settings.fsl"))
                {
                    File.CreateText("./config/settings.fsl");
                }
            }
            
        }

        private void mainNav_MouseDown(object sender, MouseButtonEventArgs e)
        {
            nav.Content = new Frame()
            {
                Content = mainpage
            };
        }

        private void accNav_MouseDown(object sender, MouseButtonEventArgs e)
        {
            nav.Content = new Frame()
            {
                Content = accounts
            };
        }
    }
}