using Newtonsoft.Json;
using StarLight_Core.Authentication;
using StarLight_Core.Enum;
using StarLight_Core.Launch;
using StarLight_Core.Models.Authentication;
using StarLight_Core.Models.Launch;
using StarLight_Core.Utilities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

            if (!File.Exists("./config/settings.fsl"))
            {
                return;
            }
            string json = File.ReadAllText("./config/settings.fsl");

            if (json == string.Empty || json == null )
            {
                return;
            }

            SettingsInfo settingsInfo = JsonConvert.DeserializeObject<SettingsInfo>(json);

            List<string> gcDir = settingsInfo.GameDirs;

            if(settingsInfo.SelectedGD == -1)
            {
                return;
            }

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

        private async void LAUNCH_Click(object sender, RoutedEventArgs e)
        {
            progress.Value = 0;
            progressText.Content = string.Empty;
            SettingsInfo settingsInfo = new();
            string json = File.ReadAllText("./config/settings.fsl");
            settingsInfo = JsonConvert.DeserializeObject<SettingsInfo>(json);

            var selected = Accounts.AccountsInfo.SelectedAccInfo;
            string[] selectedL = selected.Split("|");

            dynamic account = null;

            switch (selectedL[0])
            {
                case "0":
                    account = new OfflineAuthentication(selectedL[1]).OfflineAuth();
                    break;
                case "1":
                    account = await new MicrosoftAuthentication("e1e383f9-59d9-4aa2-bf5e-73fe83b15ba0") .MicrosoftAuthAsync(new GetTokenResponse(), refreshToken: selectedL[1] ,action: x=>
                    {
                        progressText.Content = "正在验证账户：" + x;
                    });
                    break;
            }

            LaunchConfig args = new() // 配置启动参数
            {
                Account = new()
                {
                    BaseAccount = account // 账户
                },
                GameCoreConfig = new()
                {
                    Root = settingsInfo.GameDirs[settingsInfo.SelectedGD],
                    Version = gcCombo.SelectedValue.ToString(),
                    IsVersionIsolation = true,
                },
                GameWindowConfig = new()
                {
                    Height = settingsInfo.WindowHeight,
                    Width = settingsInfo.WindowWidth,
                    IsFullScreen = settingsInfo.WindowFullScreen
                },
                JavaConfig = new()
                {
                    JavaPath = settingsInfo.JavaPath,
                    MaxMemory = settingsInfo.Memory,
                    MinMemory = 0
                }
            };

            void ReportProgress(ProgressReport report)
            {
                progress.Value = report.Percentage;
                progressText.Content = report.Description;
            }

            var launch = new MinecraftLauncher(args); // 实例化启动器

            var result = await launch.LaunchAsync(ReportProgress); // 启动

            if (result.Status == Status.Succeeded && progress.Value >= 90)
            {
                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("Minecraft启动成功！\n请耐心等待窗口出现...\n如果长时间未启动，请尝试将启动器放置在可以正常访问（非管理员）文件夹！", "启动成功", MessageBoxButton.OK, MessageBoxImage.Information);
                progress.Value = 100;
                progressText.Content = "启动成功";
            }
            else
            {
                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("请检查游戏文件是否完整，信息是否填写完毕：\n" + result.Exception.Message, "启动失败", MessageBoxButton.OK, MessageBoxImage.Error);
                progress.Value = 0;
                progressText.Content = "启动失败";
            }
        }
    }
}
