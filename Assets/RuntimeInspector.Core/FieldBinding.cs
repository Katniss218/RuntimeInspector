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
        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public bool CanRead => true;
        public bool CanWrite => true;

        public Type Type { get; private set; }

        readonly FieldInfo field;
        readonly object instance;

        static readonly Type FIELD_BINDING_TYPE = typeof( FieldBinding<> );

        protected FieldBinding( string name, string displayName, Type type, FieldInfo field, object instance )
        {
            if( field.FieldType != type )
            {
                throw new ArgumentException( "The type, and the type of the field must be the same." );
            }

            this.Name = name;
            this.DisplayName = displayName;
            this.Type = type;
            this.field = field;
            this.instance = instance;
        }

        public static IMemberBinding GetBinding( FieldInfo field, object instance )
        {
            Type type = field.FieldType;

            Type fieldBindingType = FIELD_BINDING_TYPE.MakeGenericType( type );

            IMemberBinding binding = (IMemberBinding)Activator.CreateInstance( fieldBindingType, field.Name, field.Name, field, instance );
            return binding;
        }

        public void SetValue( object value )
        {
            // this is slow, we can do better with expressions.
            field.SetValue( instance, value );
        }

        public object GetValue()
        {
            return field.GetValue( instance );
        }
    }

    public class FieldBinding<T> : FieldBinding, IMemberBinding<T>
    {
        public FieldBinding( string name, string displayName, FieldInfo field, object instance ) : base( name, displayName, typeof( T ), field, instance )
        {
        }

        new public T GetValue()
        {
            return (T)base.GetValue();
        }

        public void SetValue( T value )
        {
            base.SetValue( value );
        }
    }
}
