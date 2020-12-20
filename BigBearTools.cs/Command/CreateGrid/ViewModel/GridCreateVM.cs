using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Grid = Autodesk.Revit.DB.Grid;

namespace BigBearTools
{
    public class GridCreateVM : NotifyObject
    {
        #region 数据

        private string leftDepth;
        /// <summary>
        /// 左进深数值
        /// </summary>
        public string LeftDepth
        {
            get { return leftDepth; }
            set
            {
                leftDepth = value;
                this.RaisePropertyChanged(nameof(LeftDepth));
            }
        }
        private string rightDepth;
        /// <summary>
        /// 右进深数值
        /// </summary>
        public string RightDepth
        {
            get { return rightDepth; }
            set
            {
                rightDepth = value;
                this.RaisePropertyChanged(nameof(RightDepth));
            }
        }
        private string upDepth;
        /// <summary>
        /// 上进深数值
        /// </summary>
        public string UpDepth
        {
            get { return upDepth; }
            set
            {
                upDepth = value;
                this.RaisePropertyChanged(nameof(UpDepth));
            }
        }
        private string downDepth;
        /// <summary>
        /// 下进深数值
        /// </summary>
        public string DownDepth
        {
            get { return downDepth; }
            set
            {
                downDepth = value;
                this.RaisePropertyChanged(nameof(DownDepth));
            }
        }
        private int selectionDepth;
        /// <summary>
        /// 前一个获取焦点textbox，0为左，1为右，2为上，3为下
        /// </summary>
        public int SelectionDepth
        {
            get { return selectionDepth; }
            set
            {
                selectionDepth = value;
                this.RaisePropertyChanged(nameof(SelectionDepth));
            }
        }

        /// <summary>
        /// 得到轴网类型选择的项
        /// </summary>
        private int selectionIndex;
        public int SelectionIndex
        {
            get { return selectionIndex; }
            set
            {
                selectionIndex = value;
                this.RaisePropertyChanged(nameof(SelectionIndex));
            }
        }


        private int dimensionSelectionIndex;
        /// <summary>
        /// 得到标签类型选择的项
        /// </summary>
        public int DimensionSelectionIndex
        {
            get { return dimensionSelectionIndex; }
            set
            {
                dimensionSelectionIndex = value;
                this.RaisePropertyChanged(nameof(DimensionSelectionIndex));
            }
        }


        /// <summary>
        /// X的标头
        /// </summary>
        private string xValue;
        public string XValue
        {
            get { return xValue; }
            set
            {
                xValue = value;
                this.RaisePropertyChanged(nameof(XValue));
            }
        }
        /// <summary>
        /// Y的标头
        /// </summary>
        private string yValue;
        public string YValue
        {
            get { return yValue; }
            set
            {
                yValue = value;
                this.RaisePropertyChanged(nameof(YValue));
            }
        }


        private bool leftIsCheck;
        /// <summary>
        /// 左右联动
        /// </summary>
        public bool LeftIsCheck
        {
            get { return leftIsCheck; }
            set
            {
                leftIsCheck = value;
                this.RaisePropertyChanged(nameof(LeftIsCheck));
            }
        }


        private bool upIsCheck;
        /// <summary>
        /// 上下联动
        /// </summary>
        public bool UpIsCheck
        {
            get { return upIsCheck; }
            set
            {
                upIsCheck = value;
                this.RaisePropertyChanged(nameof(UpIsCheck));
            }
        }



        public View ShowViewDrafting;

        public UIDocument ShowUIdoc;

        public List<Grid> oldGridList = new List<Grid>();

        public List<Dimension> nwDimensionList = new List<Dimension>();

        #endregion

        #region BaseCommon集合
        /// <summary>
        /// 双击添加命令
        /// </summary>
        public BaseCommon DoubleClickCom { get; set; }
        /// <summary>
        /// 文本框更改命令
        /// </summary>
        public BaseCommon TextChangeCom { get; set; }
        
        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        public GridCreateVM(UIDocument uidoc, View vd)
        {
            this.DoubleClickCom = new BaseCommon
            {
                ExcuteAction = new Action<object>(this.DoubleClickComExecute)
            };
            this.TextChangeCom = new BaseCommon
            {
                ExcuteAction = new Action<object>(this.TextChangeComExecute)
            };
            ShowViewDrafting = vd;
            ShowUIdoc = uidoc;
            SelectionIndex = 0;
            DimensionSelectionIndex = 0;
            LeftIsCheck = true;
            UpIsCheck = true;
            var xGridList = new FilteredElementCollector(uidoc.Document).OfClass(typeof(Grid)).OfType<Grid>().Where(x => (x.Curve as Line).Direction.IsParallel(XYZ.BasisX)).OrderBy(m => (m.Curve as Line).Direction.DotProduct(XYZ.BasisY)).ToList();
            var yGridList = new FilteredElementCollector(uidoc.Document).OfClass(typeof(Grid)).OfType<Grid>().Where(x => (x.Curve as Line).Direction.IsParallel(XYZ.BasisY)).OrderBy(m => (m.Curve as Line).Direction.DotProduct(XYZ.BasisX)).ToList();
            string xName = null;
            string yName = null;
            if (yGridList.Count != 0)
            {
                yName = yGridList.Last().Name;
            }
            if (xGridList.Count != 0)
            {
                xName = xGridList.Last().Name;
            }
            XValue = GetNextGridName(yName);
            YValue = GetNextGridName(xName);
            if (XValue == null) XValue = "1";
            if (YValue == null) YValue = "A";
        }
        /// <summary>
        /// 找到下一根轴网的名称
        /// </summary>
        /// <param name="gridName"></param>
        /// <returns></returns>
        private string GetNextGridName(string gridName)
        {
            if (gridName == null) return null;
            string nextName = "";

            gridName = gridName.Replace(" ", "");

            //判断是否是数字
            bool isDouble = double.TryParse(gridName, out double nowGridName);
            if (isDouble)
            {
                nextName = (nowGridName + 1).ToString();
                return nextName;
            }
            else
            {
                //可能是1-1类型
                bool b = gridName.Contains('-');
                if (b)
                {
                    string[] sqiltXStr = gridName.Split(new char[] { '-' });
                    bool c = double.TryParse(sqiltXStr.Last(), out double cResult);

                    if (c)
                    {
                        nextName = sqiltXStr[0] + "-" + (cResult + 1).ToString();
                        return nextName;
                    }

                }
                else
                {
                    //再判断是否是字母
                    if (gridName.Length == 1)
                    {
                        int ySACII = Encoding.Default.GetBytes(gridName)[0];
                        if ((ySACII > 64 && ySACII < 91) || (ySACII > 96 && ySACII < 123))
                        {
                            if (ySACII < 91 || (ySACII > 96 && ySACII < 123))
                            {
                                nextName = (ySACII + 1).ASCIIToString();
                            }
                            else
                            {
                                nextName = (ySACII - 26).ASCIIToString() + (ySACII - 26).ASCIIToString();
                            }
                            return nextName;


                        }
                    }
                }
            }
            nextName = gridName + "-1";


            return nextName;
        }




