using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows;

namespace BigBearTools
{
    /// <summary>
    /// 关于
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class About : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            MessageBox.Show("作者BigBear，联系QQ：204188258", "BigBearTool");
            return Result.Succeeded;
        }
    }
    
}
