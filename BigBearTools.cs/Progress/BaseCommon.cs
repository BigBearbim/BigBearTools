using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BigBearTools
{
    /// <summary>
    /// 命令基类
    /// </summary>
    public class BaseCommon : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add
            {
                if (CanExcuteFunc != null)
                {
                    CommandManager.RequerySuggested += value;
                }
            }
            remove
            {
                if (CanExcuteFunc != null)
                {
                    CommandManager.RequerySuggested -= value;
                }
            }
        }
        public Func<object, bool> CanExcuteFunc { get; set; }
        public Action<object> ExcuteAction { get; set; }


        public bool CanExecute(object parameter)
        {
            if (CanExcuteFunc == null)
            {
                return true;
            }
            return CanExcuteFunc(parameter);

        }
        public void Execute(object parameter)
        {
            ExcuteAction?.Invoke(parameter);

        }
        public BaseCommon(Action<object> execute, Func<object, bool> canExecute)
        {
            ExcuteAction = execute;
            CanExcuteFunc = canExecute;
        }

        public BaseCommon()
        {
        }

    }
        
}
