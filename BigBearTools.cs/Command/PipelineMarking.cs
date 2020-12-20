using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;

namespace BigBearTools
{
    /// <summary>
    /// 多管线标注
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class PipelineMarking : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;

            return Result.Succeeded;
        }
    }
    
    
}
