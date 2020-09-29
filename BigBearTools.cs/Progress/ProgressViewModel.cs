using System.Diagnostics;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;

namespace BigBearTools
{
    public enum ProgressStatus
    {
        /// <summary>
        /// 处理中
        /// </summary>
        Progressing,

        /// <summary>
        /// 暂停
        /// </summary>
        Pause,

        /// <summary>
        /// 停止
        /// </summary>
        Stop
    }

    public class ProgressViewModel : NotifyObject
    {
        #region 命令
        public BaseCommon LoadedCmd { get; set; }

        /// <summary>
        /// 停止
        /// </summary>
        public BaseCommon StopCmd { get; set; }


        /// <summary>
        /// 关闭
        /// </summary>
        public BaseCommon CloseCmd { get; set; }
        #endregion

        #region 属性

        private string detailStr = string.Empty;
        /// <summary>
        /// 详情
        /// </summary>
        public string DetailStr
        {
            get
            {
                return detailStr;
            }
            set
            {
                detailStr = value;
                this.RaisePropertyChanged(o => DetailStr);
                System.Windows.Forms.Application.DoEvents();
            }
        }

        private int currentValue = 0;
        public int CurrentValue
        {
            get
            {
                return currentValue;
            }
            set
            {
                currentValue = value;
                Percentage = (double)CurrentValue / (double)MaxValue;
                this.RaisePropertyChanged(o => CurrentValue);
            }
        }

        private int maxValue = 100;
        /// <summary>
        /// 最大值
        /// </summary>
        public int MaxValue
        {
            get
            {
                return maxValue;
            }
            set
            {
                maxValue = value;
                Percentage = (double)CurrentValue / (double)MaxValue;
                this.RaisePropertyChanged(o => MaxValue);
            }
        }

        private double percentage;
        /// <summary>
        /// 百分比
        /// </summary>
        public double Percentage
        {
            get
            {
                return percentage;
            }
            private set
            {
                percentage = value;
                if (percentage > 0.48)
                {
                    PercentageForeground = new SolidColorBrush(Colors.White);
                }
                else
                {
                    PercentageForeground = new SolidColorBrush(Color.FromRgb(0, 114, 198));
                }
                this.RaisePropertyChanged(o => Percentage);
            }
        }

        private ProgressStatus state = ProgressStatus.Progressing;
        /// <summary>
        /// 状态
        /// </summary>
        public ProgressStatus State
        {
            get
            {
                return state;
            }
            private set
            {
                state = value;
                this.RaisePropertyChanged(o => State);
            }
        }
        private Brush percentageForeground;
        /// <summary>
        /// 百分比前景色
        /// </summary>
        public Brush PercentageForeground
        {
            get
            {
                return percentageForeground;
            }
            private set
            {
                percentageForeground = value;
                this.RaisePropertyChanged(o => PercentageForeground);
            }
        }

        private string elapsedTime = "00:00:00";
        /// <summary>
        /// 耗时
        /// </summary>
        public string ElapsedTime
        {
            get
            {
                return elapsedTime;
            }
            private set
            {
                elapsedTime = value;
                this.RaisePropertyChanged(o => ElapsedTime);
            }
        }

        private Thread elapsedThread;
        #endregion

        #region 私有变量
        /// <summary>
        /// 计时器
        /// </summary>
        private Stopwatch stopwatch;
        #endregion

        private Window window = null;

        public ProgressViewModel()
        {
            LoadedCmd = new BaseCommon()
            {
                ExcuteAction = (obj) =>
                {
                    window = obj as Window;
                    if (window != null)
                    {
                        window.Topmost = true;
                    }

                    stopwatch = new Stopwatch();
                    stopwatch.Start();

                    elapsedThread = new Thread(new ThreadStart(() =>
                    {
                        while (true)
                        {
                            window?.Dispatcher.Invoke(() =>
                            {
                                ElapsedTime = stopwatch.Elapsed.ToString(@"hh\:mm\:ss");
                            });
                            Thread.Sleep(100);
                        }
                    }));
                    elapsedThread.IsBackground = true;
                    elapsedThread.Start();
                }
            };

            #region 停止按钮
            StopCmd = new BaseCommon()
            {
                ExcuteAction = (obj) =>
                {
                    State = ProgressStatus.Stop;
                }
            };
            #endregion

            #region 关闭
            CloseCmd = new BaseCommon()
            {
                ExcuteAction = (obj) =>
                {
                    elapsedThread.Abort();
                    stopwatch.Stop();
                }
            };
            #endregion

        }
        public void ResetInfo(string details, int maxValue)
        {
            State = ProgressStatus.Progressing;
            DetailStr = details;
            CurrentValue = 0;
            MaxValue = maxValue;
        }
    }
}
