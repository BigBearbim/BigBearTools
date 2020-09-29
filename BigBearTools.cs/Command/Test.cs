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
    [Transaction(TransactionMode.Manual)]
    public class Test : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            #region 获取标签的中点
            //var a = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "选择标签");
            //var e = doc.GetElement(a);
            //XYZ z = XYZEx.GetCentelPoint(e, doc.ActiveView);
            //z.XYZTest(doc);
            #endregion
            #region 测试过滤
            //var a = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, "选择标签");
            //var e = doc.GetElement(a);
            //if(e.Category.CategoryType == CategoryType.Annotation)
            //{

            //XYZ z = XYZEx.GetCentelPoint(e, doc.ActiveView);
            //z.XYZTest(doc);
            //}

            #endregion
            //List<Element> allElement = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).ToList();
            //MessageBox.Show(allElement.Count.ToString());

            #region 研究PickBox
            //var a = uidoc.Selection.PickBox(Autodesk.Revit.UI.Selection.PickBoxStyle.Directional);
            //a.Max.XYZTest(doc);
            //a.Min.XYZTest(doc);
            #endregion

            var a = new FilteredElementCollector(doc).OfClass(typeof(TextNote)).Cast<TextNote>().ToList();
            MessageBox.Show(a.Count().ToString());
            return Result.Succeeded;
        }
    }
}
