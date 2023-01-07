using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityPlus.AssetManagement;

namespace RuntimeEditor.Core.Viewport
{
    /// <summary>
    /// Represents a translation, rotation, or scale tool's handle.
    /// </summary>
    [RequireComponent( typeof( Collider ) )]
    public class ViewportTransformHandle : MonoBehaviour
    {
        /*public struct Transformation
        {
            public Vector3 Translation { get; }
            public Quaternion Rotation { get; }
            public Vector3 Scale { get; }

            public Transformation( Vector3 translation, Quaternion rotation, Vector3 scale )
            {
                this.Translation = translation;
                this.Rotation = rotation;
                this.Scale = scale;
            }
        }*/

        public enum TransformType : byte
        {
            Translate1D,
            Rotate1D,
            Scale1D,

            Translate2D,
            Scale2D
        }

        [Flags]
        public enum TransformAxis : byte
        {
            X = 1,
            Y = 2,
            Z = 4,
            XY = X | Y,
            XZ = X | Z,
            YZ = Y | Z,
            XYZ = X | Y | Z
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

        /// <summary>
        /// The object that's affected by the transformation.
        /// </summary>
        [field: SerializeField]
        public GameObject Obj { get; set; }

        /// <summary>
        /// The camera used for raycasting and input.
        /// </summary>
        [field: SerializeField]
        public Camera Camera { get; set; }

        /// <summary>
        /// The type of transformation that this handle performs.
        /// </summary>
        [field: SerializeField]
        public TransformType Type { get; set; } = TransformType.Translate1D;

        // depending on the desired transformation space (world/local/camera), we can place the move tools appropriately.

        /// <summary>
        /// True if a transformation is currently occuring.
        /// </summary>
        public bool IsHeld { get; private set; } = false;

        public Vector3 TranslationDeltaLastFrame { get; private set; }
        public Quaternion RotationDeltaLastFrame { get; private set; }

        /// Point where the user clicked when the transformation started (in arrow's local space).
        /// </summary>
        Vector3 _start = Vector3.zero;

        /// <summary>
        /// Arrow's transform position at the start of the transformation (in world space).
        /// </summary>
        Vector3 _startPos;
        /// <summary>
        /// Arrow's forward direction at the start of the transformation (in world space).
        /// </summary>
        Vector3 _startForward;

        Matrix4x4 _startHandleToWorldTRMatrix; // Initial values of the handle, so the handle can move while the transform is active but the transform is going to act as if it didn't move.
        Matrix4x4 _startWorldToHandleTRMatrix;
        Matrix4x4 _startHandleToWorldRMatrix;
        Matrix4x4 _startWorldToHandleRMatrix;
        Matrix4x4 _startWorldToObjRMatrix;

        /// <summary>
        /// Affected object's position at the start of the transformation (in world space).
        /// </summary>
        Vector3 _startObjPos = Vector3.zero;
        /// <summary>
        /// Affected object's rotation at the start of the transformation (in world space).
        /// </summary>
        Quaternion _startObjRotation = Quaternion.identity;
        /// <summary>
        /// Affected object's scale at the start of the transformation (in object's local space).
        /// </summary>
        Vector3 _startObjScale = Vector3.one;

        Collider _collider;

        const int LAYER = 3;

        void Awake()
        {
            _collider = this.GetComponent<Collider>();
        }

        void Update()
        {
            if( Input.GetKeyDown( KeyCode.Mouse0 ) && !IsHeld )
            {
                Ray ray = this.Camera.ScreenPointToRay( Input.mousePosition );

                // arrows get drawn on top of other objects.
                if( Physics.Raycast( ray, out RaycastHit hitInfo, float.MaxValue, 1 << LAYER ) )
                {
                    if( hitInfo.collider == _collider )
                    {
                        StartTransformation();
                    }
                }
                return;
            }

            if( Input.GetKey( KeyCode.Mouse0 ) && IsHeld )
            {
                ContinueTransformation();
                return;
            }

            if( Input.GetKeyUp( KeyCode.Mouse0 ) && IsHeld )
            {
                EndTransformation();
                return;
            }
        }

