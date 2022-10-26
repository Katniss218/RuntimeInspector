using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI.Converters
{
    public class StringToBooleanConverter : IConverter<string, bool>
    {
        public bool Convert( string value )
        {
            return bool.Parse( value );
        }
    }
}