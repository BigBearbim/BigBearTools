using Autodesk.Revit.UI;
using Autodesk.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBearTools
{
    public class RibbonInfo:NotifyObject
    {
        string ribbonPanelName;
        RibbonTab ribbon;


        /// <summary>
        /// 选择的选项
        /// </summary>
        private List<RibbonInfo> selectedItems;
        public List<RibbonInfo> SelectedItems
        {
            get { return selectedItems; }
            set
            {
                selectedItems = value;
                this.RaisePropertyChanged(nameof(SelectedItems));
            }
        }

        /// <summary>
        /// 是否打开
        /// </summary>

        private bool isOpen;
        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                isOpen = value;
                this.RaisePropertyChanged(nameof(IsOpen));
            }
        }

        /// <summary>
        /// 选项卡名字
        /// </summary>
        public string RibbonPanelName { get => ribbonPanelName; set => ribbonPanelName = value; }
        /// <summary>
        /// 目标RibbonPanel
        /// </summary>
        public RibbonTab Ribbon { get => ribbon; set => ribbon = value; }
    }
}
