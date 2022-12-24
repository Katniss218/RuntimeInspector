using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeEditor.UI.Inspector.Converters
{
    public sealed class StringToSingleConverter : IConverter<string, float>
    {
        public float ConvertForward( Type _, string value )
        {
            return float.Parse( value, CultureInfo.InvariantCulture );
        }

        public string ConvertReverse( float value )
        {
            return value.ToString( CultureInfo.InvariantCulture );
        }
    }
}