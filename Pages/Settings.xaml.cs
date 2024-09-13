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

namespace FSL.Next.Pages
{
    /// <summary>
    /// Settings.xaml 的交互逻辑
    /// </summary>
    public partial class Settings : Page
    {
        public Settings()
        {
            InitializeComponent();
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
               
                gcDir.Items.Add( folderDialog.FolderName );
            }
        }
    }
}
