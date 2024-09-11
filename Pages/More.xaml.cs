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

namespace FSL.Next.Pages
{
    /// <summary>
    /// More.xaml 的交互逻辑
    /// </summary>
    public partial class More : Page
    {

        [DataContract]
        public class Daily
        {
            [DataMember]
            public string Content { get; set; }

            [DataMember]
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
    }
}
