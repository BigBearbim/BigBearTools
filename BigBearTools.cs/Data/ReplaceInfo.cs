using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBearTools
{
    public class ReplaceInfo:NotifyObject
    {
        string hwFamilyName;
        string hwFamilySymbolName;
        List<FamilySymbol> hwFamilySymbolList;
        List<FamilyInstance> hwFamilyInstanceList;
        string replaceFamilyName;
        string replaceFamilySymbolName;
        List<string> replaceFamilyList;
        List<string> replaceFSList;
        string selFamilyName;
        string selFSName;
        /// <summary>
        /// 红瓦族名称
        /// </summary>
        public string HwFamilyName { get => hwFamilyName; set => hwFamilyName = value; }
        /// <summary>
        /// 红瓦族类型名称
        /// </summary>
        public string HwFamilySymbolName { get => hwFamilySymbolName; set => hwFamilySymbolName = value; }
        /// <summary>
        /// 红瓦该类型下的集合
        /// </summary>
        public List<FamilySymbol> HwFamilySymbolList { get => hwFamilySymbolList; set => hwFamilySymbolList = value; }
        /// <summary>
        /// 替换的族名称
        /// </summary>
        public string ReplaceFamilyName { get => replaceFamilyName; set => replaceFamilyName = value; }
        /// <summary>
        /// 替换的族类型名称
        /// </summary>
        public string ReplaceFamilySymbolName { get => replaceFamilySymbolName; set => replaceFamilySymbolName = value; }
        /// <summary>
        /// 替换的族名称集合
        /// </summary>
        public List<string> ReplaceFamilyList { get => replaceFamilyList; set => replaceFamilyList = value; }
        /// <summary>
        /// 替换的族类型名称集合
        /// </summary>
        public List<string> ReplaceFSList
        {
            get { return replaceFSList; }
            set
            {
                replaceFSList = value;
                this.RaisePropertyChanged(nameof(ReplaceFSList));
            }
        }
        /// <summary>
        /// 选择的族名称
        /// </summary>
        public string SelFamilyName { get => selFamilyName; set => selFamilyName = value; }
        /// <summary>
        /// 选择的族类型名称
        /// </summary>
        public string SelFSName { get => selFSName; set => selFSName = value; }
        /// <summary>
        /// 所有该类型的族实例
        /// </summary>
        public List<FamilyInstance> HwFamilyInstanceList { get => hwFamilyInstanceList; set => hwFamilyInstanceList = value; }
    }
}
