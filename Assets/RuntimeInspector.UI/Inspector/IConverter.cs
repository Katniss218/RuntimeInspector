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
        /// Converts the input value into the output value (forward conversion).
        /// </summary>
        TOut ConvertForward( TIn value );

        /// <summary>
        /// Converts the output value into the input value (reverse conversion).
        /// </summary>
        TIn ConvertReverse( TOut value );
    }
}