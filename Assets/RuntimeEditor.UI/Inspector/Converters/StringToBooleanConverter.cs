using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeEditor.UI.Inspector.Converters
{
    public sealed class StringToBooleanConverter : IConverter<string, bool>
    {
        public bool ConvertForward( Type _, string value )
        {
            return bool.Parse( value );
        }

        public string ConvertReverse( bool value )
        {
            return value.ToString();
        }
    }
}