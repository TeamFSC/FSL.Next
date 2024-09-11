using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Windows.ApplicationModel.Background;

/*
 * 为什么写这个？
 * 当然是为了方便自己
 */

namespace FSL.Next.Utils
{
    internal class MessageBoxUtil
    {
        public static void ShowError(string locate, Exception ex)
        {
            iNKORE.UI.WPF.Modern.Controls.MessageBox.Show("很抱歉，FSL遇到错误，并且无法继续运行。\n请截图进行反馈。\n以下为基础信息：\n\nLOCATE:" + locate + "\nERROR: " + ex.Message + "\n\n" + ex, "FSL 发生了未经处理的异常",MessageBoxButton.OK,MessageBoxImage.Hand);
        }
    }
}
