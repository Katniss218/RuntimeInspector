using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core
{
    public class PropertyBinding : IMemberBinding
    {
        public string DisplayName { get; private set; }

        public Type Type { get; private set; }

        private readonly object instance;
        private readonly PropertyInfo property;

        public PropertyBinding( PropertyInfo property, object instance )
        {
            this.property = property;
            this.instance = instance;
            this.DisplayName = BindingUtils.GetDisplayName( property );
            this.Type = property.PropertyType;
        }

        public object GetValue()
        {
            return property.GetValue( instance );
        }

        public void SetValue( object value )
        {
            property.SetValue( instance, value );
        }
    }
}