using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuntimeInspector.UI.Drawers;

namespace RuntimeInspector.UI.Attributes
{
    /// <summary>
    /// Makes a member drawn as value instead of reference when drawing its parent object with the <see cref="ObjectDrawer"/>.
    /// </summary>
    [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false )]
    public class DrawAsValueAttribute : Attribute
    {
        public DrawAsValueAttribute()
        {

        }
    }
}
