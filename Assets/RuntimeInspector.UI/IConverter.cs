using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI
{
    /// <summary>
    /// Converts between 2 types.
    /// </summary>
    /// <typeparam name="TIn">The input type of the Convert method.</typeparam>
    /// <typeparam name="TOut">The output type of the Convert method.</typeparam>
    public interface IConverter<TIn, TOut>
    {
        /// <summary>
        /// Converts one type of value into another.
        /// </summary>
        TOut Convert( TIn value );
    }
}