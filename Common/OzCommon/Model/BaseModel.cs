using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace OzCommon.Model
{
    public class BaseModel : EventArgs, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Raise<T>(Expression<Func<T>> propertyExpression)
        {
            if (PropertyChanged == null)
                return;

            var body = propertyExpression.Body as MemberExpression;
            if (body == null)
                throw new ArgumentException("'propertyExpression' should be a member expression");

            var expression = body.Expression as ConstantExpression;
            if (expression == null)
                throw new ArgumentException("'propertyExpression' body should be a constant expression");

            OnPropertyChanged(body.Member.Name);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