        private void CacheStartingVariables()
        {
            _startPos = this.transform.position;
            _startForward = this.transform.forward;
            _startHandleToWorldTRMatrix = Matrix4x4.TRS( this.transform.position, this.transform.rotation, Vector3.one ); // discard the scale component.
            _startWorldToHandleTRMatrix = _startHandleToWorldTRMatrix.inverse;
            _startHandleToWorldRMatrix = Matrix4x4.TRS( Vector3.zero, this.transform.rotation, Vector3.one ); // discard the position and scale components.
            _startWorldToHandleRMatrix = _startHandleToWorldRMatrix.inverse;
            _startWorldToObjRMatrix = Matrix4x4.TRS( Vector3.zero, this.Obj.transform.rotation, Vector3.one ).inverse; // discard the position and scale components.

            _startObjPos = this.Obj.transform.position;
            _startObjRotation = this.Obj.transform.rotation;
            _startObjScale = this.Obj.transform.localScale;

            TranslationDeltaLastFrame = Vector3.zero;
            RotationDeltaLastFrame = Quaternion.identity;
        }

        public void StartTransformation()
        {
            CacheStartingVariables();

            _start = Raycast( this.Camera );
            IsHeld = true;
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

            if( Type == TransformType.Translate1D )
            {
                Translate1D( val );
                return;
            }
            if( Type == TransformType.Translate2D )
            {
                Translate2D( val );
                return;
            }
            if( Type == TransformType.Rotate1D )
            {
                Rotate1D( val );
                return;
            }

            if( Type == TransformType.Scale1D )
            {
                Scale1D( val );
                return;
            }
            if( Type == TransformType.Scale2D )
            {
                Scale2D( val );
                return;
            }
        }

        public void EndTransformation()
        {
            ContinueTransformation();
            IsHeld = false;
            TranslationDeltaLastFrame = Vector3.zero;
            RotationDeltaLastFrame = Quaternion.identity;
        }

        public static float RoundToMultiple( float value, float multiple )
        {
            return multiple * Mathf.Round( value / multiple );
        }

        public static Vector3 RoundToMultiple( Vector3 value, float multiple )
        {
            return new Vector3(
                multiple * Mathf.Round( value.x / multiple ),
                multiple * Mathf.Round( value.y / multiple ),
                multiple * Mathf.Round( value.z / multiple )
                );
        }

        void Translate1D( Vector3 val )
        {
            val -= _start; // Relative to where the user clicked, not to the origin.
                           // to add snapping, round the val to the nearest multiple of the snap step.

            if( Input.GetKey( KeyCode.LeftShift ) )
            {
                val = RoundToMultiple( val, 0.25f );
            }

            Vector3 localHitPoint = val;// Vector3.forward * val.z;
            localHitPoint.x = 0.0f;
            localHitPoint.y = 0.0f;

            Vector3 worldHitPoint = _startHandleToWorldTRMatrix * localHitPoint;

            Vector3 lastPosition = Obj.transform.position;
            Vector3 targetPosition = _startObjPos + worldHitPoint;

            Obj.transform.position = _startObjPos + worldHitPoint; // Origin is at the arrow, but the transformation has to be relative to the origin of the transformee.
            TranslationDeltaLastFrame = targetPosition - lastPosition;
        }

        void Translate2D( Vector3 val )
        {
            val -= _start; // Relative to where the user clicked, not to the origin.
                           // to add snapping, round the val to the nearest multiple of the snap step.

            if( Input.GetKey( KeyCode.LeftShift ) )
            {
                val = RoundToMultiple( val, 0.25f );
            }

            Vector3 localHitPoint = val;
            localHitPoint.z = 0.0f;

            Vector3 worldHitPoint = _startHandleToWorldTRMatrix * localHitPoint;

            Vector3 lastPosition = Obj.transform.position;
            Vector3 targetPosition = _startObjPos + worldHitPoint;

            Obj.transform.position = _startObjPos + worldHitPoint; // Origin is at the arrow, but the transformation has to be relative to the origin of the transformee.
            TranslationDeltaLastFrame = targetPosition - lastPosition;
        }

        void Rotate1D( Vector3 val )
        {
            Vector3 originToStart = _start;
            Vector3 originToCurrent = val;

            float angle = Vector3.SignedAngle( originToStart, originToCurrent, Vector3.forward );
            if( angle < 0.0f )
            {
                angle += 360.0f; // convert from [-180, 180] to [0, 360]
            }

            if( Input.GetKey( KeyCode.LeftShift ) )
            {
                angle = RoundToMultiple( angle, 22.5f );
            }

            Quaternion lastRotation = Obj.transform.rotation;

            Obj.transform.rotation = _startObjRotation; // this way we don't have to calculate the step in this frame, just reset it, and set the entire step.
            Obj.transform.Rotate( _startForward, angle, UnityEngine.Space.World );

            Quaternion targetRotation = Obj.transform.rotation;
            RotationDeltaLastFrame = lastRotation.Inverse() * targetRotation;
        }

