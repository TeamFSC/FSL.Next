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
using System.Diagnostics.Contracts;
using Newtonsoft.Json;

namespace FSL.Next.Pages
{
    /// <summary>
    /// Accounts.xaml 的交互逻辑
    /// </summary>
    public partial class Accounts : Page
    {

        public class AccountsInfo
        {
            public static AccountsDetail SelectedAccInfo { get; set; }
        }

        public class AccountsList
        {
            public List<AccountsDetail> Accounts { get; set; }
        }

        public class AccountsDetail
        {
            public string Type { get; set; }
            public string Offline_Name { get; set; }
            public string Microsoft_Name { get; set; }
            public string Microsoft_RefreshToken { get; set; }
            public List Yggdrasil_Players { get; set; }
        }

        public Accounts()
        {
            InitializeComponent();
            tips.Visibility = Visibility.Collapsed;

            // 加载账户配置
            try
            {
                string content = File.ReadAllText("./config/accounts.fsl");
                AccountsList allAccounts = JsonConvert.DeserializeObject<AccountsList>(content);

                foreach (AccountsDetail accountsDetail in allAccounts.Accounts)
                {
                    switch (accountsDetail.Type)
                    {
                        case "Offline":
                            accounts.Items.Add("离线验证：" + accountsDetail.Offline_Name);
                            break;
                        case "Microsoft":
                            accounts.Items.Add("微软验证：" + accountsDetail.Microsoft_Name);
                            break;
                        default:
                            break;
                    }
                }
            }
            catch
            {
                AccountsList allAccounts = new AccountsList()
                { 
                    Accounts = new List<AccountsDetail>()
                };
                string json = JsonConvert.SerializeObject(allAccounts, Formatting.Indented, new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                File.WriteAllText("./config/accounts.fsl", json);
            }
                
        }

        public class accInfos
        {
            public static BaseAccount baseAccount { get; set; }
        }

        private void switchAuthType(int type)
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
                    if ( ! accounts.Items.Contains("离线验证：" + offlineName.Text))
                    {
                        accounts.Items.Add("离线验证：" + offlineName.Text);

                        if (!dontSave.IsOn)
                        {
                            accAdd.IsEnabled = false;
                            accInfo.Content = "正在保存账户信息";

                            string text = await File.ReadAllTextAsync("./config/accounts.fsl");
                            AccountsList allAccounts = JsonConvert.DeserializeObject<AccountsList>(text);
                            List<AccountsDetail> list = allAccounts.Accounts;
                            list.Add(new AccountsDetail()
                            {
                                Type = "Offline",
                                Offline_Name = offlineName.Text,
                            }); 

                            AccountsList accountsList = new()
                            {
                                Accounts = list
                            };
                            string json = JsonConvert.SerializeObject(accountsList, Formatting.Indented, new JsonSerializerSettings
                            {
                                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                            });

                            await File.WriteAllTextAsync("./config/accounts.fsl",json);
                            
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

        private async void delAcc_Click(object sender, RoutedEventArgs e)
        {
            string accounts_ = await File.ReadAllTextAsync("./config/accounts.fsl");

            // 防止用户删除后没选择不小心再点一次（没错这个用户是我）
            if( accounts.SelectedIndex == -1)
            {
                return;
            }

            AccountsList allAccounts = JsonConvert.DeserializeObject<AccountsList>(accounts_);
            allAccounts.Accounts.RemoveAt(accounts.SelectedIndex);

            string json = JsonConvert.SerializeObject(allAccounts, Formatting.Indented, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            await File.WriteAllTextAsync("./config/accounts.fsl", json);
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

                if (!accounts.Items.Contains("微软验证：" + userInfo.Name))
                {
                    accounts.Items.Add("微软验证：" + userInfo.Name);
                    if (!dontSave.IsOn)
                    {
                        accAdd.IsEnabled = false;
                        accInfo.Content = "正在保存账户信息";

                        string text = await File.ReadAllTextAsync("./config/accounts.fsl");
                        AccountsList allAccounts = JsonConvert.DeserializeObject<AccountsList>(text);
                        List<AccountsDetail> list = allAccounts.Accounts;
                        list.Add(new AccountsDetail()
                        {
                            Type = "Microsoft",
                            Microsoft_Name = userInfo.Name,
                            Microsoft_RefreshToken = userInfo.RefreshToken
                        });

                        AccountsList accountsList = new()
                        {
                            Accounts = list
                        };
                        string json = JsonConvert.SerializeObject(accountsList, Formatting.Indented, new JsonSerializerSettings
                        {
                            ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                        });

                        await File.WriteAllTextAsync("./config/accounts.fsl", json);
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

        private void accounts_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string content = File.ReadAllText("./config/accounts.fsl");
            AccountsList allAccounts = JsonConvert.DeserializeObject<AccountsList>(content);

            AccountsInfo.SelectedAccInfo = allAccounts.Accounts[accounts.SelectedIndex];
        }
    }
}
