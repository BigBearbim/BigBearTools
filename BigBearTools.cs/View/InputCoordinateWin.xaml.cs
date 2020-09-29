using Autodesk.Revit.DB;
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
using Transform = Autodesk.Revit.DB.Transform;

namespace BigBearTools
{
    /// <summary>
    /// InputCoordinateWin.xaml 的交互逻辑
    /// </summary>
    public partial class InputCoordinateWin : Window
    {
        public bool IsCreate = false;
        Document nwDoc;
        public InputCoordinateWin(Document doc)
        {
            InitializeComponent();
            nwDoc = doc;
        }
        int clickCount = 0;
        private void IsOk_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                clickCount++;
                ProjectPosition oldPro = nwDoc.ActiveProjectLocation.GetProjectPosition(XYZ.Zero);
                Transform transform = Transform.CreateTranslation(new XYZ(-oldPro.EastWest, -oldPro.NorthSouth, -oldPro.Elevation));
                double xDouble = double.Parse(this.xValue.Text);
                double yDouble = double.Parse(this.yValue.Text);
                XYZ purposePoint = new XYZ(xDouble.ToFeet(), yDouble.ToFeet(), 0);
                XYZ selPointTrans = Transform.Identity.OfPoint(purposePoint);
                selPointTrans = nwDoc.ActiveProjectLocation.GetTotalTransform().OfPoint(purposePoint);
                selPointTrans.InputDetailLine(nwDoc);
                MessageBox.Show("点 " + clickCount + " :" + "\t\n" + "北/南:" + yDouble + "\t\n" + "东/西:" + xDouble + "\t\n" + "标注成功");
            }
            catch 
            {

                MessageBox.Show ("只能输入数字","BigBearTools");
            }
           
        }

        private void IsCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void SetTxT_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex re = new Regex("[^0-9]");
            e.Handled = re.IsMatch(e.Text);
        }
        

    }
}