        void Scale1D( Vector3 val )
        {
            // transform the direction if the scale (as a position, without normalizing) into the object's local space.
            Vector3 localHitPoint = val;
            // to add snapping, round the val to the nearest multiple of the snap step.

            // The vector with the "amount of scale to add" in every component.
            // Normalize so that scale is 1 if val = where the player initially clicked the handle, scale = 2 twice as far from the handle's origin.
            Vector3 scaleVector = new Vector3(
                (localHitPoint.z - _start.z) / _start.z,
                (localHitPoint.z - _start.z) / _start.z,
                (localHitPoint.z - _start.z) / _start.z );

            if( Input.GetKey( KeyCode.LeftShift ) )
            {
                scaleVector = RoundToMultiple( scaleVector, 0.25f );
            }

            // Multiply by the object's current scale.
            // if initial scale = 1 => scale per unit moved = 1
            // if initial scale = 2 => scale per unit moved = 2
            // etc...
            scaleVector.Scale( _startObjScale );

            // The direction of scale - factors by which to multiply the "amount" of scale from earlier
            // also multiply the speed of scaling by the object's initial scale.
            Vector3 worldScaleFactors = _startHandleToWorldRMatrix * new Vector3( 0.0f, 0.0f, 1.0f );
            Vector3 objScaleFactors = _startWorldToObjRMatrix * worldScaleFactors;
            objScaleFactors.x = Mathf.Abs( objScaleFactors.x );
            objScaleFactors.y = Mathf.Abs( objScaleFactors.y );
            objScaleFactors.z = Mathf.Abs( objScaleFactors.z );

            scaleVector.Scale( objScaleFactors ); // The amount of scale to add.

            Vector3 localScale = _startObjScale + scaleVector;

            Obj.transform.localScale = localScale;
        }

        void Scale2D( Vector3 val )
        {
            // I'm not 100% happy with it, but it'll do. 2D scale with arbitrary axes makes little sense conceptually anyway with the control scheme of normalizing by the start point.
            Vector3 localHitPoint = val;
            // to add snapping, round the val to the nearest multiple of the snap step.

            // The vector with the "amount of scale to add" in every component.
            // Normalize so that scale is 1 if val = where the player initially clicked the handle, scale = 2 twice as far from the handle's origin.
            Vector3 scaleToAddX = new Vector3(
                (localHitPoint.x - _start.x) / _start.x,
                (localHitPoint.x - _start.x) / _start.x,
                (localHitPoint.x - _start.x) / _start.x );
            Vector3 scaleToAddY = new Vector3(
                (localHitPoint.y - _start.y) / _start.y,
                (localHitPoint.y - _start.y) / _start.y,
                (localHitPoint.y - _start.y) / _start.y );

            if( Input.GetKey( KeyCode.LeftShift ) )
            {
                scaleToAddX = RoundToMultiple( scaleToAddX, 0.25f );

                scaleToAddY = RoundToMultiple( scaleToAddY, 0.25f );
            }

            // Multiply by the object's current scale.
            // if initial scale = 1 => scale per unit moved = 1
            // if initial scale = 2 => scale per unit moved = 2
            // etc...
            scaleToAddX.Scale( _startObjScale );
            scaleToAddY.Scale( _startObjScale );

            // The factors depend on where you drag the handle.
            // In general it tries to match the scale axis to the drag direction, but the pivot is at the vertex, so it's impossible to have a drag with purely left/right component.
            Vector3 worldScaleFactors = _startHandleToWorldRMatrix * new Vector3( (localHitPoint.x / _start.x), (localHitPoint.y / _start.y), 0.0f ).normalized; // local space coefficients
            Vector3 objScaleFactors = _startWorldToObjRMatrix * worldScaleFactors;
            objScaleFactors.x = Mathf.Abs( objScaleFactors.x );
            objScaleFactors.y = Mathf.Abs( objScaleFactors.y );
            objScaleFactors.z = Mathf.Abs( objScaleFactors.z );

            scaleToAddX.Scale( objScaleFactors ); // The amount of scale to add.
            scaleToAddY.Scale( objScaleFactors );

            Vector3 localScale = _startObjScale + (scaleToAddX + scaleToAddY) / 2.0f;

            Obj.transform.localScale = localScale;
        }

