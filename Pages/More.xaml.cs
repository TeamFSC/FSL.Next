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
using Newtonsoft.Json;
using System.Windows.Resources;
using System.Runtime.CompilerServices;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace FSL.Next.Pages
{
    /// <summary>
    /// More.xaml 的交互逻辑
    /// </summary>
    public partial class More : Page
    {

        public class Daily
        {
            public string Content { get; set; }

            public string Source { get; set; }
        }

        public More()
        {
            InitializeComponent();
            var vers = FSL.Next.MainWindow.info.version;
            copyright.Content = $"Copyright 2024 ~  {DateTime.UtcNow.Year} © awa_Eric-FSC. All rights Reserved. {vers}";
        }

        private async void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string daily = await HttpUtil.SendHttpGetRequest("https://api.polarisnetwork.cloud:5555/api/v1/Quotes/daily");

            var json = JsonConvert.DeserializeObject<Daily>(daily);

            mbGsContent.Text = json.Content;
            mbGsAuthor.Text = "——" + json.Source;
        }

        private void OpenSource(object sender, RoutedEventArgs e)
        {
            Process.Start(new ProcessStartInfo() { UseShellExecute = true, FileName = "https://github.com/TeamFSC/FSL.Next" });
        }

        private void mbLucky_Click(object sender, RoutedEventArgs e)
        {
            int seed = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day;

            Random random = new Random(seed);
            int randomInt = random.Next(-1, 101);
            string message = "FSL猜不出你的人品值...";

            if ( randomInt == 100)
            {
                message = "在这个时候，你的人品值会是？\n100！100！！100！！！\n隐藏主题...（bushi）";
            }
            else if ( randomInt >= 90)
            {
                message = $"在这个时候，你的人品值会是？\n{randomInt}...运气不错！";
            }
            else if (randomInt >= 70)
            {
                message = $"现在为止，你的人品值是：\n{randomInt}...也还可以啦！";
            }
            else if (randomInt >= 50)
            {
                message = $"现在为止，你的人品值是：\n{randomInt}...普普通通，但也没啥缺点";
            }
            else if (randomInt >= 30)
            {
                message = $"现在的人品值...\n{randomInt}...为什么会这样呢...";
            }
            else if (randomInt >= 10)
            {
                message = $"现在的人品值...\n{randomInt}...还好，没垫底";
            }
            else if (randomInt < 10 && randomInt > 0)
            {
                message = $"今天又是霉好的一天，人品值是：\n...\n{randomInt}！？不会吧...";
            }
            else if( randomInt == 0 )
            {
                message = "恭喜你，人品值是：\n0？？？ 反向欧皇（大喜）";
            }

            iNKORE.UI.WPF.Modern.Controls.MessageBox.Show(message,"今日人品");
        }

        private void mbCrash_Click(object sender, RoutedEventArgs e)
        {
            var a = 1;
            var b = 5 / (1 - a);
        }
    }
}
