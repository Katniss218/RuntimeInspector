using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RuntimeEditor.Core.Viewport
{
    /// <summary>
    /// Represents a move or scale arrow (1D) that moves/scales along an arbitrary axis.
    /// </summary>
    [RequireComponent( typeof( Collider ) )]
    public class ViewportTransformHandle : MonoBehaviour
    {
        public enum TransformType : byte
        {
            MOVE_1D, // transforms along forward.
            ROTATE_1D, // rotates around forward.
            SCALE_1D, // transforms along forward.

            MOVE_2D, // transforms along axes except forward.
            SCALE_2D // transforms along axes except forward.
        }

        // The arrow graphic is completely unnecessary, other than having a collider for the initial grabbing (to force the user to click on the arrow to start the movement).

        // Algorithm:
        // 1. Create a plane that points towards the camera, and is orthogonal to the arrow's 'forward' axis.
        // 2. Raycast from cursor to find start/end points on the plane (in local space).
        // 3. Take the forward (Z) component to the points in arrow's local space.
        //    The component has to be relative to the arrow's cached position when it started moving.

        // Move:
        // - get the displacement relative to where the player first clicked on the arrow.

        // Scale:
        // - get the displacement relative to where the player first clicked on the arrow.
        // - divide it by distance from the arrow's start position to where the player first clicked on the arrow to "normalize" it.

        // rotation is different, it uses a plane to calculate angle between the start, origin, and end.

        // rotate works by raycasting start annd end against a plane defined by the normal.
        // then we can get the normalized directions to start and end.
        // then get the angle between them, and use that angle to construct a quaternion with angle and axis.

        [field: SerializeField]
        public Transform Obj { get; set; }

        [field: SerializeField]
        public Camera Camera { get; set; }

        [field: SerializeField]
        public TransformType Type { get; set; } = TransformType.MOVE_1D;

        // depending on the desired transformation space (world/local/camera), we can place the move tools appropriately.

        // true if the arrow is currently being dragged/moved by the user.
        bool _isMoving = false;
        float _start = 0.0f;

        Vector3 _startPos;
        Vector3 _startForward;
        Matrix4x4 _startLocalToWorldMatrix;
        Matrix4x4 _startWorldToLocalMatrix;

        Vector3 _startObjPos = Vector3.zero;

        Collider _collider;

        void Awake()
        {
            _collider = this.GetComponent<Collider>();
        }

        void Update()
        {
            if( Input.GetKeyDown( KeyCode.Mouse0 ) && !_isMoving )
            {
                Ray ray = this.Camera.ScreenPointToRay( Input.mousePosition );
                if( Physics.Raycast( ray, out RaycastHit hitInfo ) && hitInfo.collider == _collider )
                {
                    StartMovement();
                }
                return;
            }

            if( Input.GetKey( KeyCode.Mouse0 ) && _isMoving )
            {
                ContinueMovement();
                return;
            }

            if( Input.GetKeyUp( KeyCode.Mouse0 ) && _isMoving )
            {
                EndMovement();
                return;
            }
        }

        private void CacheStartingVariables()
        {
            _startPos = this.transform.position;
            _startForward = this.transform.forward;
            _startLocalToWorldMatrix = this.transform.localToWorldMatrix;
            _startWorldToLocalMatrix = this.transform.worldToLocalMatrix;

            _startObjPos = this.Obj.transform.position;
        }

        public void StartMovement()
        {
            CacheStartingVariables();

            _start = Raycast( this.Camera );
            _isMoving = true;
        }

        public void ContinueMovement()
        {
            float val = Raycast( this.Camera );

            if( float.IsNaN( val ) ) // if the player tries to move the arrow to an invalid position - abort, don't move so we don't lose the object.
            {
                val = _start;
            }

            val -= _start; // Relative to where the user clicked, not to the origin.

            Vector3 localHitPoint = Vector3.forward * val;
            Vector3 worldHitPoint = _startLocalToWorldMatrix * localHitPoint;

            Obj.transform.position = _startObjPos + worldHitPoint; // Origin is at the arrow, but the transformation has to be relative to the origin of the transformee.
        }

        public void EndMovement()
        {
            ContinueMovement();
            _isMoving = false;
        }

        /// <summary>
        /// Returns the plane to use when raycasting with this arrow.
        /// </summary>
        /// <returns>The plane that operates in world space.</returns>
        private Plane GetPlane( Camera camera )
        {
            // plane faces the camera, but is then projected onto the plane normal to the forward direction of the arrow.
            Vector3 normal = (camera.transform.position - _startPos).normalized;

            normal = Vector3.ProjectOnPlane( normal, _startForward ); // project the camera-facing normal onto the forward direction to make it so that every point along the forward direction lies on the plane.

            normal.Normalize();

            Plane plane = new Plane( normal, _startPos );

            return plane;
        }

        /// <summary>
        /// Returns the distance from the arrow origin, along the arrow.
        /// </summary>
        private float Raycast( Camera camera )
        {
            Plane plane = GetPlane( camera );

            Ray ray = camera.ScreenPointToRay( Input.mousePosition ); // The ray operates in world space.

            if( plane.Raycast( ray, out float distanceAlongRay ) )
            {
                Vector3 hitPoint = ray.GetPoint( distanceAlongRay ); // relative to world origin.

                hitPoint -= _startPos; // relative to start position instead of the origin. If you set relative to current position it flickers when parented to the moving object.

                Vector3 hitPointLocal = _startWorldToLocalMatrix * hitPoint;

                return hitPointLocal.z;
            }
            else // This will only happen if we're looking at a very acute angle along or opposite to the arrow (or if we somehow can glitch the camera behind the starting point).
            {
                return float.NaN;
            }
        }
    }
}