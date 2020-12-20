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
    /// CutBuildingWallWin.xaml 的交互逻辑
    /// </summary>
    public partial class CutBuildingWallWin : Window
    {
        public bool IsReselect = false;
        public CutBuildingWallWin()
        {
            InitializeComponent();
        }

        private void SeletionBto_Click(object sender, RoutedEventArgs e)
        {
            IsReselect = true;

            this.DialogResult = false;
            this.Close();
        }

        private void IsOk_Click(object sender, RoutedEventArgs e)
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
