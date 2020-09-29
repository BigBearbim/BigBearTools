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

namespace BigBearTools
{
    /// <summary>
    /// UnifyMEPSystemWin.xaml 的交互逻辑
    /// </summary>
    public partial class UnifyMEPSystemWin : Window
    {
        public UnifyMEPSystemWin()
        {
            InitializeComponent();
        }

        private void IsOK_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void IsCancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}
