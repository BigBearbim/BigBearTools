using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Mechanical;
using System.Windows;

namespace BigBearTools
{
    [Transaction(TransactionMode.Manual)]
    public class CutBuildingWallCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //收集选择出来的构件
            List<Wall> selWallList = new List<Wall>();
            var originSelWall = uidoc.Selection.GetElementIds().Select(x => doc.GetElement(x)).OfType<Wall>().Where(m => m.StructuralUsage == Autodesk.Revit.DB.Structure.StructuralWallUsage.NonBearing).ToList();
            selWallList.AddRange(originSelWall);
            string text = "当前选择： " + selWallList.Count.ToString() + " 个构件";
            bool isClickSelection = false;
            do
            {
                CutBuildingWallWin cWin = new CutBuildingWallWin();
                cWin.SeletionText.Text = text;
                if (selWallList.Count != 0||isClickSelection) cWin.SeletionBto.Content = "重新选择>>";
                cWin.SetLeft(uidoc);
                cWin.ShowDialog();
                if (cWin.DialogResult == false && cWin.IsReselect == true)
                {
                    try
                    {
                        originSelWall = uidoc.Selection.PickObjects(Autodesk.Revit.UI.Selection.ObjectType.Element, new SelectWallFilter(), "选择建筑墙").Select(x => doc.GetElement(x)).OfType<Wall>().ToList();
                        text = "当前选择： " + originSelWall.Count.ToString() + " 个构件";
                        isClickSelection = true;
                        continue;
                    }
                    catch
                    {
                        break;
                    }

                }
                if (cWin.DialogResult == false) break;

                if (cWin.DialogResult == true)
                {
                    Transaction trans = new Transaction(doc, "梁切墙");
                    trans.Start();
                    foreach (var w in originSelWall)
                    {
                        //收集所有的梁
                        FilteredElementCollector beamInstanceList = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).OfCategory(BuiltInCategory.OST_StructuralFraming);
                        List<FamilyInstance> collisionFIList = new List<FamilyInstance>();
                        ElementIntersectsElementFilter elementIntersects = new ElementIntersectsElementFilter(w);
                        collisionFIList = beamInstanceList.WherePasses(elementIntersects).OfType<FamilyInstance>().ToList();
                        //添加已经有连接情况的梁
                        collisionFIList.AddRange(JoinGeometryUtils.GetJoinedElements(doc, w).Select(x => doc.GetElement(x)).OfType<FamilyInstance>().ToList());
                        if (collisionFIList.Count == 0) continue;
                        foreach (var beam in collisionFIList)
                        {
                            //判断是否连接，有的话取消连接
                            if (JoinGeometryUtils.AreElementsJoined(doc, w, beam) == true)
                            {
                                JoinGeometryUtils.UnjoinGeometry(doc, w, beam);
                            }
                            JoinGeometryUtils.JoinGeometry(doc, w, beam);
                        }

                    }
                    trans.Commit();
                    break;
                }



            } while (true);

            return Result.Succeeded;

        }
    }

}
