using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core
{
    public interface IMemberBinding
    {
        string DisplayName { get; }
        Type Type { get; }

        object GetValue();
        void SetValue( object value );
    }
}
