using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI.Inspector
{
    /// <summary>
    /// Converts between 2 types.
    /// </summary>
    /// <typeparam name="TIn">The input type.</typeparam>
    /// <typeparam name="TOut">The output type.</typeparam>
    public interface IConverter<TIn, TOut>
    {
        /// <summary>
        /// Converts the input value into the output value (using the Forward conversion path).
        /// </summary>
        TOut ConvertForward( Type outType, TIn value );

        /// <summary>
        /// Converts the output value into the input value (using the Reverse conversion path).
        /// </summary>
        TIn ConvertReverse( TOut value );
    }
}