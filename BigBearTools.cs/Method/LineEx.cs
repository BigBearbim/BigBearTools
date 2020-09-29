using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using View = Autodesk.Revit.DB.View;

namespace BigBearTools
{
    public static class LineEx
    {

        public static void SaveLineText(this List<List<Autodesk.Revit.DB.Line>> lineListList, string path)
        {
            string str = null;
            for (int i = 0; i < lineListList.Count; i++)
            {
                lineListList[i].ConvertAll(m =>
                {
                    XYZ sp = m.GetEndPoint(0);
                    XYZ ep = m.GetEndPoint(1);
                    str += Math.Round(sp.X, 9) + "," + Math.Round(sp.Y, 9) + "," + Math.Round(ep.X, 9) + "," + Math.Round(ep.Y, 9) + "," + "\r\n";
                    return str;
                });
                str += "----------" + "\r\n";
            }
            File.WriteAllText(path, str);
        }

        /// <summary>
        /// 封闭轮廓线首尾相连
        /// </summary>
        /// <param name="lineList"></param>
        /// <returns></returns>
        public static List<Line> OrderLineList(this List<Line> lineList)
        {
            List<Autodesk.Revit.DB.Line> lines = new List<Autodesk.Revit.DB.Line> { lineList[0] };
            lineList.RemoveAt(0);
            XYZ firstStart = lines[0].GetEndPoint(0);
            while (lineList.Count != 0)
            {
                XYZ lastEnd = lines.Last().GetEndPoint(1);
                //回到起点
                if (lastEnd.DistanceTo(firstStart) < 1 / 304.8)
                {
                    break;
                }
                int index = lineList.FindIndex(m => m.GetEndPoint(0).DistanceTo(lastEnd) < 1 / 304.8);
                if (index != -1)//最后一根线的终点与集合中某条线的起点相同
                {
                    lines.Add(lineList.ElementAt(index));
                    //移除线
                    lineList.RemoveAt(index);
                }
                else//最后一根线的终点与集合中某条线的终点相同
                {
                    index = lineList.FindIndex(m => m.GetEndPoint(1).DistanceTo(lastEnd) < 1 / 304.8);
                    if (index != -1)//如果存在就将线前后翻转一下，添加进集合
                    {
                        lines.Add(lineList.ElementAt(index).CreateReversed() as Autodesk.Revit.DB.Line);
                        //移除线
                        lineList.RemoveAt(index);
                    }
                    else//可能是没有线与它相同
                    {
                        //可能是不封闭的
                        break;
                    }
                }
            }
            CurveLoop cuLoop = CurveLoop.Create(lines.OfType<Curve>().ToList());
            if (!cuLoop.IsOpen())
                return lines;
            return null;
        }

