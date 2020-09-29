using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBearTools
{
    public static class GeoEx
    {
        /// <summary>
        /// 得到墙的最底面的线
        /// </summary>
        /// <param name="geo"></param>
        /// <returns></returns>
        public static List<Autodesk.Revit.DB.Line> GetGeoEleDownLine(GeometryElement geo)
        {
            List<Autodesk.Revit.DB.Line> geoLineList = new List<Autodesk.Revit.DB.Line>();
            foreach (GeometryObject geoObject in geo)
            {
                Solid geoSolid = geoObject as Solid;
                foreach (Face geoFace in geoSolid.Faces)
                {
                    PlanarFace temFace = geoFace as PlanarFace;
                    if (Math.Abs(temFace.FaceNormal.X) < 0.01 && Math.Abs(temFace.FaceNormal.Y) < 0.01 && temFace.FaceNormal.Z < 0)
                    {
                        foreach (EdgeArray eArr in temFace.EdgeLoops)
                        {
                            foreach (Edge e in eArr)
                            {
                                try
                                {
                                    if (e.AsCurve() is Autodesk.Revit.DB.Line l1)
                                    {
                                        geoLineList.Add(l1.SetZ());
                                    }
                                }
                                catch
                                {

                                    continue;
                                }

                            }
                        }
                        break;
                    }
                }
            }
            return geoLineList;
        }
        /// <summary>
        /// 得到墙的最底面的面
        /// </summary>
        /// <param name="geo"></param>
        /// <returns></returns>
        public static PlanarFace GetGeoEleDownFace(GeometryElement geo)
        {
            PlanarFace face = null;
            foreach (GeometryObject geoObject in geo)
            {
                Solid geoSolid = geoObject as Solid;
                foreach (Face geoFace in geoSolid.Faces)
                {
                    PlanarFace temFace = geoFace as PlanarFace;
                    if (Math.Abs(temFace.FaceNormal.X) < 0.01 && Math.Abs(temFace.FaceNormal.Y) < 0.01 && temFace.FaceNormal.Z < 0)
                    {
                        face = temFace;
                        break;
                    }
                }
            }
            return face;
        }

        #region 获取构件集合的Solids集合
        /// <summary>
        /// 获取构件集合的Solids集合
        /// </summary>
        /// <param name="elements">构件集合</param>
        /// <returns></returns>
        public static List<Solid> GetSolidByElements(this List<Element> elements)
        {
            List<Solid> result = new List<Solid>();
            Options options = new Options();
            elements.ForEach(e =>
            {
                GeometryElement geometryElement = e.get_Geometry(options);
                List<Solid> solids = GetSolids(geometryElement);
                if (solids.Count > 0)
                {
                    result.AddRange(solids);
                }
            });
            return result;
        }
        #endregion

        #region 获取所有的Solid
        /// <summary>
        /// 获取所有的Solid
        /// </summary>
        /// <param name="geometryElement"></param>
        /// <returns></returns>
        public static List<Solid> GetSolids(GeometryElement geometryElement)
        {
            List<Solid> result = new List<Solid>();
            foreach (GeometryObject geomObj in geometryElement)
            {
                Solid solid = geomObj as Solid;
                if (null != solid)
                {
                    result.Add(solid);
                    continue;
                }
                //If this GeometryObject is Instance, call AddCurvesAndSolids
                GeometryInstance geomInst = geomObj as GeometryInstance;
                if (null != geomInst)
                {
                    GeometryElement transformedGeomElem = geomInst.GetInstanceGeometry(geomInst.Transform);
                    result.AddRange(GetSolids(transformedGeomElem));
                }
            }
            return result;
        }
        #endregion
        #region Solid布尔操作
        /// <summary>
        /// Solid布尔操作
        /// </summary>
        /// <param name="solidA"></param>
        /// <param name="solidB"></param>
        /// <param name="booleanOperationsType"></param>
        /// <returns></returns>
        public static Solid SolidBooleanOperation(Solid solidA, Solid solidB, BooleanOperationsType booleanOperationsType)
        {
            Solid result = null;
            try
            {
                result = BooleanOperationsUtils.ExecuteBooleanOperation(solidA, solidB, booleanOperationsType);
            }
            catch (Exception ex)
            {
                result = BooleanOperationsUtils.ExecuteBooleanOperation(solidB, solidA, booleanOperationsType);
            }
            return result;
        }
        #endregion

        /// <summary>
        /// 拆分楼板
        /// </summary>
        /// <param name="floor"></param>
        /// <returns></returns>
        public static List<Solid> SplitFloor(this Floor floor)
        {
            List<Solid> floorSolid = new List<Solid>();
            var floorGeo = floor.get_Geometry(new Options());
            foreach (var geoObject in floorGeo)
            {
                Solid geoSolid = geoObject as Solid;
                floorSolid.AddRange(SolidUtils.SplitVolumes(geoSolid));
            }
            return floorSolid;
        }
        /// <summary>
        /// 计算几何体的中点
        /// </summary>
        /// <param name="solid"></param>
        /// <returns></returns>
        public static XYZ GetSolidCenter(this Solid solid)
        {
            XYZ centerPoint =null;
            foreach (Face geoFace in solid.Faces)
            {
                PlanarFace temFace = geoFace as PlanarFace;
                if (Math.Abs(temFace.FaceNormal.X) < 0.01 && Math.Abs(temFace.FaceNormal.Y) < 0.01 && temFace.FaceNormal.Z < 0)
                {
                    var faceBound = temFace.GetBoundingBox();
                    UV maxUV = faceBound.Max;
                    UV minUV = faceBound.Min;
                    UV uvPoint = (maxUV + minUV) / 2;
                    centerPoint = temFace.Evaluate(uvPoint).SetZ();
                    break;
                }
            }
            return centerPoint;
        }


        public static List<ElementId> GetGeoInsterEle(this Wall wall)
        {
            List<ElementId> elemIdList = new List<ElementId>();
            var geo = wall.get_Geometry(new Options());
            foreach (GeometryObject geoObject in geo)
            {
                Solid geoSolid = geoObject as Solid;
                foreach (Face geoFace in geoSolid.Faces)
                {
                    var generatingEleId = wall.GetGeneratingElementIds(geoFace).Where(x => !(elemIdList.Contains(x))).ToList();
                    generatingEleId.Remove(wall.Id);
                    elemIdList.AddRange(generatingEleId);
                }
            }
            return elemIdList;
        }
    }
}
