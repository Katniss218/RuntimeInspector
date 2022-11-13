using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI.Inspector.Converters
{
    public class StringToSingleConverter : IConverter<string, float>
    {
        public float ConvertForward( string value )
        {
            return float.Parse( value );
        }

        public string ConvertReverse( float value )
        {
            return value.ToString();
        }
    }
}