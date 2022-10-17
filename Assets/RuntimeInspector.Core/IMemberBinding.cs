using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.Core
{
    public interface IMemberBinding
    {
        string Name { get; }
        string DisplayName { get; }

        bool CanRead { get; }
        bool CanWrite { get; }

        Type Type { get; }

        object GetValue();
        void SetValue( object value );
    }

    public interface IMemberBinding<T> : IMemberBinding
    {
        T GetValue();
        void SetValue( T value );
    }

}
