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
    [Transaction(TransactionMode.Manual)]
    public class UndergroundPipeCmd : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            //实现读取窗口
            UndergroundPipeWin uWin = new UndergroundPipeWin();
            uWin.ShowDialog();

            if(uWin.DialogResult == false)
            {
                return Result.Succeeded;
            }

            string path = uWin.FilePath.Text;
            //得到表格信息
            var datiList = UnderGroundMethod.ReadUnderGroundData(path);
            //对表格信息进行分析

            

            return Result.Succeeded;
        }
    }
    
}
