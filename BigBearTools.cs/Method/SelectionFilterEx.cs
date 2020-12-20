using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBearTools
{
    public class SelectionFilterEx
    {
    }
    /// <summary>
    /// 只能选择桥架
    /// </summary>
    public class SelectCableTrayFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is CableTray) return true;
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    public class SelectWallFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Wall w)
            {
                if(w.StructuralUsage == Autodesk.Revit.DB.Structure.StructuralWallUsage.NonBearing)
                return true;
            }
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    public class SelectMEPCurveFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is MEPCurve)
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
