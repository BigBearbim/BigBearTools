using Autodesk.Revit.DB;
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
using System.IO;


namespace BigBearTools
{
    /// <summary>
    /// ReplaceComponentWin.xaml 的交互逻辑
    /// </summary>
    public partial class ReplaceComponentWin : Window
    {
        Document nwDoc;
        ObservableCollection<ReplaceInfo> nwReplaceInfos = new ObservableCollection<ReplaceInfo>();
        public ReplaceComponentWin( Document doc,ObservableCollection<ReplaceInfo> replaceInfos)
        {
            InitializeComponent();
            this.Icon = new BitmapImage(new Uri(System.IO.Path.GetDirectoryName(typeof(ReplaceComponentWin).Assembly.Location) + "\\Libs\\Image\\熊.ico"));
            this.BeamDataGrid.ItemsSource = replaceInfos;
            nwDoc = doc;
            nwReplaceInfos = replaceInfos;

        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = BeamDataGrid.SelectedIndex;
            if(index!= -1)
            {
                List<string> replaceFSNameList = new FilteredElementCollector(nwDoc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().Where(x => x.FamilyName == nwReplaceInfos[index].SelFamilyName).Select(m => m.Name).ToList();
                nwReplaceInfos[index].ReplaceFSList = replaceFSNameList;
                
            }
            
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
    }
}
