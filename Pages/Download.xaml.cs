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

using StarLight_Core.Installer;
using StarLight_Core.Downloader;
using StarLight_Core.Utilities;
using StarLight_Core.Models.Installer;

using static FSL.Next.Pages.Settings;
using System.IO;
using Newtonsoft.Json;
using StarLight_Core.Enum;
using StarLight_Core.Models.Downloader;

namespace FSL.Next.Pages
{
    /// <summary>
    /// Download.xaml 的交互逻辑
    /// </summary>
    public partial class Download : Page
    {
        public static bool isDownloading = false;
        public Download()
        {
            InitializeComponent();
            refreshVers();
        }

        public  async void refreshVers()
        {
            vers.ItemsSource = await InstallUtil.GetGameCoresAsync();
            vers.DisplayMemberPath = "Id";
            vers.SelectedValuePath = "Id";
            download.Visibility = Visibility.Collapsed;
        }

        private async void vers_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if ( isDownloading )
            {
                return;
            }
            downloadConfirm.IsEnabled = true;
            downloadCancel.IsEnabled = true;
            vers.Visibility = Visibility.Collapsed;
            download.Visibility = Visibility.Visible;
            infoVer.Content = $"游戏版本：{vers.SelectedValue}";
            infoSpeed.Content = "新建下载任务";

            downloadFabric.ItemsSource = await FabricInstaller.FetchFabricVersionsAsync(vers.SelectedValue.ToString());
            infoFabric.Content = "Fabric";
            if (downloadFabric.Items.Count == 0)
            {
                infoFabric.Content = "Fabric - 无可用版本";
            }
            downloadFabric.DisplayMemberPath = "Version";
            downloadFabric.SelectedValuePath = "Version";

            downloadForge.ItemsSource = await ForgeInstaller.FetchForgeVersionsAsync(vers.SelectedValue.ToString());
            infoForge.Content = "Forge - 开发中";
            downloadForge.DisplayMemberPath = "Version";
            downloadForge.SelectedValuePath = "Version";

            downloadForge.SelectedIndex = -1;
            downloadFabric.SelectedIndex = -1;

            image.Source = new BitmapImage(new Uri("http://xxag.top/Images/Grass_Block.png"));
        }

        private async void downloadConfirm_Click(object sender, RoutedEventArgs e)
        {
            isDownloading = true;
            downloadFabric.IsEnabled = false;
            downloadForge.IsEnabled = false;
            downloadConfirm.IsEnabled = false;
            downloadCancel.IsEnabled = false;

            SettingsInfo settingsInfo = new SettingsInfo();
            string json = File.ReadAllText("./config/settings.fsl");
            settingsInfo = JsonConvert.DeserializeObject<SettingsInfo>(json);

            if (settingsInfo == null)
            {
                return;
            }

            switch (settingsInfo.DownloadSource)
            {
                case 0:
                    DownloadAPIs.SwitchDownloadSource(DownloadSource.Official);
                    break;
                case 1:
                    DownloadAPIs.SwitchDownloadSource(DownloadSource.BmclApi);
                    break;
            }
            DownloaderConfig.MaxThreads = settingsInfo.DownloadThreads;

            string gcRoot = settingsInfo.GameDirs[settingsInfo.SelectedGD];
            MinecraftInstaller installer = new MinecraftInstaller(vers.SelectedValue.ToString(),gcRoot);

            infoSpeed.Content = "正在下载原版核心";
            image.Source = new BitmapImage(new Uri("http://xxag.top/Images/Iron_Pickaxe.png"));
            installer.OnProgressChanged += (status,progress) => 
            {
                infoVer.Content = $"{status}（{progress}%）";
                downloadProgress.Value = progress;

                if ( status.Contains("失败") )
                {
                    image.Source = new BitmapImage(new Uri("http://xxag.top/Images/Active_Redstone_Wire.png"));
                }
            };
            
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken cancellationToken = cts.Token;
            await installer.InstallAsync(vers.SelectedValue.ToString(), true, cancellationToken);

            if (downloadFabric.SelectedIndex != -1)
            {
                var fabricInstaller = new FabricInstaller(vers.SelectedValue.ToString(),downloadFabric.SelectedValue.ToString(),root:gcRoot);

                infoSpeed.Content = "正在下载 Fabric 加载器";
                image.Source = new BitmapImage(new Uri("http://xxag.top/Images/Fabric.png"));
                fabricInstaller.OnProgressChanged += (status, progress) =>
                {
                    infoVer.Content = $"{status}（{progress}%）";
                    downloadProgress.Value = progress;

                    if (status.Contains("失败"))
                    {
                        image.Source = new BitmapImage(new Uri("http://xxag.top/Images/Active_Redstone_Wire.png"));
                    }
                };

                await fabricInstaller.InstallAsync();
            }

            isDownloading = false;
            image.Source = new BitmapImage(new Uri("http://xxag.top/Images/Dirt_Path.png"));
            downloadFabric.IsEnabled = true;
            downloadForge.IsEnabled = true;
            downloadConfirm.IsEnabled = true;
            downloadCancel.IsEnabled = true;
        }

        private void downloadLoaderCancel_Click(object sender, RoutedEventArgs e)
        {
            downloadForge.SelectedIndex = -1;
            downloadFabric.SelectedIndex = -1;

            infoForge.Content = "Forge - 开发中";

            infoFabric.Content = "Fabric";
            if (downloadFabric.Items.Count == 0)
            {
                infoFabric.Content = "Fabric - 无可用版本";
            }
        }

        private void downloadFabric_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            infoFabric.Content = $"Fabric {downloadFabric.SelectedValue}";
            downloadForge.IsEnabled = false;
            infoForge.Content = "Forge - 与 Fabric 不兼容";
        }

        private void expanderLoader_Expanded(object sender, RoutedEventArgs e)
        {
            downloadLoaderCancel.Visibility = Visibility.Collapsed;
        }

        private void expanderLoader_Collapsed(object sender, RoutedEventArgs e)
        {
            downloadLoaderCancel.Visibility = Visibility.Visible;
        }

        private void downloadCancel_Click(object sender, RoutedEventArgs e)
        {
            vers.Visibility = Visibility.Visible;
            download.Visibility = Visibility.Collapsed;
        }
    }
}
