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
using System.Windows.Shapes;

using iNKORE.UI.WPF.Modern;

namespace FSL.Next
{
    /// <summary>
    /// Custom.xaml 的交互逻辑
    /// </summary>
    public partial class Custom : Page
    {
        public Custom()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("1.打开FSL根目录下的“Custom.xaml”文件\n2.编写XAML代码（基于WPF）\n3.在“Custom.xaml.cs”编写C#代码，比如这段可以显示一个弹窗\n如果有兴趣，可以自学iNKORE.WPF.UI.Modern和WPF","如何自定义主页");
        }
    }
}
