using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows;
using Autodesk.Revit.UI.Selection;

namespace BigBearTools
{
    /// <summary>
    /// 坐标系统一
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class UnifiedCoordinateCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            RevitCommandId id = RevitCommandId.LookupPostableCommandId(PostableCommand.AcquireCoordinates);
            if (uiapp.CanPostCommand(id))
            {
                uiapp.PostCommand(id);
            }
            return Result.Succeeded;
        }
    }
    public class SelectRevitLinkI : ISelectionFilter
    {

        public bool AllowElement(Element elem)
        {
            if(elem is RevitLinkInstance||elem is ImportInstance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
