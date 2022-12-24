using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuntimeEditor.UI.Inspector.Drawers;

namespace RuntimeEditor.UI.Inspector.Attributes
{
    /// <summary>
    /// Makes a member drawn as an asset when drawing its parent object with the <see cref="ObjectDrawer"/>.
    /// </summary>
    [AttributeUsage( AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false )]
    public sealed class AssetAttribute : Attribute
    {
        public AssetAttribute()
        {

        }
    }
}
