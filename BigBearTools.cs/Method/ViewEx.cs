using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using View = Autodesk.Revit.DB.View;

namespace BigBearTools
{
    public static class ViewEx
    {
        /// <summary>
        /// 新建面积方案，事务在方法外开启
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static AreaScheme CreateAreaView(Document doc, string name)
        {
            AreaScheme areaScheme = new FilteredElementCollector(doc).OfClass(typeof(AreaScheme)).Cast<AreaScheme>().FirstOrDefault();

            ICollection<ElementId> newElemIds = ElementTransformUtils.CopyElement(doc, areaScheme.Id, XYZ.Zero);
            AreaScheme newVft = doc.GetElement(newElemIds.First()) as AreaScheme;
            newVft.Name = name;
            return newVft;
        }


    }
}
