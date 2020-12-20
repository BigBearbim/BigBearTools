using Autodesk.Revit.DB;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace BigBearTools
{
    public static class UnderGroundMethod
    {
        /// <summary>
        /// 读取地下管道信息
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static List<UnderGroundInfo> ReadUnderGroundData(string path)
        {
            List<UnderGroundInfo> uInfoList = new List<UnderGroundInfo>();

            FileStream fsRead = null;
            IWorkbook wkBook = null;
            if (path != "")
            {
                //1、创建一个工作簿workBook对象
                fsRead = new FileStream(path, FileMode.Open);
                
                //将表.xlsx中的内容读取到fsRead中
                if (path.IndexOf(".xlsx") > 0) // 2007版本
                    wkBook = new XSSFWorkbook(fsRead);
                else if (path.IndexOf(".xls") > 0) // 2003版本
                    wkBook = new HSSFWorkbook(fsRead);
                //拿到第一个工作表
                ISheet sheet = wkBook.GetSheetAt(0);
                
                //遍历每一行
                for (int i = 0; i < sheet.LastRowNum; i++)
                {
                    //获取工作表中的每一行
                    IRow currentRow = sheet.GetRow(i);
                    //判断是否为“点号”
                    ICell cell = currentRow.GetCell(0);
                    string firstStr = cell.StringCellValue;
                    if(firstStr == "点号")
                    {
                        i+=2;
                        currentRow = sheet.GetRow(i);
                    }
                    else if(firstStr != "点号" &&i <=6)
                    {
                        continue;
                    }
                    

                    UnderGroundInfo uInfo = new UnderGroundInfo();
                    string tempString = currentRow.GetCell(0).StringCellValue;
                    if (tempString == "" || tempString == null) continue;
                    uInfo.PointNumber = tempString;
                    uInfo.Characteristic = currentRow.GetCell(1).StringCellValue;
                    uInfo.PointBuriedDepth = currentRow.GetCell(2).NumericCellValue.MetersToFeet();
                    uInfo.X = currentRow.GetCell(3).NumericCellValue.MetersToFeet();
                    uInfo.Y = currentRow.GetCell(4).NumericCellValue.MetersToFeet();
                    uInfo.GroundElevation = currentRow.GetCell(5).NumericCellValue;
                    string startInfo = currentRow.GetCell(6).StringCellValue;
                    if (startInfo != null && startInfo != "")
                    {
                        startInfo = startInfo.Replace(" ", "");
                        string[] spiltStr = startInfo.Split(new char[] { '/' });
                        uInfo.StartPointNumber = spiltStr[0];
                        uInfo.StartPointBuriedDepth = double.Parse(spiltStr[1]);
                        uInfo.StartGroundElevation = double.Parse(spiltStr[2]);
                    }
                    else
                    {
                        uInfo.StartPointNumber = null;
                        uInfo.StartPointBuriedDepth = 0;
                        uInfo.StartGroundElevation = 0;
                    }
                    string endInfo = currentRow.GetCell(7).StringCellValue;
                    if (endInfo != null && endInfo != "")
                    {
                        endInfo = endInfo.Replace(" ", "");
                        string[] endtStr = endInfo.Split(new char[] { '/' });
                        uInfo.EndPointNumber = endtStr[0];
                        uInfo.EndPointBuriedDepth = double.Parse(endtStr[1]);
                        uInfo.EndGroundElevation = double.Parse(endtStr[2]);
                    }
                    else
                    {
                        uInfo.EndPointNumber = null;
                        uInfo.EndPointBuriedDepth = 0;
                        uInfo.EndGroundElevation = 0;
                    }
                    try
                    {
                        string pipeLengthDouble = currentRow.GetCell(8).NumericCellValue.ToString();
                        if (pipeLengthDouble == ""|| pipeLengthDouble ==null)
                        {
                            uInfo.PipeLength = 0;
                        }
                        else
                        {
                            uInfo.PipeLength = double.Parse(pipeLengthDouble);
                        }
                    }
                    catch 
                    {
                        uInfo.PipeLength = 0;
                    }
                    try
                    {
                        string pipeTempStr = currentRow.GetCell(9).StringCellValue;
                        if (pipeTempStr == null || pipeTempStr == "")
                        {
                            uInfo.PipeSize = null;
                        }
                        else
                        {
                            uInfo.PipeSize = pipeTempStr;
                        }
                    }
                    catch 
                    {

                        uInfo.PipeSize = null;

                    }
                    try
                    {
                        string materialStr = currentRow.GetCell(15).StringCellValue;
                        if (materialStr == null || materialStr == "")
                        {
                            uInfo.Material = null;
                        }
                        else
                        {
                            uInfo.Material = materialStr;
                        }
                    }
                    catch
                    {

                        uInfo.Material = null;

                    }
                    try
                    {
                        string unitStr = currentRow.GetCell(16).StringCellValue;
                        if (unitStr == null || unitStr == "")
                        {
                            uInfo.AdministrativeUnit = null;
                        }
                        else
                        {
                            uInfo.AdministrativeUnit = unitStr;
                        }
                    }
                    catch
                    {

                        uInfo.AdministrativeUnit = null;

                    }


                    uInfoList.Add(uInfo);
                }
            }
            else
            {
                MessageBox.Show("选择文件失败");
            }
            fsRead.Close();
            wkBook.Close();
            fsRead.Dispose();


            return uInfoList;
        }

        public static Element GetElement(this string characteristic,Document doc)
        {
            Element elem = null;
            if (characteristic.Contains("点")) return null;
            //下面收集构件，如果没有就直接复制一个symbol 然后改名字

            //检修井
            //消火栓
            //电信人孔
            //阀门井
            //化粪池
            

            return elem;
        }

    }
}
