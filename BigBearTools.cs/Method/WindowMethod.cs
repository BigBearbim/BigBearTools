using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BigBearTools
{
    public static class WindowMethod
    {
        /// <summary>
        /// 设置窗体位置在左上角
        /// </summary>
        /// <param name="win"></param>
        /// <param name="ActiveUIDocument"></param>
        public static void SetLeft(this Window win, Autodesk.Revit.UI.UIDocument ActiveUIDocument)
        {
            Autodesk.Revit.UI.UIView uView = ActiveUIDocument.GetOpenUIViews().FirstOrDefault(x => x.ViewId == ActiveUIDocument.ActiveView.Id);
            var retangle = uView.GetWindowRectangle();
            win.Left = retangle.Left;
            win.Top = retangle.Top;
        }
    }
}