        /// <summary>
        /// 获取封闭的框
        /// </summary>
        /// <param name="curveList"></param>
        /// <returns></returns>
        public static List<Line> GetCloseProfiles(this List<Line> curveList, XYZ pickPoint)
        {
            //去除桥接线重复的线
            curveList = JoinLines(curveList, 0.1 / 304.8);
            #region 拆分线段
            List<Line> lineList = curveList.SelectMany(
                m =>
                {
                    List<Line> breakLineList = new List<Line>();
                    int index = curveList.IndexOf(m);
                    //存储所需要的交点
                    List<XYZ> intersectPointList = new List<XYZ>();
                    for (int i = 0; i < curveList.Count; i++)
                    {
                        if (CheckDirection(m.Direction, curveList[i].Direction)) continue;

                        m.Intersect(curveList[i], out IntersectionResultArray resultArray);
                        if (resultArray?.get_Item(0)?.XYZPoint != null) intersectPointList.Add(resultArray.get_Item(0).XYZPoint);
                    }
                    //ToDo：不太确定起终点不放进去会不会有问题
                    //点去重
                    intersectPointList = intersectPointList.Where((x, i) => intersectPointList.FindIndex(c => c.DistanceTo(x) < 1 / 304.8) == i).ToList();
                    //排列点并重新生成线段集合
                    intersectPointList = intersectPointList.OrderBy(x => m.GetEndPoint(0).DistanceTo(x)).ToList();
                    //线段集合
                    List<Line> lines = new List<Line>();
                    for (int i = 0; i < intersectPointList.Count - 1; i++)
                    {
                        Line l = Line.CreateBound(intersectPointList[i], intersectPointList[i + 1]);
                        lines.Add(l);
                    }
                    return lines;
                }
                ).ToList();
            #endregion

            #region 找轮廓

            //轮廓集合

            Line firstLine = lineList.OrderBy(x => x.Evaluate(0.5, true).DistanceTo(pickPoint)).First();
            lineList.Remove(firstLine);
            //逆时针
            if (!firstLine.IsClockwise(pickPoint))
            {
                firstLine = firstLine.CreateReversed() as Line;
            }
            List<Line> profileList = new List<Line>() { firstLine };
            XYZ firstStart = firstLine.GetEndPoint(0);
            while (lineList.Count != 0)
            {
                XYZ lastEnd = profileList.Last().GetEndPoint(1);
                //回到起点
                if (lastEnd.DistanceTo(firstStart) < 1 / 304.8)
                {
                    break;
                }
                Line targetLine = null;
                List<Line> sigleLineList = new List<Line>();
                for (int i = 0; i < lineList.Count; i++)
                {
                    Line currentLine = lineList[i];
                    double minAngle = int.MaxValue;
                    //排除平行
                    if (CheckDirection(currentLine.Direction, profileList.Last().Direction)) continue;
                    if (currentLine.GetEndPoint(0).DistanceTo(lastEnd) < 1 / 304.8)
                    {
                        lineList.RemoveAt(i);
                        i--;
                        //是否绕点顺时针
                        if (currentLine.IsClockwise(pickPoint))
                        {
                            //计算角度
                            double currentAngle = currentLine.Direction.AngleTo(lineList.Last().Direction);
                            if (currentAngle < minAngle)
                            {
                                minAngle = currentAngle;
                                targetLine = currentLine;
                            }
                        }
                        sigleLineList.Add(currentLine);

                    }
                    else if (currentLine.GetEndPoint(1).DistanceTo(lastEnd) < 1 / 304.8)
                    {
                        lineList.RemoveAt(i);
                        i--;
                        //反转线
                        currentLine = currentLine.CreateReversed() as Line;
                        //是否绕点顺时针
                        if (currentLine.IsClockwise(pickPoint))
                        {
                            //计算角度
                            double currentAngle = currentLine.Direction.AngleTo(lineList.Last().Direction);
                            if (currentAngle < minAngle)
                            {
                                minAngle = currentAngle;
                                targetLine = currentLine;
                            }
                        }
                        sigleLineList.Add(currentLine);
                    }
                }
                targetLine = sigleLineList.Count == 1 ? sigleLineList[0] : targetLine;
                if (targetLine == null)
                {
                    //不封闭
                    break;
                }
                else
                {
                    profileList.Add(targetLine);
                }
            }
            //将所有为1的轮廓进行重新的排布,拿到最大圈的轮廓
            #endregion
            var cuLoop = CurveLoop.Create(profileList.Cast<Curve>().ToList());
            return /*cuLoop.IsOpen() ? null :*/ profileList;
        }
        public static bool IsClockwise(this Line line, XYZ pickPoint)
        {
            XYZ startDirection = (line.GetEndPoint(0) - pickPoint).Normalize();
            return startDirection.CrossProduct(line.Direction).Normalize().DistanceTo(XYZ.BasisZ) > 1.99;
        }


        public static List<Line> GetCloseBoundary(this List<Line> lines, XYZ selPoint)
        {

            List<Line> lineList = new List<Line>();
            Line firstLine = lines[0];
            XYZ endDirection = (firstLine.GetEndPoint(0) - selPoint).Normalize();
            //if()

            lineList.Add(lines[0]);
            lines.RemoveAt(0);
            XYZ firstStart = lineList[0].GetEndPoint(0);
            int i = 0;
            while (i < 250)
            {
                Line lastLine = lineList.Last();
                XYZ lastStart = lastLine.GetEndPoint(0);
                XYZ lastEnd = lastLine.GetEndPoint(1);
                //回到起点
                if (lastEnd.DistanceTo(firstStart) < 1 / 304.8 && !lineList[0].IsSameLine(lastLine))
                {

                    break;
                }
                if (lastStart.DistanceTo(firstStart) < 1 / 304.8 && !lineList[0].IsSameLine(lastLine))
                {

                    break;
                }
                List<Line> tempLines = lines.FindAll(m => m.GetEndPoint(0).DistanceTo(lastEnd) < 1 / 304.8 || m.GetEndPoint(1).DistanceTo(lastEnd) < 1 / 304.8);
                if (tempLines.Count == 0)
                {
                    break;
                }
                else if (tempLines.Count == 3)
                {
                    //处理方向，以点出发
                    for (int j = 0; j < 3; j++)
                    {
                        if (!(tempLines[i].GetEndPoint(0).DistanceTo(lastEnd) < 0.1 / 304.8))
                        {
                            tempLines[i] = tempLines[i].CreateReversed() as Line;
                        }
                    }
                    //共线集合
                    var sameDirtLineList = tempLines.Where(x => CheckDirection(x.Direction, lastLine.Direction)).ToList();
                    //非共线集合
                    var diffDirtLineList = tempLines.Where(x => !CheckDirection(x.Direction, lastLine.Direction)).ToList();

                    if (sameDirtLineList.Count != 0)
                    {
                        foreach (var sameLine in sameDirtLineList)
                        {

                        }
                    }

                    Line l1 = tempLines[0];
                    if (l1.GetEndPoint(1).DistanceTo(lastEnd) < 1 / 304.8)
                    {
                        l1 = l1.CreateReversed() as Line;
                    }
                    lineList.Add(l1);
                    lines.RemoveAt(lines.FindIndex(x => x.IsSameLine(l1)));
                }
                else
                {
                    Line l1 = tempLines[0];
                    if (l1.GetEndPoint(1).DistanceTo(lastEnd) < 1 / 304.8)
                    {
                        l1 = l1.CreateReversed() as Line;
                    }
                    lineList.Add(l1);
                    lines.RemoveAt(lines.FindIndex(x => x.IsSameLine(l1)));
                }
                i++;
            }
            return lineList;
        }
       
