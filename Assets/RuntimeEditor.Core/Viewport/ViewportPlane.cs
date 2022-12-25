using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeEditor.Core.Viewport
{
    /// <summary>
    /// Represents a move/scale plane (2D) or a rotation tool (1D).
    /// </summary>
    public class ViewportPlane : MonoBehaviour
    {
        void Start()
        {
            Plane plane = new Plane( this.transform.forward, this.transform.position );
            // rotate works by raycasting start annd end against a plane defined by the normal.
            // then we can get the normalized directions to start and end.
            // then get the angle between them, and use that angle to construct a quaternion with angle and axis.
        }

        void Update()
        {

        }
    }
}