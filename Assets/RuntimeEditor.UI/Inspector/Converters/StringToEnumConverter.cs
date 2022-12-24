using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeEditor.UI.Inspector.Converters
{
    public sealed class StringToEnumConverter : IConverter<string, Enum>
    {
        public Enum ConvertForward( Type type, string value )
        {
            return (Enum)Enum.Parse( type, value );
        }

        public string ConvertReverse( Enum value )
        {
            return value.ToString();
        }
    }
}