        /// <summary>
        /// 桥接线
        /// </summary>
        /// <param name="lines"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static List<Autodesk.Revit.DB.Line> JoinLines(this List<Autodesk.Revit.DB.Line> lines, double range)
        {
            var newLines = new List<Autodesk.Revit.DB.Line>();
            //先按是否平行且间距小于range分组
            List<List<Autodesk.Revit.DB.Line>> lineListList = new List<List<Autodesk.Revit.DB.Line>>();
            for (int i = 0; i < lines.Count; i++)
            {
                int index = -1;
                for (int j = 0; j < lineListList.Count; j++)
                {
                    XYZ midPoint = lineListList[j][0].Evaluate(0.5, true);
                    Autodesk.Revit.DB.Line l = Autodesk.Revit.DB.Line.CreateBound(midPoint + 1000 * lineListList[j][0].Direction, midPoint - 1000 * lineListList[j][0].Direction);
                    XYZ pj = l.Project(lines[i].GetEndPoint(0)).XYZPoint;
                    double d = pj.DistanceTo(lines[i].GetEndPoint(0));
                    if (lineListList[j][0].Direction.IsParallel(lines[i].Direction) && d < range)
                    {
                        index = j;
                        break;
                    }
                }
                if (index == -1) lineListList.Add(new List<Autodesk.Revit.DB.Line>() { lines[i] });
                else lineListList[index].Add(lines[i]);
            }
            for (int i = 0; i < lineListList.Count; i++)
            {
                List<Autodesk.Revit.DB.Line> lineList = lineListList[i];
                //方向
                var dirt = lineList.ElementAt(0).Direction;
                //点乘信息 线索引 端点坐标 坐标点乘方向
                List<Tuple<int, XYZ, double>> lineEndInfoList = new List<Tuple<int, XYZ, double>>();
                //点乘信息添加
                for (int j = 0; j < lineList.Count; j++)
                {
                    lineEndInfoList.Add(Tuple.Create(j, lineList[j].GetEndPoint(0), lineList[j].GetEndPoint(0).DotProduct(dirt).Round(2)));
                    lineEndInfoList.Add(Tuple.Create(j, lineList[j].GetEndPoint(1), lineList[j].GetEndPoint(1).DotProduct(dirt).Round(2)));
                }
                //排序
                lineEndInfoList = lineEndInfoList.OrderBy(x => x.Item3).ToList();
                List<XYZ> xyzList = new List<XYZ>();

                while (lineEndInfoList.Count > 0)
                {
                    var first = lineEndInfoList.ElementAt(0);
                    lineEndInfoList.RemoveAt(0);
                    xyzList.Add(first.Item2);
                    //拿到第一个线对应的另外一个点
                    int index = lineEndInfoList.FindIndex(x => x.Item1 == first.Item1);
                    while (true)
                    {
                        //拿到这部分的集合
                        var tempInfoList = lineEndInfoList.GetRange(0, index + 1);

                        //最后的一个元素
                        var endTemp = tempInfoList.Last();

                        //移除该集合
                        lineEndInfoList.RemoveRange(0, index + 1);

                        //拿到只有一个点在该区域的集合
                        List<Tuple<int, XYZ, double>> remainTempList = new List<Tuple<int, XYZ, double>>();
                        //排除掉中间有2个索引的线
                        while (tempInfoList.Count > 0)
                        {
                            var firstTemp = tempInfoList.ElementAt(0);
                            tempInfoList.RemoveAt(0);
                            var tempIndex = tempInfoList.FindIndex(x => x.Item1 == firstTemp.Item1);
                            if (tempIndex != -1)
                            {
                                tempInfoList.RemoveAt(tempIndex);
                            }
                            else
                            {
                                remainTempList.Add(firstTemp);
                            }
                        }
                        if (remainTempList.Count == 0)
                        {
                            xyzList.Add(endTemp.Item2);
                            break;
                        }
                        //找对应最后的一个点的信息
                        index = remainTempList.Select(x => lineEndInfoList.FindIndex(m => m.Item1 == x.Item1)).OrderBy(x => x).Last();
                        if (index == -1)
                        {
                            if (lineEndInfoList.Count != 0 && lineEndInfoList[0].Item2.DistanceTo(endTemp.Item2) < 1 / 304.8)
                            {
                                //两线桥接--只有一个交点的情况
                                var firstIndex = lineEndInfoList[0].Item1;
                                lineEndInfoList.RemoveAt(0);
                                index = lineEndInfoList.FindIndex(x => x.Item1 == firstIndex);
                            }
                            else
                            {
                                xyzList.Add(endTemp.Item2);
                                break;
                            }
                        }
                    }
                }
                for (int k = 0; k < xyzList.Count; k += 2)
                {
                    XYZ pStart = xyzList[k];
                    XYZ pEnd = xyzList[k + 1];
                    if (pStart.DistanceTo(pEnd) > 1 / 304.8)
                    {
                        //var mc = Line.CreateBound(pStart, pEnd).GenerateMc(doc, true);
                        //mc.SetData(Properties.Resources.TestGuid, Properties.Resources.TestName, i.ToString() + "   " + lineListList[i].Count);
                        newLines.Add(Autodesk.Revit.DB.Line.CreateBound(pStart, pEnd));
                    }
                }
            }
            return newLines;

        }
        /// <summary>
        /// 是否共线
        /// </summary>
        /// <param name="line1"></param>
        /// <param name="line2"></param>
        /// <returns></returns>
        public static bool IsCollinear(this Autodesk.Revit.DB.Line line1, Autodesk.Revit.DB.Line line2, double distance = 0.05)
        {
            return line1.Direction.IsParallel(line2.Direction) && line2.DisToPoint(line1.Evaluate(0.5, true), out XYZ direction) < distance;
        }
        /// <summary>
        /// 在点的位置形成交叉
        /// </summary>
        /// <param name="point"></param>
        /// <param name="doc"></param>
        /// <returns></returns>
        public static List<ElementId> InputDetailLine(this XYZ point, Document doc)
        {
            Level lev = doc.ActiveView.GenLevel;
            var lin1 = Line.CreateBound(point.Add(new XYZ(1, 1, 0)), point.Subtract(new XYZ(1, 1, 0)));
            var lin2 = Line.CreateBound(point.Add(new XYZ(-1, 1, 0)), point.Subtract(new XYZ(-1, 1, 0)));
            var detail1 = doc.Create.NewDetailCurve(doc.ActiveView, lin1);
            var detail2 = doc.Create.NewDetailCurve(doc.ActiveView, lin2);
            return new List<ElementId> { detail1.Id, detail2.Id };
        }

