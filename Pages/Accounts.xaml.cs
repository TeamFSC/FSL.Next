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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;

using Newtonsoft.Json;
using System.IO;
using iNKORE.UI.WPF.Modern;
using System.Diagnostics;

namespace FSL.Next.Pages
{
    /// <summary>
    /// Accounts.xaml 的交互逻辑
    /// </summary>
    public partial class Accounts : Page
    {
        public Accounts()
        {
            InitializeComponent();
            tips.Visibility = Visibility.Hidden;
            offlineName.Visibility = Visibility.Collapsed;
            msStart.Visibility = Visibility.Collapsed;
        }

        private void switchAuthType(int type)
        {
            switch (type)
            {
                case 0:
                    offlineName.Visibility = Visibility.Visible;
                    offlineName.Text = "Player_Name";
                    msStart.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    msStart.Visibility = Visibility.Visible;
                    offlineName.Visibility = Visibility.Collapsed;
                    break;
            }
        }

        // 界面设计的最好看的一集！（大喜）

        private void newAcc_Click(object sender, RoutedEventArgs e)
        {
            accInfo.Content = "如有能力，FSL建议您选择正版，毕竟89又不贵...";

            offlineName.Visibility = Visibility.Collapsed;
            msStart.Visibility = Visibility.Collapsed;
            tips.Visibility = Visibility.Visible;
            accConfig.Visibility = Visibility.Visible;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switchAuthType(authType.SelectedIndex);
        }
    }
}
