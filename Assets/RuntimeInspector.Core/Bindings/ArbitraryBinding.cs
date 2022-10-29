using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core.Bindings
{
    /// <summary>
    /// A binding bound by a getter and setter methods.
    /// </summary>
    /// <remarks>
    /// Can be used to bind to ANY object. Most useful for members that have to be set via methods (like 'GetComponent<![CDATA[<T>]]>()' / 'AddComponent<![CDATA[<T>]]>()').
    /// </remarks>
    internal class ArbitraryBinding : IObjectBinding // this could be used to set the values of components on a gameobject, or assets, etc.
    {
        Func<object> _getter;
        Action<object> _setter;
        bool _isInitialized = false;
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

        internal ArbitraryBinding( Func<object> getter, Action<object> setter )
        {
            this._getter = getter;
            this._setter = setter;
        }

        public Type GetInstanceType()
            => GetValue().GetType();

        private void Recalculate()
        {
            _lastValue = _getter();
            _instanceBindings = BindingUtils.GetMembersOf( _lastValue );
            _isInitialized = true;
        }

        public object GetValue()
        {
            object newValue = _getter();
            object oldValue = _lastValue;

            if( IObjectBinding_Ex.HasChangedValue( oldValue, newValue ) )
            {
                Recalculate();
            }

            return _lastValue;
        }

        public void SetValue( object value )
            => _setter( value );
    }
}