using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows.Forms;

namespace BigBearTools
{
    /// <summary>
    /// 坐标点输入
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class InputCoordinate : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            InputCoordinateWin icWin = new InputCoordinateWin(doc);
            icWin.Show();
            
            return Result.Succeeded;
        }
    }
    
}
