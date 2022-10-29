using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core.Bindings
{
    /// <summary>
    /// A binding bound directly to a field.
    /// </summary>
    internal class FieldBinding : IObjectBinding
    {
        FieldInfo _instance; // for getvalue, setvalue
        bool _isInitialized = false;
        object _parent;
        object _lastValue;
        List<MemberBinding> _instanceBindings;

        public object LastValue
        {
            get
            {
                //if( !_isInitialized )
                //{
                //    Recalculate();
                //}
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

        public static FieldBinding Create( FieldInfo field, object parent )
        {
            return new FieldBinding()
            {
                _instance = field,
                _parent = parent
            };
        }
    }
}