using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBearTools
{
    public static class XYZEx
    {
        /// <summary>
        ///得到element的中点 
        /// </summary>
        /// <param name="elem"></param>
        /// <param name="view"></param>
        /// <returns></returns>
        public static XYZ GetCentelPoint(Element elem, View view)
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
        public static double Round(this double d1, int i = 0)
        {
            return Math.Round(d1, i);
        }
        public static XYZ SetZ(this XYZ sPoint, double z = 0)
        {
            return new XYZ(sPoint.X, sPoint.Y, z);
        }
        /// <summary>
        /// 判断点是否在轮廓内
        /// </summary>
        /// <param name="TargetPoint"></param>
        /// <param name="lines"></param>
        /// <returns></returns>
        public static bool IsInsideOutline(this XYZ TargetPoint,XYZ xYZ, List<Line> lines)
        {
            bool result = true;
            int insertCount = 0;
            Line rayLine = Line.CreateBound(TargetPoint, xYZ).SetZ(0);
            foreach (var areaLine in lines)
            {
                var interResult = areaLine.SetZ().Intersect(rayLine, out IntersectionResultArray resultArray);
                var insPoint = resultArray?.get_Item(0);
                if (insPoint != null)
                {
                    insertCount++;
                }
            }
            //如果次数为偶数就在外面，次数为奇数就在里面
            if (insertCount % 2 == 0)//偶数
            {
                return result = false;
            }
            return result;
        }

        /// <summary>
        /// 向量是否平行
        /// </summary>
        /// <param name="vector1"></param>
        /// <param name="vector2"></param>
        /// <param name="v">true为同向平行，false为反向平行，null为平行</param>
        /// <param name="tolerance">允许误差的角度</param>
        /// <returns></returns>
        public static bool IsParallel(this XYZ vector1, XYZ vector2, bool? v = null, double tolerance = 0.1)
        {
            if (v == null)
            {
                return vector1.AngleTo(vector2) > (180 - tolerance).ToRad() || vector1.DistanceTo(vector2) < tolerance.ToRad();
            }
            else if (v == true)
            {
                return vector1.AngleTo(vector2) < tolerance.ToRad();
            }
            else
            {
                return vector1.AngleTo(vector2) > (180 - tolerance).ToRad();
            }
        }
        /// <summary>
        /// 转弧度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static double ToRad<T>(this T t) where T : struct
        {
            return double.Parse(t.ToString()) * Math.PI / 180;
        }

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
        /// <summary>
        /// 点测试
        /// </summary>
        /// <param name="x"></param>
        /// <param name="doc"></param>
        /// <param name="isInTransaction"></param>
        public static void XYZTest(this XYZ x, Document doc,  bool isInTransaction = false)
        {
            Autodesk.Revit.DB. Line l = Autodesk.Revit.DB.Line.CreateBound(x, new XYZ(x.X, x.Y + 100, x.Z));
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
        /// 英尺转毫米
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double ToMM(this double b)
        {
            return UnitUtils.Convert(b, DisplayUnitType.DUT_DECIMAL_FEET, DisplayUnitType.DUT_MILLIMETERS);
        }
        /// <summary>
        /// 毫米转英尺
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double ToFeet<T>(this T b)where T:struct
        {
            double.TryParse(b.ToString(), out var d);
            return UnitUtils.Convert(d, DisplayUnitType.DUT_MILLIMETERS, DisplayUnitType.DUT_DECIMAL_FEET);
        }
        /// <summary>
        /// 平方英尺转平方米
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double ToSquareMeters(this double b)
        {
            return UnitUtils.Convert(b, DisplayUnitType.DUT_SQUARE_FEET, DisplayUnitType.DUT_SQUARE_METERS);
        }
        /// <summary>
        /// 平方米转平方英尺
        /// </summary>
        /// <param name="b"></param>
        /// <returns></returns>
        public static double ToSquareFeet(this double b)
        {
            return UnitUtils.Convert(b, DisplayUnitType.DUT_SQUARE_METERS, DisplayUnitType.DUT_SQUARE_FEET);
        }

    }
}
