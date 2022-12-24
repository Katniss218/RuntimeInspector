using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeEditor.UI.Inspector.Converters
{
    public sealed class StringToInt32Converter : IConverter<string, int>
    {
        public int ConvertForward( Type _, string value )
        {
            return int.Parse( value, CultureInfo.InvariantCulture );
        }

        public string ConvertReverse( int value )
        {
            return value.ToString( CultureInfo.InvariantCulture );
        }
    }
}