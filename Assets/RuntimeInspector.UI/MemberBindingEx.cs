using RuntimeInspector.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI
{
    public static class MemberBindingEx
    {
        public static Type GetDrawnType( this MemberBinding binding )
        {
            if( !binding.Metadata.CanRead || binding.Binding.GetValue() == null )
            {
                return binding.Metadata.Type;
            }

            return binding.Binding.GetInstanceType();
        }
    }
}
