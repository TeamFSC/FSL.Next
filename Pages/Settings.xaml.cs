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

using StarLight_Core.Utilities;

using System.IO;
using Microsoft.Win32;
using Windows.Media.Playback;

using Newtonsoft.Json;
using StarLight_Core.Models.Utilities;
using iNKORE.UI.WPF.Modern.Controls;
using System.Reflection;

using static FSL.Next.MainWindow;
using static FSL.Next.Pages.Accounts;

namespace FSL.Next.Pages
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : System.Windows.Controls.Page
    {
        public class SettingsInfo
        {
            public string JavaPath { get; set; }
            public List<string> GameDirs { get; set; }
            public int SelectedGD { get; set; }
            public int DownloadSource { get; set; }
            public int DownloadThreads { get; set; }
            public int WindowWidth { get; set; }
            public int WindowHeight { get; set; }
            public bool WindowFullScreen { get; set; }
            public int Memory { get; set; }
            public string PersonnalizeTitle { get; set; }
        }

        List<string> gcDirs = []; 

        public Settings()
        {
            InitializeComponent();

            try
            {

                if (File.ReadAllText("./config/settings.fsl") == null || File.ReadAllText("./config/settings.fsl") == string.Empty)
                {
                    return;
                }

                string json = File.ReadAllText("./config/settings.fsl");
                SettingsInfo settingsInfo = JsonConvert.DeserializeObject<SettingsInfo>(json);
                javaPath.Text = settingsInfo.JavaPath;

                foreach (string dir in settingsInfo.GameDirs)
                {
                    gcDir.Items.Add(dir); 
                    gcDirs.Add(dir);
                }
                gcDir.SelectedIndex = settingsInfo.SelectedGD;
                
                switch ( settingsInfo.DownloadSource )
                {
                    case 0:
                        dsMojang.IsChecked = true;
                        dsBA.IsChecked = false;
                        break;
                    case 1:
                        dsMojang.IsChecked = false;
                        dsBA.IsChecked = true;
                        break;
                }

                threads.Value = settingsInfo.DownloadThreads;
                windowWidth.Text = settingsInfo.WindowWidth.ToString();
                windowHeight.Text = settingsInfo.WindowHeight.ToString();
                windowFull.IsOn = settingsInfo.WindowFullScreen;

                windowTitle.Text = settingsInfo.PersonnalizeTitle;

                info.setTitle(info.thisWindow,settingsInfo.PersonnalizeTitle);
            }
            catch (Exception ex) 
            {
                if (!Directory.Exists("./config"))
                {
                    Directory.CreateDirectory("./config");
                }
                
                File.Create("./config/settings.fsl");

                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("诶呀... FSL 无法解析你的设置配置文件，\n这可能是因为修改的配置文件格式不正确导致的。\n如果这是第一次使用，请重启更新。\n该文件已被重置。", "解析设置配置失败", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        private void javaCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ( javaCombo.SelectedItem != null )
            {
                javaPath.Text = javaCombo.SelectedValue.ToString();
            }
        }

        private void javaRefresh_Click(object sender, RoutedEventArgs e)
        {
            javaCombo.ItemsSource = JavaUtil.GetJavas();
            javaCombo.DisplayMemberPath = "JavaVersion";
            javaCombo.SelectedValuePath = "JavaPath";
        }

        private void dirAdd_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog folderDialog = new OpenFolderDialog();
            folderDialog.Title = "选择 .minecraft 文件夹";
            folderDialog.RootDirectory = Directory.GetCurrentDirectory();
            var result = folderDialog.ShowDialog();
            if ( result == true)
            {
                if (!folderDialog.FolderName.Contains(".minecraft"))
                {
                    if (iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("FSL发现你的目录名似乎不包含.minecraft，\n这意味着你可能错误选择了游戏目录。\n是否继续？", "目录名异常", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        return;
                    }
                }
               
                if( !gcDir.Items.Contains(folderDialog.FolderName))
                {
                    gcDir.Items.Add(folderDialog.FolderName);
                    gcDirs.Add(folderDialog.FolderName);
                }
                
            }
        }

        private void resetFSL_Click(object sender, RoutedEventArgs e)
        {
            if (iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("确定要重置FSL吗？\n重置后，FSL存储的设置和账户数据将会完全删除，将FSL初始化。\n请慎重考虑！","确定重置FSL（1/2）",MessageBoxButton.YesNo,MessageBoxImage.Warning) == MessageBoxResult.Yes)
            {
                if (iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("确定要重置FSL吗？\n重置后，FSL存储的设置和账户数据将会完全删除，将FSL初始化。\n请慎重考虑，这是最后的确认！", "确定重置FSL（2/2）", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
                {
                    File.Delete("./config/accounts.fsl");
                    File.Delete("./config/settings.fsl");
                    Directory.Delete("./config");
                    FSL.Next.MainWindow.info.close(FSL.Next.MainWindow.info.thisWindow);
                }
            }
        }

        public void saveSettings()
        {
            int ds = 1;
            if (dsMojang.IsChecked == true)
            {
                ds = 0;
            }
            if (dsBA.IsChecked == true)
            {
                ds = 1;
            }

            SettingsInfo settingsInfo = new SettingsInfo
            {
                JavaPath = javaPath.Text,
                GameDirs = gcDirs,
                SelectedGD = gcDir.SelectedIndex,
                DownloadSource = ds,
                DownloadThreads = (int)Math.Floor(threads.Value),
                WindowHeight = windowHeight.Text != string.Empty ?  Convert.ToInt32(windowHeight.Text) : 480,
                WindowWidth =  windowWidth.Text != string.Empty ? Convert.ToInt32(windowWidth.Text) : 854,
                WindowFullScreen = windowFull.IsOn,
                Memory = (int)Math.Floor(memory.Value),
                PersonnalizeTitle = windowTitle.Text.ToString(),
            };

            string json = JsonConvert.SerializeObject(settingsInfo, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            File.WriteAllText("./config/settings.fsl", json);
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            saveSettings();
        }

        private void dirDel_Click(object sender, RoutedEventArgs e)
        {
            int index = gcDir.SelectedIndex;

            gcDir.Items.Remove(gcDir.SelectedItem);
            gcDirs.RemoveAt(index);
        }

        private void settingMove_Click(object sender, RoutedEventArgs e)
        {
            iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("FSL允许复制该启动器的本体数据到另一个FSL启动器，\n只需打开FSL本体的/config文件夹，\n将accounts.fsl（账户）和settings.fsl（设置）转移到另一个FSL的配置文件夹即可。\n如果你是开发者，也可以改这个文件的内容！\n如果改炸了，FSL也会帮你重置。", "移动启动器数据", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        private void windowTitle_TextChanged(object sender, TextChangedEventArgs e)
        {
            info.setTitle(info.thisWindow, windowTitle.Text);
        }
    }
}
