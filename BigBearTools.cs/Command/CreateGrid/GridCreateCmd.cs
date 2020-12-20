using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows;
using System.IO;
using Autodesk.Revit.DB.Events;
using System.Diagnostics;

namespace BigBearTools
{
    /// <summary>
    /// 批量创建轴网
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class GridCreateCmd : IExternalCommand
    {
        Document _allDoc;
        FamilySymbol selFamilySymbol;
        FamilyInstance createFI;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            _allDoc = doc;
            double scale = doc.ActiveView.Scale;
            //新建一个详图视图提供绘制线
            var v = new FilteredElementCollector(doc).OfClass(typeof(ViewFamilyType)).Cast<ViewFamilyType>().Where(x => x.ViewFamily == ViewFamily.Detail).FirstOrDefault();
            List<GridType> gridNameList = new FilteredElementCollector(uidoc.Document).OfClass(typeof(GridType)).OfType<GridType>().ToList();
            List<DimensionType> dimensionTypes = new FilteredElementCollector(doc).OfClass(typeof(DimensionType)).OfType<DimensionType>().ToList();
            BoundingBoxXYZ boxXYZ = new BoundingBoxXYZ();
            boxXYZ.Min = new XYZ(0, 0, 0);
            boxXYZ.Max = new XYZ(1, 1, 1);
            Transform transform = Transform.Identity;
            transform.BasisZ = -XYZ.BasisZ;
            transform.BasisX = XYZ.BasisX;
            transform.BasisY = -XYZ.BasisY;
            boxXYZ.Transform = transform;

            TransactionGroup transGroup = new TransactionGroup(doc, "创建轴网");
            transGroup.Start();

            Transaction trans = new Transaction(doc, "创建绘图视图");
            trans.Start();
            //关闭当前轴网的可见性
            Category.GetCategory(doc, BuiltInCategory.OST_Grids).set_Visible(doc.ActiveView, false);
            Category.GetCategory(doc, BuiltInCategory.OST_Dimensions).set_Visible(doc.ActiveView, false);
            ViewSection vd = ViewSection.CreateDetail(doc, v.Id, boxXYZ);
            vd.Name = "创建轴网的临时视图";
            vd.get_Parameter(BuiltInParameter.VIEWER_CROP_REGION_VISIBLE).Set(0);
            vd.get_Parameter(BuiltInParameter.VIEWER_CROP_REGION).Set(0);
            vd.IsolateCategoryTemporary(new ElementId(BuiltInCategory.OST_Grids));
            vd.IsolateCategoriesTemporary(new List<ElementId>() { new ElementId(BuiltInCategory.OST_Grids), new ElementId(BuiltInCategory.OST_Dimensions) });
            vd.ConvertTemporaryHideIsolateToPermanent();
            doc.Regenerate();
            trans.Commit();
            //新建一个预览界面
            PreviewControl pre = new PreviewControl(doc, vd.Id);
            GridCreateWin gWin = new GridCreateWin(uidoc, vd);
            gWin.GridFamilySymbol.ItemsSource = gridNameList.Select(x => x.FamilyName + "-" + x.Name);
            gWin.DimensionFamilySymbol.ItemsSource = dimensionTypes.Select(x => x.Name);
            gWin.GridFamilySymbol.SelectedIndex = 0;
            gWin.PreView.Children.Add(pre);
            gWin.ShowDialog();
            pre.Dispose();
            var gVM = gWin.DataContext as GridCreateVM;
            List<Grid> createGridList = gVM.oldGridList;
            if (gWin.DialogResult == false)
            {
                Transaction trans6 = new Transaction(doc, "删除视图");
                trans6.Start();
                doc.Regenerate();
                doc.Delete(vd.Id);
                doc.Delete(createGridList.Select(x => x.Id).ToList());
                Category.GetCategory(doc, BuiltInCategory.OST_Grids).set_Visible(doc.ActiveView, true);
                Category.GetCategory(doc, BuiltInCategory.OST_Dimensions).set_Visible(doc.ActiveView, true);
                trans6.Commit();
                transGroup.Assimilate();
                return Result.Succeeded;
            }

            //选择放置点
            XYZ selPoint = new XYZ(100, 0, 0);
           

            List<Line> gridLocationLines = createGridList.Select(x => x.Curve as Line).ToList();

            //加载族
            string path = Path.GetDirectoryName(typeof(GridCreateCmd).Assembly.Location) + "\\Libs\\Family\\公制常规注释.rfa";
            Document famDoc = commandData.Application.Application.OpenDocumentFile(path);
            //进入族中进行操作
            View famView = new FilteredElementCollector(famDoc).OfCategory(BuiltInCategory.OST_Sheets).OfType<View>().FirstOrDefault();
            double famScale = famView.Scale;
            Transaction famTrans = new Transaction(famDoc, "创建线");
            famTrans.Start();
            foreach (var item in gridLocationLines)
            {
                famDoc.FamilyCreate.NewDetailCurve(famView, item.ScalingLine(scale));
            }
            famTrans.Commit();

            var fam = famDoc.LoadFamily(doc,new projectFamLoadOption());

            famDoc.Close(false);
            FamilySymbol fs = doc.GetElement(fam.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;
            selFamilySymbol = fs;
            commandData.Application.Application.DocumentChanged += new EventHandler<DocumentChangedEventArgs>(DocumentChangedForSomething);
            try
            {
                uidoc.PromptForFamilyInstancePlacement(fs);
            }
            catch
            {

            }
            selPoint = (createFI.Location as LocationPoint).Point;
            commandData.Application.Application.DocumentChanged -= new EventHandler<DocumentChangedEventArgs>(DocumentChangedForSomething);
            Transaction trans3 = new Transaction(doc, "移动轴网");
            trans3.Start();
            ElementTransformUtils.MoveElements(doc, createGridList.Select(x => x.Id).ToList(), selPoint);
            trans3.Commit();


            Transaction trans2 = new Transaction(doc, "删除视图");
            trans2.Start();
            doc.Regenerate();
            doc.Delete(vd.Id);
            doc.Delete(createFI.Id);
            Category.GetCategory(doc, BuiltInCategory.OST_Grids).set_Visible(doc.ActiveView, true);
            Category.GetCategory(doc, BuiltInCategory.OST_Dimensions).set_Visible(doc.ActiveView, true);

            trans2.Commit();
            transGroup.Assimilate();
            return Result.Succeeded;
        }

        private void DocumentChangedForSomething(object sender, DocumentChangedEventArgs e)
        {
            if (e.GetAddedElementIds().Count != 0)
            {
                FamilyInstance elem = _allDoc.GetElement(e.GetAddedElementIds().First()) as FamilyInstance;

                if (elem!=null&&elem.Symbol.Id == selFamilySymbol.Id)
                {
                    Helper.SendKeys(Process.GetCurrentProcess(), System.Windows.Forms.Keys.Escape);
                    createFI = elem;
                }
            }

        }


    }
    public class projectFamLoadOption : IFamilyLoadOptions
    {
        bool IFamilyLoadOptions.OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            return true;
        }
        bool IFamilyLoadOptions.OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            source = FamilySource.Project;
            overwriteParameterValues = true;
            return true;
        }
    };
}
