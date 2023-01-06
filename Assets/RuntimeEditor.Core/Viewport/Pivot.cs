using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RuntimeEditor.Core.Viewport
{
    /// <summary>
    /// The origin of the transformation.
    /// </summary>
    public enum Pivot : byte
    {
        /// <summary>
        /// Origin (0, 0, 0) of the target.
        /// </summary>
        TargetOrigin

            // for world origin and others, more complicated handle algorithm will be needed to handle e.g. movement while scaling, if the pivot is not at the object's origin.
    }
}
