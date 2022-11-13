using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeInspector.UI.Inspector.Attributes
{
    /// <summary>
    /// Hides a member from the Runtime Inspector.
    /// </summary>
    [AttributeUsage(
        AttributeTargets.Field
      | AttributeTargets.Property
      | AttributeTargets.Event
      | AttributeTargets.Method
      | AttributeTargets.Constructor, AllowMultiple = false )]
    public sealed class HideAttribute : Attribute
    {
        public HideAttribute()
        {

        }
    }
}
