using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RuntimeInspector.Core;

namespace RuntimeInspector.UI
{
    /// <summary>
    /// Tells the DrawerProvider that a specified drawer can be used to draw a specified type.
    /// </summary>
    /// <remarks>
    /// Guarantees that the <see cref="ObjectGraphNode.GetValue()"/> will return an object assignable to the <see cref="DrawnType"/>.
    /// </remarks>
    public class DrawerOfAttribute : Attribute
    {
        /// <summary>
        /// The type that this drawer can draw.
        /// </summary>
        public Type DrawnType { get; set; }

        public DrawerOfAttribute( Type drawnType )
        {
            this.DrawnType = drawnType;
        }
    }
}