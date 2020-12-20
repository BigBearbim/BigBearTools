using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace BigBearTools
{
    class BigBearToolAddin : IExternalApplication
    {
        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            //添加选项卡名称
            application.CreateRibbonTab("BigBearTool");
            //添加选项卡功能
            RibbonPanel panel3 = application.CreateRibbonPanel("BigBearTool", "构件处理");
            panel3.AddSeparator();
            RibbonPanel panel1 = application.CreateRibbonPanel("BigBearTool", "坐标分区");
            panel1.AddSeparator();

            RibbonPanel panel2 = application.CreateRibbonPanel("BigBearTool", "机电分区");
            panel2.AddSeparator();
            RibbonPanel panel5 = application.CreateRibbonPanel("BigBearTool", "结构分区");
            panel5.AddSeparator();
            RibbonPanel panel4 = application.CreateRibbonPanel("BigBearTool", "关于");


            //添加按钮
            PushButtonData pd1 = new PushButtonData("xx1", "坐标系统一", typeof(BigBearToolAddin).Assembly.Location, typeof(UnifiedCoordinateCmd).FullName);
            pd1.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(typeof(BigBearToolAddin).Assembly.Location) + "\\Libs\\Image\\坐标系统一.png"));
            PushButtonData pd2 = new PushButtonData("xx2", "坐标值查询", typeof(BigBearToolAddin).Assembly.Location, typeof(QueryCoordinateCmd).FullName);
            pd2.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(typeof(BigBearToolAddin).Assembly.Location) + "\\Libs\\Image\\坐标点查询.png"));
            PushButtonData pd3 = new PushButtonData("xx3", "坐标点输入", typeof(BigBearToolAddin).Assembly.Location, typeof(InputCoordinate).FullName);
            pd3.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(typeof(BigBearToolAddin).Assembly.Location) + "\\Libs\\Image\\坐标点输入.png"));
            PushButtonData pd4 = new PushButtonData("xx4", "关于", typeof(BigBearToolAddin).Assembly.Location, typeof(About).FullName);
            pd4.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(typeof(BigBearToolAddin).Assembly.Location) + "\\Libs\\Image\\Revit.png"));
            PushButtonData pd5 = new PushButtonData("xx5", "桥架类型统一", typeof(BigBearToolAddin).Assembly.Location, typeof(UnifyCTSystem).FullName);
            pd5.ToolTip = "统一相连接的桥架类型";
            pd5.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(typeof(BigBearToolAddin).Assembly.Location) + "\\Libs\\Image\\统一桥架类型.png"));
            PushButtonData pd6 = new PushButtonData("xx6", "去除重复", typeof(BigBearToolAddin).Assembly.Location, typeof(Purgemd).FullName);
            pd6.ToolTip = "去除重叠的风管、管道、载入族三种类型";
            pd6.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(typeof(BigBearToolAddin).Assembly.Location) + "\\Libs\\Image\\重叠.png"));
            PushButtonData pd7 = new PushButtonData("xx7", "梁替换", typeof(BigBearToolAddin).Assembly.Location, typeof(ReplaceComponent).FullName);
            pd7.ToolTip = "选择替换由红瓦插件生成的梁";
            pd7.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(typeof(BigBearToolAddin).Assembly.Location) + "\\Libs\\Image\\梁替换.png"));

            PushButtonData pd8 = new PushButtonData("xx8", "构件连接处理", typeof(BigBearToolAddin).Assembly.Location, typeof(CutBuildingWallCmd).FullName);
            pd8.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(typeof(BigBearToolAddin).Assembly.Location) + "\\Libs\\Image\\构件.png"));
            PushButtonData pd9 = new PushButtonData("xx9", "批量轴网", typeof(BigBearToolAddin).Assembly.Location, typeof(UndergroundPipeCmd).FullName);
            pd9.LargeImage = new BitmapImage(new Uri(Path.GetDirectoryName(typeof(BigBearToolAddin).Assembly.Location) + "\\Libs\\Image\\轴网.png"));


            //把按钮添加到选项卡中
            #region 坐标功能
            PushButton pb1 = panel1.AddItem(pd1) as PushButton;
            PushButton pb2 = panel1.AddItem(pd2) as PushButton;
            PushButton pb3 = panel1.AddItem(pd3) as PushButton;
            #endregion
            #region 机电功能
            PushButton pb5 = panel2.AddItem(pd5) as PushButton;
            #endregion
            #region 结构分组
            PushButton pb7 = panel5.AddItem(pd7) as PushButton;
            #endregion
            #region 构件处理
            PushButton pb9 = panel3.AddItem(pd9) as PushButton;
            PushButton pb6 = panel3.AddItem(pd6) as PushButton;
            PushButton pb8 = panel3.AddItem(pd8) as PushButton;
            #endregion
            #region 关于
            PushButton pb4 = panel4.AddItem(pd4) as PushButton;
            #endregion



            return Result.Succeeded;

        }
    }
}
