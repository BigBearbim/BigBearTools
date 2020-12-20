using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows.Forms;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;

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

            //var a = new FilteredElementCollector(doc).OfClass(typeof(TextNote)).Cast<TextNote>().ToList();
            //MessageBox.Show(a.Count().ToString());

            #region 创建基于工作平面的灯
            //FamilySymbol fs = doc.GetElement(new ElementId(730909)) as FamilySymbol;
            //fs.Activate();
            //XYZ selPoint = uidoc.Selection.PickPoint();
            ////创建一个平面
            //Transaction trans = new Transaction(doc, "123");
            //trans.Start();
            //Plane plane = Plane.CreateByNormalAndOrigin(new XYZ(0, 0, -1), selPoint);
            //SketchPlane sp = SketchPlane.Create(doc, plane);
            ////doc.ActiveView.SketchPlane = sp;
            ////doc.ActiveView.ShowActiveWorkPlane();
            //Level l = doc.ActiveView.GenLevel;
            //FamilyInstance instance = doc.Create.NewFamilyInstance(selPoint, fs, sp, l, StructuralType.NonStructural);
            //trans.Commit();
            #endregion
            //Reference selRefe = uidoc.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.LinkedElement, "请选择链接模型中的灯具");


            
            return Result.Succeeded;
        }
    }
    public class SeltectedLight : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            Document doc = elem.Document;
            RevitLinkInstance revitLink = elem as RevitLinkInstance;
            if(revitLink.Category.Id == new ElementId(BuiltInCategory.OST_LightingFixtures))
            {
                return true;
            }

            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
