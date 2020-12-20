using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BigBearTools
{
    /// <summary>
    /// UndergroundPipeWin.xaml 的交互逻辑
    /// </summary>
    public partial class UndergroundPipeWin : Window
    {
        public UndergroundPipeWin()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }

        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Excel文件|*.xlsx|Excel文件|*.xls";
            ofd.ShowDialog();
            string filePath = ofd.FileName;
            this.FilePath.Text = filePath;
        }
    }
}
