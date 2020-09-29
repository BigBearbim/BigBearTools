using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace BigBearTools
{
    public class NotifyObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        public void RaisePropertyChanged(string paraName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(paraName));
        }

        public  void  RaisePropertyChanged<R>(Expression<Func<INotifyPropertyChanged,R>> expr)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(((MemberExpression)expr.Body).Member.Name));
        }
    }
}
