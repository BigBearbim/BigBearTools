using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// RibbonPanelSwitchWin.xaml 的交互逻辑
    /// </summary>
    public partial class RibbonPanelSwitchWin : Window
    {
        ObservableCollection<RibbonInfo> nwRibbonInfoList;
        public RibbonPanelSwitchWin(ObservableCollection<RibbonInfo> ribbonInfoList)
        {
            InitializeComponent();
            nwRibbonInfoList = ribbonInfoList;
            this.RibbonDataGrid.ItemsSource = ribbonInfoList;
        }

        private void CheckAll_Click(object sender, RoutedEventArgs e)
        {
            foreach (var ribbonInfo in nwRibbonInfoList)
            {
                ribbonInfo.IsOpen = true;
            }
        }

        private void InvertSelection_Click(object sender, RoutedEventArgs e)
        {
            foreach (var ribbonInfo in nwRibbonInfoList)
            {
                if (ribbonInfo.IsOpen == false)
                {

                    ribbonInfo.IsOpen = true;
                }
                else
                {
                    ribbonInfo.IsOpen = false;

                }
            }
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

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            //拿到表格的选中的所有行
            var sItems = RibbonDataGrid.SelectedItems;
            foreach (RibbonInfo item in sItems)
            {
                item.IsOpen = (RibbonDataGrid.SelectedItem as RibbonInfo).IsOpen;
            }
            var newItemSource = RibbonDataGrid.ItemsSource;
            RibbonDataGrid.ItemsSource = null;
            RibbonDataGrid.ItemsSource = newItemSource;
        }
    }
}
