using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI.Converters
{
    public class StringToBooleanConverter : IConverter<string, bool>
    {
        public bool ConvertForward( string value )
        {
            return bool.Parse( value );
        }

        public string ConvertReverse( bool value )
        {
            return value.ToString();
        }
    }
}