        public static bool IsSameLine(this Autodesk.Revit.DB.Line l1, Autodesk.Revit.DB.Line l2)
        {
            if (l1.GetEndPoint(0).DistanceTo(l2.GetEndPoint(0)) < 1 / 304.8 && l1.GetEndPoint(1).DistanceTo(l2.GetEndPoint(1)) < 1 / 304.8)
                return true;
            else if (l1.GetEndPoint(1).DistanceTo(l2.GetEndPoint(0)) < 1 / 304.8 && l1.GetEndPoint(0).DistanceTo(l2.GetEndPoint(1)) < 1 / 304.8)
                return true;
            else
                return false;
        }

        /// <summary>
        /// 测试线
        /// </summary>
        /// <param name="l"></param>
        /// <param name="doc"></param>
        /// <param name="isInTransaction"></param>
        public static void LineTest(this Autodesk.Revit.DB.Line l, Document doc, bool isInTransaction = false)
        {
            XYZ basicZ = XYZ.BasisZ;
            if (l.Direction.AngleTo(XYZ.BasisZ) < 0.0001 || l.Direction.AngleTo(-XYZ.BasisZ) < 0.0001)
                basicZ = XYZ.BasisY;
            XYZ normal = basicZ.CrossProduct(l.Direction).Normalize();
            Plane pl = Plane.CreateByNormalAndOrigin(normal, l.GetEndPoint(0));
            Transaction transCreate = new Transaction(doc, "创建模型线");
            if (!isInTransaction)
                transCreate.Start();
            SketchPlane sktpl = SketchPlane.Create(doc, pl);
            ModelCurve mc = doc.Create.NewModelCurve(l, sktpl);
            if (!isInTransaction)
                transCreate.Commit();
        }

