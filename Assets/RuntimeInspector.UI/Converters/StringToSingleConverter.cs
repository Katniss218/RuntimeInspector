using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI.Converters
{
    public class StringToSingleConverter : IConverter<string, float>
    {
        public float Convert( string value )
        {
            return float.Parse( value );
        }
    }
}