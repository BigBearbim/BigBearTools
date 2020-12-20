using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBearTools
{
    public static class GridHandelMethod
    {
        #region LineEx
        /// <summary>
        /// 按照比例缩放线
        /// </summary>
        /// <param name="line"></param>
        /// <param name="scaling"></param>
        /// <returns></returns>
        public static Line ScalingLine(this Line line, double scaling = 1)
        {
            XYZ startPoint = line.GetEndPoint(0);
            XYZ endPoint = line.GetEndPoint(1);
            startPoint = new XYZ(startPoint.X / scaling, startPoint.Y / scaling, startPoint.Z / scaling);
            endPoint = new XYZ(endPoint.X / scaling, endPoint.Y / scaling, endPoint.Z / scaling);
            return Line.CreateBound(startPoint, endPoint);
        }

        public static List<Grid> CreateGrid(this List<Tuple<Line, string>> lines, Document doc, GridType gridType, string xStr, string yStr)
        {
            List<Grid> grids = new List<Grid>();
            List<Tuple<Line, string>> upAndDownList = lines.Where(x => x.Item1.Direction.IsParallel(XYZ.BasisX)).OrderBy(x => x.Item1.GetEndPoint(0).DotProduct(XYZ.BasisY)).ToList();
            var leftAndRigthList = lines.Where(x => x.Item1.Direction.IsParallel(XYZ.BasisY)).OrderBy(x => x.Item1.GetEndPoint(0).DotProduct(XYZ.BasisX)).ToList();
            List<string> xGridNameList = new List<string>();
            List<string> yGridNameList = new List<string>();

            #region X轴名称处理
            //对x轴进行处理
            bool a = double.TryParse(xStr, out double result);
            if (a)//确定是数值
            {
                for (int i = 0; i < leftAndRigthList.Count; i++)
                {
                    Grid g = Grid.Create(doc, leftAndRigthList[i].Item1);
                    g.ChangeTypeId(gridType.Id);
                    g.Name = (result + i).ToString();

                    grids.Add(g);
                }
            }
            else
            {
                //可能是1-1类型
                bool b = xStr.Contains('-');
                if (b)
                {
                    xStr.Replace(" ", "");
                    string[] sqiltXStr = xStr.Split(new char[] { '-' });
                    bool c = double.TryParse(sqiltXStr.Last(), out double cResult);

                    if (c)
                    {
                        for (int i = 0; i < leftAndRigthList.Count; i++)
                        {
                            Grid g = Grid.Create(doc, leftAndRigthList[i].Item1);
                            g.ChangeTypeId(gridType.Id);
                            g.Name = sqiltXStr[0] + "-" + (cResult + i).ToString();
                            grids.Add(g);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < leftAndRigthList.Count; i++)
                        {
                            Grid g = Grid.Create(doc, leftAndRigthList[i].Item1);
                            g.ChangeTypeId(gridType.Id);
                            g.Name = xStr + $"-{i}";
                            grids.Add(g);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < leftAndRigthList.Count; i++)
                    {
                        Grid g = Grid.Create(doc, leftAndRigthList[i].Item1);
                        g.ChangeTypeId(gridType.Id);
                        g.Name = xStr + $"-{i+1}";
                        grids.Add(g);
                    }
                }

            }
            #endregion

            #region Y轴名称处理
            //A~Z对应的码是65~90,a~z对应的码是97~122,A~Y,a~y 只需在原有的码+1,再输出即可
            //单个字母的时候
            if (yStr.Length == 1)
            {
                int ySACII = Encoding.Default.GetBytes(yStr)[0];
                if ((ySACII > 64 && ySACII < 91) || (ySACII > 96 && ySACII < 123))
                {
                    for (int i = 0; i < upAndDownList.Count; i++)
                    {
                        Grid g = Grid.Create(doc, upAndDownList[i].Item1);
                        g.ChangeTypeId(gridType.Id);
                        if (i != 0)
                        {
                            ySACII += 1;
                        }
                        if (ySACII < 91 || (ySACII > 96 && ySACII < 123))
                        {
                            g.Name = ySACII.ASCIIToString();
                        }
                        else
                        {
                            g.Name = (ySACII - 26).ASCIIToString() + (ySACII - 26).ASCIIToString();
                        }
                        grids.Add(g);
                    }
                }
            }
            else
            {
                for (int i = 0; i < upAndDownList.Count; i++)
                {
                    Grid g = Grid.Create(doc, upAndDownList[i].Item1);
                    g.ChangeTypeId(gridType.Id);
                    g.Name = yStr + "-" + (i + 1).ToString();
                    grids.Add(g);
                }
            }
            #endregion



            return grids;
        }

        /// <summary>
        /// 字符转ASCII
        /// </summary>
        /// <param name="character"></param>
        /// <returns></returns>
        public static int StringToASCII(this string character, out bool isSucceed)
        {
            isSucceed = true;
            if (character.Length == 1)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                int intAsciiCode = (int)asciiEncoding.GetBytes(character)[0];
                return (intAsciiCode);
            }
            else
            {
                isSucceed = false;
                return -1;
            }

        }
        /// <summary>
        /// 
        ///ASCII码转字符
        /// </summary>
        /// <param name="asciiCode"></param>
        /// <returns></returns>
        public static string ASCIIToString(this int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return (strCharacter);
            }
            else
            {
                return "";
            }
        }


        /// <summary>
        /// 根据string 生成轴网线
        /// </summary>
        /// <param name="allValueStringList"></param>
        /// <param name="originPoint"></param>
        /// <returns></returns>
        public static List<Tuple<Line, string>> GetTargetLineList(this List<string> allValueStringList, XYZ originPoint)
        {
            //0为左进深
            List<double> leftDoubleList = allValueStringList[0].TransformStringList(out double leftDouble);
            //1为右进深
            List<double> rightDoubleList = allValueStringList[1].TransformStringList(out double rightDouble);
            //2为上进深
            List<double> upDoubleList = allValueStringList[2].TransformStringList(out double upDouble);
            //3为下进深
            List<double> downDoubleList = allValueStringList[3].TransformStringList(out double downDouble);

            List<double> lengthList = new List<double>() { leftDouble, rightDouble, upDouble, downDouble };

            //横向线段的长度
            double horizontalDouble = lengthList.Max();
            //纵向线段的长度
            double verticalDouble = lengthList.Max();
            if (horizontalDouble == 0) horizontalDouble = 1.ToFeet();
            if (verticalDouble == 0) verticalDouble = 1.ToFeet();



            //确定的两根线
            Line xLocationLine = Line.CreateBound(originPoint.Add(-XYZ.BasisX * 2500.ToFeet()), originPoint.Add(XYZ.BasisX * (horizontalDouble + 2500.ToFeet())));
            Line yLocationLine = Line.CreateBound(originPoint.Add(-XYZ.BasisY * 2500.ToFeet()), originPoint.Add(XYZ.BasisY * (verticalDouble + 2500.ToFeet())));

            //存储线的数据,1：左右、2：上下
            List<Tuple<Line, string>> lineInfoList = new List<Tuple<Line, string>>()
            {
                new Tuple<Line, string>(xLocationLine,"99"),
                new Tuple<Line, string>(yLocationLine,"99"),
            };

            //目标点、移动方向、是否是左右进深
            List<Tuple<XYZ, XYZ, bool>> infoList = new List<Tuple<XYZ, XYZ, bool>>()
            {
            };

            //左进深的点集合
            leftDoubleList.TransformDoubleList(originPoint, false).ForEach(x => infoList.Add(new Tuple<XYZ, XYZ, bool>(x, -XYZ.BasisX, true)));
            //右进深的点集合
            rightDoubleList.TransformDoubleList(originPoint.Add(XYZ.BasisX * horizontalDouble), false).ForEach(x => infoList.Add(new Tuple<XYZ, XYZ, bool>(x, XYZ.BasisX, true)));
            //上进深的点集合
            upDoubleList.TransformDoubleList(originPoint.Add(XYZ.BasisY * verticalDouble)).ForEach(x => infoList.Add(new Tuple<XYZ, XYZ, bool>(x, XYZ.BasisY, false)));
            //下进深的点集合
            downDoubleList.TransformDoubleList(originPoint).ForEach(x => infoList.Add(new Tuple<XYZ, XYZ, bool>(x, -XYZ.BasisY, false)));



            while (infoList.Count > 0)
            {
                var first = infoList[0];
                infoList.RemoveAt(0);
                List<Tuple<XYZ, XYZ, bool>> findList = new List<Tuple<XYZ, XYZ, bool>>();
                Line l1;
                if (first.Item3)
                {
                    findList = infoList.FindAll(x => x.Item1.Y == first.Item1.Y && x.Item3 == true);

                    if (findList.Count == 0)
                    {
                        l1 = Line.CreateBound(first.Item1.Add(first.Item2 * 2500.ToFeet()), first.Item1.Add(-first.Item2 * (horizontalDouble)));

                        lineInfoList.Add(new Tuple<Line, string>(l1, "1"));

                    }
                    else if (findList.Count == 1)
                    {
                        infoList.Remove(findList[0]);
                        l1 = Line.CreateBound(first.Item1.Add(first.Item2 * 2500.ToFeet()), findList[0].Item1.Add(findList[0].Item2 * 2500.ToFeet()));
                        lineInfoList.Add(new Tuple<Line, string>(l1, "1"));
                    }
                }
                else
                {
                    findList = infoList.FindAll(x => x.Item1.X == first.Item1.X && x.Item3 == false);
                    if (findList.Count == 0)
                    {
                        l1 = Line.CreateBound(first.Item1.Add(first.Item2 * 2500.ToFeet()), first.Item1.Add(-first.Item2 * (verticalDouble)));

                        lineInfoList.Add(new Tuple<Line, string>(l1, "2"));

                    }
                    else if (findList.Count == 1)
                    {
                        infoList.Remove(findList[0]);
                        l1 = Line.CreateBound(first.Item1.Add(first.Item2 * 2500.ToFeet()), findList[0].Item1.Add(findList[0].Item2 * 2500.ToFeet()));
                        lineInfoList.Add(new Tuple<Line, string>(l1, "2"));
                    }
                }

            }
            return lineInfoList;



        }
        #endregion

        #region GridEx
        /// <summary>
        /// 添加注释
        /// </summary>
        /// <param name="grids"></param>
        /// <param name="doc"></param>
        /// <param name="view"></param>
        public static void CreateAnnotation(this List<Grid> grids, Document doc, View view,DimensionType dType)
        {
            //先对轴网进行分类
            List<Grid> horGridList = grids.Where(x => (x.Curve as Line).Direction.IsParallel(XYZ.BasisX)).OrderBy(x => (x.Curve as Line).Direction.DotProduct(XYZ.BasisY)).ToList();
            List<Grid> verGridList = grids.Where(x => (x.Curve as Line).Direction.IsParallel(XYZ.BasisY)).OrderBy(x => (x.Curve as Line).Direction.DotProduct(XYZ.BasisX)).ToList();

            Transaction trans = new Transaction(doc, "创建标注");
            trans.Start();
            for (int i = 0; i < horGridList.Count - 1; i++)
            {
                //第一条线的Ref
                Line l1 = horGridList[i].Curve as Line;
                if (l1.Direction.IsParallel(XYZ.BasisX, false))
                {
                    l1 = l1.CreateReversed() as Line;
                }
                Reference l1Ref = new Reference(horGridList[i]);
                Line l2 = horGridList[i + 1].Curve as Line;
                if (l2.Direction.IsParallel(XYZ.BasisX, false))
                {
                    l2 = l2.CreateReversed() as Line;
                }
                Reference l2Ref = new Reference(horGridList[i + 1]);
                double midPointDistance = l1.Evaluate(0.5, false).DistanceTo(l2.Evaluate(0.5, false));
                Line dirFirstLine = Line.CreateBound(l1.GetEndPoint(0).Add(1200.ToFeet() * XYZ.BasisX), l1.GetEndPoint(0).Add(1200.ToFeet() * XYZ.BasisX).Add(XYZ.BasisY * midPointDistance));
                Line dirEndLine = Line.CreateBound(l1.GetEndPoint(1).Add(-1200.ToFeet() * XYZ.BasisX), l1.GetEndPoint(1).Add(-1200.ToFeet() * XYZ.BasisX).Add(XYZ.BasisY * midPointDistance));
                var array = new ReferenceArray();
                array.Append(l1Ref);
                array.Append(l2Ref);
                var a = doc.Create.NewDimension(view, dirFirstLine, array);
                a.ChangeTypeId(dType.Id);
                ElementTransformUtils.RotateElement(doc, a.Id, Line.CreateBound(a.Origin, a.Origin.Add(10 * XYZ.BasisZ)), 180.ToRad());
                var b = doc.Create.NewDimension(view, dirEndLine, array);
                b.ChangeTypeId(dType.Id);
            }
            for (int i = 0; i < verGridList.Count - 1; i++)
            {
                //第一条线的Ref
                Line l1 = verGridList[i].Curve as Line;
                if (l1.Direction.IsParallel(XYZ.BasisY, false))
                {
                    l1 = l1.CreateReversed() as Line;
                }
                Reference l1Ref = new Reference(verGridList[i]);
                Line l2 = verGridList[i + 1].Curve as Line;
                if (l2.Direction.IsParallel(XYZ.BasisY, false))
                {
                    l2 = l2.CreateReversed() as Line;
                }
                Reference l2Ref = new Reference(verGridList[i + 1]);
                double midPointDistance = l1.Evaluate(0.5, false).DistanceTo(l2.Evaluate(0.5, false));
                Line dirFirstLine = Line.CreateBound(l1.GetEndPoint(0).Add(1200.ToFeet() * XYZ.BasisY), l1.GetEndPoint(0).Add(1200.ToFeet() * XYZ.BasisY).Add(XYZ.BasisX * midPointDistance));
                Line dirEndLine = Line.CreateBound(l1.GetEndPoint(1).Add(-1200.ToFeet() * XYZ.BasisY), l1.GetEndPoint(1).Add(-1200.ToFeet() * XYZ.BasisY).Add(XYZ.BasisX * midPointDistance));
                var array = new ReferenceArray();
                array.Append(l1Ref);
                array.Append(l2Ref);
                var a = doc.Create.NewDimension(view, dirFirstLine, array);
                a.ChangeTypeId(dType.Id);
                var b = doc.Create.NewDimension(view, dirEndLine, array);
                ElementTransformUtils.RotateElement(doc, b.Id, Line.CreateBound(b.Origin, b.Origin.Add(10 * XYZ.BasisZ)), 180.ToRad());
                b.ChangeTypeId(dType.Id);
            }

            //整体标注
            if(horGridList.Count > 1)
            {
                //第一条线的Ref
                Line l1 = horGridList[0].Curve as Line;
                if (l1.Direction.IsParallel(XYZ.BasisX, false))
                {
                    l1 = l1.CreateReversed() as Line;
                }
                Reference l1Ref = new Reference(horGridList[0]);
                Line l2 = horGridList.Last().Curve as Line;
                if (l2.Direction.IsParallel(XYZ.BasisX, false))
                {
                    l2 = l2.CreateReversed() as Line;
                }
                Reference l2Ref = new Reference(horGridList.Last());
                double midPointDistance = l1.Evaluate(0.5, false).DistanceTo(l2.Evaluate(0.5, false));
                Line dirFirstLine = Line.CreateBound(l1.GetEndPoint(0).Add(600.ToFeet() * XYZ.BasisX), l1.GetEndPoint(0).Add(600.ToFeet() * XYZ.BasisX).Add(XYZ.BasisY * midPointDistance));
                Line dirEndLine = Line.CreateBound(l1.GetEndPoint(1).Add(-600.ToFeet() * XYZ.BasisX), l1.GetEndPoint(1).Add(-600.ToFeet() * XYZ.BasisX).Add(XYZ.BasisY * midPointDistance));
                var array = new ReferenceArray();
                array.Append(l1Ref);
                array.Append(l2Ref);
                var a = doc.Create.NewDimension(view, dirFirstLine, array);
                a.ChangeTypeId(dType.Id);
                ElementTransformUtils.RotateElement(doc, a.Id, Line.CreateBound(a.Origin, a.Origin.Add(10 * XYZ.BasisZ)), 180.ToRad());
                var b = doc.Create.NewDimension(view, dirEndLine, array);
                b.ChangeTypeId(dType.Id);
            }
            if (verGridList.Count > 1)
            {
                //第一条线的Ref
                Line l1 = verGridList[0].Curve as Line;
                if (l1.Direction.IsParallel(XYZ.BasisY, false))
                {
                    l1 = l1.CreateReversed() as Line;
                }
                Reference l1Ref = new Reference(verGridList[0]);
                Line l2 = verGridList.Last().Curve as Line;
                if (l2.Direction.IsParallel(XYZ.BasisY, false))
                {
                    l2 = l2.CreateReversed() as Line;
                }
                Reference l2Ref = new Reference(verGridList.Last());
                double midPointDistance = l1.Evaluate(0.5, false).DistanceTo(l2.Evaluate(0.5, false));
                Line dirFirstLine = Line.CreateBound(l1.GetEndPoint(0).Add(600.ToFeet() * XYZ.BasisY), l1.GetEndPoint(0).Add(600.ToFeet() * XYZ.BasisY).Add(XYZ.BasisX * midPointDistance));
                Line dirEndLine = Line.CreateBound(l1.GetEndPoint(1).Add(-600.ToFeet() * XYZ.BasisY), l1.GetEndPoint(1).Add(-600.ToFeet() * XYZ.BasisY).Add(XYZ.BasisX * midPointDistance));
                var array = new ReferenceArray();
                array.Append(l1Ref);
                array.Append(l2Ref);
                var a = doc.Create.NewDimension(view, dirFirstLine, array);
                a.ChangeTypeId(dType.Id);
                var b = doc.Create.NewDimension(view, dirEndLine, array);
                ElementTransformUtils.RotateElement(doc, b.Id, Line.CreateBound(b.Origin, b.Origin.Add(10 * XYZ.BasisZ)), 180.ToRad());
                b.ChangeTypeId(dType.Id);
            }
            

            trans.Commit();

        }
        #endregion

        #region XYZEx
        /// <summary>
        /// 根据数值集合生成点,true为X轴方向的点，false为Y轴方向的点
        /// </summary>
        /// <param name="dList"></param>
        /// <param name="originPoint"></param>
        /// <param name="isX"></param>
        /// <returns></returns>
        public static List<XYZ> TransformDoubleList(this List<double> dList, XYZ originPoint, bool isX = true)
        {
            List<XYZ> result = new List<XYZ>();
            if (isX)
            {
                XYZ resultPoint = originPoint;
                foreach (var d in dList)
                {
                    resultPoint = new XYZ(resultPoint.X + d, resultPoint.Y, 0);
                    result.Add(resultPoint);
                }
            }
            else
            {
                XYZ resultPoint = originPoint;
                foreach (var d in dList)
                {
                    resultPoint = new XYZ(resultPoint.X, resultPoint.Y + d, 0);
                    result.Add(resultPoint);
                }
            }
            return result;
        }

        #endregion

        #region StringEx
        /// <summary>
        /// 转换数据
        /// </summary>
        /// <param name="targetStr"></param>
        /// <returns></returns>
        public static List<double> TransformStringList(this string targetStr, out double maxDouble, double scaleDouble = 1)
        {
            //返回的最大值
            maxDouble = 0;
            //排除为空的现象
            if (targetStr == null)
            {

                return new List<double>();
            }
            List<double> targetDoubleList = new List<double>();
            //排除空格
            targetStr = targetStr.Replace(" ", "");//排除中间
            targetStr = targetStr.Trim();//排除前后
            //先分成数组
            string[] splitArr = targetStr.Split(new char[] { ',', '，' });
            //转换数据
            for (int i = 0; i < splitArr.Length; i++)
            {
                if (splitArr[i] == null || splitArr[i] == "") continue;
                if (splitArr[i].Contains("*"))
                {
                    string[] splitArrArr = splitArr[i].Split(new char[] { '*' });
                    int index = 1;
                    if(splitArrArr.Length == 2)
                    {
                        try
                        {
                            index = (int)double.Parse(splitArrArr[1]);
                        }
                        catch 
                        {
                        }
                    }
                    for (int j = 0; j < index; j++)
                    {
                        double b = (double.Parse(splitArrArr[0]) / scaleDouble).ToFeet();
                        targetDoubleList.Add(b);
                    }

                }
                else
                {
                    double b = (double.Parse(splitArr[i]) / scaleDouble).ToFeet();
                    targetDoubleList.Add(b);
                }

            }
            foreach (var t in targetDoubleList)
            {
                maxDouble += t;
            }
            return targetDoubleList;
        }
        #endregion

    }
}
