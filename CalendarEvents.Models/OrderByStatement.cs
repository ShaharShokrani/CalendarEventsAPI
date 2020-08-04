using System;
using System.ComponentModel;
using System.Reflection;

namespace CalendarEvents.Models
{
    public class OrderByStatement<TEntity>
    {
        public string PropertyName { get; set; }
        public OrderByDirection Direction { get; set; }
        public bool IsValid
        {
            get
            {
                PropertyInfo propertyInfo = typeof(TEntity).GetProperty(this.PropertyName);

                if (propertyInfo != null)
                {
                    return true;
                }

                return false;
            }
        }
    }
}