        /// <summary>
        /// 设置z轴参数
        /// </summary>
        /// <param name="line"></param>
        /// <param name="z"></param>
        /// <returns></returns>
        public static Autodesk.Revit.DB.Line SetZ(this Autodesk.Revit.DB.Line line, double z = 0)
        {
            if (line.Direction.IsParallel(XYZ.BasisZ))
                throw new ArgumentException("线无法进行Z值设置");
            if (line.GetEndPoint(0).SetZ(z).DistanceTo(line.GetEndPoint(1).SetZ(z)) < 1 / 304.8)
                throw new ArgumentException("线太短");
            return Autodesk.Revit.DB.Line.CreateBound(line.GetEndPoint(0).SetZ(z), line.GetEndPoint(1).SetZ(z));
        }

        public static Curve SetZ(this Curve curve, double z = 0)
        {
            var line = curve as Autodesk.Revit.DB.Line;
            if (line.Direction.IsParallel(XYZ.BasisZ))
                throw new ArgumentException("线无法进行Z值设置");
            if (line.GetEndPoint(0).SetZ(z).DistanceTo(line.GetEndPoint(1).SetZ(z)) < 1 / 304.8)
                throw new ArgumentException("线太短");
            return Autodesk.Revit.DB.Line.CreateBound(line.GetEndPoint(0).SetZ(z), line.GetEndPoint(1).SetZ(z));
        }
        /// <summary>
        /// 点到线的距离
        /// </summary>
        /// <param name="line"></param>
        /// <param name="point"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static double DisToPoint(this Autodesk.Revit.DB.Line line, XYZ point, out XYZ direction)
        {
            var nwLine = line.SetZ();
            nwLine.MakeUnbound();
            XYZ projectPoint = nwLine.Project(point.SetZ()).XYZPoint;
            double distance = point.SetZ().DistanceTo(projectPoint);
            direction = (point.SetZ() - projectPoint).Normalize();
            return distance;

        }
        public static double DisToPoint(this Autodesk.Revit.DB.Line line, XYZ point)
        {
            var nwLine = line.SetZ();
            nwLine.MakeUnbound();
            XYZ projectPoint = nwLine.Project(point.SetZ()).XYZPoint;
            double distance = point.SetZ().DistanceTo(projectPoint);
            return distance;

        }

        /// <summary>
        /// 获取封闭的框
        /// </summary>
        /// <param name="curveList"></param>
        /// <returns></returns>
        //public static List<Line> GetCloseProfiles(this List<Line> curveList,XYZ pickPoint)
        //{
        //    //去除桥接线重复的线
        //    curveList = JoinLines(curveList, 0.1 / 304.8);
        //    #region 拆分线段
        //    List<Line> lineList = curveList.SelectMany(
        //        m =>
        //        {
        //            List<Line> breakLineList = new List<Line>();
        //            int index = curveList.IndexOf(m);
        //            //存储所需要的交点
        //            List<XYZ> intersectPointList = new List<XYZ>();
        //            for (int i = 0; i < curveList.Count; i++)
        //            {
        //                if (CheckDirection(m.Direction, curveList[i].Direction)) continue;

        //                m.Intersect(curveList[i], out IntersectionResultArray resultArray);
        //                if (resultArray?.get_Item(0)?.XYZPoint != null) intersectPointList.Add(resultArray.get_Item(0).XYZPoint);
        //            }
        //            //ToDo：不太确定起终点不放进去会不会有问题
        //            //点去重
        //            intersectPointList = intersectPointList.Where((x, i) => intersectPointList.FindIndex(c => c.DistanceTo(x) < 1 / 304.8) == i).ToList();
        //            //排列点并重新生成线段集合
        //            intersectPointList = intersectPointList.OrderBy(x => m.GetEndPoint(0).DistanceTo(x)).ToList();
        //            //线段集合
        //            List<Line> lines = new List<Line>();
        //            for (int i = 0; i < intersectPointList.Count - 1; i++)
        //            {
        //                Line l = Line.CreateBound(intersectPointList[i], intersectPointList[i + 1]);
        //                lines.Add(l);
        //            }
        //            return lines;
        //        }
        //        ).ToList();
        //    #endregion

        //    #region 找轮廓

        //    //轮廓集合

