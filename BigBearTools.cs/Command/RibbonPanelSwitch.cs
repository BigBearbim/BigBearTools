using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using System.Collections.ObjectModel;
using Autodesk.Windows;

namespace BigBearTools
{
    /// <summary>
    /// 选项卡开关
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class RibbonPanelSwitch : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            ObservableCollection<RibbonInfo> ribbonInfoList = new ObservableCollection<RibbonInfo>();
            List<RibbonTab> a = ComponentManager.Ribbon.Tabs.Cast<RibbonTab>().ToList();
            
            //去除一些不需要的名称
            List<string> notNeedString = new List<string>()
            {
                "创建","内建模型","内建体量","族编辑器","附加模块"
            };
            foreach (var rp in a)
            {
                if (notNeedString.Contains(rp.AutomationName)) continue;
                if (rp.Id.Contains("Family")||rp.Id== "Second_Modify") continue;
                RibbonInfo rInfo = new RibbonInfo();
                rInfo.IsOpen = rp.IsVisible;
                rInfo.RibbonPanelName = rp.AutomationName;
                rInfo.Ribbon = rp;
                ribbonInfoList.Add(rInfo);
            }
            RibbonPanelSwitchWin prWin = new RibbonPanelSwitchWin(ribbonInfoList);
            prWin.ShowDialog();
            if (prWin.DialogResult == false) return Result.Succeeded;
            var items = prWin.RibbonDataGrid.Items;
            foreach (var item in items)
            {
                RibbonInfo selRInfo = item as RibbonInfo;
                selRInfo.Ribbon.IsVisible = selRInfo.IsOpen;
            }
            return Result.Succeeded;
        }
    }
}
