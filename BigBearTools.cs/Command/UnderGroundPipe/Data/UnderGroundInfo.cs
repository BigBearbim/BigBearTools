using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBearTools
{
    public class UnderGroundInfo
    {
        #region 数值
        string pointNumber;
        string characteristic;
        double pointBuriedDepth;
        double x;
        double y;
        double groundElevation;
        string startPointNumber;
        double startPointBuriedDepth;
        double startGroundElevation;
        string endPointNumber;
        double endPointBuriedDepth;
        double endGroundElevation;
        double pipeLength;
        string pipeSize;
        string material;
        string administrativeUnit;
        Element creatElement;

        #endregion

        /// <summary>
        /// 点号
        /// </summary>
        public string PointNumber { get => pointNumber; set => pointNumber = value; }
        /// <summary>
        /// 特征
        /// </summary>
        public string Characteristic { get => characteristic; set => characteristic = value; }
        /// <summary>
        /// 点埋深
        /// </summary>
        public double PointBuriedDepth { get => pointBuriedDepth; set => pointBuriedDepth = value; }
        /// <summary>
        /// X坐标
        /// </summary>
        public double X { get => x; set => x = value; }
        /// <summary>
        /// Y坐标
        /// </summary>
        public double Y { get => y; set => y = value; }
        /// <summary>
        /// 地点高程
        /// </summary>
        public double GroundElevation { get => groundElevation; set => groundElevation = value; }
        /// <summary>
        /// 管线起点号
        /// </summary>
        public string StartPointNumber { get => startPointNumber; set => startPointNumber = value; }
        /// <summary>
        /// 起点埋深
        /// </summary>
        public double StartPointBuriedDepth { get => startPointBuriedDepth; set => startPointBuriedDepth = value; }
        /// <summary>
        /// 起点高程
        /// </summary>
        public double StartGroundElevation { get => startGroundElevation; set => startGroundElevation = value; }
        /// <summary>
        /// 管线终点号
        /// </summary>
        public string EndPointNumber { get => endPointNumber; set => endPointNumber = value; }
        /// <summary>
        /// 管线终点埋深
        /// </summary>
        public double EndPointBuriedDepth { get => endPointBuriedDepth; set => endPointBuriedDepth = value; }
        /// <summary>
        /// 管线终点高程
        /// </summary>
        public double EndGroundElevation { get => endGroundElevation; set => endGroundElevation = value; }
        /// <summary>
        /// 管段长
        /// </summary>
        public double PipeLength { get => pipeLength; set => pipeLength = value; }
        /// <summary>
        /// 断面尺寸，为string的原因是，存在100X200的格式
        /// </summary>
        public string PipeSize { get => pipeSize; set => pipeSize = value; }
        /// <summary>
        /// 材质
        /// </summary>
        public string Material { get => material; set => material = value; }
        /// <summary>
        /// 权属单位
        /// </summary>
        public string AdministrativeUnit { get => administrativeUnit; set => administrativeUnit = value; }
        /// <summary>
        /// 所对应的构件
        /// </summary>
        public Element CreatElement { get => creatElement; set => creatElement = value; }
    }
}