        //    Line firstLine= lineList.OrderBy(x => x.Evaluate(0.5, true).DistanceTo(pickPoint)).First();
        //    lineList.Remove(firstLine);
        //    //这里判断线的方向，是不是从点出发
        //    XYZ endDirection = (firstLine.GetEndPoint(0) - pickPoint).Normalize();
        //    //逆时针
        //    if (firstLine.Direction.CrossProduct(endDirection).DistanceTo(XYZ.BasisZ)>0.001)
        //    {
        //        firstLine = firstLine.CreateReversed() as Line;
        //    }
        //    List<Line> profileList = new List<Line>() { firstLine};
        //    XYZ firstStart = firstLine.GetEndPoint(0);
        //    while (lineList.Count!=0)
        //    {
        //        XYZ lastEnd = profileList.Last().GetEndPoint(1);
        //        //回到起点
        //        if (lastEnd.DistanceTo(firstStart) < 1 / 304.8)
        //        {
        //            break;
        //        }
        //        for (int i = 0; i < lineList.Count; i++)
        //        {
        //            line
        //        }
        //        if (index != -1)//最后一根线的终点与集合中某条线的起点相同
        //        {
        //            lines.Add(lineList.ElementAt(index));
        //            //移除线
        //            lineList.RemoveAt(index);
        //        }
        //        else//最后一根线的终点与集合中某条线的终点相同
        //        {
        //            index = lineList.FindIndex(m => m.GetEndPoint(1).DistanceTo(lastEnd) < 1 / 304.8);
        //            if (index != -1)//如果存在就将线前后翻转一下，添加进集合
        //            {
        //                lines.Add(lineList.ElementAt(index).CreateReversed() as Autodesk.Revit.DB.Line);
        //                //移除线
        //                lineList.RemoveAt(index);
        //            }
        //            else//可能是没有线与它相同
        //            {
        //                //可能是不封闭的
        //                break;
        //            }
        //        }


        //    }
        //    //将所有为1的轮廓进行重新的排布,拿到最大圈的轮廓

        //    #endregion
        //}

        /// <summary>
        /// 获取封闭的框
        /// </summary>
        /// <param name="curveList"></param>
        /// <returns></returns>
        public static List<List<Line>> GetCloseProfiles(this List<Line> curveList)
        {
            //去除桥接线重复的线
            curveList = JoinLines(curveList, 0.1 / 304.8);
            #region 拆分线段
            List<Line> lineList = curveList.SelectMany(
                m =>
                {
                    List<Line> breakLineList = new List<Line>();
                    int index = curveList.IndexOf(m);
                    //存储所需要的交点
                    List<XYZ> intersectPointList = new List<XYZ>();
                    for (int i = 0; i < curveList.Count; i++)
                    {
                        if (CheckDirection(m.Direction, curveList[i].Direction)) continue;

                        m.Intersect(curveList[i], out IntersectionResultArray resultArray);
                        if (resultArray?.get_Item(0)?.XYZPoint != null) intersectPointList.Add(resultArray.get_Item(0).XYZPoint);
                    }
                    //ToDo：不太确定起终点不放进去会不会有问题
                    //点去重
                    intersectPointList = intersectPointList.Where((x, i) => intersectPointList.FindIndex(c => c.DistanceTo(x) < 1 / 304.8) == i).ToList();
                    //排列点并重新生成线段集合
                    intersectPointList = intersectPointList.OrderBy(x => m.GetEndPoint(0).DistanceTo(x)).ToList();
                    //线段集合
                    List<Line> lines = new List<Line>();
                    for (int i = 0; i < intersectPointList.Count - 1; i++)
                    {
                        Line l = Line.CreateBound(intersectPointList[i], intersectPointList[i + 1]);
                        lines.Add(l);
                    }
                    return lines;
                }
                ).ToList();
            #endregion

            #region 找轮廓
            //轮廓集合
            List<List<Line>> profileList = new List<List<Line>>();
            List<Tuple<int, Line>> lineTpList = lineList.Select(x => Tuple.Create(2, x.SetZ())).ToList();
            int tt = 0;
            while (lineTpList.Count(m => m.Item1 == 2) != 0 || tt < 250)
            {
                int index = lineTpList.FindIndex(m => m.Item1 == 2);
                Line l = lineTpList[index].Item2;
                //下根线的索引
                int nextIndex = index;
                //记录索引
                List<int> indexList = new List<int>();
                //绕顺时针找到与该线夹角最小的线，组成一个回路
                do
                {
                    Tuple<int, bool> tp = FindNextLine(lineTpList.Select(m => m.Item2).ToList(), nextIndex, nextIndex == index, out bool b, out bool end);
                    if (end) break;
                    //如果顺时针没有找到下一条线的情况下，在方法里面已经将线的方向反转，需要到外部更新该线的数据
                    if (b)
                    {
                        lineTpList[index] = Tuple.Create(lineTpList[index].Item1, lineTpList[index].Item2.CreateReversed() as Line);
                    }
                    //下一个线的索引
                    nextIndex = tp.Item1;
                    if (tp.Item2)//如果需要进行反转
                    {
                        lineTpList[tp.Item1] = Tuple.Create(lineTpList[tp.Item1].Item1, lineTpList[tp.Item1].Item2.CreateReversed() as Line);
                    }
                    if (indexList.Contains(nextIndex)) break;
                    indexList.Add(nextIndex);

                } while (index != nextIndex);

                List<Line> profile = indexList.Select(m => lineTpList[m].Item2).ToList();
                //判断是因为一开始有可能某条线顺时针无法找到下一条直线
                if (profile.Count != 0)
                    profileList.Add(profile);
                //组成轮廓的线的索引全部减1
                for (int i = 0; i < lineTpList.Count; i++)
                {
                    if (indexList.Contains(i))
                        lineTpList[i] = Tuple.Create(lineTpList[i].Item1 - 1, lineTpList[i].Item2);
                }
                //最后一步清除等于0的Line
                lineTpList = lineTpList.Where(m => m.Item1 > 0).ToList();

                tt++;
            }
            //将所有为1的轮廓进行重新的排布,拿到最大圈的轮廓
            List<List<Line>> linesList = FindCurveLoop(lineTpList.Select(m => m.Item2).ToList());
            List<Line> maxLineList = linesList.OrderByDescending(m =>
            {
                double length = 0;
                m.ForEach(x => length += x.Length);
                return length;
            }).First();
            linesList.Remove(maxLineList);
            //添加玻璃轮廓
            profileList.AddRange(linesList);
            //最大一圈的轮廓，在最后面
            profileList.Add(maxLineList);
            return profileList;
            #endregion
        }

