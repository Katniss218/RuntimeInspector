using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI.Converters
{
    public class StringToInt32Converter : IConverter<string, int>
    {
        public int ConvertForward( string value )
        {
            return int.Parse( value );
        }

        public string ConvertReverse( int value )
        {
            return value.ToString();
        }
    }
}