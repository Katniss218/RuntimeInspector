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
        List<MemberBinding> GetInstanceMembers();

        /// <summary>
        /// Returns the actual value bound to this binding.
        /// </summary>
        object GetValue(); // Used when displaying objects.

        /// <summary>
        /// Sets the actual value bound to this binding.
        /// </summary>
        void SetValue( object value ); // Used when submitting objects.
    }
}