        /// <summary>
        /// 发现封闭的CurveLoop
        /// </summary>
        /// <param name="lineList"></param>
        /// <returns></returns>
        public static List<List<Line>> FindCurveLoop(List<Line> lineList)
        {
            List<Line> curveList = lineList.ToList();
            List<List<Line>> lineListList = new List<List<Line>>();
            while (curveList.Count != 0)
            {
                List<Line> partLineList = new List<Line>() { curveList[0] };
                curveList.RemoveAt(0);
                XYZ firstStart = partLineList.ElementAt(0).GetEndPoint(0);
                while (true)
                {
                    XYZ lastEnd = partLineList.Last().GetEndPoint(1);
                    //回到起点
                    if (lastEnd.DistanceTo(firstStart) < 0.0001) break;
                    int index = curveList.FindIndex(m => m.GetEndPoint(0).DistanceTo(lastEnd) < 0.0001);
                    if (index != -1)//最后一根线的终点与集合中某条线的起点相同
                    {
                        partLineList.Add(curveList.ElementAt(index));
                        //移除线
                        curveList.RemoveAt(index);
                    }
                    else//最后一根线的终点与集合中某条线的终点相同
                    {
                        index = curveList.FindIndex(m => m.GetEndPoint(1).DistanceTo(lastEnd) < 0.0001);
                        if (index != -1)//如果存在就将线前后翻转一下，添加进集合
                        {
                            partLineList.Add(curveList.ElementAt(index).CreateReversed() as Line);
                            //移除线
                            curveList.RemoveAt(index);
                        }
                        else//可能是没有线与它相同
                        {
                            //可能是不封闭的
                            break;
                        }
                    }
                }
                CurveLoop cuLoop = new CurveLoop();
                partLineList.ForEach(m => cuLoop.Append(m));
                if (!cuLoop.IsOpen())
                    lineListList.Add(partLineList);
            }
            return lineListList;
        }
        /// <summary>
        /// 找与传入线段顺时针夹角最小的下一根线
        /// </summary>
        /// <param name="lines">线集合</param>
        /// <param name="currentIndex">当前线索引</param>
        /// <returns>下一根线的索引，与是否需要进行反转线段</returns>
        public static Tuple<int, bool> FindNextLine(List<Line> lines, int currentIndex, bool isFirst, out bool b, out bool isEnd)
        {
            isEnd = false;
            b = false;
            Line lin = lines.ElementAt(currentIndex);
            //找到与该线有交点的线集合
            List<Tuple<int, Line>> joinTpList = new List<Tuple<int, Line>>();
            lines.Where((m, i) =>
            {
                if (lines.IndexOf(m) == currentIndex) return false;
                //排除共线的
                //if (CheckDirection(m.Direction, lin.Direction)) return false;
                if (lin.GetEndPoint(1).DistanceTo(m.GetEndPoint(0)) < 0.1 / 304.8)
                {
                    joinTpList.Add(Tuple.Create(i, m));
                    return true;
                };
                if (lin.GetEndPoint(1).DistanceTo(m.GetEndPoint(1)) < 0.1 / 304.8)
                {
                    joinTpList.Add(Tuple.Create(i, m));
                    return true;
                }; ;
                return false;
            }).ToList();
            //共线集合
            var sameDirtLineList = joinTpList.Where(x => CheckDirection(x.Item2.Direction, lin.Direction)).ToList();
            //非共线集合
            var diffDirtLineList = joinTpList.Where(x => !CheckDirection(x.Item2.Direction, lin.Direction)).ToList();
            //找到与该线顺时针方向的最小夹角的线
            int index2 = int.MaxValue;
            double angle = int.MaxValue;
            Tuple<int, bool> tp = null;
            for (int i = 0; i < diffDirtLineList.Count; i++)
            {
                //排除三点不是顺时针旋转的线
                Line nl = diffDirtLineList[i].Item2;
                //这里判断线的方向，是不是从点出发
                int end = nl.GetEndPoint(0).DistanceTo(lin.GetEndPoint(1)) < 0.1 / 304.8 ? 0 : 1;
                XYZ dirt = end == 0 ? nl.Direction : -1 * nl.Direction;
                //判断两条线是顺时针还是逆时针，和z轴相同是逆时针
                if (!dirt.CrossProduct(lin.Direction).Normalize().IsAlmostEqualTo(XYZ.BasisZ))
                {
                    continue;
                }
                index2 = angle < dirt.AngleTo(lin.Direction) ? index2 : diffDirtLineList[i].Item1;
                tp = angle < dirt.AngleTo(lin.Direction) ? tp : Tuple.Create(index2, end == 1);


                angle = angle < dirt.AngleTo(lin.Direction) ? angle : dirt.AngleTo(lin.Direction);
            }
            #region 针对第一条线找不到顺时针方向的解决方法

            //如果第一条线找不到顺时针的线,把当前的线反一下
            if (tp == null && isFirst)
            {
                lin = lin.CreateReversed() as Line;
                b = true;
                //找到与该线有交点的线集合,且不共线的集合
                diffDirtLineList = new List<Tuple<int, Line>>();
                lines.Where((m, i) =>
                {
                    if (lines.IndexOf(m) == currentIndex) return false;
                    //排除共线的
                    if (CheckDirection(m.Direction, lin.Direction)) return false;
                    if (lin.GetEndPoint(1).DistanceTo(m.GetEndPoint(0)) < 0.1 / 304.8)
                    {
                        diffDirtLineList.Add(Tuple.Create(i, m));
                        return true;
                    };
                    if (lin.GetEndPoint(1).DistanceTo(m.GetEndPoint(1)) < 0.1 / 304.8)
                    {
                        diffDirtLineList.Add(Tuple.Create(i, m));
                        return true;
                    }; ;
                    return false;
                }).ToList();
                for (int i = 0; i < diffDirtLineList.Count; i++)
                {
                    //排除三点不是顺时针旋转的线
                    Line nl = diffDirtLineList[i].Item2;
                    int end = nl.GetEndPoint(0).DistanceTo(lin.GetEndPoint(1)) < 0.1 / 304.8 ? 0 : 1;
                    XYZ dirt = end == 0 ? nl.Direction : -nl.Direction;
                    if (!dirt.CrossProduct(lin.Direction).Normalize().IsAlmostEqualTo(XYZ.BasisZ))
                    {
                        MessageBox.Show(dirt.CrossProduct(lin.Direction).Normalize().Z.ToString() + "\t\n" + diffDirtLineList.Count() + "\t\n" + end + "这里是第一次翻转");
                        continue;
                    }
                    index2 = angle < dirt.AngleTo(lin.Direction) ? index2 : diffDirtLineList[i].Item1;
                    tp = angle < dirt.AngleTo(lin.Direction) ? tp : Tuple.Create(index2, end == 1);
                    angle = angle < dirt.AngleTo(lin.Direction) ? angle : dirt.AngleTo(lin.Direction);
                }
            }
            #endregion
            //如果还是没有找到顺时针的线,则找相同线上的线段
            if (tp == null)
            {
                var a = diffDirtLineList.Count;
                var d = sameDirtLineList.Count;
                if (diffDirtLineList.Count == 0 && sameDirtLineList.Count == 0)
                {
                    isEnd = true;
                }
                if (diffDirtLineList.Count == 1 && sameDirtLineList.Count == 0)
                {
                    Line nl = diffDirtLineList[0].Item2;
                    int end = nl.GetEndPoint(0).DistanceTo(lin.GetEndPoint(1)) < 0.1 / 304.8 ? 0 : 1;
                    tp = Tuple.Create(diffDirtLineList[0].Item1, end == 1);
                }
                for (int i = 0; i < sameDirtLineList.Count; i++)
                {
                    Line nl = sameDirtLineList[i].Item2;
                    int end = nl.GetEndPoint(0).DistanceTo(lin.GetEndPoint(1)) < 0.1 / 304.8 ? 0 : 1;
                    tp = Tuple.Create(sameDirtLineList[i].Item1, end == 1);
                    break;
                }
            }
            return tp;
        }

        /// <summary>
        /// 判断单位向量是否共线
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <param name="precision"></param>
        /// <returns></returns>
        public static bool CheckDirection(XYZ vector1, XYZ vector2, int precision = 4)
        {
            XYZ point1 = vector1 + vector2;
            XYZ point2 = vector1 - vector2;
            double x = Math.Round(point1.DistanceTo(new XYZ()), precision);
            double y = Math.Round(point2.DistanceTo(new XYZ()), precision);
            if (x == 0 || y == 0)
            {
                return true;
            }
            return false;
        }


    }
}
