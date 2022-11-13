using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuntimeInspector.UI.Inspector.Drawers;

namespace RuntimeInspector.UI.Inspector.Attributes
{
    /// <summary>
    /// Makes a member drawn as value instead of reference when drawing its parent object with the <see cref="ObjectDrawer"/>.
    /// </summary>
    [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false )]
    public sealed class DrawAsValueAttribute : Attribute
    {
        public DrawAsValueAttribute()
        {

        }
    }
}
