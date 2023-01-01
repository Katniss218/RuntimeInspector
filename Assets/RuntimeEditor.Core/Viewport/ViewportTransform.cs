using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeEditor.Core.Viewport
{
    /// <summary>
    /// A move/rotate/scale transform on a specified target object.
    /// </summary>
    public class ViewportTransform : MonoBehaviour
    {
        public Transform Target { get; set; }

        /// <summary>
        /// The coordinate system that the transform works in.
        /// </summary>
        public CoordinateSystem CoordinateSystem { get; set; }

        /// <summary>
        /// The pivot point used as the transform's origin.
        /// </summary>
        public Pivot Pivot { get; set; }

        List<ViewportTransformHandle> _handles;

        // specify different pivots
        // specify different coordinate frames.

        // managed the arroes/planes generated for an object, updates them every frame to reflect the camera orientation.


        // should be rendered on a separate layer. (gizmo layer)

        void Update()
        {
            // keep track of old camera position and rotation. Only recalculate the arrows/planes and invalidate transform if the pos/rot changed.
        }
    }
}