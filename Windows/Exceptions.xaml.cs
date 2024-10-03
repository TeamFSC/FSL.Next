using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

namespace FSL.Next.Windows
{
    /// <summary>
    /// Exception.xaml 的交互逻辑
    /// </summary>
    public partial class Exceptions : Window
    {
        public class ExceptionWindow
        {
            public static dynamic thisWindow { get; set; }
            public static async void ShowException(Exception ex)
            {
                var window = thisWindow;
                window.Show();
                window.Activate();
                window.exception.Content = $"{ex.Message} - {DateTime.Now} - {MainWindow.info.version}";
                window.info.Content = ex;
            }
        }
        public Exceptions()
        {
            InitializeComponent();
            
        }

        private void copy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText($"{exception.Content.ToString()}\n{info.Content.ToString()}");
        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            window.Close();
            FSL.Next.MainWindow.info.close(FSL.Next.MainWindow.info.thisWindow);
        }

        private void ignore_Click(object sender, RoutedEventArgs e)
        {
            window.Close();
        }
    }
}
