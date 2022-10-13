using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core
{
    public class FieldBinding : IMemberBinding
    {
        public string DisplayName { get; private set; }

        public Type Type { get; private set; }

        private readonly object instance;
        private readonly FieldInfo field;

        public FieldBinding( FieldInfo field, object instance )
        {
            this.field = field;
            this.instance = instance;
            this.DisplayName = BindingUtils.GetDisplayName( field );
            this.Type = field.FieldType;
        }

        public object GetValue()
        {
            return field.GetValue( instance );
        }

        public void SetValue( object value )
        {
            field.SetValue( instance, value );
        }
    }
}
