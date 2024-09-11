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
using StarLight_Core.Authentication;
using StarLight_Core.Models.Authentication;
using iNKORE.UI.WPF.Modern.Common.IconKeys;

using FSL.Next.Utils;
using Windows.ApplicationModel.UserDataTasks;

namespace FSL.Next.Pages
{
    /// <summary>
    /// Accounts.xaml 的交互逻辑
    /// </summary>
    public partial class Accounts : Page
    {
        List<string> accountsInfo = [];

        public Accounts()
        {
            InitializeComponent();
            tips.Visibility = Visibility.Collapsed;

            try
            {
                string[] content = File.ReadAllLines("./config/accounts.fsl");
                foreach(string line in content)
                {
                    if( line != string.Empty)
                    {
                        accountsInfo.Add(line);
                        string[] info = line.Split("|");
                        if (info[1] == "0")
                        {
                            accounts.Items.Add("离线验证："+info[0]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                if ( !Directory.Exists("./config") )
                {
                    Directory.CreateDirectory("./config");
                }
                File.Create("./config/accounts.fsl");
                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("诶呀... FSL 无法解析你的账户配置文件，\n这可能是因为修改的配置文件格式不正确导致的。\n如果这是第一次使用，请重启更新。\n该文件已被重置，如有问题，请查看错误信息：\n\n" + ex,"解析账户配置失败",MessageBoxButton.OK,MessageBoxImage.Hand);
            }
        }

        public class accInfos
        {
            public static BaseAccount baseAccount { get; set; }
        }

        private void switchAuthType(int type)
        {
            try
            {

                tips.Visibility = Visibility.Visible;
                accConfig.Visibility = Visibility.Visible;

                switch (type)
                {
                    case 0:
                        offlineName.Visibility = Visibility.Visible;
                        offlineName.Text = "Player_Name";
                        msStart.Visibility = Visibility.Collapsed;
                        break;

                    case 1:
                        offlineName.Visibility = Visibility.Collapsed;
                        msStart.Visibility = Visibility.Visible;
                        break;
            }
                        
            }
            catch (Exception ex)
            {
                MessageBoxUtil.ShowError("Accounts/switchAuthType",ex);

                /*
                if( !Directory.Exists("./logs") )
                {
                    Directory.CreateDirectory("./logs");
                }

                var logName = "./logs/FSLError-Lastet.txt";

                File.CreateText(logName);
                File.WriteAllLines(logName, ["- - - - - - - - - - - - -","FSL.Next Error Log", "- - - - - - - - - - - - -", "LOCATE: Accounts-switchAuthType", "EXCEPTION: "+ex,"",""+"FSL-Version: ",FSL.Next.MainWindow.info.version]);
                */
            }
            
        }

        // 界面设计的最好看的一集！（大喜）

        private void newAcc_Click(object sender, RoutedEventArgs e)
        {
            accInfo.Content = "如有能力，FSL建议您选择正版，毕竟89又不贵...";

            authType.SelectedIndex = -1;

            offlineName.Visibility = Visibility.Collapsed;
            msStart.Visibility = Visibility.Collapsed;
            tips.Visibility = Visibility.Visible;
            accConfig.Visibility = Visibility.Visible;
            accCannot.Visibility = Visibility.Collapsed;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switchAuthType(authType.SelectedIndex);
        }

        private async void accAdd_Click(object sender, RoutedEventArgs e)
        {
            switch ( authType.SelectedIndex )
            {
                case 0:
                    if ( ! (offlineName.Text.Contains("|") || accounts.Items.Contains("离线验证：" + offlineName.Text)))
                    {
                        accounts.Items.Add("离线验证：" + offlineName.Text);
                        accountsInfo.Add(offlineName.Text + "|0");
                        if (!dontSave.IsOn)
                        {
                            accAdd.IsEnabled = false;
                            accInfo.Content = "正在保存账户信息";
                            await File.AppendAllLinesAsync("./config/accounts.fsl", [offlineName.Text + "|0"]);
                            
                            accInfo.Content = "账户添加完成";
                            accAdd.IsEnabled = true;
                        }
                        tips.Visibility = Visibility.Collapsed;
                    }
                    else
                    {
                        accCannot.Visibility = Visibility.Visible;
                    }

                    break;
            }
            
        }

        private void accCancel_Click(object sender, RoutedEventArgs e)
        {
            tips.Visibility = Visibility.Collapsed;
        }

        private void delAcc_Click(object sender, RoutedEventArgs e)
        {
            string[] accounts_ = File.ReadAllLines("./config/accounts.fsl");
            accountsInfo.Clear();

            // 转换数组到列表
            foreach (string line in accounts_)
            {
                accountsInfo.Add(line);
            }

            // 防止用户删除后没选择不小心再点一次（没错这个用户是我）
            if( accounts.SelectedIndex == -1)
            {
                return;
            }

            accountsInfo.Remove(accountsInfo[accounts.SelectedIndex]);

            accounts.Items.Remove(accounts.SelectedItem);

            File.WriteAllLines("./config/accounts.fsl", accountsInfo);
        }

        private async void msStart_Click(object sender, RoutedEventArgs e)
        {
            var msAuth = new MicrosoftAuthentication("e1e383f9-59d9-4aa2-bf5e-73fe83b15ba0"); 
            var deviceCodeInfo = await msAuth.RetrieveDeviceCodeInfo();
            Process.Start(new ProcessStartInfo
            {
                FileName = deviceCodeInfo.VerificationUri,
                UseShellExecute = true
            });

            try
            {
                Clipboard.Clear();

                Clipboard.SetText(deviceCodeInfo.UserCode);

                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("验证地址：" + deviceCodeInfo.VerificationUri + "，\n请输入下方的代码来完成验证，切勿泄露给他人。\n" + deviceCodeInfo.UserCode + "\nFSL已自动复制代码，验证成功后，请关闭浏览器，等待结果！", "Microsoft 账户验证", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("验证地址：" + deviceCodeInfo.VerificationUri + "，\n请输入下方的代码来完成验证，切勿泄露给他人。\n" + deviceCodeInfo.UserCode + "\n验证成功后，请关闭浏览器，等待结果！\n另外，FSL在试图复制代码时发生错误，所以请牢记你的代码。", "Microsoft 账户验证", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            accInfo.Content = "正在等待验证";
            

            var tokenInfo = await msAuth.GetTokenResponse(deviceCodeInfo);


            try
            {
                MicrosoftAccount userInfo;
                userInfo = await msAuth.MicrosoftAuthAsync(tokenInfo, x =>
                {
                    accInfo.Content = (x);
                });

                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show( "你现在已经成功登录到你的Microsoft账户，可以继续。\n" + "你的账户名为：" + userInfo.Name + "\n感谢购买Minecraft，祝您游戏愉快！", "Microsoft 账户验证",MessageBoxButton.OK,MessageBoxImage.Information);

                accountsInfo.Add(userInfo.RefreshToken + "|1");

                if (!accounts.Items.Contains("微软验证：" + userInfo.Name + "（" + userInfo.Uuid + "）"))
                {
                    accounts.Items.Add("微软验证：" + userInfo.Name + "（" + userInfo.Uuid + "）");
                    if (!dontSave.IsOn)
                    {
                        accAdd.IsEnabled = false;
                        accInfo.Content = "正在保存账户信息";
                        await File.AppendAllLinesAsync("./config/accounts.fsl", [userInfo.RefreshToken + "|1"]);

                        accInfo.Content = "账户添加完成";
                        accAdd.IsEnabled = true;
                    }
                    await Task.Delay(1000);
                    tips.Visibility = Visibility.Collapsed;
                }
                else
                {
                    accCannot.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                iNKORE.UI.WPF.Modern.Controls.MessageBox.Show( "Microsoft 登录失败，如果你还没有购买 Minecraft Java 版，请去 Minecraft 官网或 Microsoft Xbox 购买！\n或者，是因为作者没有测试，导致验证器炸了...\n" + ex.Message,"Microsoft 账户验证",MessageBoxButton.OK,MessageBoxImage.Error) ;
            }
        }
    }
}
