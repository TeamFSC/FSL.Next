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
            download.Visibility = Visibility.Visible;
            infoVer.Content = $"游戏版本：{vers.SelectedValue} ，模组加载器请在下方选择";
            infoSpeed.Content = "新建下载任务";
            downloadFabric.ItemsSource = await FabricInstaller.FetchFabricVersionsAsync(vers.SelectedValue.ToString());
            downloadFabric.DisplayMemberPath = "Version";
            downloadFabric.SelectedValuePath = "Version";
        }

        private async void downloadConfirm_Click(object sender, RoutedEventArgs e)
        {
            isDownloading = true;
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
            installer.OnProgressChanged += (status,progress) => 
            {
                infoVer.Content = $"{status}（{progress}%）";
                downloadProgress.Value = progress;
            };
            
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken cancellationToken = cts.Token;
            await installer.InstallAsync(vers.SelectedValue.ToString(), true, cancellationToken);

            if (downloadFabric.SelectedIndex != -1)
            {
                var fabricInstaller = new FabricInstaller(vers.SelectedValue.ToString(),downloadFabric.SelectedValue.ToString(),root:gcRoot);

                infoSpeed.Content = "正在下载 Fabric 加载器";
                fabricInstaller.OnProgressChanged += (status, progress) =>
                {
                    infoVer.Content = $"{status}（{progress}%）";
                    downloadProgress.Value = progress;
                };

                await fabricInstaller.InstallAsync();
            }

            isDownloading = false;
        }

        private void downloadLoaderCancel_Click(object sender, RoutedEventArgs e)
        {
            downloadForge.SelectedIndex = -1;
            downloadFabric.SelectedIndex = -1;
        }
    }
}
