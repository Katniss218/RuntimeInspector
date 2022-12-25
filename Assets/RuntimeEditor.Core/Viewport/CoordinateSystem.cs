using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeEditor.Core.Viewport
{
    public enum CoordinateSystem : byte
    {
        /// <summary>
        /// World space.
        /// </summary>
        World,
        /// <summary>
        /// Local space of the target.
        /// </summary>
        Local,
        /// <summary>
        /// Local space of the camera.
        /// </summary>
        View
    }
}
