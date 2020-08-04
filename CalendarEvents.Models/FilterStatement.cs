using System;
using System.ComponentModel;
using System.Reflection;

namespace CalendarEvents.Models
{
    /// <summary>
    /// Defines how a property should be filtered.
    /// </summary>
    public class FilterStatement<TEntity>
    {
        /// <summary>
        /// Name of the property.
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// Express the interaction between the property and the constant value 
        /// defined in this filter statement.
        /// </summary>
        public FilterOperation Operation { get; set; }
        /// <summary>
        /// Constant value that will interact with the property defined in this filter statement.
        /// </summary>
        public object Value { get; set; }
        public bool IsValid
        {
            get
            {
                if (this.PropertyName == null || this.Value == null)
                {
                    return false;
                }

                PropertyInfo propertyInfo = typeof(TEntity).GetProperty(this.PropertyName);

                if (propertyInfo != null && // First validate if PropertyName Is valid.
                    Operation != FilterOperation.Undefined)
                {
                    Type propType = propertyInfo.PropertyType;
                    TypeConverter converter = TypeDescriptor.GetConverter(propType);
                    if (converter.CanConvertFrom(this.Value.GetType()))
                    {
                        return true;
                    }
                }

                return false;
            }
        }
    }
}
