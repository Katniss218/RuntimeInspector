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
        object _parent;

        public Type GetInstanceType()
            => GetValue().GetType();

        public List<MemberBinding> GetInstanceMembers()
            => BindingUtils.GetMembersOf( GetValue() );

        public object GetValue()
            => _instance.GetValue( _parent );

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