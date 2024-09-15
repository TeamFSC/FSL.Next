using Newtonsoft.Json;
using StarLight_Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
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
using static FSL.Next.Pages.Settings;

namespace FSL.Next.Pages
{
    /// <summary>
    /// Mainpage.xaml 的交互逻辑
    /// </summary>
    public partial class Mainpage : Page
    {
        public Mainpage()
        {
            InitializeComponent();
            gcCombo.Width = LAUNCH.Width;

            FSL.Next.Custom customs = new FSL.Next.Custom();

            custom.Content = new Frame() { Content = customs };

            string json = File.ReadAllText("./config/settings.fsl");

            if (json == string.Empty || json == null)
            {
                return;
            }

            SettingsInfo settingsInfo = JsonConvert.DeserializeObject<SettingsInfo>(json);

            List<string> gcDir = settingsInfo.GameDirs;
            gcCombo.ItemsSource = GameCoreUtil.GetGameCores(gcDir[settingsInfo.SelectedGD]);
            gcCombo.DisplayMemberPath = "Id";
            gcCombo.SelectedValuePath = "Id";
        }

        private void refresh_Click(object sender, RoutedEventArgs e)
        {
            string json = File.ReadAllText("./config/settings.fsl");

            if ( json == string.Empty || json == null)
            {
                return;
            }
            SettingsInfo settingsInfo = JsonConvert.DeserializeObject<SettingsInfo>(json);

            List<string> gcDir = settingsInfo.GameDirs;
            gcCombo.ItemsSource = GameCoreUtil.GetGameCores(gcDir[settingsInfo.SelectedGD]);
            gcCombo.DisplayMemberPath = "Id";
            gcCombo.SelectedValuePath = "Id";
        }
    }
}
