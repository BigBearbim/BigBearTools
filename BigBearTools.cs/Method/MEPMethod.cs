using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBearTools
{
    public static class MEPMethod
    {
        /// <summary>
        /// 找管线对应的相连的管件和管线
        /// </summary>
        /// <param name="mepCurve">管线</param>
        /// <param name="linkId">某个端点连接的上一级管件的ID,如果是起始管道则为ElementId.InvalidElementId</param>
        /// <param name="firstElement">第一个选择的构件</param>
        /// <returns></returns>
        public static List<ElementId> GetLinkMepElement(this MEPCurve mepCurve, ElementId linkId,List<ElementId> linkEleID)
        {
            List<ElementId> linkElementIds = new List<ElementId> { mepCurve.Id };
            //拿到两端连接件
            List<Connector> connertorList = mepCurve.ConnectorManager.Connectors.Cast<Connector>().Where(x => x.IsConnected && x.AllRefs.Cast<Connector>().FirstOrDefault(m => m.Owner.Id == linkId) == null).ToList();

            foreach (var con in connertorList)
            {
                //找到对应连接的连接件
                List<Connector> linkConList = con.AllRefs.Cast<Connector>().Where(x => x.Owner.Id != con.Owner.Id).ToList();
                //添加连接到的构件
                var linkFitting = mepCurve.Document.GetElement(linkConList.FirstOrDefault().Owner.Id) as FamilyInstance;

                //拿到管件连接的所有管线
                List<ElementId> linkIds = GetLinkMepElement(linkFitting, mepCurve.Id, linkEleID);
                linkElementIds.AddRange(linkIds);
            }
            return linkElementIds;
        }
        /// <summary>
        /// 找管件对应的相连的管件和管线
        /// </summary>
        /// <param name="fitting">管件</param>
        /// <param name="linkId">某个端点连接的上一级管件或者管道的ID,如果是起始管件则为ElementId.InvalidElementId</param>
        /// <returns></returns>
        public static List<ElementId> GetLinkMepElement(FamilyInstance fitting, ElementId linkId, List<ElementId> linkEleID)
        {
            List<ElementId> linkElementIds = new List<ElementId> { fitting.Id };
            //拿到所有构件的连接件
            var fittingLinkConList = fitting.MEPModel.ConnectorManager.Connectors.Cast<Connector>().Where(x => x.IsConnected && x.AllRefs.Cast<Connector>().FirstOrDefault(m => m.Owner.Id == linkId) == null);
            foreach (var con in fittingLinkConList)
            {
                //找到对应连接的连接件
                List<Connector> linkConList = con.AllRefs.Cast<Connector>().Where(x => x.Owner.Id != con.Owner.Id).ToList();
                //添加连接到的构件
                var linkElement = fitting.Document.GetElement(linkConList.FirstOrDefault().Owner.Id);
                if (linkElement is FamilyInstance)
                {
                    linkElementIds.AddRange(GetLinkMepElement(linkElement as FamilyInstance, fitting.Id, linkEleID));
                }
                else
                {
                    //如果存在就跳出，不存在就加入
                    if (linkEleID.Contains(linkElement.Id)) break;
                    else linkEleID.Add(linkElement.Id);
                    linkElementIds.AddRange(GetLinkMepElement(linkElement as MEPCurve, fitting.Id, linkEleID));
                }

            }
            return linkElementIds;
        }

    }
}
