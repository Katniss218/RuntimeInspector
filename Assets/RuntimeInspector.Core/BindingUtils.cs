using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core
{
    public static class BindingUtils
    {
        public static string GetDisplayName( FieldInfo field )
        {
            return field.Name;
        }

        public static string GetDisplayName( PropertyInfo property )
        {
            return property.Name;
        }
    }
}
