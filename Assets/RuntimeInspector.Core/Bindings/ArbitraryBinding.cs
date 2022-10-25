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

        internal ArbitraryBinding( Func<object> getter, Action<object> setter )
        {
            this._getter = getter;
            this._setter = setter;
        }

        public Type GetInstanceType()
            => GetValue().GetType();

        public List<MemberBinding> GetInstanceMembers()
            => BindingUtils.GetMembersOf( GetValue() );

        public object GetValue()
            => _getter();

        public void SetValue( object value )
            => _setter( value );
    }
}