using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.UI.Selection;
using System.Windows.Forms;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Electrical;

namespace BigBearTools
{
    /// <summary>
    /// 统一桥架类型
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class UnifyCTSystem : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            UIDocument uidoc = commandData.Application.ActiveUIDocument;
            Document doc = uidoc.Document;
            List<FamilySymbol> allFamilySymbol = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).OfCategory(BuiltInCategory.OST_CableTrayFitting).OfType<FamilySymbol>().ToList();
            Reference selCableTray;
            try
            {
                selCableTray = uidoc.Selection.PickObject(ObjectType.Element, new SelectCableTrayFilter(), "请选择需要统一类型的桥架");
            }
            catch (Exception)
            {
                MessageBox.Show("取消选择构件", "BigBearTool");
                return Result.Succeeded;
            }
            CableTray selMep = doc.GetElement(selCableTray) as CableTray;
            string selMepName = selMep.Name;
            //获取和它相连接的其他构件
            List<ElementId> linkCTId = selMep.GetLinkMepElement(ElementId.InvalidElementId,new List<ElementId>() { selMep.Id });
            Transaction trans = new Transaction(doc, "改变构件类型");
            trans.Start();
            foreach (var ctID in linkCTId)
            {
                Element ele = doc.GetElement(ctID);
                if (ele is CableTray linkCT)
                {
                    linkCT.ChangeTypeId(selMep.GetTypeId());
                }
                else if (ele is FamilyInstance ctFitting)
                {
                    try
                    {
                        var ctFitFSymbol = allFamilySymbol.Where(x => x.FamilyName == ctFitting.Symbol.FamilyName && x.Name == selMepName).FirstOrDefault();
                        ctFitting.ChangeTypeId(ctFitFSymbol.Id);
                    }
                    catch 
                    {

                        continue;
                    }
                }
               
                
            }
            trans.Commit();
            MessageBox.Show("替换成功", "BigBearTool");
            return Result.Succeeded;
        }
    }

   
}
