using System.Configuration;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace FSL.Next
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 处理来自主线程的所有未捕获异常
            this.DispatcherUnhandledException += Current_DispatcherUnhandledException;

            // 处理来自非主线程的所有未捕获异常
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                FSL.Next.Windows.Exceptions.ExceptionWindow.ShowException(e.Exception as Exception);
            }
            finally
            {
                e.Handled = true;
            }
           
        }

        private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            try
            {
                FSL.Next.Windows.Exceptions.ExceptionWindow.ShowException(e.ExceptionObject as Exception);
            }
            catch
            {

            }
        }

        private void UIThread_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            try
            {
                FSL.Next.Windows.Exceptions.ExceptionWindow.ShowException(e.Exception as Exception);
            }
            finally
            {
                e.Handled = true;
            }
            
        }



















    }

}
