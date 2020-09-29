using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.IO;
using System.Windows.Forms;

namespace BigBearTools
{
    /// <summary>
    /// 坐标值查询
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class QueryCoordinateCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            if (doc.ActiveView is View3D)
            {
                MessageBox.Show("该命令仅支持在平面视图中使用", "BigBearTool");
                return Result.Succeeded;
            }
            #region 载入族
            //查找族参数
            FamilySymbol queryFS = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_GenericModel).Cast<FamilySymbol>().FirstOrDefault(x => x.Name == "点标记");
            if (queryFS == null)
            {
                string path = Path.GetDirectoryName(typeof(QueryCoordinateCmd).Assembly.Location) + "\\Libs\\Family\\点标记.rfa";
                Transaction trans = new Transaction(doc, "载入族文件");
                trans.Start();
                doc.LoadFamily(path, out Family queryF);
                queryFS = doc.GetElement(queryF.GetFamilySymbolIds().FirstOrDefault()) as FamilySymbol;
                trans.Commit();
            }
            #endregion
            XYZ selPoint;
            try
            {
                selPoint = uidoc.Selection.PickPoint("请选择坐标点");
            }
            catch
            {
                return Result.Succeeded;
            }
            XYZ nwSelPoint = new XYZ(selPoint.X, selPoint.Y, selPoint.Z);
            ProjectPosition oldPro = doc.ActiveProjectLocation.GetProjectPosition(XYZ.Zero);
            Transform transform = Transform.CreateTranslation(new XYZ(oldPro.EastWest, oldPro.NorthSouth, oldPro.Elevation));
            XYZ selPointTrans = transform.OfPoint(nwSelPoint);
            Transaction trans2 = new Transaction(doc, "生成点的坐标");
            trans2.Start();
            if (!queryFS.IsActive)
            {
                queryFS.Activate();
            }
            FamilyInstance pointFI = doc.Create.NewFamilyInstance(selPoint, queryFS, Autodesk.Revit.DB.Structure.StructuralType.NonStructural);
            pointFI.GetParameters("东/西").First().Set(selPointTrans.X.ToMM().Round(2).ToString());
            pointFI.GetParameters("北/南").First().Set(selPointTrans.Y.ToMM().Round(2).ToString());
            trans2.Commit();
            return Result.Succeeded;
        }
    }

}