        #region Execute集合
        /// <summary>
        /// 双击命令操作
        /// </summary>
        /// <param name="obj"></param>
        private void DoubleClickComExecute(object obj)
        {
            string mouseDoubleText;
            if (obj != null)
            {
                //拿到双击的文本
                mouseDoubleText = obj.ToString();
                switch (selectionDepth)
                {
                    case 0:
                        if (LeftDepth == null || LeftDepth == "")
                        {
                            LeftDepth = mouseDoubleText;
                        }
                        else
                        {

                            LeftDepth += ("," + mouseDoubleText);
                        }
                        break;
                    case 1:
                        if (RightDepth == null || RightDepth == "")
                        {
                            RightDepth = mouseDoubleText;
                        }
                        else
                        {

                            RightDepth += ("," + mouseDoubleText);
                        }
                        break;
                    case 2:
                        if (UpDepth == null || UpDepth == "")
                        {
                            UpDepth = mouseDoubleText;
                        }
                        else
                        {

                            UpDepth += ("," + mouseDoubleText);
                        }
                        break;
                    case 3:
                        if (DownDepth == null || DownDepth == "")
                        {
                            DownDepth = mouseDoubleText;
                        }
                        else
                        {

                            DownDepth += ("," + mouseDoubleText);
                        }
                        break;
                }
            }


        } 
        /// <summary>
        /// 文本修改命令操作
        /// </summary>
        /// <param name="obj"></param>
        private void TextChangeComExecute(object obj)
        {
            Document doc = ShowUIdoc.Document;
            #region 获得数据
            GridCreateVM gVM = obj as GridCreateVM;
            if (gVM.LeftIsCheck)
            {
                gVM.RightDepth = gVM.LeftDepth;
            }
            if (gVM.UpIsCheck)
            {
                gVM.DownDepth = gVM.UpDepth;
            }
            //得到选择的轴网族类型
            List<GridType> GridFamilyList = new FilteredElementCollector(ShowUIdoc.Document).OfClass(typeof(GridType)).OfType<GridType>().ToList();
            var selGridType = GridFamilyList[gVM.SelectionIndex];
            List<DimensionType> dimensionTypes = new FilteredElementCollector(ShowUIdoc.Document).OfClass(typeof(DimensionType)).OfType<DimensionType>().ToList();
            var selDimensionType = dimensionTypes[DimensionSelectionIndex];
            List<string> allValueStringList = new List<string>() { gVM.LeftDepth, gVM.RightDepth, gVM.UpDepth, gVM.DownDepth };
            var lineInfoList = allValueStringList.GetTargetLineList(new XYZ(0, 0, 0));
            #endregion

            #region 创建轴网

            if (oldGridList != null || oldGridList.Count != 0)
            {
                Transaction trans3 = new Transaction(doc, "删除轴网");
                trans3.Start();
                doc.Delete(oldGridList.Select(x => x.Id).ToList());
                doc.Regenerate();
                trans3.Commit();
            }
            Transaction trans = new Transaction(doc, "生成轴网");
            trans.Start();
            //把两边的标头都打开
            selGridType.get_Parameter(BuiltInParameter.GRID_BUBBLE_END_1).Set(1);
            selGridType.get_Parameter(BuiltInParameter.GRID_BUBBLE_END_2).Set(1);

            oldGridList = lineInfoList.CreateGrid(doc, selGridType, gVM.XValue, gVM.YValue);
            //生成注释
            trans.Commit();
            oldGridList.CreateAnnotation(doc, gVM.ShowViewDrafting, selDimensionType);
            oldGridList.CreateAnnotation(doc, doc.ActiveView, selDimensionType);


            ShowUIdoc.ShowElements(oldGridList.Select(x => x.Id).ToList());
            #endregion

        }

        #endregion
    }
}
