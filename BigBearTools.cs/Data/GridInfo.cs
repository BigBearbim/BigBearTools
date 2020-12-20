using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BigBearTools
{
    public class GridInfo:NotifyObject
    {

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
    }
}
