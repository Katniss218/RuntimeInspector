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
        object _parent;

        public Type GetInstanceType()
            => GetValue().GetType();

        public List<MemberBinding> GetInstanceMembers()
            => BindingUtils.GetMembersOf( GetValue() );

        public object GetValue()
            => _instance.GetValue( _parent );

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