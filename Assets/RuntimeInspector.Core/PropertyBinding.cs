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
        public string Name { get; private set; }

        public string DisplayName { get; private set; }

        public bool CanRead { get; private set; }
        public bool CanWrite { get; private set; }

        public Type Type { get; private set; }

        private readonly PropertyInfo _property;
        private readonly object _instance;

        static readonly Type _propertyBindingType = typeof( PropertyBinding<> );

        protected PropertyBinding( string name, string displayName, Type type, PropertyInfo property, object instance )
        {
            if( property.PropertyType != type )
            {
                throw new ArgumentException( "The type, and the type of the field must be the same." );
            }

            this.Name = name;
            this.DisplayName = displayName;
            this.Type = type;
            this._property = property;
            this._instance = instance;
            this.CanRead = property.CanRead;
            this.CanWrite = property.CanWrite;
        }

        public static IMemberBinding GetBinding( PropertyInfo property, object instance )
        {
            Type type = property.PropertyType;

            Type propertyBindingType = _propertyBindingType.MakeGenericType( type );

            IMemberBinding binding = (IMemberBinding)Activator.CreateInstance( propertyBindingType, property.Name, property.Name, property, instance );
            return binding;
        }

        public void SetValue( object value )
        {
            // this is slow, we can do better with expressions.
            _property.SetValue( _instance, value );
        }

        public object GetValue()
        {
            return _property.GetValue( _instance );
        }
    }

    public class PropertyBinding<T> : PropertyBinding, IMemberBinding<T>
    {
        public PropertyBinding( string name, string displayName, PropertyInfo property, object instance ) : base( name, displayName, typeof( T ), property, instance )
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