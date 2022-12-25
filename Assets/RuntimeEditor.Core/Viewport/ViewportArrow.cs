using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeEditor.Core.Viewport
{
    /// <summary>
    /// Represents a move or scale arrow (1D) that moves/scales along an arbitrary axis.
    /// </summary>
    public class ViewportArrow : MonoBehaviour
    {
        // depending on the desired transformation space (world/local/camera), we can place the move tools appropriately.


        void Start()
        {
            // plane faces the camera, but is then projected onto the plane normal to the forward direction of the arrow.
            Vector3 normal = Camera.main.transform.position - this.transform.position;
            Vector3 point = this.transform.position;

            Vector3.ProjectOnPlane( normal, this.transform.forward ); // project the camera-facing normal onto the forward direction to make it so that every point along the forward direction lies on the plane.
            Plane plane = new Plane( normal, point );

            Ray ray = new Ray();
            if( plane.Raycast( ray, out float distanceAlongRay ) ) // should always be true, since we can only move the objects that we see, so the plane won't be behind us.
            {
                Vector3 hitPoint = ray.GetPoint( distanceAlongRay );
                // raycast to find start and end points on the plane.
                // convert the hit point to the arrow's local space.
                // take the forward (Z) components (will be in local space) of start and end.
                // subtract `end - start` to get the displacement.


                // for scale - divide the displacement by distance `start - transform.position` to "normalize" it.
                // then we convert the arrow's forward direction to the target object's space, and move/scale along the resulting direction and displacement.

                // rotation is different, it uses a plane.
            }
            else
            {
                Debug.LogWarning( "Tried to move an object that is behind the camera." );
            }
        }

        void Update()
        {

        }
    }
}