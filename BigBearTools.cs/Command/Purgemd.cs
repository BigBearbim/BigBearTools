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
    /// 删除重复构件
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class Purgemd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //收集失败构件的ID
            List<ElementId> failElementList = new List<ElementId>();
            //把需要删除的构件收集起来
            List<Element> removeElemList = new List<Element>();
            //收集所有需要的构件
            List<Element> allElement = new FilteredElementCollector(doc).OfClass(typeof(FamilyInstance)).Where(x => x.Category.CategoryType != CategoryType.Annotation).ToList();
            var mepElemList = new FilteredElementCollector(doc).OfClass(typeof(MEPCurve)).ToList();
            allElement.AddRange(mepElemList);
            //开启进度条
            ProgressViewModel mvm = new ProgressViewModel();
            ProgressWindow progressWin = new ProgressWindow()
            {
                DataContext = mvm,
                WindowStartupLocation = WindowStartupLocation.CenterScreen

            };
            mvm.DetailStr = "正在遍历构件";
            mvm.MaxValue = allElement.Count;
            progressWin.Show();
            //通过判断是否相交
            for (int i = 0; i < allElement.Count; i++)
            {
                try
                {
                    var elem = allElement[i];
                    if (removeElemList.FindIndex(x => x.Id == elem.Id) != -1) continue;
                    XYZ elemCenPoint = GetCentelPoint(elem, doc.ActiveView);
                    if (elemCenPoint == null) continue;
                    FilteredElementCollector intercol = new FilteredElementCollector(doc);
                    ElementIntersectsElementFilter filter = new ElementIntersectsElementFilter(elem);
                    intercol.WherePasses(filter);
                    foreach (var collisionElm in intercol)
                    {
                        //类别ID判断
                        if (collisionElm.Category.Id != elem.Category.Id) continue;
                        //类型ID判断
                        if (collisionElm.GetTypeId() != elem.GetTypeId()) continue;
                        //计算中点
                        XYZ colElemCenPoint = GetCentelPoint(collisionElm, doc.ActiveView);
                        if (colElemCenPoint == null) continue;
                        if (elemCenPoint.DistanceTo(colElemCenPoint) > 20 / 304.8) continue;
                        removeElemList.Add(collisionElm);
                    }
                }
                catch
                {
                    failElementList.Add(allElement[i].Id);
                }
                //进度条加1
                mvm.CurrentValue++;
                ProgressWindow.DoEvents();
            }
            
            if (removeElemList.Count != 0)
            {
                //更新进度条
                mvm.ResetInfo("正在删除 重复构件", removeElemList.Count);
                //收集类型
                List<Tuple<string, int>> dataList = new List<Tuple<string, int>>();
                foreach (var reElement in removeElemList)
                {
                    string reElemName = doc.GetElement(reElement.GetTypeId()).Name;
                    int index = dataList.FindIndex(x => x.Item1 == reElemName);
                    if (index == -1)
                    {
                        dataList.Add(new Tuple<string, int>(reElemName, 1));
                    }
                    else
                    {
                        dataList[index] = new Tuple<string, int>(dataList[index].Item1, dataList[index].Item2 + 1);
                    }
                    mvm.CurrentValue++;
                    ProgressWindow.DoEvents();
                }
                Transaction trans = new Transaction(doc, "Delete Overlapping Elements");
                trans.Start();
                doc.Delete(removeElemList.Select(x=>x.Id).ToList());
                trans.Commit();
                //关闭窗口
                progressWin?.Close();
                //编辑输出信息
                string inputData= "输出结果："+"\t\n";
                foreach (var data in dataList)
                {
                    inputData += "“" + data.Item1 + "” 类型重复的构件数量为" + data.Item2 + "个"+"\t\n";
                }
                inputData += "一共删除" + removeElemList.Count + "个构件";
                MessageBox.Show(inputData, "BigBearTools");
            }
            else
            {
                //关闭窗口
                progressWin?.Close();
                MessageBox.Show("没有重复的构件", "BigBearTools");
            }
            if (failElementList.Count != 0)
            {
                string failID = "错误构件ID：" + "\t\n";
                foreach (var failElemID in failElementList)
                {
                    failID += failElemID.ToString() + "\t\n";
                }
                MessageBox.Show(failID, "BigBearTools");
            }


            return Result.Succeeded;
        }
        public XYZ GetCentelPoint (Element elem,View view)
        {
            XYZ centelPoint;
            try
            {
                XYZ maxPoint = elem.get_BoundingBox(view).Max;
                XYZ minPoint = elem.get_BoundingBox(view).Min;
                centelPoint = (maxPoint + minPoint) / 2;
            }
            catch 
            {
                centelPoint = null;
            }
           
            return centelPoint;
        }
    }
}
