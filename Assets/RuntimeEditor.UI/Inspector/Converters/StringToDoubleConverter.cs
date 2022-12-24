using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeEditor.UI.Inspector.Converters
{
    public sealed class StringToDoubleConverter : IConverter<string, double>
    {
        public double ConvertForward( Type _, string value )
        {
            return double.Parse( value, CultureInfo.InvariantCulture );
        }

        public string ConvertReverse( double value )
        {
            return value.ToString( CultureInfo.InvariantCulture );
        }
    }
}