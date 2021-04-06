using Newtonsoft.Json;
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
        public FilterType FilterType { get; set; }        
        /// <summary>
        /// Name of the property.
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// Express the interaction between the property and the constant value 
        /// defined in this filter statement.
        /// </summary>        
        /// <summary>
        /// Constant value that will interact with the property defined in this filter statement.
        /// </summary>        
        public string ValueJson { get; set; }
        public bool IsValid
        {
            get
            {
                if (this.FilterType == FilterType.Undefined || this.ValueJson == null)
                {
                    return false;
                }

                //PropertyInfo propertyInfo = typeof(TEntity).GetProperty(this.PropertyName);

                //if (propertyInfo != null && // First validate if PropertyName Is valid.
                //    Operation != FilterOperation.Undefined)
                //{
                //    Type propType = propertyInfo.PropertyType;
                //    TypeConverter converter = TypeDescriptor.GetConverter(propType);
                //    if (converter.CanConvertFrom(new MyContext(new EventModel(), this.PropertyName), this.Value.GetType()))
                //    {
                //        return true;
                //    }
                //}

                return true;
            }
        }        
    }

    //public class MyContext : ITypeDescriptorContext
    //{
    //    public MyContext(object instance, string propertyName)
    //    {
    //        Instance = instance;
    //        PropertyDescriptor = TypeDescriptor.GetProperties(instance)[propertyName];
    //    }

    //    public object Instance { get; private set; }
    //    public PropertyDescriptor PropertyDescriptor { get; private set; }
    //    public IContainer Container { get; private set; }

    //    public void OnComponentChanged()
    //    {
    //    }

    //    public bool OnComponentChanging()
    //    {
    //        return true;
    //    }

    //    public object GetService(Type serviceType)
    //    {
    //        return null;
    //    }
    //}
}