        /// <summary>
        /// Returns the plane to use when raycasting with this transform handle.
        /// </summary>
        /// <returns>The plane that operates in world space.</returns>
        private Plane GetPlane( Camera camera )
        {
            Vector3 normal;
            if( Type == TransformType.Translate1D || Type == TransformType.Scale1D )
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

                Vector3 hitPointLocal = _startWorldToHandleTRMatrix * hitPoint;

                return hitPointLocal;
            }
            else // This will only happen if we're looking at a very acute angle along or opposite to the arrow (or if we somehow can glitch the camera behind the starting point).
            {
                return new Vector3( float.NaN, float.NaN, float.NaN );
            }
        }

        public static ViewportTransformHandle Create( Camera raycastCamera, GameObject obj, TransformType handleType, Transform parent, Vector3 localPosition, Quaternion localRotation, TransformAxis axis )
        {
            GameObject gameObject = new GameObject( $"Transform Handle [{handleType}, {axis}]" );
            gameObject.layer = LAYER;

            gameObject.transform.SetParent( parent );
            gameObject.transform.localPosition = localPosition;
            gameObject.transform.localRotation = localRotation;
            gameObject.transform.localScale = Vector3.one;

            MeshFilter meshFilter = gameObject.AddComponent<MeshFilter>();

            MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;

            if( axis == TransformAxis.X || axis == TransformAxis.YZ )
            {
                meshRenderer.material = AssetRegistry<Material>.GetAsset( "RuntimeEditor/Materials/coordX" );
            }
            else if( axis == TransformAxis.Y || axis == TransformAxis.XZ )
            {
                meshRenderer.material = AssetRegistry<Material>.GetAsset( "RuntimeEditor/Materials/coordY" );
            }
            else if( axis == TransformAxis.Z || axis == TransformAxis.XY )
            {
                meshRenderer.material = AssetRegistry<Material>.GetAsset( "RuntimeEditor/Materials/coordZ" );
            }

            if( handleType == TransformType.Translate1D )
            {
                CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
                collider.radius = 0.25f;
                collider.height = 1.5f;
                collider.direction = 2; // 0 = x-axis, 1 = y-axis, 2 = z-axis
                collider.center = new Vector3( 0.0f, 0.0f, 0.75f );

                meshFilter.mesh = AssetRegistry<Mesh>.GetAsset( "RuntimeEditor/Meshes/ViewportArrow" );
            }
            else if( handleType == TransformType.Scale1D )
            {
                CapsuleCollider collider = gameObject.AddComponent<CapsuleCollider>();
                collider.radius = 0.25f;
                collider.height = 1.5f;
                collider.direction = 2; // 0 = x-axis, 1 = y-axis, 2 = z-axis
                collider.center = new Vector3( 0.0f, 0.0f, 0.75f );

                meshFilter.mesh = AssetRegistry<Mesh>.GetAsset( "RuntimeEditor/Meshes/ViewportArrowScale" );
            }
            else if( handleType == TransformType.Translate2D
                  || handleType == TransformType.Scale2D )
            {
                BoxCollider collider = gameObject.AddComponent<BoxCollider>();
                collider.size = new Vector3( 1.0f, 1.0f, 0.125f );
                collider.center = new Vector3( 0.5f, 0.5f, 0.0f );

                meshFilter.mesh = AssetRegistry<Mesh>.GetAsset( "RuntimeEditor/Meshes/ViewportPlane" );
            }
            else if( handleType == TransformType.Rotate1D )
            {
                BoxCollider collider = gameObject.AddComponent<BoxCollider>();
                collider.size = new Vector3( 3.0f, 3.0f, 0.125f );
                collider.center = new Vector3( 0.0f, 0.0f, 0.0f );

                meshFilter.mesh = AssetRegistry<Mesh>.GetAsset( "RuntimeEditor/Meshes/ViewportPlaneLarge" );
            }

            ViewportTransformHandle handle = gameObject.AddComponent<ViewportTransformHandle>();
            handle.Obj = obj;
            handle.Camera = raycastCamera;
            handle.Type = handleType;

            return handle;
        }
    }
}