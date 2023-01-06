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
        Vector3 _start = Vector3.zero;

        Vector3 _startPos;
        Vector3 _startForward;
        Matrix4x4 _startHandleToWorldMatrix; // Initial value of the handle, so the handle can move while the transform is active but the transform is going to act as if it didn't move.
        Matrix4x4 _startWorldToHandleMatrix;
        Matrix4x4 _startHandleToWorldDirection;
        Matrix4x4 _startWorldToHandleDirection;
        Matrix4x4 _startWorldToObjDirection;

        Vector3 _startObjPos = Vector3.zero;
        Vector3 _startObjScale = Vector3.one;

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

                // arrows get drawn on top of other objects.
                // We need to account for the possibility that those objects, that are actually in front, have colliders that occlude the arrow.
                RaycastHit[] hits = Physics.RaycastAll( ray );
                foreach( var hitInfo in hits )
                {
                    if( hitInfo.collider == _collider )
                    {
                        StartTransformation();
                        return;
                    }
                }
                return;
            }

            if( Input.GetKey( KeyCode.Mouse0 ) && _isMoving )
            {
                ContinueTransformation();
                return;
            }

            if( Input.GetKeyUp( KeyCode.Mouse0 ) && _isMoving )
            {
                EndTransformation();
                return;
            }
        }

        private void CacheStartingVariables()
        {
            _startPos = this.transform.position;
            _startForward = this.transform.forward;
            _startHandleToWorldMatrix = this.transform.localToWorldMatrix;
            _startWorldToHandleMatrix = this.transform.worldToLocalMatrix;
            _startHandleToWorldDirection = Matrix4x4.TRS( Vector3.zero, this.transform.rotation, Vector3.one );
            _startWorldToHandleDirection = _startHandleToWorldDirection.inverse;
            _startWorldToObjDirection = Matrix4x4.TRS( Vector3.zero, this.Obj.transform.rotation, Vector3.one ).inverse; // discard the scale component.

            _startObjPos = this.Obj.transform.position;
            _startObjScale = this.Obj.transform.localScale;
        }

        public void StartTransformation()
        {
            CacheStartingVariables();

            _start = Raycast( this.Camera );
            _isMoving = true;
        }

        public void ContinueTransformation()
        {
            Vector3 val = Raycast( this.Camera ); // val is where the player clicked, in arrow space.

            // if the player tries to move the arrow to an invalid position - abort, don't move so we don't lose the object.
            if( float.IsNaN( val.x ) )
            {
                val.x = _start.x;
            }
            if( float.IsNaN( val.y ) )
            {
                val.y = _start.y;
            }
            if( float.IsNaN( val.z ) )
            {
                val.z = _start.z;
            }

            if( Type == TransformType.MOVE_1D )
            {
                Move1D( val );
                return;
            }
            if( Type == TransformType.MOVE_2D )
            {
                Move2D( val );
                return;
            }


            if( Type == TransformType.SCALE_1D )
            {
                Scale1D( val );
                return;
            }
            if( Type == TransformType.SCALE_2D )
            {
                Scale2D( val );
                return;
            }
        }

        void Move1D( Vector3 val )
        {
            val -= _start; // Relative to where the user clicked, not to the origin.

            Vector3 localHitPoint = val;// Vector3.forward * val.z;
            localHitPoint.x = 0.0f;
            localHitPoint.y = 0.0f;

            Vector3 worldHitPoint = _startHandleToWorldMatrix * localHitPoint;

            Obj.transform.position = _startObjPos + worldHitPoint; // Origin is at the arrow, but the transformation has to be relative to the origin of the transformee.
        }

        void Move2D( Vector3 val )
        {
            val -= _start; // Relative to where the user clicked, not to the origin.

            Vector3 localHitPoint = val;
            localHitPoint.z = 0.0f;

            Vector3 worldHitPoint = _startHandleToWorldMatrix * localHitPoint;

            Obj.transform.position = _startObjPos + worldHitPoint; // Origin is at the arrow, but the transformation has to be relative to the origin of the transformee.
        }

        void Scale1D( Vector3 val )
        {
            // transform the direction if the scale (as a position, without normalizing) into the object's local space.
            // 
            Vector3 localHitPoint = val;

            // the vector with the "amount of scale to add" in every component.
            // normalize so that scale is 1 if val = where the player initially clicked the handle, scale = 2 twice as far from the handle's origin.
            Vector3 scaleVector = new Vector3(
                (localHitPoint.z - _start.z) / _start.z,
                (localHitPoint.z - _start.z) / _start.z,
                (localHitPoint.z - _start.z) / _start.z );

            // Multiply by the object's current scale.
            // if initial scale = 1 => scale per unit moved = 1
            // if initial scale = 2 => scale per unit moved = 2
            // etc...
            scaleVector.Scale( _startObjScale );

#warning TODO - breaks with rotated objects.

            // The direction of scale - factors by which to multiply the "amount" of scale from earlier
            // also multiply the speed of scaling by the object's initial scale.
            Vector3 worldScaleFactors = _startHandleToWorldDirection * new Vector3( 0.0f, 0.0f, 1.0f );
            Vector3 objScaleFactors = _startWorldToObjDirection * worldScaleFactors;
            objScaleFactors.x = Mathf.Abs( objScaleFactors.x );
            objScaleFactors.y = Mathf.Abs( objScaleFactors.y );
            objScaleFactors.z = Mathf.Abs( objScaleFactors.z );

            scaleVector.Scale( objScaleFactors ); // The amount of scale to add.

            Vector3 localScale = _startObjScale + scaleVector;

            Obj.transform.localScale = localScale;
        }

        void Scale2D( Vector3 val )
        {
            val = _start;
            val.Scale( new Vector3( 2.0f, 2.0f, 1.0f ) );
            Vector3 localHitPoint = val;


            Vector3 scaleToAddX = new Vector3(
                (localHitPoint.x - _start.x) / _start.x,
                (localHitPoint.x - _start.x) / _start.x,
                (localHitPoint.x - _start.x) / _start.x );
            Vector3 scaleToAddY = new Vector3(
                (localHitPoint.y - _start.y) / _start.y,
                (localHitPoint.y - _start.y) / _start.y,
                (localHitPoint.y - _start.y) / _start.y );

            scaleToAddX.Scale( _startObjScale );
            scaleToAddY.Scale( _startObjScale );

            Vector3 worldScaleFactors = _startHandleToWorldDirection * new Vector3( 1.0f, 1.0f, 0.0f ); // local space coefficients
            Vector3 objScaleFactors = _startWorldToObjDirection * worldScaleFactors;
            objScaleFactors.x = Mathf.Abs( objScaleFactors.x );
            objScaleFactors.y = Mathf.Abs( objScaleFactors.y );
            objScaleFactors.z = Mathf.Abs( objScaleFactors.z );

            scaleToAddX.Scale( objScaleFactors ); // The amount of scale to add.
            scaleToAddY.Scale( objScaleFactors );

            Vector3 localScale = _startObjScale + scaleToAddX + scaleToAddY;

            Obj.transform.localScale = localScale;
        }

        public void EndTransformation()
        {
            ContinueTransformation();
            _isMoving = false;
        }

        /// <summary>
        /// Returns the plane to use when raycasting with this transform handle.
        /// </summary>
        /// <returns>The plane that operates in world space.</returns>
        private Plane GetPlane( Camera camera )
        {
            Vector3 normal;
            if( Type == TransformType.MOVE_1D || Type == TransformType.SCALE_1D )
            {
                // plane faces the camera, but is then projected onto the plane normal to the forward direction of the arrow.
                normal = (camera.transform.position - _startPos).normalized;

                normal = Vector3.ProjectOnPlane( normal, _startForward ); // project the camera-facing normal onto the forward direction to make it so that every point along the forward direction lies on the plane.

                normal.Normalize();
            }
            else
            {
                normal = this.transform.forward;
            }

            Plane plane = new Plane( normal, _startPos );

            return plane;
        }

        /// <summary>
        /// Returns the click point, in transform handle's local space.
        /// </summary>
        private Vector3 Raycast( Camera camera )
        {
            Plane plane = GetPlane( camera );

            Ray ray = camera.ScreenPointToRay( Input.mousePosition ); // The ray operates in world space.

            if( plane.Raycast( ray, out float distanceAlongRay ) )
            {
                Vector3 hitPoint = ray.GetPoint( distanceAlongRay ); // relative to world origin.

                hitPoint -= _startPos; // relative to start position instead of the origin. If you set relative to current position it flickers when parented to the moving object.

                Vector3 hitPointLocal = _startWorldToHandleMatrix * hitPoint;

                return hitPointLocal;
            }
            else // This will only happen if we're looking at a very acute angle along or opposite to the arrow (or if we somehow can glitch the camera behind the starting point).
            {
                return new Vector3( float.NaN, float.NaN, float.NaN );
            }
        }
    }
}