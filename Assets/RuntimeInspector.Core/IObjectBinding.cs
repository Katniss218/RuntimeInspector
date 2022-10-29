using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Returns the value cached from the last call to <see cref="GetValue"/>.
        /// </summary>
        object LastValue { get; }

        // <summary>
        /// Type of the bound instance (instance-dependent).
        /// </summary>
        Type GetInstanceType();

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
        /// Sets the actual value bound to this binding.
        /// </summary>
        void SetValue( object value );
    }

    public static class IObjectBinding_Ex
    {
        public static bool HasChangedValue( object oldValue, object newValue )
        {
            if( newValue is null )
            {
                return oldValue is not null;
            }
            return !newValue.Equals( oldValue );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>
        /// Be wary of calling this for write-only properties.
        /// </remarks>
        public static bool HasChangedValue( this IObjectBinding binding, out object newValue )
        {
            object oldValue = binding.LastValue;
            newValue = binding.GetValue();

            return HasChangedValue( oldValue, newValue );
        }
    }
}