using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Windows;
using System.Collections.ObjectModel;

namespace BigBearTools
{
    /// <summary>
    /// 替换构件
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class ReplaceComponent : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //收集所有梁的族类型
            List<FamilySymbol> allBeamFSList = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilySymbol)).OfType<FamilySymbol>().ToList();
            //收集所有梁的族名称
            List<string> beamFamilyNameList = new List<string>();
            beamFamilyNameList = new FilteredElementCollector(doc).OfClass(typeof(Family)).OfType<Family>().Where(x => x.Name.Contains("梁")).Select(x => x.Name).ToList();
            
            //收集所有红瓦梁族
            List<FamilyInstance> allHWBeamFamilyInstance = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_StructuralFraming).OfClass(typeof(FamilyInstance)).OfType<FamilyInstance>().Where(x=>x.Symbol.Family.Name.Contains("HW")).ToList();
            //按照族类型分组
            List<Tuple<string, string, List<FamilyInstance>>> famInstanceTupleList = new List<Tuple<string, string, List<FamilyInstance>>>();
            for (int i = 0; i < allHWBeamFamilyInstance.Count; i++)
            {
                string familyName = allHWBeamFamilyInstance[i].Symbol.Family.Name;
                string familySymbolName = allHWBeamFamilyInstance[i].Symbol.Name;
                var index = famInstanceTupleList.FindIndex(x => x.Item1 == familyName && x.Item2 == familySymbolName);
                if(index == -1)
                {
                    
                    famInstanceTupleList.Add(new Tuple<string, string, List<FamilyInstance>>(familyName, familySymbolName, new List<FamilyInstance>() { allHWBeamFamilyInstance[i] }));
                }
                else
                {
                    var a = famInstanceTupleList[index];
                    famInstanceTupleList.RemoveAt(index);
                    List<FamilyInstance> nwFIList = a.Item3;
                    nwFIList.Add(allHWBeamFamilyInstance[i]);
                    famInstanceTupleList.Add(new Tuple<string, string, List<FamilyInstance>>(familyName, familySymbolName, nwFIList));
                }
            }
            if (famInstanceTupleList.Count == 0) MessageBox.Show("可能没有红瓦的族", "BigBearTools");
            //设置初始化的值
            List<string> beamFirstFSNameList = allBeamFSList.FindAll(x => x.FamilyName == beamFamilyNameList[0]).Select(m => m.Name).ToList();

            //记录传到界面的数据
            ObservableCollection<ReplaceInfo> replaceInfos = new ObservableCollection<ReplaceInfo>();
            foreach (var hwData in famInstanceTupleList)
            {
                ReplaceInfo rInfo = new ReplaceInfo();
                rInfo.HwFamilyName = hwData.Item1;
                rInfo.HwFamilySymbolName = hwData.Item2;
                rInfo.HwFamilyInstanceList = hwData.Item3;
                rInfo.ReplaceFamilyList = beamFamilyNameList;
                rInfo.ReplaceFSList = beamFirstFSNameList;
                replaceInfos.Add(rInfo);
            }
            ReplaceComponentWin rWin = new ReplaceComponentWin(doc, replaceInfos);
            rWin.ShowDialog();

            //取消操作
            if (rWin.DialogResult == false) return Result.Succeeded;

            var items = rWin.BeamDataGrid.ItemsSource;
            Transaction trans = new Transaction(doc, "修改类型");
            trans.Start();
            foreach (ReplaceInfo beamInfo in items)
            {
                var targetTuple = famInstanceTupleList.Find(x => x.Item1 == beamInfo.HwFamilyName && x.Item2 == beamInfo.HwFamilySymbolName);
                if (targetTuple == null) continue;
                List<FamilyInstance> selFamilyInstance = targetTuple.Item3;
                FamilySymbol fs = allBeamFSList.Find(x => x.Name == beamInfo.SelFSName && x.FamilyName == beamInfo.SelFamilyName);
                if (fs == null) continue;
                if (!fs.IsActive)
                {
                    fs.Activate();
                }
                foreach (var selInstance in selFamilyInstance)
                {
                    selInstance.ChangeTypeId(fs.Id);
                }
            }
            trans.Commit();
            return Result.Succeeded;
        }
    }
    
}
