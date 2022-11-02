using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core
{
    /// <summary>
    /// Holds instance-dependent information about an object.
    /// </summary>
    public interface IObjectBinding
    {
        /// <summary>
        /// Members of the bound instance (instance-dependent).
        /// </summary>
        /// <remarks>
        /// Not every type has them defined. Primitives, such as 'int' don't.
        /// </remarks>
        List<MemberBinding> InstanceMembers { get; }

        /// <summary>
        /// Returns the actual value bound to this binding. Caches the value as <see cref="LastValue"/> property until the next call to <see cref="GetValue"/>.
        /// </summary>
        object GetValue();

        /// <summary>
        /// Returns the object containing this binding, if applicable.
        /// </summary>
        /// <remarks>
        /// For the <see cref="Array.Length"/> field, it will return the array.
        /// </remarks>
        object Parent { get; }

        /// <summary>
        /// Sets the actual value bound to this binding.
        /// </summary>
        void SetValue( object value );
    }

    public static class IObjectBinding_Ex
    {
        public static Type GetInstanceType( this IObjectBinding binding )
        {
            Type value = binding.GetValue().GetType();
            return value;
        }

        public static bool IsDifferent( object oldValue, object newValue )
        {
            if( newValue is null )
            {
                return oldValue is not null;
            }
            return !newValue.Equals( oldValue );
        }
    }
}