using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core.Bindings
{
    /// <summary>
    /// A binding bound to a property.
    /// </summary>
    /// <remarks>
    /// The property might be read-only or write-only.
    /// </remarks>
    internal class PropertyBinding : IObjectBinding
    {
        PropertyInfo _instance; // for getvalue, setvalue
        bool _isInitialized = false;
        object _parent;
        object _lastValue;
        List<MemberBinding> _instanceBindings;

        public object LastValue
        {
            get
            {
                return _lastValue;
            }
        }

        public List<MemberBinding> InstanceMembers
        {
            get
            {
                return _instanceBindings;
            }
        }

        public Type GetInstanceType()
            => GetValue().GetType();

        private void Recalculate()
        {
            _lastValue = _instance.GetValue( _parent );
            _instanceBindings = BindingUtils.GetMembersOf( _lastValue );
            _isInitialized = true;
        }

        public object GetValue()
        {
            object newValue = _instance.GetValue( _parent );
            object oldValue = _lastValue;

            if( IObjectBinding_Ex.HasChangedValue( oldValue, newValue ) )
            {
                Recalculate();
            }

            return _lastValue;
        }

        public void SetValue( object value )
            => _instance.SetValue( _parent, value );

        public static PropertyBinding Create( PropertyInfo property, object parent )
        {
            return new PropertyBinding()
            {
                _instance = property,
                _parent = parent
            };
        }
    }
}