using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using TextBox = System.Windows.Controls.TextBox;

namespace BigBearTools
{
    /// <summary>
    /// GridCreateWin.xaml 的交互逻辑
    /// </summary>
    public partial class GridCreateWin : Window
    {
        public GridCreateWin(UIDocument uidoc , View vd)
        {
            InitializeComponent();
            //常见值设置
            List<string> DepthValueList = new List<string>();
            for (int i = 300; i < 6600; i+=300)
            {
                DepthValueList.Add(i.ToString());
            }
            this.DepthValue.ItemsSource = DepthValueList;
            var gCVM = new GridCreateVM(uidoc, vd);
            gCVM.SelectionIndex = 0;
            this.DataContext = gCVM;
            //设置输入的线都在最后
            this.LeftDepth.SelectionStart = (this.DataContext as GridCreateVM).LeftDepth?.Length??0;
            this.RightDepth.SelectionStart = (this.DataContext as GridCreateVM).RightDepth?.Length ?? 0;
            this.UpDepth.SelectionStart = (this.DataContext as GridCreateVM).UpDepth?.Length ?? 0;
            this.DownDepth.SelectionStart = (this.DataContext as GridCreateVM).DownDepth?.Length ?? 0;
            this.LeftDepth.Focus();
            
        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            (this.PreView.Children.Cast<UIElement>().FirstOrDefault() as PreviewControl).Dispose();
            this.DialogResult = true;
            this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            (this.PreView.Children.Cast<UIElement>().FirstOrDefault() as PreviewControl).Dispose();
            this.DialogResult = false;
            this.Close();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            GridCreateVM gVM = this.DataContext as GridCreateVM;
            TextBox tBox = sender as TextBox;
            switch (tBox.Name)
            {
                case "LeftDepth":
                    gVM.SelectionDepth = 0;
                    break;
                case "RightDepth":
                    gVM.SelectionDepth = 1;
                    break;
                case "UpDepth":
                    gVM.SelectionDepth = 2;
                    break;
                case "DownDepth":
                    gVM.SelectionDepth = 3;
                    break;
            }
        }

        private void tb_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {

            Regex re = new Regex("[^0-9*,]+");
            e.Handled = re.IsMatch(e.Text);
        }

        private void RigthWithLeft_Checked(object sender, RoutedEventArgs e)
        {
            this.RightDepth.Text = this.LeftDepth.Text;
            this.RightDepth.IsEnabled = false;
        }

        private void RigthWithLeft_Click(object sender, RoutedEventArgs e)
        {
            if(RigthWithLeft.IsChecked == false)
            {
                if(this.RightDepth.IsEnabled == false)
                {
                    this.RightDepth.IsEnabled = true;
                }
            }
        }

        private void DownWithTop_Checked(object sender, RoutedEventArgs e)
        {
            this.DownDepth.Text = this.UpDepth.Text;
            this.DownDepth.IsEnabled = false;
        }

        private void DownWithTop_Click(object sender, RoutedEventArgs e)
        {
            if (DownWithTop.IsChecked == false)
            {
                if (this.DownDepth.IsEnabled == false)
                {
                    this.DownDepth.IsEnabled = true;
                }
            }
        }
    }
}
